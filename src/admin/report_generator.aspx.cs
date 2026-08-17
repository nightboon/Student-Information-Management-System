using System;
using System.Linq;
using System.Web.UI.WebControls;
using src.services;

namespace src.admin
{
    public partial class report_generator : src.security.AdminPage
    {
        private const int PageSize = 10;
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

            ddlProgrammeStatus.Items.Clear();
            ddlProgrammeStatus.Items.Add(new ListItem("Any", ""));
            ddlProgrammeStatus.Items.Add(new ListItem("Healthy", "Healthy"));
            ddlProgrammeStatus.Items.Add(new ListItem("Watch", "Watch"));
            ddlProgrammeStatus.Items.Add(new ListItem("At Risk", "At Risk"));
            ddlProgrammeStatus.Items.Add(new ListItem("N/A", "N/A"));
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            ResetPageIndexes();
            BindPreview();
        }

        protected void PreviewPage_Command(object sender, CommandEventArgs e)
        {
            var parts = (e.CommandArgument ?? "").ToString().Split(':');
            if (parts.Length != 2) return;

            string reportKey = parts[0];
            int pageIndex = GetPageIndex(reportKey);
            SetPageIndex(reportKey, parts[1] == "next" ? pageIndex + 1 : pageIndex - 1);
            hdnReportType.Value = reportKey;

            BindPreview();
        }

        private void BindPreview()
        {
            string semesterId = EmptyToNull(ddlSemester.SelectedValue);
            string programmeId = EmptyToNull(ddlProgramme.SelectedValue);
            string academicStatus = ddlStatus.SelectedValue;
            string programmeStatus = ddlProgrammeStatus.SelectedValue;

            DateTime? dateFrom = null;
            DateTime? dateTo = null;

            var studentReportData = reportService.GetStudentAcademicReport(
                semesterId,
                programmeId,
                academicStatus,
                dateFrom,
                dateTo
            );

            int studentPageIndex = NormalizePageIndex("student", studentReportData.Count);
            rptPreview.DataSource = studentReportData.Skip(studentPageIndex * PageSize).Take(PageSize).ToList();
            rptPreview.DataBind();

            litPreviewCount.Text = BuildPageSummary(studentPageIndex, studentReportData.Count, "record(s)");
            SetPagerState(btnStudentPrev, btnStudentNext, studentPageIndex, studentReportData.Count);

            emptyPreviewPanel.Visible = studentReportData.Count == 0;

            var programmeReportData = reportService.GetProgrammePerformanceReport(
                semesterId,
                programmeId,
                programmeStatus,
                dateFrom,
                dateTo
            );

            int programmePageIndex = NormalizePageIndex("programme", programmeReportData.Count);
            rptProgrammePreview.DataSource = programmeReportData.Skip(programmePageIndex * PageSize).Take(PageSize).ToList();
            rptProgrammePreview.DataBind();

            litProgrammePreviewCount.Text = BuildPageSummary(programmePageIndex, programmeReportData.Count, "programme performance record(s)");
            SetPagerState(btnProgrammePrev, btnProgrammeNext, programmePageIndex, programmeReportData.Count);

            emptyProgrammePreviewPanel.Visible = programmeReportData.Count == 0;

            var courseReportData = reportService.GetCoursePerformanceReport(
                semesterId,
                programmeId,
                dateFrom,
                dateTo
            );

            int coursePageIndex = NormalizePageIndex("course", courseReportData.Count);
            rptCoursePreview.DataSource = courseReportData.Skip(coursePageIndex * PageSize).Take(PageSize).ToList();
            rptCoursePreview.DataBind();

            litCoursePreviewCount.Text = BuildPageSummary(coursePageIndex, courseReportData.Count, "course performance record(s)");
            SetPagerState(btnCoursePrev, btnCourseNext, coursePageIndex, courseReportData.Count);

            emptyCoursePreviewPanel.Visible = courseReportData.Count == 0;

            var attendanceReportData = reportService.GetAttendanceSummaryReport(
                semesterId,
                programmeId,
                dateFrom,
                dateTo
            );

            int attendancePageIndex = NormalizePageIndex("attendance", attendanceReportData.Count);
            rptAttendancePreview.DataSource = attendanceReportData.Skip(attendancePageIndex * PageSize).Take(PageSize).ToList();
            rptAttendancePreview.DataBind();

            litAttendancePreviewCount.Text = BuildPageSummary(attendancePageIndex, attendanceReportData.Count, "course attendance record(s)");
            SetPagerState(btnAttendancePrev, btnAttendanceNext, attendancePageIndex, attendanceReportData.Count);

            emptyAttendancePreviewPanel.Visible = attendanceReportData.Count == 0;

            var atRiskReportData = reportService.GetAtRiskStudentReport(
                semesterId,
                programmeId,
                dateFrom,
                dateTo
            );

            int atRiskPageIndex = NormalizePageIndex("atrisk", atRiskReportData.Count);
            rptAtRiskPreview.DataSource = atRiskReportData.Skip(atRiskPageIndex * PageSize).Take(PageSize).ToList();
            rptAtRiskPreview.DataBind();

            litAtRiskPreviewCount.Text = BuildPageSummary(atRiskPageIndex, atRiskReportData.Count, "at-risk student(s)");
            SetPagerState(btnAtRiskPrev, btnAtRiskNext, atRiskPageIndex, atRiskReportData.Count);

            emptyAtRiskPreviewPanel.Visible = atRiskReportData.Count == 0;

        }

        private int NormalizePageIndex(string reportKey, int totalCount)
        {
            int lastPageIndex = Math.Max(0, (int)Math.Ceiling(totalCount / (decimal)PageSize) - 1);
            int pageIndex = Math.Max(0, Math.Min(GetPageIndex(reportKey), lastPageIndex));
            SetPageIndex(reportKey, pageIndex);
            return pageIndex;
        }

        private int GetPageIndex(string reportKey)
        {
            object value = ViewState["PageIndex_" + reportKey];
            return value == null ? 0 : Convert.ToInt32(value);
        }

        private void SetPageIndex(string reportKey, int pageIndex)
        {
            ViewState["PageIndex_" + reportKey] = Math.Max(0, pageIndex);
        }

        private void ResetPageIndexes()
        {
            SetPageIndex("student", 0);
            SetPageIndex("programme", 0);
            SetPageIndex("course", 0);
            SetPageIndex("attendance", 0);
            SetPageIndex("atrisk", 0);
        }

        private static string BuildPageSummary(int pageIndex, int totalCount, string label)
        {
            if (totalCount == 0) return "Showing 0 of 0 " + label + ".";

            int start = pageIndex * PageSize + 1;
            int end = Math.Min(start + PageSize - 1, totalCount);
            return "Showing " + start + "-" + end + " of " + totalCount + " " + label + ".";
        }

        private static void SetPagerState(LinkButton previousButton, LinkButton nextButton, int pageIndex, int totalCount)
        {
            bool hasMultiplePages = totalCount > PageSize;
            previousButton.Visible = hasMultiplePages;
            nextButton.Visible = hasMultiplePages;
            previousButton.Enabled = pageIndex > 0;
            nextButton.Enabled = (pageIndex + 1) * PageSize < totalCount;
        }

        private static string EmptyToNull(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

    }
}
