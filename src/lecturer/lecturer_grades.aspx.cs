using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using src.services;

namespace student_information_management_system
{
    public partial class lecturer_grades : src.security.LecturerPage
    {
        private Lecturer _lecturer;
        private List<LecturerAssessmentOption> _assessments = new List<LecturerAssessmentOption>();
        private List<LecturerGradeRow> _rows = new List<LecturerGradeRow>();
        private int _assessmentId;
        private int? _offeringFilter;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();

            _lecturer = Session["user_id"] != null ? LecturerService.GetByUserId((int)Session["user_id"]) : null;
            if (_lecturer == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            int offeringId;
            if (int.TryParse(Request.QueryString["offering"], out offeringId) && offeringId > 0)
                _offeringFilter = offeringId;

            _assessments = LecturerPortalService.GetAssessments(_lecturer.LecturerId);
            if (_offeringFilter.HasValue)
                _assessments = _assessments.FindAll(a => a.OfferingId == _offeringFilter.Value);
            if (!IsPostBack)
            {
                assessmentSelect.DataSource = _assessments;
                assessmentSelect.DataTextField = "Label";
                assessmentSelect.DataValueField = "AssessmentId";
                assessmentSelect.DataBind();
                if (_assessments.Count > 0) assessmentSelect.SelectedIndex = 0;
            }

            if (!int.TryParse(assessmentSelect.SelectedValue, out _assessmentId) && _assessments.Count > 0)
                _assessmentId = _assessments[0].AssessmentId;

            LoadRows();
        }

        protected void AssessmentChanged(object sender, EventArgs e)
        {
            LoadRows();
        }

        protected void SaveDraft_Click(object sender, EventArgs e)
        {
            LecturerPortalService.SaveAssessmentMarks(_lecturer.LecturerId, _assessmentId, ReadSubmittedMarks());
            statusMessage.Text = "Marks saved as draft.";
            statusBanner.Visible = true;
            LoadRows();
        }

        protected void Publish_Click(object sender, EventArgs e)
        {
            LecturerPortalService.SaveAssessmentMarks(_lecturer.LecturerId, _assessmentId, ReadSubmittedMarks());
            LecturerPortalService.PublishOfferingGrades(_lecturer.LecturerId, _assessmentId);
            statusMessage.Text = "Marks saved and final course grades published to students.";
            statusBanner.Visible = true;
            LoadRows();
        }

        private void LoadRows()
        {
            _rows = _assessmentId > 0
                ? LecturerPortalService.GetGradeRows(_lecturer.LecturerId, _assessmentId)
                : new List<LecturerGradeRow>();
            gradeRepeater.DataSource = _rows;
            gradeRepeater.DataBind();
            emptyPanel.Visible = _rows.Count == 0;
        }

        private IDictionary<int, decimal?> ReadSubmittedMarks()
        {
            var marks = new Dictionary<int, decimal?>();
            foreach (RepeaterItem item in gradeRepeater.Items)
            {
                var hidden = item.FindControl("studentId") as HiddenField;
                var input = item.FindControl("marksInput") as TextBox;
                int studentId;
                if (hidden == null || input == null || !int.TryParse(hidden.Value, out studentId)) continue;

                decimal parsed;
                if (string.IsNullOrWhiteSpace(input.Text)) marks[studentId] = null;
                else if (decimal.TryParse(input.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
                    marks[studentId] = parsed;
            }
            return marks;
        }

        protected int StudentCount
        {
            get { return _rows.Count; }
        }

        protected string MarksDisplay
        {
            get { return _rows.FindAll(r => r.HasMarks).Count.ToString(CultureInfo.InvariantCulture); }
        }

        protected int PendingCount
        {
            get { return _rows.Count - _rows.FindAll(r => r.HasMarks).Count; }
        }

        protected string AverageDisplay
        {
            get
            {
                decimal total = 0m;
                int count = 0;
                foreach (var row in _rows)
                {
                    if (!row.Marks.HasValue) continue;
                    total += row.Marks.Value;
                    count++;
                }
                return count == 0 ? "-" : Math.Round(total / count, 1).ToString("0.#", CultureInfo.InvariantCulture);
            }
        }

        protected string Html(object value)
        {
            return HttpUtility.HtmlEncode(value == null ? "" : value.ToString());
        }

        protected string MarksValue(object value)
        {
            return value == null || value == DBNull.Value ? "" : Convert.ToDecimal(value).ToString("0.##", CultureInfo.InvariantCulture);
        }

        protected string GradeBadgeClass(object grade)
        {
            string g = grade == null ? "" : grade.ToString();
            if (g == "N/A") return "bg-slate-100 text-slate-600";
            if (g == "F" || g == "D") return "bg-[#e0162b]/10 text-[#a01020]";
            if (g.StartsWith("A")) return "bg-emerald-50 text-emerald-700";
            return "bg-blue-50 text-blue-700";
        }
    }
}
