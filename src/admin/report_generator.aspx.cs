using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using src.services;
using src.services.admin;

namespace src.admin
{
    public partial class report_generator : src.security.AdminPage
    {
        private readonly ReportService reportService = new ReportService();
        private readonly AdminPortalService adminService = new AdminPortalService();

        protected string SelectedReportKey { get; private set; }
        protected string ReportTitle { get; private set; }
        protected string PreviewHeadersHtml { get; private set; }
        protected string PreviewRowsHtml { get; private set; }
        protected string PreviewCountText { get; private set; }
        protected bool HasPreviewRows { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            SelectedReportKey = NormalizeReport(Request.QueryString["report"]);
            if (!IsPostBack) LoadFilters();
            BindPreview();
        }

        private void LoadFilters()
        {
            ddlSemester.DataSource = reportService.GetSemesters();
            ddlSemester.DataTextField = "Name";
            ddlSemester.DataValueField = "SemesterId";
            ddlSemester.DataBind();
            ddlSemester.Items.Insert(0, new ListItem("All semesters", ""));

            ddlProgramme.DataSource = reportService.GetProgrammes();
            ddlProgramme.DataTextField = "DisplayName";
            ddlProgramme.DataValueField = "ProgrammeId";
            ddlProgramme.DataBind();
            ddlProgramme.Items.Insert(0, new ListItem("All programmes", ""));

            ddlStatus.Items.Clear();
            ddlStatus.Items.Add(new ListItem("Any", ""));
            ddlStatus.Items.Add(new ListItem("Good Standing", "Good Standing"));
            ddlStatus.Items.Add(new ListItem("Probation", "Probation"));
            ddlStatus.Items.Add(new ListItem("At Risk", "At Risk"));
            ddlStatus.Items.Add(new ListItem("Pending", "Pending"));
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            BindPreview();
        }

        private void BindPreview()
        {
            var headers = new List<string>();
            var rows = new List<IList<string>>();
            var programme = ddlProgramme.SelectedValue;

            if (SelectedReportKey == "programme" || SelectedReportKey == "enrolment")
            {
                ReportTitle = SelectedReportKey == "programme" ? "Programme Performance Report" : "Enrolment Summary Report";
                headers.AddRange(new[] { "Programme", "Name", "Courses", "Students", "Status" });
                foreach (var item in adminService.GetProgrammes().Where(p => string.IsNullOrWhiteSpace(programme) || p.Id == programme))
                    rows.Add(new[] { item.Code, item.Name, item.CourseCount.ToString(), item.StudentCount.ToString(), item.Status });
            }
            else if (SelectedReportKey == "course")
            {
                ReportTitle = "Course Performance Report";
                headers.AddRange(new[] { "Course", "Title", "Programme", "Enrolled", "Pass Rate", "Attendance" });
                foreach (var item in adminService.GetCourseMetrics().Where(c => string.IsNullOrWhiteSpace(programme) || c.Programme == programme))
                    rows.Add(new[] { item.Code, item.Title, item.Programme, item.Enrolled.ToString(), item.PassRate.ToString("0.0") + "%", item.AverageAttendance.ToString("0.0") + "%" });
            }
            else if (SelectedReportKey == "attendance")
            {
                ReportTitle = "Attendance Summary Report";
                headers.AddRange(new[] { "Student ID", "Name", "Programme", "Semester", "Attendance", "Standing" });
                foreach (var item in adminService.GetStudentPerformanceRows().Where(s => string.IsNullOrWhiteSpace(programme) || s.Programme == programme))
                    rows.Add(new[] { item.StudentId, item.Name, item.Programme, item.Semester.ToString(), item.Attendance.ToString("0.0") + "%", item.Standing });
            }
            else
            {
                var data = reportService.GetStudentAcademicReport(
                    ddlSemester.SelectedValue, programme, ddlStatus.SelectedValue,
                    ToNullableDate(txtDateFrom.Text), ToNullableDate(txtDateTo.Text));
                if (SelectedReportKey == "risk")
                {
                    ReportTitle = "At-Risk Student Report";
                    data = data.Where(x => x.Status == "At Risk" || x.Status == "Probation").ToList();
                }
                else if (SelectedReportKey == "top")
                {
                    ReportTitle = "Top-Performing Student Report";
                    data = data.Where(x => x.CGPA.HasValue && x.CGPA.Value >= 3.7m).ToList();
                }
                else ReportTitle = "Student Academic Report";

                headers.AddRange(new[] { "ID", "Name", "Programme", "Semester", "CGPA", "Status" });
                foreach (var item in data)
                    rows.Add(new[] { item.StudentNo, item.StudentName, item.Programme, item.SemesterName, item.CGPADisplay, item.Status });
            }

            var preview = rows.Take(10).ToList();
            PreviewHeadersHtml = string.Join("", headers.Select(h => "<th class=\"py-2 pr-4 text-left uppercase\" style=\"font-size:11px;font-weight:600\">" + Html(h) + "</th>"));
            var body = new StringBuilder();
            foreach (var row in preview)
                body.Append("<tr class=\"border-b border-slate-100\" style=\"font-size:12.5px\">")
                    .Append(string.Join("", row.Select(v => "<td class=\"py-2 pr-4 text-slate-700\">" + Html(v) + "</td>")))
                    .Append("</tr>");
            PreviewRowsHtml = body.ToString();
            PreviewCountText = "Showing " + preview.Count + " of " + rows.Count + " record(s).";
            HasPreviewRows = rows.Count > 0;
        }

        private static string NormalizeReport(string value)
        {
            var allowed = new[] { "student", "programme", "course", "attendance", "risk", "top", "enrolment" };
            return allowed.Contains(value ?? "") ? value : "student";
        }

        private static DateTime? ToNullableDate(string value)
        {
            DateTime result;
            return DateTime.TryParse(value, out result) ? result : (DateTime?)null;
        }

        private static string Html(string value) { return HttpUtility.HtmlEncode(value ?? ""); }
    }
}
