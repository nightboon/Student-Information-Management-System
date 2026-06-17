using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using src.services;

namespace src.admin
{
    public partial class academic_performance : src.security.AdminPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSendWarnings_Click(object sender, EventArgs e)
        {
            var r = AtRiskWarningService.Run();
            pnlResult.Visible = true;

            string css, msg;
            if (!r.Ran)
            {
                css = "mt-4 rounded-lg border px-4 py-3 border-amber-200 bg-amber-50 text-amber-800";
                msg = "Could not run: at least two ended semesters are required to assess "
                    + "two consecutive semesters of low GPA.";
            }
            else
            {
                css = r.Failed > 0
                    ? "mt-4 rounded-lg border px-4 py-3 border-amber-200 bg-amber-50 text-amber-800"
                    : "mt-4 rounded-lg border px-4 py-3 border-emerald-200 bg-emerald-50 text-emerald-800";
                msg = "<span style=\"font-weight:600\">At-risk check complete</span> "
                    + "(" + Encode(r.Sem1) + " &amp; " + Encode(r.Sem2) + "). "
                    + "Checked " + r.Checked + " student(s) with grades in both semesters &middot; "
                    + r.AtRisk + " at risk &middot; "
                    + r.Emailed + " emailed &middot; "
                    + r.Skipped + " already warned &middot; "
                    + r.Failed + " failed.";

                if (r.Failed > 0 && r.Errors.Count > 0)
                {
                    var shown = r.Errors.Take(5).Select(Encode);
                    msg += "<div class=\"mt-1\" style=\"font-size:12.5px\">"
                        + string.Join("<br/>", shown)
                        + (r.Errors.Count > 5 ? "<br/>&hellip; and " + (r.Errors.Count - 5) + " more." : "")
                        + "</div>";
                }
            }

            pnlResult.CssClass = css;
            litResult.Text = "<div style=\"font-size:13px;line-height:1.5\">" + msg + "</div>";
        }

        private static string Encode(string value)
        {
            return HttpUtility.HtmlEncode(value ?? "");
        }
    }
}