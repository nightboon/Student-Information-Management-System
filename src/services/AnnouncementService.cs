using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using src.db;

namespace src.services
{
    /// <summary>
    /// One announcement shown to a student.
    /// </summary>
    public class Announcement
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>One announcement on a course-detail page, with author display info.</summary>
    public class CourseAnnouncement
    {
        public string AuthorName { get; set; }
        public string AuthorRole { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPinned { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool HasAttachment { get; set; }
    }

    /// <summary>
    /// One notification row for the student notifications inbox. Carries the
    /// originating course and author so the page can render list and detail
    /// without a second lookup.
    /// </summary>
    public class StudentNotification
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPinned { get; set; }
        public bool HasAttachment { get; set; }
        public string FileUrl { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string AuthorName { get; set; }
        public string AuthorRole { get; set; }
        public bool IsRead { get; set; }
    }

    /// <summary>
    /// Read-only access to announcements targeting a student's current-semester
    /// enrolled course offerings. Returns an empty list when there are none. SQL
    /// exceptions are not caught here; they propagate to the caller.
    /// </summary>
    public static class AnnouncementService
    {
        // DISTINCT guards against an announcement targeting several of the
        // student's offerings producing duplicate rows. TOP 5 keeps the
        // dashboard widget concise; newest first.
        private const string SelectAnnouncements =
            "SELECT DISTINCT TOP 5 an.announcement_id, an.title, " +
            "CAST(an.content AS varchar(max)) AS content, an.created_at " +
            "FROM ANNOUNCEMENTS an " +
            "JOIN ANNOUNCEMENT_TARGETS at ON at.announcement_id = an.announcement_id " +
            "JOIN COURSE_OFFERINGS o ON at.offering_id = o.offering_id " +
            "JOIN SEMESTERS sem ON o.semester_id = sem.semester_id " +
            "JOIN ENROLMENTS e ON e.offering_id = o.offering_id " +
            "JOIN STUDENTS s ON e.student_id = s.student_id " +
            "WHERE s.user_id = @userId AND sem.is_current = 1 AND e.status = 'ENROLLED' " +
            "ORDER BY an.created_at DESC";

        public static List<Announcement> GetForStudent(int userId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectAnnouncements, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                var announcements = new List<Announcement>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        announcements.Add(new Announcement
                        {
                            AnnouncementId = (int)reader["announcement_id"],
                            Title = reader["title"].ToString(),
                            Content = reader["content"].ToString(),
                            CreatedAt = (DateTime)reader["created_at"]
                        });
                    }
                }
                return announcements;
            }
        }

        // Announcements targeting one offering, pinned first then newest first.
        // Author name comes from LECTURERS when the creator is a lecturer; role
        // from USERS. file_url presence drives the attachment flag.
        private const string SelectByOffering =
            "SELECT DISTINCT an.announcement_id, an.title, " +
            "CAST(an.content AS varchar(max)) AS content, an.created_at, an.is_pinned, " +
            "CASE WHEN an.file_url IS NULL THEN 0 ELSE 1 END AS has_attachment, an.file_url, " +
            "u.role, ISNULL(l.full_name, u.username) AS author_name " +
            "FROM ANNOUNCEMENTS an " +
            "JOIN ANNOUNCEMENT_TARGETS at ON at.announcement_id = an.announcement_id " +
            "JOIN USERS u ON u.user_id = an.created_by " +
            "LEFT JOIN LECTURERS l ON l.user_id = u.user_id " +
            "WHERE at.offering_id = @offeringId " +
            "ORDER BY an.is_pinned DESC, an.created_at DESC";

        public static List<CourseAnnouncement> GetByOffering(int offeringId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectByOffering, conn))
            {
                cmd.Parameters.AddWithValue("@offeringId", offeringId);
                var list = new List<CourseAnnouncement>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CourseAnnouncement
                        {
                            Title = reader["title"].ToString(),
                            Content = reader["content"].ToString(),
                            CreatedAt = (DateTime)reader["created_at"],
                            IsPinned = (bool)reader["is_pinned"],
                            HasAttachment = (int)reader["has_attachment"] == 1,
                            AuthorName = reader["author_name"].ToString(),
                            AuthorRole = reader["role"].ToString()
                        });
                    }
                }
                return list;
            }
        }

        // Every announcement targeting an offering the student is enrolled in,
        // across all semesters, pinned first then newest first. One announcement
        // can target several of the student's offerings; CROSS APPLY TOP 1 picks
        // a single course for display so the inbox shows one row per announcement.
        // Author name comes from LECTURERS when available, else the USERS name;
        // the role drives the notification category on the page.
        private const string SelectAllForStudent =
            "SELECT an.announcement_id, an.title, " +
            "CAST(an.content AS varchar(max)) AS content, an.created_at, an.is_pinned, " +
            "CASE WHEN an.file_url IS NULL THEN 0 ELSE 1 END AS has_attachment, an.file_url, " +
            "ISNULL(l.full_name, u.username) AS author_name, u.role, " +
            "ca.course_code, ca.course_name, " +
            "CASE WHEN nr.announcement_id IS NULL THEN 0 ELSE 1 END AS is_read " +
            "FROM ANNOUNCEMENTS an " +
            "JOIN USERS u ON u.user_id = an.created_by " +
            "LEFT JOIN LECTURERS l ON l.user_id = u.user_id " +
            "CROSS APPLY ( " +
            "  SELECT TOP 1 c.course_code, c.course_name " +
            "  FROM ANNOUNCEMENT_TARGETS at " +
            "  JOIN COURSE_OFFERINGS o ON at.offering_id = o.offering_id " +
            "  JOIN COURSES c ON o.course_id = c.course_id " +
            "  JOIN ENROLMENTS e ON e.offering_id = o.offering_id " +
            "  JOIN STUDENTS s ON e.student_id = s.student_id " +
            "  WHERE at.announcement_id = an.announcement_id " +
            "    AND s.user_id = @userId AND e.status = 'ENROLLED' " +
            "  ORDER BY c.course_code " +
            ") ca " +
            "LEFT JOIN NOTIFICATION_READS nr ON nr.announcement_id = an.announcement_id AND nr.user_id = @userId " +
            "ORDER BY an.is_pinned DESC, an.created_at DESC";

        private const string VisibleNotificationIdsForStudent =
            "SELECT DISTINCT an.announcement_id " +
            "FROM ANNOUNCEMENTS an " +
            "JOIN ANNOUNCEMENT_TARGETS at ON at.announcement_id = an.announcement_id " +
            "JOIN COURSE_OFFERINGS o ON at.offering_id = o.offering_id " +
            "JOIN ENROLMENTS e ON e.offering_id = o.offering_id " +
            "JOIN STUDENTS s ON e.student_id = s.student_id " +
            "WHERE s.user_id = @userId AND e.status = 'ENROLLED'";

        // Lecturer inbox: announcements they authored plus admin-authored
        // system notices. OUTER APPLY keeps system notices visible even when
        // they are not tied to a course offering.
        private const string SelectAllForLecturer =
            "SELECT an.announcement_id, an.title, " +
            "CAST(an.content AS varchar(max)) AS content, an.created_at, an.is_pinned, " +
            "CASE WHEN an.file_url IS NULL THEN 0 ELSE 1 END AS has_attachment, an.file_url, " +
            "ISNULL(l.full_name, u.username) AS author_name, u.role, " +
            "ISNULL(ca.course_code, '') AS course_code, ISNULL(ca.course_name, '') AS course_name, " +
            "CASE WHEN nr.announcement_id IS NULL THEN 0 ELSE 1 END AS is_read " +
            "FROM ANNOUNCEMENTS an " +
            "JOIN USERS u ON u.user_id = an.created_by " +
            "LEFT JOIN LECTURERS l ON l.user_id = u.user_id " +
            "OUTER APPLY ( " +
            "  SELECT TOP 1 c.course_code, c.course_name " +
            "  FROM ANNOUNCEMENT_TARGETS at " +
            "  JOIN COURSE_OFFERINGS o ON at.offering_id = o.offering_id " +
            "  JOIN COURSES c ON o.course_id = c.course_id " +
            "  WHERE at.announcement_id = an.announcement_id " +
            "  ORDER BY c.course_code " +
            ") ca " +
            "LEFT JOIN NOTIFICATION_READS nr ON nr.announcement_id = an.announcement_id AND nr.user_id = @userId " +
            "WHERE an.created_by = @userId OR u.role = 'ADMIN' " +
            "ORDER BY an.is_pinned DESC, an.created_at DESC";

        private const string VisibleNotificationIdsForLecturer =
            "SELECT DISTINCT an.announcement_id " +
            "FROM ANNOUNCEMENTS an " +
            "JOIN USERS u ON u.user_id = an.created_by " +
            "WHERE an.created_by = @userId OR u.role = 'ADMIN'";

        public static List<StudentNotification> GetAllForStudent(int userId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectAllForStudent, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                var list = new List<StudentNotification>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new StudentNotification
                        {
                            AnnouncementId = (int)reader["announcement_id"],
                            Title = reader["title"].ToString(),
                            Content = reader["content"].ToString(),
                            CreatedAt = (DateTime)reader["created_at"],
                            IsPinned = (bool)reader["is_pinned"],
                            HasAttachment = (int)reader["has_attachment"] == 1,
                            FileUrl = reader["file_url"] == DBNull.Value ? "" : reader["file_url"].ToString(),
                            AuthorName = reader["author_name"].ToString(),
                            AuthorRole = reader["role"].ToString(),
                            CourseCode = reader["course_code"].ToString(),
                            CourseName = reader["course_name"].ToString(),
                            IsRead = (int)reader["is_read"] == 1
                        });
                    }
                }
                return list;
            }
        }

        public static List<StudentNotification> GetAllForLecturer(int userId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectAllForLecturer, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                var list = new List<StudentNotification>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new StudentNotification
                        {
                            AnnouncementId = (int)reader["announcement_id"],
                            Title = reader["title"].ToString(),
                            Content = reader["content"].ToString(),
                            CreatedAt = (DateTime)reader["created_at"],
                            IsPinned = (bool)reader["is_pinned"],
                            HasAttachment = (int)reader["has_attachment"] == 1,
                            FileUrl = reader["file_url"] == DBNull.Value ? "" : reader["file_url"].ToString(),
                            AuthorName = reader["author_name"].ToString(),
                            AuthorRole = reader["role"].ToString(),
                            CourseCode = reader["course_code"].ToString(),
                            CourseName = reader["course_name"].ToString(),
                            IsRead = (int)reader["is_read"] == 1
                        });
                    }
                }
                return list;
            }
        }

        public static int GetUnreadCountForStudent(int userId)
        {
            return GetUnreadCount(userId, VisibleNotificationIdsForStudent);
        }

        public static int GetUnreadCountForLecturer(int userId)
        {
            return GetUnreadCount(userId, VisibleNotificationIdsForLecturer);
        }

        private static int GetUnreadCount(int userId, string visibleSql)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM (" + visibleSql + ") visible " +
                "LEFT JOIN NOTIFICATION_READS nr ON nr.announcement_id = visible.announcement_id AND nr.user_id = @userId " +
                "WHERE nr.announcement_id IS NULL";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                return (int)cmd.ExecuteScalar();
            }
        }

        public static void MarkRead(int userId, int announcementId)
        {
            MarkRead(userId, announcementId, VisibleNotificationIdsForStudent);
        }

        public static void MarkReadForLecturer(int userId, int announcementId)
        {
            MarkRead(userId, announcementId, VisibleNotificationIdsForLecturer);
        }

        private static void MarkRead(int userId, int announcementId, string visibleSql)
        {
            string sql =
                "IF EXISTS (SELECT 1 FROM (" + visibleSql + ") visible WHERE visible.announcement_id = @announcementId) " +
                "BEGIN " +
                "  IF EXISTS (SELECT 1 FROM NOTIFICATION_READS WHERE user_id = @userId AND announcement_id = @announcementId) " +
                "    UPDATE NOTIFICATION_READS SET read_at = SYSUTCDATETIME() WHERE user_id = @userId AND announcement_id = @announcementId; " +
                "  ELSE " +
                "    INSERT INTO NOTIFICATION_READS (user_id, announcement_id, read_at) VALUES (@userId, @announcementId, SYSUTCDATETIME()); " +
                "END";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@announcementId", announcementId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void MarkUnread(int userId, int announcementId)
        {
            const string sql =
                "DELETE FROM NOTIFICATION_READS " +
                "WHERE user_id = @userId AND announcement_id = @announcementId";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@announcementId", announcementId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void MarkAllRead(int userId)
        {
            MarkAllRead(userId, VisibleNotificationIdsForStudent);
        }

        public static void MarkAllReadForLecturer(int userId)
        {
            MarkAllRead(userId, VisibleNotificationIdsForLecturer);
        }

        private static void MarkAllRead(int userId, string visibleSql)
        {
            string sql =
                "INSERT INTO NOTIFICATION_READS (user_id, announcement_id, read_at) " +
                "SELECT @userId, visible.announcement_id, SYSUTCDATETIME() " +
                "FROM (" + visibleSql + ") visible " +
                "WHERE NOT EXISTS ( " +
                "  SELECT 1 FROM NOTIFICATION_READS nr " +
                "  WHERE nr.user_id = @userId AND nr.announcement_id = visible.announcement_id " +
                ")";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
