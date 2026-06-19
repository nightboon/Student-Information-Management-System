using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using src.services.admin;

namespace src.admin
{
    public partial class add_drop_requests : src.security.AdminPage
    {
        private readonly AdminPortalService service = new AdminPortalService();

        protected int PendingCount { get; private set; }
        protected int ApprovedCount { get; private set; }
        protected int RejectedCount { get; private set; }
        protected int TotalCount { get; private set; }
        protected string RequestRowsHtml { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var rows = service.GetAddDropRequests();
            PendingCount = rows.Count(r => IsPending(r.Status));
            ApprovedCount = rows.Count(r => IsApproved(r.Status));
            RejectedCount = rows.Count(r => IsRejected(r.Status));
            TotalCount = rows.Count;
            RequestRowsHtml = BuildRows(rows);
        }

        protected string Number(int value) { return value.ToString("N0"); }

        private static string BuildRows(IEnumerable<AdminAddDropRequestRow> rows)
        {
            var html = new StringBuilder();
            foreach (var r in rows)
            {
                var status = DisplayStatus(r.Status);
                var type = r.Type == "Drop" ? "Drop" : "Add";
                html.Append("<tr data-row data-search=\"").Append(Attr((r.RequestId + " " + r.StudentName + " " + r.StudentId + " " + r.CourseCode).ToLowerInvariant())).Append("\" data-programme=\"").Append(Attr(r.Programme)).Append("\" data-type=\"").Append(type).Append("\" data-status=\"").Append(status).Append("\" class=\"border-b border-slate-100 hover:bg-slate-50/60\">");
                html.Append("<td class=\"px-6 py-3 text-slate-500\" style=\"font-size:12.5px\"><span class=\"font-medium\">REQ-").Append(r.RequestId.ToString("0000")).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><div class=\"text-slate-900 font-medium\">").Append(Html(r.StudentName)).Append("</div><div class=\"text-slate-500\" style=\"font-size:11.5px\">").Append(Html(r.StudentId)).Append("</div></td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(r.Programme)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-center\" style=\"font-size:12.5px\">").Append(r.Semester).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><div class=\"text-slate-900 font-medium\">").Append(Html(r.CourseCode)).Append("</div><div class=\"text-slate-500\" style=\"font-size:11.5px\">").Append(Html(r.CourseName)).Append("</div></td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 ").Append(type == "Add" ? "bg-emerald-50 text-emerald-700 border-emerald-100" : "bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20").Append("\" style=\"font-size:11.5px;font-weight:600\">").Append(type).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 max-w-xs\" style=\"font-size:12.5px\"><div class=\"truncate text-slate-600\" title=\"").Append(Attr(r.Reason)).Append("\">").Append(Html(string.IsNullOrWhiteSpace(r.Reason) ? "Course registration request" : r.Reason)).Append("</div></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-500\" style=\"font-size:12.5px\">-</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 ").Append(StatusBadge(status)).Append("\" style=\"font-size:11.5px;font-weight:600\">").Append(status).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-right\" style=\"font-size:12.5px\">");
                if (status == "Pending")
                {
                    html.Append("<div class=\"flex items-center justify-end gap-1\"><button type=\"button\" data-request-action data-request-id=\"").Append(r.RequestId).Append("\" data-next-status=\"ENROLLED\" class=\"inline-flex h-7 items-center rounded-md bg-emerald-50 border border-emerald-200 px-2 text-emerald-700 hover:bg-emerald-100\" style=\"font-size:11.5px;font-weight:600\">Approve</button><button type=\"button\" data-request-action data-request-id=\"").Append(r.RequestId).Append("\" data-next-status=\"REJECTED\" class=\"inline-flex h-7 items-center rounded-md bg-[#e0162b]/10 border border-[#e0162b]/20 px-2 text-[#a01020] hover:bg-[#e0162b]/15\" style=\"font-size:11.5px;font-weight:600\">Reject</button></div>");
                }
                else
                {
                    html.Append("<span class=\"text-slate-400\" style=\"font-size:12px\">Resolved</span>");
                }
                html.Append("</td></tr>");
            }
            html.Append("<tr data-table-empty style=\"display:none\"><td colspan=\"10\" class=\"py-10 text-center text-slate-400\" style=\"font-size:13px\">No requests match your filters.</td></tr>");
            return html.ToString();
        }

        [WebMethod(EnableSession = true)]
        public static object SetRequestStatus(int enrollmentId, string status)
        {
            EnsureAdmin();
            new AdminPortalService().SetEnrollmentStatus(enrollmentId, status);
            return new { ok = true };
        }

        private static bool IsPending(string status) { return DisplayStatus(status) == "Pending"; }
        private static bool IsApproved(string status) { return DisplayStatus(status) == "Approved"; }
        private static bool IsRejected(string status) { return DisplayStatus(status) == "Rejected"; }
        private static string DisplayStatus(string status)
        {
            var s = (status ?? "").Trim().ToUpperInvariant();
            if (s == "ENROLLED" || s == "APPROVED") return "Approved";
            if (s == "REJECTED") return "Rejected";
            return "Pending";
        }

        private static string StatusBadge(string status)
        {
            if (status == "Approved") return "bg-emerald-50 text-emerald-700 border-emerald-100";
            if (status == "Rejected") return "bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20";
            return "bg-amber-50 text-amber-700 border-amber-100";
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

        private static string Html(string value) { return HttpUtility.HtmlEncode(value ?? ""); }
        private static string Attr(string value) { return HttpUtility.HtmlAttributeEncode(value ?? ""); }
    }
}
