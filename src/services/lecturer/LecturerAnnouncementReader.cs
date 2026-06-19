using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using src.db;
using static src.services.ServiceMap;

namespace src.services
{
    public static class LecturerAnnouncementReader
    {
        // Announcements across the lecturer's offerings, newest first.
        // offeringId filters to a single offering when provided.
        public static List<LecturerAnnouncementRow> GetAnnouncements(UserContext user, int? offeringId)
        {
            var rows = new List<LecturerAnnouncementRow>();
            if (user == null || !user.IsLecturer) return rows;

            string sql =
                "SELECT an.announcement_id, an.offer_id, an.title, an.message, an.created_at, " +
                "c.course_code + ' - ' + c.course_name AS target_courses " +
                "FROM ANNOUNCEMENTS an " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = an.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "WHERE " + ServiceAccess.VisibleOfferScope("co") + " " +
                "AND (@offerId = 0 OR an.offer_id = @offerId) " +
                "ORDER BY an.created_at DESC";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                cmd.Parameters.AddWithValue("@offerId", offeringId.GetValueOrDefault());
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(new LecturerAnnouncementRow
                        {
                            AnnouncementId = IntValue(reader["announcement_id"]),
                            Title = Text(reader["title"]),
                            Content = Text(reader["message"]),
                            FileUrl = "",
                            CreatedAt = DateValue(reader["created_at"]) ?? DateTime.Now,
                            IsPinned = false,
                            TargetCourses = Text(reader["target_courses"])
                        });
                    }
                }
            }
            return rows;
        }

        public static int Add(UserContext user, LecturerAnnouncementInput input)
        {
            if (user == null || input == null || input.OfferId <= 0) return 0;

            const string sql =
                "INSERT INTO ANNOUNCEMENTS (offer_id, title, message, created_at) " +
                "OUTPUT INSERTED.announcement_id " +
                "VALUES (@offerId, @title, @message, @createdAt)";

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageOffer(conn, user, input.OfferId)) return 0;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@offerId", input.OfferId);
                    cmd.Parameters.AddWithValue("@title", ServiceAccess.DbValue(input.Title));
                    cmd.Parameters.AddWithValue("@message", ServiceAccess.DbValue(input.Message));
                    cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);
                    var id = cmd.ExecuteScalar();
                    return id == null || id == DBNull.Value ? 0 : Convert.ToInt32(id);
                }
            }
        }

        public static bool Delete(UserContext user, int announcementId)
        {
            if (user == null) return false;
            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageAnnouncement(conn, user, announcementId)) return false;
                using (var cmd = new SqlCommand("DELETE FROM ANNOUNCEMENTS WHERE announcement_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", announcementId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
