using System;
using System.Web;

namespace src.services
{
    /// <summary>
    /// Shared body for the ChangePassword page methods on the student, lecturer,
    /// and admin account pages. Resolves the signed-in user from the session and
    /// returns a plain object the front-end JSON-serializes as { ok, message }.
    /// </summary>
    public static class AccountWebMethods
    {
        public static object ChangePassword(string currentPassword, string newPassword)
        {
            var ctx = HttpContext.Current;
            if (ctx == null || ctx.Session == null || ctx.Session["user_id"] == null)
            {
                if (ctx != null) ctx.Response.StatusCode = 401;
                return new { ok = false, message = "Your session has expired. Please sign in again." };
            }

            int userId = Convert.ToInt32(ctx.Session["user_id"]);
            var result = AccountPasswordService.ChangePassword(userId, currentPassword, newPassword);
            return new { ok = result.Ok, message = result.Message };
        }

        public static object SaveProfile(string phone, string mailingAddress)
        {
            var ctx = HttpContext.Current;
            var user = ctx == null ? null : UserContextFactory.FromSession(ctx.Session);
            if (user == null)
            {
                if (ctx != null) ctx.Response.StatusCode = 401;
                return new { ok = false, message = "Your session has expired. Please sign in again." };
            }

            var result = AccountProfileService.SaveProfile(user, phone, mailingAddress);
            return new { ok = result.Ok, message = result.Message };
        }
    }
}
