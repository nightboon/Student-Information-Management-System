using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using src.services.admin;

namespace src.admin
{
    public partial class admin_dashboard : src.security.AdminPage
    {
        private readonly AdminPortalService service = new AdminPortalService();

        protected AdminDashboardData Dashboard { get; private set; }
        protected string ProgrammeEnrolmentHtml { get; private set; }
        protected string PendingActionsHtml { get; private set; }
        protected string AdminNoticesHtml { get; private set; }

        protected string Greeting
        {
            get
            {
                int hour = DateTime.Now.Hour;
                if (hour >= 5 && hour < 12) return "Good Morning";
                if (hour >= 12 && hour < 17) return "Good Afternoon";
                if (hour >= 17 && hour < 21) return "Good Evening";
                return "Good Night";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Dashboard = service.GetDashboard();
            ProgrammeEnrolmentHtml = BuildProgrammeEnrolment(Dashboard);
            PendingActionsHtml = BuildPendingActions(Dashboard);
            AdminNoticesHtml = BuildAdminNotices(Dashboard);
        }

        protected string Number(int value)
        {
            return value.ToString("N0");
        }

        protected string Percent(decimal value)
        {
            return value.ToString("0.0") + "%";
        }

        protected string DecimalNumber(decimal value)
        {
            return value.ToString("0.00");
        }

        private static string BuildProgrammeEnrolment(AdminDashboardData dashboard)
        {
            var rows = dashboard.Programmes ?? new System.Collections.Generic.List<ProgrammeEnrolment>();
            var max = 1;
            foreach (var row in rows)
            {
                if (row.Students > max) max = row.Students;
            }

            var html = new StringBuilder();
            foreach (var row in rows)
            {
                var width = row.Students <= 0 ? 0 : (int)Math.Round(row.Students * 100m / max);
                html.Append("<div class=\"flex items-center gap-3\">");
                html.Append("<div class=\"w-12 text-slate-500\" style=\"font-size:11.5px;font-weight:600\">").Append(HttpUtility.HtmlEncode(row.Code)).Append("</div>");
                html.Append("<div class=\"flex-1\">");
                html.Append("<div class=\"flex items-center justify-between\"><div class=\"text-slate-900\" style=\"font-size:13px;font-weight:500\">").Append(HttpUtility.HtmlEncode(row.Name)).Append("</div><div class=\"text-slate-500\" style=\"font-size:12.5px\">").Append(row.Students.ToString("N0")).Append(" students</div></div>");
                html.Append("<div class=\"mt-1.5 h-2 w-full overflow-hidden rounded-full bg-slate-100\"><div class=\"h-full rounded-full bg-[#e0162b]\" style=\"width:").Append(width).Append("%\"></div></div>");
                html.Append("</div></div>");
            }

            if (html.Length == 0)
            {
                html.Append("<div class=\"text-slate-400\" style=\"font-size:13px\">No programme enrolment data found.</div>");
            }

            return html.ToString();
        }

        private static string BuildPendingActions(AdminDashboardData dashboard)
        {
            var html = new StringBuilder();

            if (dashboard.PendingRequests > 0)
                html.Append(PendingActionItem("bg-amber-50 text-amber-700", "inbox",
                    dashboard.PendingRequests + (dashboard.PendingRequests == 1 ? " add/drop request" : " add/drop requests") + " pending",
                    "Submitted and awaiting review"));

            if (dashboard.AtRiskStudents > 0)
                html.Append(PendingActionItem("bg-[#e0162b]/10 text-[#e0162b]", "alert-triangle",
                    dashboard.AtRiskStudents + (dashboard.AtRiskStudents == 1 ? " at-risk student" : " at-risk students"),
                    "Academic support needed"));

            if (dashboard.CriticalAtRiskStudents > 0)
                html.Append(PendingActionItem("bg-[#e0162b]/10 text-[#e0162b]", "alert-triangle",
                    dashboard.CriticalAtRiskStudents + (dashboard.CriticalAtRiskStudents == 1 ? " critical student" : " critical students"),
                    "CGPA below 2.00 — intervention required"));

            foreach (var notice in dashboard.RecentNotices ?? new List<AdminDashboardNotice>())
            {
                var daysLeft = (notice.Date.Date - DateTime.Today).Days;
                if (daysLeft >= 0 && daysLeft <= 14)
                {
                    var when = daysLeft == 0 ? "Today" : daysLeft == 1 ? "Tomorrow" : "In " + daysLeft + " days";
                    html.Append(PendingActionItem("bg-amber-50 text-amber-700", "calendar",
                        notice.Title, when));
                }
            }

            if (html.Length == 0)
                html.Append("<li class=\"px-3 py-4 text-slate-400\" style=\"font-size:13px\">No pending actions at this time.</li>");

            return html.ToString();
        }

        private static string PendingActionItem(string iconClass, string icon, string title, string subtitle)
        {
            return "<li class=\"flex items-start gap-3 rounded-xl px-3 py-3 hover:bg-slate-50 transition-colors\">" +
                   "<span class=\"mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg " + iconClass + "\"><i data-lucide=\"" + icon + "\" class=\"h-4 w-4\"></i></span>" +
                   "<div class=\"min-w-0 flex-1\">" +
                   "<div class=\"text-slate-900 truncate\" style=\"font-size:13px;font-weight:600\">" + HttpUtility.HtmlEncode(title) + "</div>" +
                   "<div class=\"text-slate-500 truncate\" style=\"font-size:12px\">" + HttpUtility.HtmlEncode(subtitle) + "</div>" +
                   "</div></li>";
        }

        private static string BuildAdminNotices(AdminDashboardData dashboard)
        {
            var notices = dashboard.RecentNotices ?? new List<AdminDashboardNotice>();
            if (notices.Count == 0)
                return "<li class=\"text-slate-400\" style=\"font-size:13px\">No recent notices.</li>";

            var html = new StringBuilder();
            foreach (var notice in notices)
            {
                html.Append("<li class=\"border-b border-slate-100 pb-4 last:border-b-0 last:pb-0\">");
                html.Append("<div class=\"flex items-center justify-between gap-2\">");
                html.Append("<div class=\"text-slate-900\" style=\"font-size:13px;font-weight:600\">").Append(HttpUtility.HtmlEncode(notice.Title)).Append("</div>");
                html.Append("<div class=\"text-slate-400 shrink-0\" style=\"font-size:11.5px\">").Append(notice.Date.ToString("MMM d, yyyy")).Append("</div>");
                html.Append("</div>");
                html.Append("<div class=\"mt-1 text-slate-500\" style=\"font-size:12.5px;line-height:1.55\">").Append(HttpUtility.HtmlEncode(notice.Description)).Append("</div>");
                html.Append("</li>");
            }
            return html.ToString();
        }
    }
}
