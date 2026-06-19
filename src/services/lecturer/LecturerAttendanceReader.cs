using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class LecturerAttendanceReader
    {
        public static AttendanceOffering GetAttendanceOffering(UserContext user, int offeringId)
        {
            if (user == null) return null;

            const string sql =
                "SELECT co.offer_id, c.course_code, c.course_name, c.colour, " +
                "(SELECT COUNT(*) FROM ENROLLMENTS e WHERE e.offer_id = co.offer_id AND e.status = 'ENROLLED') AS enrolled_count " +
                "FROM COURSE_OFFERINGS co JOIN COURSES c ON c.course_id = co.course_id " +
                "WHERE co.offer_id = @offerId";

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageOffer(conn, user, offeringId)) return null;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@offerId", offeringId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        var code = Text(reader["course_code"]);
                        return new AttendanceOffering
                        {
                            OfferingId = IntValue(reader["offer_id"]),
                            CourseCode = code,
                            CourseName = Text(reader["course_name"]),
                            EnrolledCount = IntValue(reader["enrolled_count"]),
                            Color = ColorOrFallback(Text(reader["colour"]), code)
                        };
                    }
                }
            }
        }

        public static List<RosterEntry> GetAttendanceRoster(UserContext user, int offeringId, DateTime date)
        {
            var rows = new List<RosterEntry>();
            if (user == null) return rows;

            const string sql =
                "SELECT e.enrollment_id, s.student_id, s.student_name, s.student_email, s.icon, ISNULL(ar.status, '') AS status " +
                "FROM ENROLLMENTS e " +
                "JOIN STUDENTS s ON s.student_id = e.student_id " +
                "LEFT JOIN ATTENDANCE_SESSIONS ats ON ats.offer_id = e.offer_id AND ats.session_date = @date " +
                "LEFT JOIN ATTENDANCE_RECORDS ar ON ar.session_id = ats.session_id AND ar.student_id = s.student_id " +
                "WHERE e.offer_id = @offerId AND e.status = 'ENROLLED' " +
                "ORDER BY s.student_name";

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageOffer(conn, user, offeringId)) return rows;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@offerId", offeringId);
                    cmd.Parameters.AddWithValue("@date", date.Date);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rows.Add(new RosterEntry
                            {
                                EnrolmentId = IntValue(reader["enrollment_id"]),
                                StudentId = Text(reader["student_id"]),
                                FullName = Text(reader["student_name"]),
                                Email = Text(reader["student_email"]),
                                IconPath = Text(reader["icon"]),
                                Status = Text(reader["status"])
                            });
                        }
                    }
                }
            }
            return rows;
        }

        // statuses: enrollment_id -> status. Returns number of records written.
        public static int SaveAttendance(UserContext user, int offeringId, DateTime date, IDictionary<int, string> statuses)
        {
            if (user == null || statuses == null || statuses.Count == 0) return 0;

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageOffer(conn, user, offeringId)) return 0;

                int sessionId = FindOrCreateSession(conn, user, offeringId, date);
                if (sessionId == 0) return 0;

                // Map enrollment_id -> student_id for this offering.
                var studentByEnrol = new Dictionary<int, string>();
                using (var cmd = new SqlCommand(
                    "SELECT enrollment_id, student_id FROM ENROLLMENTS WHERE offer_id = @offerId AND status = 'ENROLLED'", conn))
                {
                    cmd.Parameters.AddWithValue("@offerId", offeringId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            studentByEnrol[IntValue(reader["enrollment_id"])] = Text(reader["student_id"]);
                    }
                }

                int written = 0;
                foreach (var pair in statuses)
                {
                    string studentId;
                    if (!studentByEnrol.TryGetValue(pair.Key, out studentId)) continue;
                    if (string.IsNullOrWhiteSpace(pair.Value)) continue;

                    const string sql =
                        "UPDATE ATTENDANCE_RECORDS SET status = @status " +
                        "WHERE session_id = @sessionId AND student_id = @studentId; " +
                        "IF @@ROWCOUNT = 0 " +
                        "INSERT INTO ATTENDANCE_RECORDS (session_id, student_id, status) " +
                        "VALUES (@sessionId, @studentId, @status);";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", pair.Value);
                        cmd.Parameters.AddWithValue("@sessionId", sessionId);
                        cmd.Parameters.AddWithValue("@studentId", studentId);
                        cmd.ExecuteNonQuery();
                    }
                    written++;
                }
                return written;
            }
        }

        private static int FindOrCreateSession(SqlConnection conn, UserContext user, int offeringId, DateTime date)
        {
            using (var cmd = new SqlCommand(
                "SELECT TOP 1 session_id FROM ATTENDANCE_SESSIONS WHERE offer_id = @offerId AND session_date = @date ORDER BY session_id", conn))
            {
                cmd.Parameters.AddWithValue("@offerId", offeringId);
                cmd.Parameters.AddWithValue("@date", date.Date);
                var existing = cmd.ExecuteScalar();
                if (existing != null && existing != DBNull.Value) return Convert.ToInt32(existing);
            }

            // Default times from the offering's first timetable row, else 08:00-09:00.
            TimeSpan start = new TimeSpan(8, 0, 0), end = new TimeSpan(9, 0, 0);
            using (var cmd = new SqlCommand(
                "SELECT TOP 1 start_time, end_time FROM TIMETABLES WHERE offer_id = @offerId ORDER BY timetable_id", conn))
            {
                cmd.Parameters.AddWithValue("@offerId", offeringId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        start = TimeValue(reader["start_time"]) ?? start;
                        end = TimeValue(reader["end_time"]) ?? end;
                    }
                }
            }

            var profile = LecturerProfileReader.GetProfile(user);
            const string insert =
                "INSERT INTO ATTENDANCE_SESSIONS (offer_id, session_date, start_time, end_time, created_by) " +
                "OUTPUT INSERTED.session_id VALUES (@offerId, @date, @start, @end, @createdBy)";
            using (var cmd = new SqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@offerId", offeringId);
                cmd.Parameters.AddWithValue("@date", date.Date);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);
                cmd.Parameters.AddWithValue("@createdBy", ServiceAccess.DbValue(profile == null ? null : profile.LecturerId));
                var id = cmd.ExecuteScalar();
                return id == null || id == DBNull.Value ? 0 : Convert.ToInt32(id);
            }
        }
    }
}
