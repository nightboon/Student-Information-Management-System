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

            var term = AcademicTermReader.GetRegistrationTerm();
            return new StudentEnrollmentPage
            {
                Term = term,
                Window = AcademicTermReader.BuildRegistrationWindow(term),
                SemesterNo = account.CurrentSemesterNo,
                Offerings = term == null ? new List<StudentOfferingOption>() : GetOfferingOptions(user, account.StudentId, term)
            };
        }

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
                "WHERE co.academic_year = @academicYear AND co.semester = @semester AND co.status = 'ACTIVE' " +
                "ORDER BY c.course_code, t.day_of_week, t.start_time";

            var options = new List<StudentOfferingOption>();
            var byOffer = new Dictionary<int, StudentOfferingOption>();
            var schedules = new Dictionary<int, List<string>>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@studentId", studentId);
                cmd.Parameters.AddWithValue("@academicYear", term.AcademicYear);
                cmd.Parameters.AddWithValue("@semester", term.Name);
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

            var inserted = 0;
            using (var conn = Db.OpenConnection())
            {
                foreach (var offerId in offeringIds.Distinct())
                {
                    const string sql =
                        "INSERT INTO ENROLLMENTS (student_id, offer_id, course_id, semester, status) " +
                        "SELECT @studentId, co.offer_id, co.course_id, co.academic_year + ' ' + co.semester, 'ENROLLED' " +
                        "FROM COURSE_OFFERINGS co " +
                        "WHERE co.offer_id = @offerId AND co.status = 'ACTIVE' " +
                        "AND NOT EXISTS (SELECT 1 FROM ENROLLMENTS e WHERE e.student_id = @studentId AND e.offer_id = co.offer_id); " +
                        "SELECT @@ROWCOUNT";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", account.StudentId);
                        cmd.Parameters.AddWithValue("@offerId", offerId);
                        inserted += Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            return inserted;
        }
    }
}
