using System;
using System.Data.SqlClient;
using src.db;

namespace src.services
{
    /// <summary>
    /// A lecturer's profile row from the LECTURERS table.
    /// </summary>
    public class Lecturer
    {
        public int LecturerId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string MailingAddress { get; set; }
        public string IconPath { get; set; }
    }

    /// <summary>
    /// Read-only access to lecturer profiles. Returns null when the user id has
    /// no LECTURERS row. SQL exceptions are not caught here; they propagate to
    /// the caller.
    /// </summary>
    public static class LecturerService
    {
        private const string SelectByUser =
            "SELECT l.lecturer_id, l.user_id, l.full_name, l.department, " +
            "u.email, u.username, " +
            "ISNULL(u.phone, '') AS phone, " +
            "ISNULL(u.mailing_address, '') AS mailing_address, " +
            "ISNULL(u.icon_path, '') AS icon_path " +
            "FROM LECTURERS l " +
            "JOIN USERS u ON u.user_id = l.user_id " +
            "WHERE l.user_id = @userId";

        public static Lecturer GetByUserId(int userId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectByUser, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new Lecturer
                    {
                        LecturerId = (int)reader["lecturer_id"],
                        UserId = (int)reader["user_id"],
                        FullName = reader["full_name"].ToString(),
                        Department = reader["department"].ToString(),
                        Email = reader["email"].ToString(),
                        Username = reader["username"].ToString(),
                        Phone = reader["phone"].ToString(),
                        MailingAddress = reader["mailing_address"].ToString(),
                        IconPath = reader["icon_path"].ToString()
                    };
                }
            }
        }

        // Scheduled classes for the lecturer on the given weekday (e.g. "Monday").
        private const string CountClassesOnDay =
            "SELECT COUNT(*) FROM TEACHINGS t " +
            "JOIN TIMETABLES tt ON tt.offering_id = t.offering_id " +
            "WHERE t.lecturer_id = @lecturerId AND tt.day_of_week = @day";

        /// <summary>Number of classes the lecturer teaches on the given weekday.</summary>
        public static int CountClassesOn(int lecturerId, DayOfWeek day)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(CountClassesOnDay, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                // DayOfWeek.ToString() yields invariant English ("Monday"), matching
                // the day_of_week values stored in TIMETABLES.
                cmd.Parameters.AddWithValue("@day", day.ToString());
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Submissions to the lecturer's assignments that have not been marked yet
        // (marks is NULL once a submission is graded it carries a mark).
        private const string CountUnmarkedSubmissions =
            "SELECT COUNT(*) FROM SUBMISSIONS s " +
            "JOIN ASSIGNMENTS a ON s.assignment_id = a.assignment_id " +
            "WHERE a.lecturer_id = @lecturerId AND s.marks IS NULL";

        /// <summary>Number of ungraded submissions awaiting the lecturer's review.</summary>
        public static int CountSubmissionsToReview(int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(CountUnmarkedSubmissions, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
