using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using src.db;
using static src.services.ServiceMap;

namespace src.services
{
    public static class LecturerMaterialReader
    {
        public static List<LecturerMaterialRow> GetMaterials(UserContext user, int? offeringId)
        {
            var rows = new List<LecturerMaterialRow>();
            if (user == null || !user.IsLecturer) return rows;

            string sql =
                "SELECT mat.material_id, md.offer_id, mat.title, mat.file_url, mat.uploaded_at, " +
                "c.course_code, c.course_name " +
                "FROM MATERIALS mat " +
                "JOIN MODULES md ON md.module_id = mat.module_id " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = md.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "WHERE " + ServiceAccess.VisibleOfferScope("co") + " " +
                "AND (@offerId = 0 OR md.offer_id = @offerId) " +
                "ORDER BY mat.uploaded_at DESC, mat.material_id DESC";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                cmd.Parameters.AddWithValue("@offerId", offeringId.GetValueOrDefault());
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(new LecturerMaterialRow
                        {
                            MaterialId = IntValue(reader["material_id"]),
                            OfferingId = IntValue(reader["offer_id"]),
                            CourseCode = Text(reader["course_code"]),
                            CourseName = Text(reader["course_name"]),
                            Title = Text(reader["title"]),
                            FileUrl = Text(reader["file_url"]),
                            UploadedAt = DateValue(reader["uploaded_at"]) ?? DateTime.Now,
                            Description = "",
                            FileType = "",
                            MaterialType = "",
                            DueDate = null,
                            Weight = null
                        });
                    }
                }
            }
            return rows;
        }

        // Inserts into the offering's first module. Returns the new material id, or 0.
        public static int Add(UserContext user, LecturerMaterialInput input)
        {
            if (user == null || input == null || input.OfferingId <= 0) return 0;

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageOffer(conn, user, input.OfferingId)) return 0;

                string moduleId;
                using (var cmd = new SqlCommand(
                    "SELECT TOP 1 module_id FROM MODULES WHERE offer_id = @offerId ORDER BY module_id", conn))
                {
                    cmd.Parameters.AddWithValue("@offerId", input.OfferingId);
                    var value = cmd.ExecuteScalar();
                    if (value == null || value == DBNull.Value) return 0;
                    moduleId = value.ToString();
                }

                const string sql =
                    "INSERT INTO MATERIALS (module_id, title, file_url, uploaded_at) " +
                    "OUTPUT INSERTED.material_id " +
                    "VALUES (@moduleId, @title, @fileUrl, @uploadedAt)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@moduleId", moduleId);
                    cmd.Parameters.AddWithValue("@title", ServiceAccess.DbValue(input.Title));
                    cmd.Parameters.AddWithValue("@fileUrl", ServiceAccess.DbValue(input.FileUrl));
                    cmd.Parameters.AddWithValue("@uploadedAt", input.UploadedAt == default(DateTime) ? DateTime.Now : input.UploadedAt);
                    var id = cmd.ExecuteScalar();
                    return id == null || id == DBNull.Value ? 0 : Convert.ToInt32(id);
                }
            }
        }

        public static bool Delete(UserContext user, int materialId)
        {
            if (user == null) return false;
            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageMaterial(conn, user, materialId)) return false;
                using (var cmd = new SqlCommand("DELETE FROM MATERIALS WHERE material_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", materialId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
