using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using src.db;
using static src.services.ServiceMap;

namespace src.services
{
    public static class ExtensionNotificationService
    {
        public const string NotificationType = "EXTENSION";

        public static void EnsureTables()
        {
            const string sql =
                "IF OBJECT_ID('dbo.EXTENSION_NOTIFICATIONS', 'U') IS NULL BEGIN " +
                "CREATE TABLE dbo.EXTENSION_NOTIFICATIONS (" +
                "notification_id int IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
                "student_id varchar(20) NOT NULL, assignment_id int NOT NULL, offer_id int NOT NULL, " +
                "extension_deadline datetime2 NOT NULL, created_at datetime2 NOT NULL " +
                "CONSTRAINT DF_EXTENSION_NOTIFICATIONS_created_at DEFAULT SYSDATETIME(), " +
                "CONSTRAINT FK_EXTENSION_NOTIFICATIONS_STUDENTS FOREIGN KEY (student_id) REFERENCES dbo.STUDENTS(student_id) ON DELETE CASCADE, " +
                "CONSTRAINT FK_EXTENSION_NOTIFICATIONS_ASSIGNMENTS FOREIGN KEY (assignment_id) REFERENCES dbo.ASSIGNMENTS(assignment_id) ON DELETE CASCADE, " +
                "CONSTRAINT UQ_EXTENSION_NOTIFICATIONS_submission UNIQUE (student_id, assignment_id)" +
                "); END; " +
                "IF OBJECT_ID('dbo.EXTENSION_NOTIFICATION_READS', 'U') IS NULL BEGIN " +
                "CREATE TABLE dbo.EXTENSION_NOTIFICATION_READS (" +
                "user_id int NOT NULL, notification_id int NOT NULL, " +
                "read_at datetime2 NOT NULL CONSTRAINT DF_EXTENSION_NOTIFICATION_READS_read_at DEFAULT SYSDATETIME(), " +
                "CONSTRAINT PK_EXTENSION_NOTIFICATION_READS PRIMARY KEY (user_id, notification_id), " +
                "CONSTRAINT FK_EXTENSION_NOTIFICATION_READS_USERS FOREIGN KEY (user_id) REFERENCES dbo.USERS(user_id) ON DELETE CASCADE, " +
                "CONSTRAINT FK_EXTENSION_NOTIFICATION_READS_NOTIFICATIONS FOREIGN KEY (notification_id) REFERENCES dbo.EXTENSION_NOTIFICATIONS(notification_id) ON DELETE CASCADE" +
                "); END;";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        public static void InsertGrantedExtension(
            SqlConnection conn, SqlTransaction transaction, int submissionId, DateTime deadline)
        {
            const string sql =
                "INSERT INTO EXTENSION_NOTIFICATIONS " +
                "(student_id, assignment_id, offer_id, extension_deadline, created_at) " +
                "SELECT sub.student_id, sub.assignment_id, a.offer_id, @deadline, SYSDATETIME() " +
                "FROM SUBMISSIONS sub JOIN ASSIGNMENTS a ON a.assignment_id = sub.assignment_id " +
                "WHERE sub.submission_id = @submissionId;";
            using (var cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.Add("@submissionId", SqlDbType.Int).Value = submissionId;
                cmd.Parameters.Add("@deadline", SqlDbType.DateTime2).Value = deadline;
                cmd.ExecuteNonQuery();
            }
        }

        public static List<StudentPortalNotification> GetForUser(UserContext user)
        {
            var rows = new List<StudentPortalNotification>();
            if (user == null || !user.IsStudent || user.UserId <= 0) return rows;
            EnsureTables();

            const string sql =
                "SELECT n.notification_id, n.offer_id, n.extension_deadline, n.created_at, " +
                "a.title AS assessment_title, c.course_code, c.course_name, " +
                "ISNULL(l.lecturer_name, 'Lecturer') AS lecturer_name, " +
                "CASE WHEN r.notification_id IS NULL THEN 0 ELSE 1 END AS is_read " +
                "FROM EXTENSION_NOTIFICATIONS n " +
                "JOIN STUDENTS s ON s.student_id = n.student_id AND s.user_id = @userId " +
                "JOIN ASSIGNMENTS a ON a.assignment_id = n.assignment_id " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = n.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "LEFT JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id " +
                "LEFT JOIN EXTENSION_NOTIFICATION_READS r ON r.notification_id = n.notification_id AND r.user_id = @userId " +
                "ORDER BY n.created_at DESC, n.notification_id DESC";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = user.UserId;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = IntValue(reader["notification_id"]);
                        string assessment = Text(reader["assessment_title"]);
                        DateTime deadline = DateValue(reader["extension_deadline"]) ?? DateTime.Now;
                        rows.Add(new StudentPortalNotification
                        {
                            NotificationId = id,
                            NotificationType = NotificationType,
                            AnnouncementId = id,
                            OfferId = IntValue(reader["offer_id"]),
                            CourseCode = Text(reader["course_code"]),
                            CourseName = Text(reader["course_name"]),
                            Title = "Late submission extension granted: " + assessment,
                            Content = "You may submit " + assessment + " until " +
                                deadline.ToString("d MMM yyyy, HH:mm") +
                                ". Your submission will be marked as late.",
                            AuthorName = Text(reader["lecturer_name"]),
                            AuthorRole = "EXTENSION",
                            IsPinned = false,
                            HasAttachment = false,
                            FileUrl = "",
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
            if (user == null || !user.IsStudent || notificationId <= 0) return false;
            EnsureTables();
            const string sql =
                "IF EXISTS (SELECT 1 FROM EXTENSION_NOTIFICATIONS n JOIN STUDENTS s ON s.student_id = n.student_id " +
                "WHERE n.notification_id = @notificationId AND s.user_id = @userId) " +
                "AND NOT EXISTS (SELECT 1 FROM EXTENSION_NOTIFICATION_READS " +
                "WHERE user_id = @userId AND notification_id = @notificationId) " +
                "INSERT INTO EXTENSION_NOTIFICATION_READS (user_id, notification_id) VALUES (@userId, @notificationId);";
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
            if (user == null || !user.IsStudent || notificationId <= 0) return false;
            EnsureTables();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(
                "DELETE r FROM EXTENSION_NOTIFICATION_READS r " +
                "JOIN EXTENSION_NOTIFICATIONS n ON n.notification_id = r.notification_id " +
                "JOIN STUDENTS s ON s.student_id = n.student_id " +
                "WHERE r.user_id = @userId AND r.notification_id = @notificationId AND s.user_id = @userId", conn))
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
