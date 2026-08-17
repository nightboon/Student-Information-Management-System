using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class StudentEnrolmentReader
    {
        private const decimal DefaultFeePerCredit = 150m;
        private const int DefaultCapacity = 40;

        public static StudentEnrollmentPage GetEnrollmentPage(UserContext user)
        {
            if (!IsStudent(user)) return null;

            var account = StudentProfileReader.GetAccount(user);
            if (account == null) return null;

            var semesterCount = account.ProgrammeSemesterCount;
            var isGraduated = semesterCount > 0 && account.CurrentSemesterNo > semesterCount;
            var term = AcademicTermReader.GetRegistrationTerm(user);
            var alreadyRegisteredCount = term == null ? 0 : GetRegisteredCount(account.StudentId, term);
            return new StudentEnrollmentPage
            {
                Term = term,
                Window = AcademicTermReader.BuildRegistrationWindow(term, alreadyRegisteredCount > 0),
                SemesterNo = account.CurrentSemesterNo,
                SemesterCount = semesterCount,
                Offerings = (isGraduated || term == null) ? new List<StudentOfferingOption>() : GetOfferingOptions(user, account.StudentId, term),
                AlreadyRegisteredCount = alreadyRegisteredCount
            };
        }

        /// <summary>How many courses the student is already registered for in the given term.</summary>
        private static int GetRegisteredCount(string studentId, StudentRegistrationTerm term)
        {
            const string sql =
                "SELECT COUNT(*) FROM ENROLLMENTS e JOIN COURSE_OFFERINGS co ON co.offer_id=e.offer_id " +
                "WHERE e.student_id = @studentId AND co.session_id=@sessionId " +
                "AND e.status IN ('ENROLLED', 'PENDING')";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@studentId", studentId);
                cmd.Parameters.AddWithValue("@sessionId", term.SessionId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>Course codes the student has passed with a grade of C- or above (prerequisite-eligible).</summary>
        private static HashSet<string> GetPassedCourseIds(SqlConnection conn, string studentId)
        {
            const string sql =
                "SELECT DISTINCT co.course_id FROM GRADES g " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = g.offer_id " +
                "WHERE g.student_id = @studentId AND g.letter_grade IS NOT NULL AND g.letter_grade NOT IN ('D', 'F')";
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@studentId", studentId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) set.Add(Text(reader["course_id"]));
                }
            }
            return set;
        }

        private static bool IsPrerequisiteMet(string prerequisites, HashSet<string> passedCourseIds)
        {
            if (string.IsNullOrWhiteSpace(prerequisites)) return true;
            var required = prerequisites.Trim();
            if (string.Equals(required, "None", StringComparison.OrdinalIgnoreCase)) return true;
            return passedCourseIds.Contains(required);
        }

        /// <summary>SQL fragment (uses @studentId) verifying a course's prerequisite, if any, has been passed with a grade of C- or above.</summary>
        private const string PrerequisiteMetSql =
            "(c.prerequisites IS NULL OR c.prerequisites = '' OR c.prerequisites = 'None' " +
            "OR EXISTS (SELECT 1 FROM GRADES g JOIN COURSE_OFFERINGS gco ON gco.offer_id = g.offer_id " +
            "WHERE g.student_id = @studentId AND gco.course_id = c.prerequisites " +
            "AND g.letter_grade IS NOT NULL AND g.letter_grade NOT IN ('D', 'F')))";

        private static List<StudentOfferingOption> GetOfferingOptions(UserContext user, string studentId, StudentRegistrationTerm term)
        {
            const string sql =
                "SELECT co.offer_id, co.course_id, c.course_code, c.course_name, c.credit_hour, c.prerequisites, c.colour, " +
                "ISNULL(l.lecturer_name, '') AS lecturer_name, ISNULL(t.day_of_week, '') AS day_of_week, t.start_time, t.end_time, ISNULL(t.room, '') AS room, " +
                "(SELECT COUNT(*) FROM ENROLLMENTS e2 WHERE e2.offer_id = co.offer_id AND e2.status IN ('ENROLLED', 'PENDING')) AS enrolled_count, " +
                "(SELECT TOP 1 e3.status FROM ENROLLMENTS e3 WHERE e3.offer_id = co.offer_id AND e3.student_id = @studentId ORDER BY e3.enrollment_id DESC) AS my_status " +
                "FROM COURSE_OFFERINGS co " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "LEFT JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id " +
                "LEFT JOIN TIMETABLES t ON t.offer_id = co.offer_id " +
                "WHERE co.session_id = @sessionId AND co.status = 'ACTIVE' " +
                // Only courses belonging to the student's own programme.
                "AND c.programme_id = (SELECT s.programme_id FROM STUDENTS s WHERE s.student_id = @studentId) " +
                // Hide courses the student is ENROLLED or has PENDING requests for in other semesters.
                // Courses enrolled in the current registration term still appear so the student can
                // see them and request to drop them during the Add/Drop period.
                "AND NOT EXISTS (SELECT 1 FROM ENROLLMENTS e4 JOIN COURSES c4 ON c4.course_id = e4.course_id " +
                "WHERE e4.student_id = @studentId AND c4.course_name = c.course_name " +
                "AND e4.semester <> @academicYear + ' ' + @semester " +
                "AND e4.status IN ('ENROLLED', 'PENDING')) " +
                "ORDER BY c.course_code, t.day_of_week, t.start_time";

            var options = new List<StudentOfferingOption>();
            var byOffer = new Dictionary<int, StudentOfferingOption>();
            var schedules = new Dictionary<int, List<string>>();
            using (var conn = Db.OpenConnection())
            {
                var passedCourseIds = GetPassedCourseIds(conn, studentId);
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    cmd.Parameters.AddWithValue("@academicYear", term.AcademicYear);
                    cmd.Parameters.AddWithValue("@semester", term.Name);
                    cmd.Parameters.AddWithValue("@sessionId", term.SessionId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var offerId = IntValue(reader["offer_id"]);
                            StudentOfferingOption option;
                            if (!byOffer.TryGetValue(offerId, out option))
                            {
                                var code = Text(reader["course_code"]);
                                option = new StudentOfferingOption
                                {
                                    OfferingId = offerId,
                                    CourseId = Text(reader["course_id"]),
                                    CourseCode = code,
                                    CourseName = Text(reader["course_name"]),
                                    Description = "Course registration for " + Text(reader["course_name"]) + ".",
                                    LecturerName = LecturerOrFallback(Text(reader["lecturer_name"])),
                                    CreditHours = IntValue(reader["credit_hour"]),
                                    Prerequisites = Text(reader["prerequisites"]),
                                    PrerequisiteMet = IsPrerequisiteMet(Text(reader["prerequisites"]), passedCourseIds),
                                    EnrolledCount = IntValue(reader["enrolled_count"]),
                                    Capacity = DefaultCapacity,
                                    MyStatus = Text(reader["my_status"]),
                                    FeePerCredit = DefaultFeePerCredit,
                                    Color = ColorOrFallback(Text(reader["colour"]), code),
                                    Schedule = "TBA"
                                };
                                byOffer[offerId] = option;
                                schedules[offerId] = new List<string>();
                                options.Add(option);
                            }

                            var day = Text(reader["day_of_week"]);
                            if (!string.IsNullOrWhiteSpace(day))
                            {
                                var start = TimeValue(reader["start_time"]);
                                var end = TimeValue(reader["end_time"]);
                                schedules[offerId].Add(ShortDay(day) + " " + FormatTime(start) + "-" + FormatTime(end) + " " + Text(reader["room"]));
                            }
                        }
                    }
                }
            }

            foreach (var option in options)
            {
                var parts = schedules[option.OfferingId].Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                option.Schedule = parts.Count == 0 ? "TBA" : string.Join(", ", parts);
            }
            return options;
        }

        public static int Enrol(UserContext user, int[] offeringIds)
        {
            if (!IsStudent(user) || offeringIds == null || offeringIds.Length == 0) return 0;

            var account = StudentProfileReader.GetAccount(user);
            if (account == null) return 0;

            if (account.ProgrammeSemesterCount > 0 && account.CurrentSemesterNo >= account.ProgrammeSemesterCount)
                return 0;

            // Block enrollment when today is outside every session's registration window.
            var term = AcademicTermReader.GetRegistrationTerm(user);
            var hasEnrollment = term != null && GetRegisteredCount(account.StudentId, term) > 0;
            if (!AcademicTermReader.IsRegistrationOpen(hasEnrollment)) return 0;

            var inserted = 0;
            using (var conn = Db.OpenConnection())
            {
                foreach (var offerId in offeringIds.Distinct())
                {
                    const string sql =
                        "INSERT INTO ENROLLMENTS (student_id, offer_id, course_id, semester, status) " +
                        "SELECT @studentId, co.offer_id, co.course_id, co.academic_year + ' ' + co.semester, 'ENROLLED' " +
                        "FROM COURSE_OFFERINGS co JOIN COURSES c ON c.course_id = co.course_id " +
                        "WHERE co.offer_id = @offerId AND co.session_id=@sessionId AND co.status = 'ACTIVE' " +
                        "AND NOT EXISTS (SELECT 1 FROM ENROLLMENTS e WHERE e.student_id = @studentId AND e.offer_id = co.offer_id) " +
                        "AND " + PrerequisiteMetSql + "; " +
                        "SELECT @@ROWCOUNT";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", account.StudentId);
                        cmd.Parameters.AddWithValue("@offerId", offerId);
                        cmd.Parameters.AddWithValue("@sessionId", term.SessionId);
                        inserted += Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                if (inserted > 0 && term != null)
                {
                    var termLabel = term.AcademicYear + " " + term.Name;
                    const string updateSession =
                        "UPDATE STUDENTS SET session = @session " +
                        "WHERE student_id = @studentId AND (session IS NULL OR session = '' OR session <> @session)";
                    using (var cmd = new SqlCommand(updateSession, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", account.StudentId);
                        cmd.Parameters.AddWithValue("@session", termLabel);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return inserted;
        }

        public static int RequestAdd(UserContext user, int offeringId)
        {
            if (!IsStudent(user)) return 0;

            var account = StudentProfileReader.GetAccount(user);
            if (account == null) return 0;

            var term = AcademicTermReader.GetRegistrationTerm(user);
            var hasEnrollment = term != null && GetRegisteredCount(account.StudentId, term) > 0;
            if (!AcademicTermReader.IsAddDropOpen(hasEnrollment)) return 0;

            using (var conn = Db.OpenConnection())
            {
                const string sql =
                    "INSERT INTO ENROLLMENTS (student_id, offer_id, course_id, semester, status, request_type) " +
                    "SELECT @studentId, co.offer_id, co.course_id, co.academic_year + ' ' + co.semester, 'PENDING', 'ADD' " +
                    "FROM COURSE_OFFERINGS co JOIN COURSES c ON c.course_id = co.course_id " +
                    "WHERE co.offer_id = @offerId AND co.session_id=@sessionId AND co.status = 'ACTIVE' " +
                    "AND NOT EXISTS (SELECT 1 FROM ENROLLMENTS e WHERE e.student_id = @studentId " +
                    "AND e.offer_id = @offerId AND e.status IN ('ENROLLED', 'PENDING')) " +
                    "AND " + PrerequisiteMetSql + "; " +
                    "SELECT @@ROWCOUNT";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@studentId", account.StudentId);
                    cmd.Parameters.AddWithValue("@offerId", offeringId);
                    cmd.Parameters.AddWithValue("@sessionId", term.SessionId);
                    var result = cmd.ExecuteScalar();
                    return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
                }
            }
        }

        public static bool RequestDrop(UserContext user, int offeringId)
        {
            if (!IsStudent(user)) return false;

            var account = StudentProfileReader.GetAccount(user);
            if (account == null) return false;

            var term = AcademicTermReader.GetRegistrationTerm(user);
            var hasEnrollment = term != null && GetRegisteredCount(account.StudentId, term) > 0;
            if (!AcademicTermReader.IsAddDropOpen(hasEnrollment)) return false;

            using (var conn = Db.OpenConnection())
            {
                const string sql =
                    "UPDATE ENROLLMENTS SET status = 'PENDING', request_type = 'DROP' " +
                    "WHERE student_id = @studentId AND offer_id = @offerId AND status = 'ENROLLED' " +
                    "AND EXISTS(SELECT 1 FROM COURSE_OFFERINGS co WHERE co.offer_id=@offerId AND co.session_id=@sessionId); " +
                    "SELECT @@ROWCOUNT";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@studentId", account.StudentId);
                    cmd.Parameters.AddWithValue("@offerId", offeringId);
                    cmd.Parameters.AddWithValue("@sessionId", term.SessionId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }
    }
}
