using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using src.services;

namespace src.controls
{
    public partial class sidebar : UserControl
    {
        private int _unreadNotificationCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] == null) return;

            var user = UserContextFactory.FromSession(Session);
            var readIds = Session["student_notification_read_ids"] as ISet<int> ?? new HashSet<int>();
            _unreadNotificationCount = StudentPortalService.GetNotifications(user, readIds).Count(n => !n.IsRead);
        }

        protected int UnreadNotificationCount
        {
            get { return _unreadNotificationCount; }
        }

        protected string NotificationBadgeText
        {
            get { return _unreadNotificationCount > 9 ? "9+" : _unreadNotificationCount.ToString(); }
        }

        protected string NotificationBadgeVisibilityClass
        {
            get { return _unreadNotificationCount > 0 ? "" : " hidden"; }
        }
    }
}
