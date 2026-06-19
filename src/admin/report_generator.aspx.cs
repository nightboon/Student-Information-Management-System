using System;
using System.Linq;
using System.Web.UI.WebControls;
using src.services;

namespace src.admin
{
    public partial class report_generator : src.security.AdminPage
    {
        private readonly ReportService reportService = new ReportService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFilters();
                BindPreview();
            }
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
            string semesterId = EmptyToNull(ddlSemester.SelectedValue);
            string programmeId = EmptyToNull(ddlProgramme.SelectedValue);
            string status = ddlStatus.SelectedValue;

            DateTime? dateFrom = ToNullableDate(txtDateFrom.Text);
            DateTime? dateTo = ToNullableDate(txtDateTo.Text);

            var studentReportData = reportService.GetStudentAcademicReport(
                semesterId,
                programmeId,
                status,
                dateFrom,
                dateTo
            );

            rptPreview.DataSource = studentReportData.Take(10).ToList();
            rptPreview.DataBind();

            litPreviewCount.Text = "Showing " + Math.Min(10, studentReportData.Count) + " of " + studentReportData.Count + " record(s).";

            emptyPreviewPanel.Visible = studentReportData.Count == 0;

            var programmeReportData = reportService.GetProgrammePerformanceReport(
                semesterId,
                programmeId,
                status,
                dateFrom,
                dateTo
            );

            rptProgrammePreview.DataSource = programmeReportData.Take(10).ToList();
            rptProgrammePreview.DataBind();

            litProgrammePreviewCount.Text = "Showing " + Math.Min(10, programmeReportData.Count) + " of " + programmeReportData.Count + " programme performance record(s).";

            emptyProgrammePreviewPanel.Visible = programmeReportData.Count == 0;

            var courseReportData = reportService.GetCoursePerformanceReport(
                semesterId,
                programmeId,
                dateFrom,
                dateTo
            );

            rptCoursePreview.DataSource = courseReportData.Take(10).ToList();
            rptCoursePreview.DataBind();

            litCoursePreviewCount.Text = "Showing " + Math.Min(10, courseReportData.Count) + " of " + courseReportData.Count + " course performance record(s).";

            emptyCoursePreviewPanel.Visible = courseReportData.Count == 0;

            var attendanceReportData = reportService.GetAttendanceSummaryReport(
                semesterId,
                programmeId,
                dateFrom,
                dateTo
            );

            rptAttendancePreview.DataSource = attendanceReportData.Take(10).ToList();
            rptAttendancePreview.DataBind();

            litAttendancePreviewCount.Text = "Showing " + Math.Min(10, attendanceReportData.Count) + " of " + attendanceReportData.Count + " course attendance record(s).";

            emptyAttendancePreviewPanel.Visible = attendanceReportData.Count == 0;
        }

        private static string EmptyToNull(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private DateTime? ToNullableDate(string value)
        {
            DateTime result;

            if (DateTime.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }
    }
}
