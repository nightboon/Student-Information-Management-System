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
            int? semesterId = ToNullableInt(ddlSemester.SelectedValue);
            int? programmeId = ToNullableInt(ddlProgramme.SelectedValue);
            string status = ddlStatus.SelectedValue;

            DateTime? dateFrom = ToNullableDate(txtDateFrom.Text);
            DateTime? dateTo = ToNullableDate(txtDateTo.Text);

            var reportData = reportService.GetStudentAcademicReport(
                semesterId,
                programmeId,
                status,
                dateFrom,
                dateTo
            );

            rptPreview.DataSource = reportData.Take(10).ToList();
            rptPreview.DataBind();

            litPreviewCount.Text = "Showing " + Math.Min(10, reportData.Count) + " of " + reportData.Count + " record(s).";

            emptyPreviewPanel.Visible = reportData.Count == 0;
        }

        private int? ToNullableInt(string value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return null;
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