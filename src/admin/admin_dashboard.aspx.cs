using System;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            Dashboard = service.GetDashboard();
            ProgrammeEnrolmentHtml = BuildProgrammeEnrolment(Dashboard);
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
    }
}
