using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using src.services;

namespace src.shared
{
    public partial class notification : src.security.StudentPage
    {
        private const string ReadNotificationIdsKey = "student_notification_read_ids";

        protected List<StudentPortalNotification> Notifications = new List<StudentPortalNotification>();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (Session["user_id"] == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }

            var user = UserContextFactory.FromSession(Session);
            Notifications = StudentPortalService.GetNotifications(user, ReadIds(Session));

            notificationsRepeater.DataSource = Notifications;
            notificationsRepeater.DataBind();
            emptyPanel.Visible = Notifications.Count == 0;
        }

        protected int UnreadCount
        {
            get { return Notifications.Count(n => !n.IsRead); }
        }

        protected string ReadFlag(object isRead)
        {
            return ((bool)isRead) ? "true" : "false";
        }

        protected string Category(object role)
        {
            return string.Equals(role as string, "ADMIN", StringComparison.OrdinalIgnoreCase)
                ? "SYSTEM"
                : "ANNOUNCEMENT";
        }

        protected string CourseLabel(StudentPortalNotification n)
        {
            if (n == null) return "";
            return n.CourseCode + " - " + n.CourseName;
        }

        protected string ListTime(DateTime dt)
        {
            DateTime now = DateTime.Now;
            if (dt.Date == now.Date) return dt.ToString("h:mm tt");
            if (dt.Date == now.Date.AddDays(-1)) return "Yesterday";
            if (dt.Year == now.Year) return dt.ToString("d MMM");
            return dt.ToString("d MMM yyyy");
        }

        protected string FullTime(DateTime dt)
        {
            return dt.ToString("d MMM yyyy - HH:mm");
        }

        protected string PinnedFlag(object isPinned)
        {
            return ((bool)isPinned) ? "true" : "false";
        }

        private static UserContext CurrentUserOrReject()
        {
            HttpContext context = HttpContext.Current;
            if (context == null || context.Session == null || context.Session["user_id"] == null)
            {
                if (context != null) context.Response.StatusCode = 401;
                return null;
            }

            var user = UserContextFactory.FromSession(context.Session);
            if (user == null)
            {
                context.Response.StatusCode = 401;
            }
            return user;
        }

        private static HashSet<int> ReadIds(HttpSessionState session)
        {
            var existing = session[ReadNotificationIdsKey] as HashSet<int>;
            if (existing != null) return existing;

            existing = new HashSet<int>();
            session[ReadNotificationIdsKey] = existing;
            return existing;
        }

        private static string BadgeText(int unreadCount)
        {
            return unreadCount > 9 ? "9+" : unreadCount.ToString();
        }

        private static object CountResponse(UserContext user, HashSet<int> readIds)
        {
            int unreadCount = StudentPortalService.GetNotifications(user, readIds).Count(n => !n.IsRead);
            return new
            {
                ok = true,
                unreadCount = unreadCount,
                badgeText = BadgeText(unreadCount)
            };
        }

        [WebMethod(EnableSession = true)]
        public static object MarkRead(int announcementId)
        {
            var user = CurrentUserOrReject();
            if (user == null) return new { ok = false };

            var readIds = ReadIds(HttpContext.Current.Session);
            readIds.Add(announcementId);
            return CountResponse(user, readIds);
        }

        [WebMethod(EnableSession = true)]
        public static object MarkUnread(int announcementId)
        {
            var user = CurrentUserOrReject();
            if (user == null) return new { ok = false };

            var readIds = ReadIds(HttpContext.Current.Session);
            readIds.Remove(announcementId);
            return CountResponse(user, readIds);
        }

        [WebMethod(EnableSession = true)]
        public static object MarkAllRead()
        {
            var user = CurrentUserOrReject();
            if (user == null) return new { ok = false };

            var readIds = ReadIds(HttpContext.Current.Session);
            foreach (var notification in StudentPortalService.GetNotifications(user, readIds))
            {
                readIds.Add(notification.AnnouncementId);
            }
            return CountResponse(user, readIds);
        }
    }
}
