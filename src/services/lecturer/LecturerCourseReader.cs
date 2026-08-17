using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class LecturerCourseReader
    {
        public static List<LecturerCourseCard> GetCourses(UserContext user)
        {
            var courses = new List<LecturerCourseCard>();
            if (user == null || !user.IsLecturer) return courses;

            var current = AcademicTermReader.GetCurrentTerm();
            var currentLabel = TermLabel(current);

            string sql =
                "SELECT co.offer_id, c.course_id, c.course_code, c.course_name, c.credit_hour, c.colour, " +
                "co.academic_year, co.semester, co.academic_year + ' ' + co.semester AS semester_name, " +
                "(SELECT COUNT(*) FROM ENROLLMENTS e WHERE e.offer_id = co.offer_id AND e.status = 'ENROLLED') AS enrolled_count " +
                "FROM COURSE_OFFERINGS co " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "WHERE " + ServiceAccess.VisibleOfferScope("co") + " " +
                "ORDER BY co.academic_year DESC, co.semester, c.course_code";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var code = Text(reader["course_code"]);
                        var semesterName = Text(reader["semester_name"]);
                        courses.Add(new LecturerCourseCard
                        {
                            OfferingId = IntValue(reader["offer_id"]),
                            CourseId = Text(reader["course_id"]),
                            CourseCode = code,
                            CourseName = Text(reader["course_name"]),
                            AcademicYear = Text(reader["academic_year"]),
                            Semester = Text(reader["semester"]),
                            SemesterName = semesterName,
                            CreditHours = IntValue(reader["credit_hour"]),
                            EnrolledCount = IntValue(reader["enrolled_count"]),
                            Color = ColorOrFallback(Text(reader["colour"]), code),
                            Status = DeriveStatus(semesterName, currentLabel, current)
                        });
                    }
                }
            }
            return courses;
        }

        // "In progress" when the offering term is the current term; otherwise compare
        // the offering's session start date to today via the session lookup.
        private static string DeriveStatus(string semesterName, string currentLabel, StudentRegistrationTerm current)
        {
            if (IsSameTerm(semesterName, currentLabel)) return "In progress";
            var lookup = AcademicTermReader.GetSessionLookup();
            DateTime start;
            if (lookup.TryGetValue(semesterName, out start))
                return start.Date > DateTime.Today ? "Upcoming" : "Completed";
            return "Completed";
        }

        public static CourseDashboardStats GetCourseStats(UserContext user, int offeringId)
        {
            if (user == null) return null;

            string sql =
                "SELECT co.offer_id, c.course_code, c.course_name, c.credit_hour, c.colour, c.prerequisites, " +
                "co.academic_year + ' ' + co.semester AS semester_name, " +
                "ISNULL(p.education_level, '') AS education_level, " +
                "(SELECT COUNT(*) FROM ENROLLMENTS e WHERE e.offer_id = co.offer_id AND e.status = 'ENROLLED') AS enrolled_count, " +
                "(SELECT COUNT(*) FROM ENROLLMENTS e WHERE e.offer_id = co.offer_id AND e.status = 'PENDING') AS pending_count, " +
                "(SELECT COUNT(DISTINCT a.assignment_id) FROM ASSIGNMENTS a " +
                " JOIN MATERIALS mat ON mat.assignment_id = a.assignment_id " +
                " WHERE a.offer_id = co.offer_id AND mat.weight IS NOT NULL AND mat.weight > 0) AS assessment_count, " +
                "(SELECT COUNT(*) FROM MATERIALS m JOIN MODULES md ON md.module_id = m.module_id WHERE md.offer_id = co.offer_id) AS material_count " +
                "FROM COURSE_OFFERINGS co " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "LEFT JOIN PROGRAMMES p ON p.programme_id = c.programme_id " +
                "WHERE co.offer_id = @offerId AND " + ServiceAccess.VisibleOfferScope("co");

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageOffer(conn, user, offeringId)) return null;

                CourseDashboardStats stats;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    ServiceAccess.AddUserContextParameters(cmd, user);
                    cmd.Parameters.AddWithValue("@offerId", offeringId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        var code = Text(reader["course_code"]);
                        var name = Text(reader["course_name"]);
                        stats = new CourseDashboardStats
                        {
                            OfferingId = IntValue(reader["offer_id"]),
                            CourseCode = code,
                            CourseName = name,
                            Description = "Course materials, assignments, announcements, and grades for " + name + ".",
                            LevelLabel = string.IsNullOrWhiteSpace(Text(reader["education_level"])) ? "Course" : Text(reader["education_level"]),
                            CreditHours = IntValue(reader["credit_hour"]),
                            SemesterName = Text(reader["semester_name"]),
                            Color = ColorOrFallback(Text(reader["colour"]), code),
                            EnrolledCount = IntValue(reader["enrolled_count"]),
                            PendingCount = IntValue(reader["pending_count"]),
                            AssessmentCount = IntValue(reader["assessment_count"]),
                            MaterialCount = IntValue(reader["material_count"])
                        };
                    }
                }

                FillCourseMetrics(conn, offeringId, stats);
                return stats;
            }
        }

        // PendingGrading = submissions for this offering's assignments with no marks yet.
        // AverageGrade   = average submission percentage (marks_obtained / total_marks).
        // AttendanceRate = attended (present or late) over recorded attendance for the offering.
        private static void FillCourseMetrics(SqlConnection conn, int offeringId, CourseDashboardStats stats)
        {
            const string gradeSql =
                "SELECT a.total_marks, sub.marks_obtained, sub.status " +
                "FROM ASSIGNMENTS a " +
                "JOIN MATERIALS mat ON mat.assignment_id = a.assignment_id " +
                "JOIN SUBMISSIONS sub ON sub.assignment_id = a.assignment_id " +
                "WHERE a.offer_id = @offerId AND mat.weight IS NOT NULL AND mat.weight > 0";

            int pending = 0, graded = 0;
            decimal sumPercent = 0m;
            using (var cmd = new SqlCommand(gradeSql, conn))
            {
                cmd.Parameters.AddWithValue("@offerId", offeringId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var marks = DecimalValue(reader["marks_obtained"]);
                        if (!marks.HasValue) { pending++; continue; }
                        var max = IntValue(reader["total_marks"]);
                        if (max <= 0) max = 100;
                        graded++;
                        sumPercent += Math.Round(marks.Value / max * 100m, 2);
                    }
                }
            }
            stats.PendingGrading = pending;
            stats.AverageGrade = graded == 0 ? (decimal?)null : Math.Round(sumPercent / graded, 1);

            const string attSql =
                "SELECT ar.status FROM ATTENDANCE_SESSIONS ats " +
                "JOIN ATTENDANCE_RECORDS ar ON ar.session_id = ats.session_id " +
                "WHERE ats.offer_id = @offerId";
            int total = 0, present = 0;
            using (var cmd = new SqlCommand(attSql, conn))
            {
                cmd.Parameters.AddWithValue("@offerId", offeringId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        total++;
                        if (IsAttendedStatus(Text(reader["status"]))) present++;
                    }
                }
            }
            stats.AttendanceRate = total == 0 ? (decimal?)null : Math.Round((decimal)present / total, 4);
        }

        public static List<EnrolledStudentRow> GetRoster(UserContext user, int offeringId)
        {
            var rows = new List<EnrolledStudentRow>();
            if (user == null) return rows;

            const string sql =
                "SELECT s.student_id, s.student_name, s.student_email, s.phone, s.icon, e.status, " +
                "ISNULL(p.programme_name, '') AS programme_name, ISNULL(p.programme_code, '') AS programme_code " +
                "FROM ENROLLMENTS e " +
                "JOIN STUDENTS s ON s.student_id = e.student_id " +
                "LEFT JOIN PROGRAMMES p ON p.programme_id = s.programme_id " +
                "WHERE e.offer_id = @offerId AND e.status IN ('ENROLLED', 'PENDING') " +
                "ORDER BY e.status, s.student_name";

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageOffer(conn, user, offeringId)) return rows;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@offerId", offeringId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rows.Add(new EnrolledStudentRow
                            {
                                StudentId = Text(reader["student_id"]),
                                FullName = Text(reader["student_name"]),
                                Email = Text(reader["student_email"]),
                                Phone = Text(reader["phone"]),
                                ProgrammeName = Text(reader["programme_name"]),
                                ProgrammeCode = Text(reader["programme_code"]),
                                IconPath = Text(reader["icon"]),
                                IsPending = string.Equals(Text(reader["status"]), "PENDING", System.StringComparison.OrdinalIgnoreCase)
                            });
                        }
                    }
                }
            }
            return rows;
        }
    }
}
