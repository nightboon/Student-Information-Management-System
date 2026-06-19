using System;
using System.Web;
using System.Web.SessionState;

namespace src.services
{
    public static class UserContextFactory
    {
        public static UserContext FromCurrent()
        {
            var context = HttpContext.Current;
            return context == null ? null : FromSession(context.Session);
        }

        public static UserContext FromSession(HttpSessionState session)
        {
            if (session == null || session["user_id"] == null || session["role"] == null)
            {
                return null;
            }

            int userId;
            if (!int.TryParse(session["user_id"].ToString(), out userId))
            {
                return null;
            }

            return new UserContext
            {
                UserId = userId,
                Role = session["role"].ToString()
            };
        }
    }
}
