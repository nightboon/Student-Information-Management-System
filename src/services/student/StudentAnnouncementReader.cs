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

            var sql =
                "SELECT an.announcement_id, an.offer_id, an.title, CAST(an.message AS varchar(max)) AS content, an.created_at, " +
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
                        list.Add(new StudentCourseAnnouncement
                        {
                            AnnouncementId = IntValue(reader["announcement_id"]),
                            OfferId = IntValue(reader["offer_id"]),
                            CourseCode = Text(reader["course_code"]),
                            CourseName = Text(reader["course_name"]),
                            Title = Text(reader["title"]),
                            Content = Text(reader["content"]),
                            AuthorName = LecturerOrFallback(Text(reader["lecturer_name"])),
                            AuthorRole = "LECTURER",
                            IsPinned = false,
                            HasAttachment = false,
                            CreatedAt = DateValue(reader["created_at"]) ?? DateTime.MinValue
                        });
                    }
                }
            }
            return list;
        }

        public static List<StudentPortalNotification> GetNotifications(UserContext user, ISet<int> readIds)
        {
            var announcements = GetAnnouncements(user, null);
            return announcements.Select(a => new StudentPortalNotification
            {
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
                CreatedAt = a.CreatedAt,
                IsRead = readIds != null && readIds.Contains(a.AnnouncementId)
            }).ToList();
        }
    }
}
