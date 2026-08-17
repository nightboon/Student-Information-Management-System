using System;
using System.Text;
using System.Web;
using System.Web.Services;
using src.services;

namespace src.admin
{
    public partial class admin_notifications : src.security.AdminPage
    {
        protected string NotificationRowsHtml { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            NotificationRowsHtml = BuildRows();
        }

        private static string BuildRows()
        {
            var html = new StringBuilder();
            foreach (var n in AdminNotificationService.GetAll())
            {
                var audience = AdminNotificationService.AudienceLabel(n.TargetRole);
                var search = (n.Title + " " + n.Message + " " + audience + " " + n.CreatedByName).ToLowerInvariant();

                html.Append("<tr data-row data-id=\"").Append(n.NotificationId)
                    .Append("\" data-title=\"").Append(Attr(n.Title))
                    .Append("\" data-message=\"").Append(Attr(n.Message))
                    .Append("\" data-audience=\"").Append(Attr(n.TargetRole))
                    .Append("\" data-audience-label=\"").Append(Attr(audience))
                    .Append("\" data-author=\"").Append(Attr(n.CreatedByName))
                    .Append("\" data-created=\"").Append(Attr(FullTime(n.CreatedAt)))
                    .Append("\" data-search=\"").Append(Attr(search))
                    .Append("\" class=\"border-b border-slate-100 hover:bg-slate-50/60\">");

                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><button type=\"button\" data-admin-view-notification data-drawer-open=\"notification-drawer\" class=\"text-left text-slate-900 font-medium hover:text-[#a01020]\">")
                    .Append(Html(n.Title)).Append("</button></td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center rounded-full border px-2 py-0.5 ")
                    .Append(AudienceBadge(n.TargetRole))
                    .Append("\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(audience)).Append("</span></td>");
                html.Append("<td class=\"max-w-xl px-6 py-3 text-slate-600\" style=\"font-size:12.5px;line-height:1.5\"><span class=\"line-clamp-2\">")
                    .Append(Html(Summary(n.Message))).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">")
                    .Append(Html(n.CreatedByName)).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-500\" style=\"font-size:12.5px\">")
                    .Append(Html(ListTime(n.CreatedAt))).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-right\"><div class=\"flex items-center justify-end gap-1\">")
                    .Append("<button type=\"button\" data-admin-view-notification data-drawer-open=\"notification-drawer\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50\" aria-label=\"View notification\"><i data-lucide=\"eye\" class=\"h-3.5 w-3.5\"></i></button>")
                    .Append("<button type=\"button\" data-admin-edit-notification data-modal-open=\"notification-modal\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50\" aria-label=\"Edit notification\"><i data-lucide=\"pencil\" class=\"h-3.5 w-3.5\"></i></button>")
                    .Append("</div></td></tr>");
            }

            html.Append("<tr data-table-empty style=\"display:none\"><td colspan=\"6\" class=\"px-6 py-12 text-center text-slate-400\" style=\"font-size:13px\">No notifications match your filters.</td></tr>");
            return html.ToString();
        }

        [WebMethod(EnableSession = true)]
        public static object SaveNotification(AdminNotificationInput request)
        {
            try
            {
                var user = CurrentAdmin();
                var id = AdminNotificationService.Save(user, request);
                return new { ok = true, notificationId = id };
            }
            catch (Exception ex)
            {
                return new { ok = false, message = ex.Message };
            }
        }

        private static UserContext CurrentAdmin()
        {
            var ctx = HttpContext.Current;
            if (ctx == null || ctx.Session == null || ctx.Session["user_id"] == null ||
                !string.Equals(ctx.Session["role"] as string, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpException(403, "Admin session required.");
            }
            return UserContextFactory.FromSession(ctx.Session);
        }

        private static string AudienceBadge(string targetRole)
        {
            if (string.Equals(targetRole, "STUDENT", StringComparison.OrdinalIgnoreCase))
                return "bg-emerald-50 text-emerald-700 border-emerald-100";
            if (string.Equals(targetRole, "LECTURER", StringComparison.OrdinalIgnoreCase))
                return "bg-sky-50 text-sky-700 border-sky-100";
            return "bg-slate-100 text-slate-600 border-slate-200";
        }

        private static string Summary(string value)
        {
            var text = (value ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
            while (text.Contains("  ")) text = text.Replace("  ", " ");
            return text.Length <= 130 ? text : text.Substring(0, 127) + "...";
        }

        private static string ListTime(DateTime dt)
        {
            DateTime now = DateTime.Now;
            if (dt.Date == now.Date) return dt.ToString("h:mm tt");
            if (dt.Date == now.Date.AddDays(-1)) return "Yesterday";
            return dt.Year == now.Year ? dt.ToString("d MMM") : dt.ToString("d MMM yyyy");
        }

        private static string FullTime(DateTime dt)
        {
            return dt.ToString("d MMM yyyy - HH:mm");
        }

        private static string Html(string value) { return HttpUtility.HtmlEncode(value ?? ""); }
        private static string Attr(string value)
        {
            var normalized = (value ?? "").Replace("\r\n", "\n").Replace("\r", "\n");
            return HttpUtility.HtmlAttributeEncode(normalized).Replace("\n", "&#10;");
        }
    }
}
