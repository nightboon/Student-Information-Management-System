using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using src.db;

namespace src.services
{
    /// <summary>
    /// Updates contact fields and passwords on the USERS table.
    /// </summary>
    public static class UserAccountService
    {
        public static bool UpdateContactInfo(int userId, string phone, string mailingAddress)
        {
            const string sql =
                "UPDATE USERS SET phone = @phone, mailing_address = @address WHERE user_id = @userId";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone.Trim());
                cmd.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(mailingAddress) ? (object)DBNull.Value : mailingAddress.Trim());
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static string ChangePassword(int userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
                return "Please fill in all password fields.";

            if (newPassword.Length < 8)
                return "New password must be at least 8 characters.";

            if (string.Equals(currentPassword, newPassword, StringComparison.Ordinal))
                return "New password must differ from the current one.";

            string currentHash = null;
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand("SELECT password_hash FROM USERS WHERE user_id = @userId", conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                currentHash = cmd.ExecuteScalar() as string;
            }

            if (currentHash == null)
                return "Account not found.";

            if (!string.Equals(currentHash, Sha256Hex(currentPassword), StringComparison.OrdinalIgnoreCase))
                return "Current password is incorrect.";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand("UPDATE USERS SET password_hash = @hash WHERE user_id = @userId", conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@hash", Sha256Hex(newPassword));
                cmd.ExecuteNonQuery();
            }

            return null;
        }

        private static string Sha256Hex(string value)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
