using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using src.db;

namespace src.services
{
    /// <summary>
    /// A course offering the lecturer is taking attendance for, with the course
    /// metadata the take-attendance page header needs.
    /// </summary>
    public class AttendanceOffering
    {
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Color { get; set; }
        public int EnrolledCount { get; set; }
    }

    /// <summary>
    /// One enrolled student on the attendance roster, with the status already
    /// recorded for the session date ("" when nothing has been marked yet).
    /// </summary>
    public class RosterEntry
    {
        public int EnrolmentId { get; set; }
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Reads the attendance roster for one of a lecturer's offerings and records
    /// attendance against it. Every method is scoped to offerings the lecturer
    /// actually teaches, so a lecturer can never read or write attendance for a
    /// class that is not theirs. SQL exceptions are not caught here; they
    /// propagate to the caller.
    /// </summary>
    public static class LecturerAttendanceService
    {
        // Allowed attendance states. Anything else is ignored on save.
        private static readonly HashSet<string> ValidStatuses =
            new HashSet<string>(StringComparer.Ordinal) { "PRESENT", "LATE", "ABSENT" };

        // Offering header, but only when the lecturer teaches it (the EXISTS guard
        // is the authorisation check used by every entry point on the page).
        private const string SelectOffering =
            "SELECT c.course_code, c.course_name, ISNULL(c.color, '') AS color, " +
            "(SELECT COUNT(*) FROM ENROLMENTS en WHERE en.offering_id = o.offering_id " +
            "AND en.status = 'ENROLLED') AS enrolled_count " +
            "FROM COURSE_OFFERINGS o " +
            "JOIN COURSES c ON o.course_id = c.course_id " +
            "WHERE o.offering_id = @offeringId " +
            "AND EXISTS (SELECT 1 FROM TEACHINGS t " +
            "WHERE t.offering_id = o.offering_id AND t.lecturer_id = @lecturerId)";

        /// <summary>
        /// The offering's header, or null when the offering does not exist or the
        /// lecturer does not teach it.
        /// </summary>
        public static AttendanceOffering GetOffering(int offeringId, int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectOffering, conn))
            {
                cmd.Parameters.AddWithValue("@offeringId", offeringId);
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new AttendanceOffering
                    {
                        OfferingId = offeringId,
                        CourseCode = reader["course_code"].ToString(),
                        CourseName = reader["course_name"].ToString(),
                        Color = reader["color"].ToString(),
                        EnrolledCount = Convert.ToInt32(reader["enrolled_count"])
                    };
                }
            }
        }

        // Enrolled students for the offering plus any attendance already recorded
        // for the given date, alphabetical by name.
        private const string SelectRoster =
            "SELECT e.enrolment_id, st.student_id, st.full_name, u.email, " +
            "ISNULL(a.status, '') AS status " +
            "FROM ENROLMENTS e " +
            "JOIN STUDENTS st ON st.student_id = e.student_id " +
            "JOIN USERS u ON u.user_id = st.user_id " +
            "LEFT JOIN ATTENDANCE a ON a.enrolment_id = e.enrolment_id AND a.attendance_date = @date " +
            "WHERE e.offering_id = @offeringId AND e.status = 'ENROLLED' " +
            "ORDER BY st.full_name";

        /// <summary>
        /// The enrolled roster for the offering on <paramref name="date"/>, with
        /// each student's already-recorded status (empty when unmarked).
        /// </summary>
        public static List<RosterEntry> GetRoster(int offeringId, DateTime date)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectRoster, conn))
            {
                cmd.Parameters.AddWithValue("@offeringId", offeringId);
                cmd.Parameters.Add("@date", SqlDbType.Date).Value = date.Date;
                var roster = new List<RosterEntry>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roster.Add(new RosterEntry
                        {
                            EnrolmentId = (int)reader["enrolment_id"],
                            StudentId = (int)reader["student_id"],
                            FullName = reader["full_name"].ToString(),
                            Email = reader["email"].ToString(),
                            Status = reader["status"].ToString()
                        });
                    }
                }
                return roster;
            }
        }

        // ATTENDANCE has no unique key on (enrolment_id, attendance_date), so we
        // upsert by hand: update the existing row, and insert only when nothing was
        // updated. Both halves are constrained to enrolments that belong to this
        // offering, so a tampered enrolment id can never write a stray row.
        private const string UpsertAttendance =
            "UPDATE a SET a.status = @status " +
            "FROM ATTENDANCE a " +
            "JOIN ENROLMENTS e ON e.enrolment_id = a.enrolment_id " +
            "WHERE a.enrolment_id = @enrolmentId AND a.attendance_date = @date " +
            "AND e.offering_id = @offeringId; " +
            "IF @@ROWCOUNT = 0 " +
            "INSERT INTO ATTENDANCE (enrolment_id, attendance_date, status) " +
            "SELECT @enrolmentId, @date, @status FROM ENROLMENTS e " +
            "WHERE e.enrolment_id = @enrolmentId AND e.offering_id = @offeringId;";

        /// <summary>
        /// Records attendance for the given date. Each entry maps an enrolment id to
        /// a status (PRESENT / LATE / ABSENT); entries with any other status are
        /// skipped. Writes are wrapped in a single transaction. Returns the number
        /// of students recorded.
        /// </summary>
        public static int Save(int offeringId, DateTime date, IDictionary<int, string> statusByEnrolment)
        {
            if (statusByEnrolment == null || statusByEnrolment.Count == 0) return 0;

            int saved = 0;
            using (var conn = Db.OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                foreach (var pair in statusByEnrolment)
                {
                    var status = pair.Value == null ? "" : pair.Value.ToUpperInvariant();
                    if (!ValidStatuses.Contains(status)) continue;

                    using (var cmd = new SqlCommand(UpsertAttendance, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@enrolmentId", pair.Key);
                        cmd.Parameters.AddWithValue("@offeringId", offeringId);
                        cmd.Parameters.Add("@date", SqlDbType.Date).Value = date.Date;
                        cmd.Parameters.AddWithValue("@status", status);
                        saved += cmd.ExecuteNonQuery() > 0 ? 1 : 0;
                    }
                }
                tx.Commit();
            }
            return saved;
        }
    }
}
