using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using src.db;
using static src.services.ServiceMap;

namespace src.services
{
    public class SubmissionNotificationItem
    {
        public int NotificationId { get; set; }
        public int SubmissionId { get; set; }
        public int OfferingId { get; set; }
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string AssessmentTitle { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string SubmissionStatus { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public static class SubmissionNotificationService
    {
        public const string NotificationType = "SUBMISSION";

        public static void EnsureTables()
        {
            const string sql =
                "IF OBJECT_ID('dbo.SUBMISSION_NOTIFICATIONS', 'U') IS NULL BEGIN " +
                "CREATE TABLE dbo.SUBMISSION_NOTIFICATIONS (" +
                "notification_id int IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
                "submission_id int NOT NULL, student_id varchar(20) NOT NULL, assignment_id int NOT NULL, " +
                "offer_id int NOT NULL, submitted_at datetime2 NOT NULL, submission_status varchar(20) NOT NULL, " +
                "created_at datetime2 NOT NULL CONSTRAINT DF_SUBMISSION_NOTIFICATIONS_created_at DEFAULT SYSDATETIME(), " +
                "CONSTRAINT FK_SUBMISSION_NOTIFICATIONS_STUDENTS FOREIGN KEY (student_id) REFERENCES dbo.STUDENTS(student_id) ON DELETE CASCADE, " +
                "CONSTRAINT FK_SUBMISSION_NOTIFICATIONS_ASSIGNMENTS FOREIGN KEY (assignment_id) REFERENCES dbo.ASSIGNMENTS(assignment_id) ON DELETE CASCADE" +
                "); END; " +
                "IF OBJECT_ID('dbo.SUBMISSION_NOTIFICATION_READS', 'U') IS NULL BEGIN " +
                "CREATE TABLE dbo.SUBMISSION_NOTIFICATION_READS (" +
                "user_id int NOT NULL, notification_id int NOT NULL, " +
                "read_at datetime2 NOT NULL CONSTRAINT DF_SUBMISSION_NOTIFICATION_READS_read_at DEFAULT SYSDATETIME(), " +
                "CONSTRAINT PK_SUBMISSION_NOTIFICATION_READS PRIMARY KEY (user_id, notification_id), " +
                "CONSTRAINT FK_SUBMISSION_NOTIFICATION_READS_USERS FOREIGN KEY (user_id) REFERENCES dbo.USERS(user_id) ON DELETE CASCADE, " +
                "CONSTRAINT FK_SUBMISSION_NOTIFICATION_READS_NOTIFICATIONS FOREIGN KEY (notification_id) REFERENCES dbo.SUBMISSION_NOTIFICATIONS(notification_id) ON DELETE CASCADE" +
                "); END;";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        public static void InsertSubmitted(
            SqlConnection conn, SqlTransaction transaction, int submissionId, DateTime submittedAt, string status)
        {
            const string sql =
                "INSERT INTO SUBMISSION_NOTIFICATIONS " +
                "(submission_id, student_id, assignment_id, offer_id, submitted_at, submission_status, created_at) " +
                "SELECT sub.submission_id, sub.student_id, sub.assignment_id, a.offer_id, @submittedAt, @status, SYSDATETIME() " +
                "FROM SUBMISSIONS sub JOIN ASSIGNMENTS a ON a.assignment_id = sub.assignment_id " +
                "WHERE sub.submission_id = @submissionId;";
            using (var cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.Add("@submissionId", SqlDbType.Int).Value = submissionId;
                cmd.Parameters.Add("@submittedAt", SqlDbType.DateTime2).Value = submittedAt;
                cmd.Parameters.Add("@status", SqlDbType.VarChar, 20).Value = status;
                if (cmd.ExecuteNonQuery() != 1)
                    throw new InvalidOperationException("The lecturer submission notification could not be created.");
            }
        }

        public static List<SubmissionNotificationItem> GetForUser(UserContext user)
        {
            var rows = new List<SubmissionNotificationItem>();
            if (user == null || !user.IsLecturer || user.UserId <= 0) return rows;
            EnsureTables();

            const string sql =
                "SELECT n.notification_id, n.submission_id, n.offer_id, n.submitted_at, n.submission_status, n.created_at, " +
                "co.academic_year, co.semester, c.course_code, c.course_name, a.title AS assessment_title, " +
                "s.student_id, s.student_name, CASE WHEN r.notification_id IS NULL THEN 0 ELSE 1 END AS is_read " +
                "FROM SUBMISSION_NOTIFICATIONS n " +
                "JOIN ASSIGNMENTS a ON a.assignment_id = n.assignment_id " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = n.offer_id " +
                "JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id AND l.user_id = @userId " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "JOIN STUDENTS s ON s.student_id = n.student_id " +
                "LEFT JOIN SUBMISSION_NOTIFICATION_READS r ON r.notification_id = n.notification_id AND r.user_id = @userId " +
                "ORDER BY n.created_at DESC, n.notification_id DESC";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = user.UserId;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(new SubmissionNotificationItem
                        {
                            NotificationId = IntValue(reader["notification_id"]),
                            SubmissionId = IntValue(reader["submission_id"]),
                            OfferingId = IntValue(reader["offer_id"]),
                            AcademicYear = Text(reader["academic_year"]),
                            Semester = Text(reader["semester"]),
                            CourseCode = Text(reader["course_code"]),
                            CourseName = Text(reader["course_name"]),
                            AssessmentTitle = Text(reader["assessment_title"]),
                            StudentId = Text(reader["student_id"]),
                            StudentName = Text(reader["student_name"]),
                            SubmissionStatus = Text(reader["submission_status"]),
                            SubmittedAt = DateValue(reader["submitted_at"]) ?? DateTime.Now,
                            CreatedAt = DateValue(reader["created_at"]) ?? DateTime.Now,
                            IsRead = Convert.ToBoolean(reader["is_read"])
                        });
                    }
                }
            }
            return rows;
        }

        public static bool MarkRead(UserContext user, int notificationId)
        {
            if (user == null || !user.IsLecturer || notificationId <= 0) return false;
            EnsureTables();
            const string sql =
                "IF EXISTS (SELECT 1 FROM SUBMISSION_NOTIFICATIONS n " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = n.offer_id " +
                "JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id " +
                "WHERE n.notification_id = @notificationId AND l.user_id = @userId) " +
                "AND NOT EXISTS (SELECT 1 FROM SUBMISSION_NOTIFICATION_READS " +
                "WHERE user_id = @userId AND notification_id = @notificationId) " +
                "INSERT INTO SUBMISSION_NOTIFICATION_READS (user_id, notification_id) VALUES (@userId, @notificationId);";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                AddParameters(cmd, user.UserId, notificationId);
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public static bool MarkUnread(UserContext user, int notificationId)
        {
            if (user == null || !user.IsLecturer || notificationId <= 0) return false;
            EnsureTables();
            const string sql =
                "DELETE r FROM SUBMISSION_NOTIFICATION_READS r " +
                "JOIN SUBMISSION_NOTIFICATIONS n ON n.notification_id = r.notification_id " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = n.offer_id " +
                "JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id " +
                "WHERE r.user_id = @userId AND r.notification_id = @notificationId AND l.user_id = @userId;";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                AddParameters(cmd, user.UserId, notificationId);
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public static void MarkAllRead(UserContext user, IEnumerable<int> notificationIds)
        {
            if (notificationIds == null) return;
            foreach (int id in notificationIds) MarkRead(user, id);
        }

        private static void AddParameters(SqlCommand cmd, int userId, int notificationId)
        {
            cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
            cmd.Parameters.Add("@notificationId", SqlDbType.Int).Value = notificationId;
        }
    }
}
