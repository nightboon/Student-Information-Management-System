using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Services;
using src.services.admin;

namespace src.admin
{
    public partial class academic_calendar : src.security.AdminPage
    {
        private readonly AdminPortalService service = new AdminPortalService();

        protected int EventCount { get; private set; }
        protected string EventRowsHtml { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var events = service.GetCalendarEvents();
            EventCount = events.Count;
            EventRowsHtml = BuildRows(events);
        }

        private static string BuildRows(IEnumerable<AdminCalendarEventRow> events)
        {
            var html = new StringBuilder();
            foreach (var item in events)
            {
                html.Append("<tr data-row data-session-id=\"").Append(Attr(item.SessionId)).Append("\" data-academic-year=\"").Append(Attr(item.AcademicYear)).Append("\" data-semester-name=\"").Append(Attr(item.SemesterName)).Append("\" data-start-date=\"").Append(item.SessionStartDate.ToString("yyyy-MM-dd")).Append("\" data-end-date=\"").Append(item.SessionEndDate.ToString("yyyy-MM-dd")).Append("\" data-status=\"").Append(Attr(item.Status)).Append("\" data-search=\"").Append(Attr((item.Title + " " + item.Type).ToLowerInvariant())).Append("\" data-sem=\"").Append(Attr(item.Semester)).Append("\" data-type=\"").Append(Attr(item.Type)).Append("\" class=\"border-b border-slate-100 hover:bg-slate-50/60\">");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"text-slate-900 font-medium\">").Append(Html(item.Title)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(item.Type)).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(item.StartDate.ToString("dd MMM yyyy")).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(item.EndDate.ToString("dd MMM yyyy")).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(item.Semester)).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 ").Append(StatusBadge(item.Status)).Append("\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(item.Status)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-right\" style=\"font-size:12.5px\"><div class=\"flex items-center justify-end gap-1\"><button type=\"button\" data-calendar-edit data-modal-open=\"event-modal\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50\"><i data-lucide=\"pencil\" class=\"h-3.5 w-3.5\"></i></button><button type=\"button\" data-calendar-delete data-session-id=\"").Append(Attr(item.SessionId)).Append("\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-[#e0162b] hover:bg-[#e0162b]/5\"><i data-lucide=\"trash-2\" class=\"h-3.5 w-3.5\"></i></button></div></td></tr>");
            }
            html.Append("<tr data-table-empty style=\"display:none\"><td colspan=\"7\" class=\"px-6 py-12 text-center text-slate-400\" style=\"font-size:13px\">No events match your filters.</td></tr>");
            return html.ToString();
        }

        [WebMethod(EnableSession = true)]
        public static object SaveAcademicSession(AdminAcademicSessionSaveRequest request)
        {
            EnsureAdmin();
            new AdminPortalService().SaveAcademicSession(request);
            return new { ok = true };
        }

        [WebMethod(EnableSession = true)]
        public static object DeleteAcademicSession(string sessionId)
        {
            EnsureAdmin();
            new AdminPortalService().DeleteAcademicSession(sessionId);
            return new { ok = true };
        }

        private static void EnsureAdmin()
        {
            var ctx = HttpContext.Current;
            if (ctx == null || ctx.Session == null || ctx.Session["user_id"] == null ||
                !string.Equals(ctx.Session["role"] as string, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpException(403, "Admin session required.");
            }
        }

        private static string StatusBadge(string status)
        {
            if (status == "Completed") return "bg-slate-100 text-slate-600 border-slate-200";
            if (status == "Active") return "bg-emerald-50 text-emerald-700 border-emerald-100";
            return "bg-amber-50 text-amber-700 border-amber-100";
        }

        private static string Html(string value) { return HttpUtility.HtmlEncode(value ?? ""); }
        private static string Attr(string value) { return HttpUtility.HtmlAttributeEncode(value ?? ""); }
    }
}
