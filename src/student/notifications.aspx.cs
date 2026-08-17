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
            ImportSessionReadIds(user);
            Notifications = StudentPortalService.GetNotifications(user, NotificationReadService.GetReadIds(user));

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

        protected string Category(object dataItem)
        {
            var notification = dataItem as StudentPortalNotification;
            if (notification == null) return "ANNOUNCEMENT";
            if (string.Equals(notification.NotificationType, GradeNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase))
                return "GRADE";
            if (string.Equals(notification.NotificationType, ExtensionNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase))
                return "EXTENSION";
            return string.Equals(notification.AuthorRole, "ADMIN", StringComparison.OrdinalIgnoreCase)
                ? "SYSTEM" : "ANNOUNCEMENT";
        }

        protected string CourseLabel(StudentPortalNotification n)
        {
            if (n == null) return "";
            if (string.Equals(n.NotificationType, AdminNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase))
                return "Registrar";
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

        protected string AttachmentUrl(object fileUrl)
        {
            var url = fileUrl as string;
            return string.IsNullOrEmpty(url) ? "" : ResolveUrl(url);
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

        private void ImportSessionReadIds(UserContext user)
        {
            var existing = Session[ReadNotificationIdsKey] as IEnumerable<int>;
            if (existing == null) return;
            NotificationReadService.Import(user, existing);
            Session.Remove(ReadNotificationIdsKey);
        }

        private static string BadgeText(int unreadCount)
        {
            return unreadCount > 9 ? "9+" : unreadCount.ToString();
        }

        private static object CountResponse(UserContext user)
        {
            var readIds = NotificationReadService.GetReadIds(user);
            int unreadCount = StudentPortalService.GetNotifications(user, readIds).Count(n => !n.IsRead);
            return new
            {
                ok = true,
                unreadCount = unreadCount,
                badgeText = BadgeText(unreadCount)
            };
        }

        private static void SetReadState(UserContext user, string notificationType, int notificationId, bool read)
        {
            if (string.Equals(notificationType, ExtensionNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase))
            {
                if (read) ExtensionNotificationService.MarkRead(user, notificationId);
                else ExtensionNotificationService.MarkUnread(user, notificationId);
                return;
            }

            if (string.Equals(notificationType, GradeNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase))
            {
                if (read) GradeNotificationService.MarkRead(user, notificationId);
                else GradeNotificationService.MarkUnread(user, notificationId);
                return;
            }

            if (string.Equals(notificationType, AdminNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase))
            {
                if (read) AdminNotificationService.MarkRead(user, notificationId);
                else AdminNotificationService.MarkUnread(user, notificationId);
                return;
            }

            if (read) NotificationReadService.MarkRead(user, notificationId);
            else NotificationReadService.MarkUnread(user, notificationId);
        }

        [WebMethod(EnableSession = true)]
        public static object MarkRead(string notificationType, int notificationId)
        {
            var user = CurrentUserOrReject();
            if (user == null) return new { ok = false };

            SetReadState(user, notificationType, notificationId, true);
            return CountResponse(user);
        }

        [WebMethod(EnableSession = true)]
        public static object MarkUnread(string notificationType, int notificationId)
        {
            var user = CurrentUserOrReject();
            if (user == null) return new { ok = false };

            SetReadState(user, notificationType, notificationId, false);
            return CountResponse(user);
        }

        [WebMethod(EnableSession = true)]
        public static object MarkAllRead()
        {
            var user = CurrentUserOrReject();
            if (user == null) return new { ok = false };

            var readIds = NotificationReadService.GetReadIds(user);
            var notifications = StudentPortalService.GetNotifications(user, readIds);
            NotificationReadService.MarkAllRead(
                user,
                notifications
                    .Where(n => !string.Equals(n.NotificationType, AdminNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase) &&
                                !string.Equals(n.NotificationType, GradeNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase) &&
                                !string.Equals(n.NotificationType, ExtensionNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase))
                    .Select(n => n.NotificationId));
            AdminNotificationService.MarkAllRead(
                user,
                notifications
                    .Where(n => string.Equals(n.NotificationType, AdminNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase))
                    .Select(n => n.NotificationId));
            GradeNotificationService.MarkAllRead(
                user,
                notifications
                    .Where(n => string.Equals(n.NotificationType, GradeNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase))
                    .Select(n => n.NotificationId));
            ExtensionNotificationService.MarkAllRead(
                user,
                notifications
                    .Where(n => string.Equals(n.NotificationType, ExtensionNotificationService.NotificationType, StringComparison.OrdinalIgnoreCase))
                    .Select(n => n.NotificationId));
            return CountResponse(user);
        }
    }
}
