using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class StudentAnnouncementReader
    {
        public static List<StudentCourseAnnouncement> GetAnnouncements(UserContext user, int? offeringId)
        {
            if (user == null) return new List<StudentCourseAnnouncement>();

            EnsureAnnouncementColumns();

            var sql =
                "SELECT an.announcement_id, an.offer_id, an.title, CAST(an.message AS varchar(max)) AS content, an.created_at, " +
                "ISNULL(an.file_url, '') AS file_url, ISNULL(an.is_pinned, 0) AS is_pinned, " +
                "c.course_code, c.course_name, ISNULL(l.lecturer_name, '') AS lecturer_name " +
                "FROM ANNOUNCEMENTS an " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = an.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "LEFT JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id " +
                "WHERE " + ServiceAccess.VisibleOfferScope("co") + " ";

            if (offeringId.HasValue) sql += "AND an.offer_id = @offerId ";
            sql += "ORDER BY an.created_at DESC, an.announcement_id DESC";

            var list = new List<StudentCourseAnnouncement>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                if (offeringId.HasValue) cmd.Parameters.AddWithValue("@offerId", offeringId.Value);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var fileUrl = Text(reader["file_url"]);
                        list.Add(new StudentCourseAnnouncement
                        {
                            NotificationId = IntValue(reader["announcement_id"]),
                            NotificationType = "ANNOUNCEMENT",
                            AnnouncementId = IntValue(reader["announcement_id"]),
                            OfferId = IntValue(reader["offer_id"]),
                            CourseCode = Text(reader["course_code"]),
                            CourseName = Text(reader["course_name"]),
                            Title = Text(reader["title"]),
                            Content = Text(reader["content"]),
                            AuthorName = LecturerOrFallback(Text(reader["lecturer_name"])),
                            AuthorRole = "LECTURER",
                            IsPinned = Convert.ToBoolean(reader["is_pinned"]),
                            HasAttachment = !string.IsNullOrEmpty(fileUrl),
                            FileUrl = fileUrl,
                            CreatedAt = DateValue(reader["created_at"]) ?? DateTime.MinValue
                        });
                    }
                }
            }
            return list;
        }

        private static void EnsureAnnouncementColumns()
        {
            const string sql =
                "IF COL_LENGTH('ANNOUNCEMENTS', 'file_url') IS NULL " +
                "ALTER TABLE ANNOUNCEMENTS ADD file_url varchar(255) NULL; " +
                "IF COL_LENGTH('ANNOUNCEMENTS', 'is_pinned') IS NULL " +
                "ALTER TABLE ANNOUNCEMENTS ADD is_pinned bit NOT NULL CONSTRAINT DF_ANNOUNCEMENTS_is_pinned DEFAULT(0)";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static List<StudentPortalNotification> GetNotifications(UserContext user, ISet<int> readIds)
        {
            var announcements = GetAnnouncements(user, null);
            var notifications = announcements.Select(a => new StudentPortalNotification
            {
                NotificationId = a.NotificationId,
                NotificationType = a.NotificationType,
                AnnouncementId = a.AnnouncementId,
                OfferId = a.OfferId,
                CourseCode = a.CourseCode,
                CourseName = a.CourseName,
                Title = a.Title,
                Content = a.Content,
                AuthorName = a.AuthorName,
                AuthorRole = a.AuthorRole,
                IsPinned = a.IsPinned,
                HasAttachment = a.HasAttachment,
                FileUrl = a.FileUrl,
                CreatedAt = a.CreatedAt,
                IsRead = readIds != null && readIds.Contains(a.AnnouncementId)
            }).ToList();

            var adminReadIds = AdminNotificationService.GetReadIds(user);
            notifications.AddRange(AdminNotificationService.GetForUser(user, adminReadIds)
                .Select(AdminNotificationService.ToStudentNotification)
                .Where(n => n != null));
            notifications.AddRange(GradeNotificationService.GetForUser(user));
            notifications.AddRange(ExtensionNotificationService.GetForUser(user));

            return notifications
                .OrderByDescending(n => n.CreatedAt)
                .ThenByDescending(n => n.NotificationId)
                .ToList();
        }
    }
}
