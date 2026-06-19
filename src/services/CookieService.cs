using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using src.db;

namespace src.services
{
    public class CookieUser
    {
        public int UserId { get; set; }
        public string Role { get; set; }
    }

    public static class CookieService
    {
        private const string AuthCookieName = "auth_token";
        private const int RememberMeDays = 30;

        public static CookieUser GetRememberedUser(HttpContext ctx)
        {
            var rawToken = GetRequestToken(ctx);
            if (string.IsNullOrEmpty(rawToken)) return null;

            try
            {
                var tokenHash = Sha256Hex(rawToken);
                CookieUser user = null;
                System.DateTime expiresAt = System.DateTime.MinValue;

                using (var conn = Db.OpenConnection())
                using (var cmd = new SqlCommand(
                    "SELECT u.user_id, u.role, t.expires_at " +
                    "FROM USER_COOKIES t JOIN USERS u ON t.user_id = u.user_id " +
                    "WHERE t.cookie_hash = @hash", conn))
                {
                    cmd.Parameters.AddWithValue("@hash", tokenHash);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new CookieUser
                            {
                                UserId = (int)reader["user_id"],
                                Role = reader["role"].ToString()
                            };
                            expiresAt = (System.DateTime)reader["expires_at"];
                        }
                    }
                }

                if (user == null)
                {
                    ExpireAuthCookie(ctx);
                    return null;
                }

                if (expiresAt <= System.DateTime.UtcNow)
                {
                    DeleteToken(tokenHash);
                    ExpireAuthCookie(ctx);
                    return null;
                }

                return user;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Remember-me auto-login failed: " + ex.Message);
                return null;
            }
        }

        public static void IssueRememberMeCookie(int userId)
        {
            try
            {
                var rawToken = GenerateRawToken();
                var tokenHash = Sha256Hex(rawToken);
                var expiresAt = System.DateTime.UtcNow.AddDays(RememberMeDays);

                using (var conn = Db.OpenConnection())
                using (var cmd = new SqlCommand(
                    "INSERT INTO USER_COOKIES (user_id, cookie_hash, expires_at) VALUES (@uid, @hash, @exp)", conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@hash", tokenHash);
                    cmd.Parameters.AddWithValue("@exp", expiresAt);
                    cmd.ExecuteNonQuery();
                }

                var ctx = HttpContext.Current;
                if (ctx == null) return;

                var cookie = new HttpCookie(AuthCookieName, rawToken)
                {
                    HttpOnly = true,
                    Secure = ctx.Request.IsSecureConnection,
                    Expires = System.DateTime.Now.AddDays(RememberMeDays),
                    Path = "/"
                };
                ctx.Response.Cookies.Add(cookie);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("IssueRememberMeCookie failed: " + ex.Message);
            }
        }

        public static void RevokeRememberMeCookie(HttpContext ctx)
        {
            var rawToken = GetRequestToken(ctx);

            if (!string.IsNullOrEmpty(rawToken))
            {
                try
                {
                    DeleteToken(Sha256Hex(rawToken));
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine("Logout token cleanup failed: " + ex.Message);
                }
            }

            ExpireAuthCookie(ctx);
        }

        private static string GetRequestToken(HttpContext ctx)
        {
            if (ctx == null) return null;

            var cookie = ctx.Request.Cookies[AuthCookieName];
            return cookie == null ? null : cookie.Value;
        }

        private static void DeleteToken(string tokenHash)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand("DELETE FROM USER_COOKIES WHERE cookie_hash = @hash", conn))
            {
                cmd.Parameters.AddWithValue("@hash", tokenHash);
                cmd.ExecuteNonQuery();
            }
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

        private static string GenerateRawToken()
        {
            var bytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return System.Convert.ToBase64String(bytes);
        }

        private static void ExpireAuthCookie(HttpContext ctx)
        {
            if (ctx == null) return;

            var expired = new HttpCookie(AuthCookieName, "")
            {
                HttpOnly = true,
                Expires = System.DateTime.Now.AddDays(-1),
                Path = "/"
            };
            ctx.Response.Cookies.Add(expired);
        }
    }
}
