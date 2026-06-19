using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using src.db;
using src.security;
using src.services;

namespace src.login
{
    public partial class login : System.Web.UI.Page
    {
        protected HiddenField ShowPwCard;
        protected HiddenField StoredEmail;
        protected HiddenField ServerEmailError;
        protected HiddenField ServerPwError;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(System.DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();

            if (!IsPostBack)
            {
                ShowPwCard.Value = "";
                StoredEmail.Value = "";
                ServerEmailError.Value = "";
                ServerPwError.Value = "";
            }

            if (Session["user_id"] != null)
            {
                Response.Redirect(src.security.RoleRoutes.HomePageFor(Session["role"] as string));
                return;
            }

            var rememberedUser = CookieService.GetRememberedUser(HttpContext.Current);
            if (rememberedUser == null) return;

            Session["user_id"] = rememberedUser.UserId;
            Session["role"] = rememberedUser.Role;
            Response.Redirect(src.security.RoleRoutes.HomePageFor(rememberedUser.Role));
        }

        protected void EmailSubmit_Click(object sender, System.EventArgs e)
        {
            ServerEmailError.Value = "";
            ServerPwError.Value = "";

            string email = (Request.Form["email"] ?? "").Trim();

            if (email.Length == 0)
            {
                ServerEmailError.Value = "Please enter your email.";
                return;
            }

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand("SELECT 1 FROM USERS WHERE email = @email", conn))
            {
                cmd.Parameters.AddWithValue("@email", email);
                if (cmd.ExecuteScalar() != null)
                {
                    ShowPwCard.Value = "true";
                    StoredEmail.Value = email;
                    return;
                }
            }

            ServerEmailError.Value = "No account found for this email.";
        }

        protected void PwSubmit_Click(object sender, System.EventArgs e)
        {
            ServerEmailError.Value = "";
            ServerPwError.Value = "";

            string email    = (StoredEmail.Value ?? "").Trim();
            string password = Request.Form["password"] ?? "";
            bool   remember = Request.Form["remember"] == "on";

            if (email.Length == 0 || password.Length == 0)
            {
                ShowPwCard.Value = "true";
                ServerPwError.Value = "Missing credentials.";
                return;
            }

            var inputHash = Sha256Hex(password);
            int userId = 0;
            string role = null;
            bool authenticated = false;

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand("SELECT user_id, password_hash, role, username FROM USERS WHERE email = @email", conn))
            {
                cmd.Parameters.AddWithValue("@email", email);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var DbHash = reader["password_hash"].ToString();
                        if (string.Equals(DbHash, inputHash, System.StringComparison.OrdinalIgnoreCase))
                        {
                            userId = (int)reader["user_id"];
                            role = reader["role"].ToString();
                            Session["user_id"] = userId;
                            Session["role"] = role;
                            Session["username"] = reader["username"].ToString();
                            authenticated = true;
                        }
                    }
                }
            }

            if (authenticated)
            {
                if (remember) CookieService.IssueRememberMeCookie(userId);
                Response.Redirect(src.security.RoleRoutes.HomePageFor(role));
                return;
            }

            ShowPwCard.Value = "true";
            StoredEmail.Value = email;
            ServerPwError.Value = "Incorrect password.";
        }

        [WebMethod(EnableSession = true)]
        public static object Logout()
        {
            var ctx = HttpContext.Current;
            CookieService.RevokeRememberMeCookie(ctx);
            ctx.Session.Abandon();
            return new { ok = true };
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
