using System;
using System.Data.SqlClient;
using src.db;

namespace src.services
{
    public static class AccountProfileService
    {
        public class SaveResult
        {
            public bool Ok;
            public string Message;
        }

        public static SaveResult SaveProfile(UserContext user, string phone, string mailingAddress)
        {
            if (user == null)
                return Fail("Your session has expired. Please sign in again.");

            if (!user.IsStudent && !user.IsLecturer)
                return Fail("Profile editing is not available for this account.");

            phone = Normalize(phone);
            mailingAddress = Normalize(mailingAddress);

            if (phone.Length > 50)
                return Fail("Phone must be 50 characters or fewer.");

            if (mailingAddress.Length > 255)
                return Fail("Mailing address must be 255 characters or fewer.");

            string sql = user.IsStudent
                ? "UPDATE STUDENTS SET phone = @phone, mailing_address = @address WHERE user_id = @userId"
                : "UPDATE LECTURERS SET phone = @phone, mailing_address = @address WHERE user_id = @userId";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@phone", DbValue(phone));
                cmd.Parameters.AddWithValue("@address", DbValue(mailingAddress));
                cmd.Parameters.AddWithValue("@userId", user.UserId);

                int rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                    return Fail("Account profile not found.");
            }

            return new SaveResult { Ok = true, Message = "Profile saved." };
        }

        private static string Normalize(string value)
        {
            return (value ?? "").Trim();
        }

        private static object DbValue(string value)
        {
            return string.IsNullOrEmpty(value) ? (object)DBNull.Value : value;
        }

        private static SaveResult Fail(string message)
        {
            return new SaveResult { Ok = false, Message = message };
        }
    }
}
