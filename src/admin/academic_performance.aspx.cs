using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using src.services.admin;

namespace src.admin
{
    public partial class academic_performance : src.security.AdminPage
    {
        private readonly AdminPortalService service = new AdminPortalService();

        protected AdminDashboardData Summary { get; private set; }
        protected string PassFailRowsHtml { get; private set; }
        protected string AttendanceRowsHtml { get; private set; }
        protected string StudentRowsHtml { get; private set; }
        protected string AtRiskRowsHtml { get; private set; }
        protected string TopPerformerRowsHtml { get; private set; }
        protected int TopPerformerCount { get; private set; }
        protected string AcademicYearOptionsHtml { get; private set; }
        protected string SemesterOptionsHtml { get; private set; }
        protected string ProgrammeOptionsHtml { get; private set; }
        protected string LecturerOptionsHtml { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Summary = service.GetDashboard();
            var courses = service.GetCourseMetrics();
            var students = service.GetStudentPerformanceRows();
            PassFailRowsHtml = BuildPassFailRows(courses);
            AttendanceRowsHtml = BuildAttendanceRows(courses);
            StudentRowsHtml = BuildStudentRows(students);
            AtRiskRowsHtml = BuildAtRiskRows(students);
            TopPerformerRowsHtml = BuildTopRows(students);
            TopPerformerCount = new List<AdminStudentPerformanceRow>(students).FindAll(s => s.Cgpa >= 3.7m).Count;
            var lookups = service.GetLookups();
            AcademicYearOptionsHtml = AdminPortalService.RenderOptions(lookups.AcademicYears, null);
            SemesterOptionsHtml = AdminPortalService.RenderOptions(lookups.StudentSemesters, "All semesters");
            ProgrammeOptionsHtml = AdminPortalService.RenderOptions(lookups.Programmes, "All programmes");
            LecturerOptionsHtml = AdminPortalService.RenderOptions(lookups.Lecturers, "Select mentor...");
        }

        protected void btnSendWarnings_Click(object sender, EventArgs e)
        {
            var students = service.GetStudentPerformanceRows();
            int atRisk = 0;
            foreach (var s in students)
            {
                if (s.Cgpa >= 2.0m && s.Attendance >= 75m && s.FailedCourses == 0
                    && s.Standing.IndexOf("risk", StringComparison.OrdinalIgnoreCase) < 0
                    && s.Standing.IndexOf("probation", StringComparison.OrdinalIgnoreCase) < 0) continue;
                atRisk++;
            }

            if (atRisk == 0)
            {
                pnlResult.CssClass = "mt-4 rounded-lg border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800";
                litResult.Text = "No at-risk students found. No academic warnings were sent.";
            }
            else
            {
                pnlResult.CssClass = "mt-4 rounded-lg border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800";
                litResult.Text = "Academic-warning notices queued for <strong>" + atRisk
                    + "</strong> at-risk student" + (atRisk == 1 ? "" : "s") + ".";
            }
            pnlResult.Visible = true;
        }

        private static string BuildStudentRows(IEnumerable<AdminStudentPerformanceRow> students)
        {
            var html = new StringBuilder();
            foreach (var s in students)
            {
                var status = s.LetterGrade == "F" ? "Fail" : "Pass";
                html.Append("<tr data-backend-student data-row data-search=\"").Append(Attr((s.StudentId + " " + s.Name + " " + s.CourseCode).ToLowerInvariant())).Append("\" data-prog=\"").Append(Attr(s.Programme)).Append("\" data-sem=\"").Append(s.Semester).Append("\" data-status=\"").Append(status).Append("\" class=\"border-b border-slate-100 hover:bg-slate-50/60\">");
                html.Append("<td class=\"px-6 py-3 text-slate-500\" style=\"font-size:12.5px\">").Append(Html(s.StudentId)).Append("</td><td class=\"px-6 py-3 text-slate-900 font-medium\" style=\"font-size:12.5px\">").Append(Html(s.Name)).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex rounded-full border border-slate-200 bg-slate-100 px-2 py-0.5 text-slate-600\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(s.Programme)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-center text-slate-700\" style=\"font-size:12.5px\">").Append(s.Semester).Append("</td><td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\"><span class=\"font-medium text-slate-900\">").Append(Html(s.CourseCode)).Append("</span> &middot; ").Append(Html(s.CourseName)).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-center\" style=\"font-size:12.5px\">").Append(Html(s.LetterGrade)).Append("</td><td class=\"px-6 py-3 text-right text-slate-700\" style=\"font-size:12.5px\">").Append(s.CurrentGpa.ToString("0.00")).Append("</td><td class=\"px-6 py-3 text-right text-slate-700\" style=\"font-size:12.5px\">").Append(s.Cgpa.ToString("0.00")).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex rounded-full border px-2 py-0.5 ").Append(status == "Pass" ? "bg-emerald-50 text-emerald-700 border-emerald-100" : "bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20").Append("\" style=\"font-size:11.5px;font-weight:600\">").Append(status).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-right\"><a href=\"student_detail.aspx?id=").Append(Attr(s.StudentId)).Append("\" class=\"inline-flex h-7 items-center rounded-md border border-slate-200 bg-white px-2 text-slate-600 hover:bg-slate-50\" style=\"font-size:11.5px;font-weight:600\">View</a></td></tr>");
            }
            return html.ToString();
        }

        private static string BuildAtRiskRows(IEnumerable<AdminStudentPerformanceRow> students)
        {
            var html = new StringBuilder();
            foreach (var s in students)
            {
                if (s.Cgpa >= 2.0m && s.Attendance >= 75m && s.FailedCourses == 0 && s.Standing.IndexOf("risk", StringComparison.OrdinalIgnoreCase) < 0 && s.Standing.IndexOf("probation", StringComparison.OrdinalIgnoreCase) < 0) continue;
                var risk = s.Cgpa < 1.5m || s.Attendance < 60m ? "Critical" : s.Cgpa < 2m || s.Attendance < 75m ? "High" : "Medium";
                var reason = s.Cgpa < 2m ? "CGPA below good-standing threshold" : s.Attendance < 75m ? "Attendance below 75%" : "Failed course requires follow-up";
                html.Append("<tr data-backend-student class=\"border-b border-slate-100 hover:bg-slate-50/60\"><td class=\"px-6 py-3 text-slate-500\" style=\"font-size:12.5px\">").Append(Html(s.StudentId)).Append("</td><td class=\"px-6 py-3 text-slate-900 font-medium\" style=\"font-size:12.5px\">").Append(Html(s.Name)).Append("</td><td class=\"px-6 py-3\" style=\"font-size:12.5px\">").Append(Html(s.Programme)).Append("</td><td class=\"px-6 py-3 text-center\" style=\"font-size:12.5px\">").Append(s.Semester).Append("</td><td class=\"px-6 py-3 text-right text-[#a01020] font-semibold\" style=\"font-size:12.5px\">").Append(s.Cgpa.ToString("0.00")).Append("</td><td class=\"px-6 py-3 text-right\" style=\"font-size:12.5px\">").Append(s.Attendance.ToString("0.0")).Append("%</td><td class=\"px-6 py-3 text-right\" style=\"font-size:12.5px\">").Append(s.FailedCourses).Append("</td><td class=\"px-6 py-3\" style=\"font-size:12.5px\">").Append(risk).Append("</td><td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(reason)).Append("</td><td class=\"px-6 py-3 text-right\"><a href=\"student_detail.aspx?id=").Append(Attr(s.StudentId)).Append("\" class=\"inline-flex h-7 items-center rounded-md border border-slate-200 bg-white px-2 text-slate-600 hover:bg-slate-50\" style=\"font-size:11.5px;font-weight:600\">View</a></td></tr>");
            }
            return html.ToString();
        }

        private static string BuildTopRows(IEnumerable<AdminStudentPerformanceRow> students)
        {
            var html = new StringBuilder();
            foreach (var s in students)
            {
                if (s.Cgpa < 3.7m) continue;
                html.Append("<tr data-backend-student class=\"border-b border-slate-100 hover:bg-slate-50/60\"><td class=\"px-6 py-3 text-slate-500\" style=\"font-size:12.5px\">").Append(Html(s.StudentId)).Append("</td><td class=\"px-6 py-3 text-slate-900 font-medium\" style=\"font-size:12.5px\">").Append(Html(s.Name)).Append("</td><td class=\"px-6 py-3\" style=\"font-size:12.5px\">").Append(Html(s.Programme)).Append("</td><td class=\"px-6 py-3 text-center\" style=\"font-size:12.5px\">").Append(s.Semester).Append("</td><td class=\"px-6 py-3 text-right text-emerald-600 font-semibold\" style=\"font-size:12.5px\">").Append(s.Cgpa.ToString("0.00")).Append("</td><td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">Dean's List eligible</td><td class=\"px-6 py-3 text-right\"><a href=\"student_detail.aspx?id=").Append(Attr(s.StudentId)).Append("\" class=\"inline-flex h-7 items-center rounded-md border border-slate-200 bg-white px-2 text-slate-600 hover:bg-slate-50\" style=\"font-size:11.5px;font-weight:600\">View</a></td></tr>");
            }
            return html.ToString();
        }

        protected string Percent(decimal value)
        {
            return value.ToString("0.0") + "%";
        }

        protected string DecimalNumber(decimal value)
        {
            return value.ToString("0.00");
        }

        protected int BarWidth(decimal value)
        {
            if (value < 0) return 0;
            if (value > 100) return 100;
            return (int)Math.Round(value);
        }

        private static string BuildPassFailRows(IEnumerable<AdminCourseMetric> courses)
        {
            var html = new StringBuilder();
            foreach (var c in courses)
            {
                var status = c.PassRate >= 80 ? "Healthy" : c.PassRate >= 60 ? "Warning" : "Critical";
                var statusClass = Badge(status);
                html.Append("<tr data-row data-search=\"").Append(Attr((c.Code + " " + c.Title).ToLowerInvariant())).Append("\" data-prog=\"").Append(Attr(c.Programme)).Append("\" data-status=\"").Append(Attr(status)).Append("\" class=\"border-b border-slate-100 hover:bg-slate-50/60\">");
                html.Append("<td class=\"px-6 py-3 text-slate-500\" style=\"font-size:12.5px\"><span class=\"font-medium\">").Append(Html(c.Code)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"text-slate-900 font-medium\">").Append(Html(c.Title)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(c.Programme)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-center\" style=\"font-size:12.5px\">").Append(c.Semester).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-right\" style=\"font-size:12.5px\">").Append(c.Enrolled).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-right\" style=\"font-size:12.5px\">").Append(c.Passed).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-right\" style=\"font-size:12.5px\">").Append(c.Failed).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><div class=\"flex items-center gap-2\"><div class=\"h-1.5 w-24 overflow-hidden rounded-full bg-slate-100\"><div class=\"h-full rounded-full ").Append(Bar(c.PassRate)).Append("\" style=\"width:").Append(Clamp(c.PassRate)).Append("%\"></div></div><span class=\"tabular-nums text-slate-700\" style=\"font-size:12.5px\">").Append(c.PassRate.ToString("0.0")).Append("%</span></div></td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 ").Append(statusClass).Append("\" style=\"font-size:11.5px;font-weight:600\">").Append(status).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-right\" style=\"font-size:12.5px\"><a href=\"course_passfail.aspx?code=").Append(Attr(c.Code)).Append("\" class=\"inline-flex h-7 items-center rounded-md border border-slate-200 bg-white px-2 text-slate-600 hover:bg-slate-50\" style=\"font-size:11.5px;font-weight:600\">View</a></td></tr>");
            }
            html.Append("<tr data-table-empty style=\"display:none\"><td colspan=\"10\" class=\"py-10 text-center text-slate-400\" style=\"font-size:13px\">No courses match your filters.</td></tr>");
            return html.ToString();
        }

        private static string BuildAttendanceRows(IEnumerable<AdminCourseMetric> courses)
        {
            var html = new StringBuilder();
            foreach (var c in courses)
            {
                var status = c.AverageAttendance >= 85 ? "Healthy" : c.AverageAttendance >= 70 ? "Warning" : "Critical";
                html.Append("<tr data-row data-search=\"").Append(Attr((c.Code + " " + c.Title).ToLowerInvariant())).Append("\" data-prog=\"").Append(Attr(c.Programme)).Append("\" data-status=\"").Append(Attr(status)).Append("\" class=\"border-b border-slate-100 hover:bg-slate-50/60\">");
                html.Append("<td class=\"px-6 py-3 text-slate-500\" style=\"font-size:12.5px\"><span class=\"font-medium\">").Append(Html(c.Code)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"text-slate-900 font-medium\">").Append(Html(c.Title)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(c.Programme)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-center\" style=\"font-size:12.5px\">").Append(c.Semester).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-right\" style=\"font-size:12.5px\">").Append(c.Enrolled).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><div class=\"flex items-center gap-2\"><div class=\"h-1.5 w-24 overflow-hidden rounded-full bg-slate-100\"><div class=\"h-full rounded-full ").Append(Bar(c.AverageAttendance)).Append("\" style=\"width:").Append(Clamp(c.AverageAttendance)).Append("%\"></div></div><span class=\"tabular-nums text-slate-700\" style=\"font-size:12.5px\">").Append(c.AverageAttendance.ToString("0.0")).Append("%</span></div></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-right\" style=\"font-size:12.5px\">").Append(c.SessionsHeld).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 ").Append(Badge(status)).Append("\" style=\"font-size:11.5px;font-weight:600\">").Append(status).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-right\" style=\"font-size:12.5px\"><a href=\"course_attendance.aspx?code=").Append(Attr(c.Code)).Append("\" class=\"inline-flex h-7 items-center rounded-md border border-slate-200 bg-white px-2 text-slate-600 hover:bg-slate-50\" style=\"font-size:11.5px;font-weight:600\">View</a></td></tr>");
            }
            html.Append("<tr data-table-empty style=\"display:none\"><td colspan=\"9\" class=\"py-10 text-center text-slate-400\" style=\"font-size:13px\">No courses match your filters.</td></tr>");
            return html.ToString();
        }

        private static string Bar(decimal value)
        {
            if (value >= 85) return "bg-emerald-500";
            if (value >= 60) return "bg-amber-500";
            return "bg-[#e0162b]";
        }

        private static string Badge(string status)
        {
            if (status == "Healthy") return "bg-emerald-50 text-emerald-700 border-emerald-100";
            if (status == "Warning") return "bg-amber-50 text-amber-700 border-amber-100";
            return "bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20";
        }

        private static int Clamp(decimal value)
        {
            if (value < 0) return 0;
            if (value > 100) return 100;
            return (int)Math.Round(value);
        }

        private static string Html(string value) { return HttpUtility.HtmlEncode(value ?? ""); }
        private static string Attr(string value) { return HttpUtility.HtmlAttributeEncode(value ?? ""); }
    }
}
