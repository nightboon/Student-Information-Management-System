using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using src.security;
using src.services;

namespace src.shared
{
    public partial class notification : SecurePage
    {
        protected List<StudentNotification> Notifications = new List<StudentNotification>();

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (string.Equals(CurrentRole, RoleRoutes.Lecturer, StringComparison.OrdinalIgnoreCase))
            {
                MasterPageFile = "~/shared/LecturerLayout.master";
            }
        }

        protected override void EnforceRole()
        {
            if (string.Equals(CurrentRole, RoleRoutes.Student, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(CurrentRole, RoleRoutes.Lecturer, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Response.Redirect(RoleRoutes.HomePageFor(CurrentRole));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (Session["user_id"] == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            int userId = (int)Session["user_id"];
            Notifications = IsLecturer
                ? AnnouncementService.GetAllForLecturer(userId)
                : AnnouncementService.GetAllForStudent(userId);

            notificationsRepeater.DataSource = Notifications;
            notificationsRepeater.DataBind();
            emptyPanel.Visible = Notifications.Count == 0;
        }

        private bool IsLecturer
        {
            get { return string.Equals(CurrentRole, RoleRoutes.Lecturer, StringComparison.OrdinalIgnoreCase); }
        }

        protected int UnreadCount
        {
            get { return Notifications.Count(n => !n.IsRead); }
        }

        protected string NotificationSummaryText
        {
            get
            {
                return IsLecturer
                    ? "Announcements you sent and registrar system updates."
                    : "Updates from your courses, lecturers, and the registrar.";
            }
        }

        protected string ReadFlag(object isRead)
        {
            return ((bool)isRead) ? "true" : "false";
        }

        // ADMIN-authored posts read as system notices; everything else is a
        // course announcement. Used for the badge label and colour.
        protected string Category(object role)
        {
            return string.Equals(role as string, "ADMIN", StringComparison.OrdinalIgnoreCase)
                ? "SYSTEM"
                : "ANNOUNCEMENT";
        }

        protected string CourseLabel(StudentNotification n)
        {
            if (string.Equals(n.AuthorRole, RoleRoutes.Admin, StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrEmpty(n.CourseCode))
            {
                return "System notification";
            }

            if (string.IsNullOrEmpty(n.CourseCode)) return "General";

            return n.CourseCode + " · " + n.CourseName;
        }

        // Compact stamp for the list column.
        protected string ListTime(DateTime dt)
        {
            DateTime now = DateTime.Now;
            if (dt.Date == now.Date) return dt.ToString("h:mm tt");
            if (dt.Date == now.Date.AddDays(-1)) return "Yesterday";
            if (dt.Year == now.Year) return dt.ToString("d MMM");
            return dt.ToString("d MMM yyyy");
        }

        // Full stamp for the detail header.
        protected string FullTime(DateTime dt)
        {
            return dt.ToString("d MMM yyyy · HH:mm");
        }

        protected string PinnedFlag(object isPinned)
        {
            return ((bool)isPinned) ? "true" : "false";
        }

        protected string AttachmentUrl(StudentNotification n)
        {
            return n != null && n.HasAttachment && !string.IsNullOrWhiteSpace(n.FileUrl)
                ? ResolveUrl(n.FileUrl)
                : "";
        }

        protected string AttachmentName(StudentNotification n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.FileUrl)) return "";
            return Path.GetFileName(n.FileUrl);
        }

        private static int CurrentUserIdOrReject()
        {
            HttpContext context = HttpContext.Current;
            if (context == null || context.Session == null || context.Session["user_id"] == null)
            {
                if (context != null) context.Response.StatusCode = 401;
                return 0;
            }

            return (int)context.Session["user_id"];
        }

        private static bool CurrentUserIsLecturer()
        {
            HttpContext context = HttpContext.Current;
            return context != null &&
                   context.Session != null &&
                   string.Equals(context.Session["role"] as string, RoleRoutes.Lecturer, StringComparison.OrdinalIgnoreCase);
        }

        private static string BadgeText(int unreadCount)
        {
            return unreadCount > 9 ? "9+" : unreadCount.ToString();
        }

        private static object CountResponse(int userId)
        {
            int unreadCount = CurrentUserIsLecturer()
                ? AnnouncementService.GetUnreadCountForLecturer(userId)
                : AnnouncementService.GetUnreadCountForStudent(userId);
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
            int userId = CurrentUserIdOrReject();
            if (userId == 0) return new { ok = false };

            if (CurrentUserIsLecturer())
                AnnouncementService.MarkReadForLecturer(userId, announcementId);
            else
                AnnouncementService.MarkRead(userId, announcementId);

            return CountResponse(userId);
        }

        [WebMethod(EnableSession = true)]
        public static object MarkUnread(int announcementId)
        {
            int userId = CurrentUserIdOrReject();
            if (userId == 0) return new { ok = false };

            AnnouncementService.MarkUnread(userId, announcementId);
            return CountResponse(userId);
        }

        [WebMethod(EnableSession = true)]
        public static object MarkAllRead()
        {
            int userId = CurrentUserIdOrReject();
            if (userId == 0) return new { ok = false };

            if (CurrentUserIsLecturer())
                AnnouncementService.MarkAllReadForLecturer(userId);
            else
                AnnouncementService.MarkAllRead(userId);

            return CountResponse(userId);
        }
    }
}
