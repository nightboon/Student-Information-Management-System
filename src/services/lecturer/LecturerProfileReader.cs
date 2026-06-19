using System;
using System.Data.SqlClient;
using src.db;
using static src.services.ServiceMap;

namespace src.services
{
    public static class LecturerProfileReader
    {
        public static LecturerProfile GetProfile(UserContext user)
        {
            if (user == null || !user.IsLecturer) return null;

            const string sql =
                "SELECT TOP 1 lecturer_id, lecturer_name, lecturer_email, phone, " +
                "mailing_address, department_id, icon " +
                "FROM LECTURERS WHERE user_id = @userId";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", user.UserId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new LecturerProfile
                    {
                        LecturerId = Text(reader["lecturer_id"]),
                        FullName = Text(reader["lecturer_name"]),
                        Email = Text(reader["lecturer_email"]),
                        Phone = Text(reader["phone"]),
                        MailingAddress = Text(reader["mailing_address"]),
                        DepartmentId = Text(reader["department_id"]),
                        IconPath = Text(reader["icon"])
                    };
                }
            }
        }
    }
}
