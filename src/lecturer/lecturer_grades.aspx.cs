using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        private bool _hasInvalidMarks;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();
            Page.Form.Enctype = "multipart/form-data";

            _lecturer = Session["user_id"] != null ? LecturerService.GetByUserId((int)Session["user_id"]) : null;
            if (_lecturer == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            int offeringId;
            if (int.TryParse(Request.QueryString["offering"], out offeringId) && offeringId > 0)
                _offeringFilter = offeringId;
            else
            {
                Response.Redirect("~/lecturer/lecturer_courses.aspx");
                return;
            }

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

            if (!IsPostBack)
            {
                LoadRows();
            }
        }

        protected void AssessmentChanged(object sender, EventArgs e)
        {
            LoadRows();
        }

        protected void SaveDraft_Click(object sender, EventArgs e)
        {
            var updates = ReadSubmittedUpdates();
            if (_hasInvalidMarks)
            {
                ShowStatus("Marks must be between 0 and 100.", false);
                return;
            }

            LecturerPortalService.SaveAssessmentRows(_lecturer.LecturerId, _assessmentId, updates);
            ShowStatus("Marks saved as draft.", true);
            LoadRows();
        }

        protected void Publish_Click(object sender, EventArgs e)
        {
            var updates = ReadSubmittedUpdates();
            if (_hasInvalidMarks)
            {
                ShowStatus("Marks must be between 0 and 100.", false);
                return;
            }

            LecturerPortalService.SaveAssessmentRows(_lecturer.LecturerId, _assessmentId, updates);
            LecturerPortalService.PublishOfferingGrades(_lecturer.LecturerId, _assessmentId);
            ShowStatus("Marks saved and final course grades published to students.", true);
            LoadRows();
        }

        protected void gradeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var marksInput = e.Item.FindControl("marksInput") as TextBox;
            if (marksInput == null) return;

            marksInput.Attributes["min"] = "0";
            marksInput.Attributes["max"] = "100";
            marksInput.Attributes["step"] = "1";
        }

        protected void gradeRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "SaveReview") return;

            int submissionId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out submissionId)) return;

            var feedbackInput = e.Item.FindControl("reviewFeedbackInput") as TextBox;
            var annotatedInput = e.Item.FindControl("annotatedFileInput") as FileUpload;
            string annotatedUrl = "";

            if (annotatedInput != null && annotatedInput.HasFile)
            {
                try
                {
                    annotatedUrl = SaveAnnotatedFile(annotatedInput, submissionId);
                }
                catch (InvalidOperationException ex)
                {
                    ShowStatus(ex.Message, false);
                    return;
                }
            }

            LecturerPortalService.SaveSubmissionReview(
                _lecturer.LecturerId,
                submissionId,
                feedbackInput == null ? "" : feedbackInput.Text,
                annotatedUrl);
            ShowStatus("Feedback saved.", true);
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

        private IDictionary<int, LecturerGradeUpdate> ReadSubmittedUpdates()
        {
            var updates = new Dictionary<int, LecturerGradeUpdate>();
            foreach (RepeaterItem item in gradeRepeater.Items)
            {
                var hidden = item.FindControl("studentId") as HiddenField;
                var submissionHidden = item.FindControl("submissionId") as HiddenField;
                var input = item.FindControl("marksInput") as TextBox;
                int studentId;
                if (hidden == null || input == null || !int.TryParse(hidden.Value, out studentId)) continue;

                decimal parsed;
                decimal? mark = null;
                if (!string.IsNullOrWhiteSpace(input.Text)
                    && decimal.TryParse(input.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed)
                    && parsed >= 0m
                    && parsed <= 100m)
                {
                    mark = parsed;
                }
                else if (!string.IsNullOrWhiteSpace(input.Text))
                {
                    _hasInvalidMarks = true;
                    continue;
                }

                int submissionId;
                updates[studentId] = new LecturerGradeUpdate
                {
                    Marks = mark,
                    SubmissionId = submissionHidden != null && int.TryParse(submissionHidden.Value, out submissionId)
                        ? (int?)submissionId
                        : null,
                    Feedback = ""
                };
            }
            return updates;
        }

        private string SaveAnnotatedFile(FileUpload upload, int submissionId)
        {
            string extension = Path.GetExtension(upload.FileName);
            if (string.IsNullOrWhiteSpace(extension)) extension = ".pdf";
            extension = extension.ToLowerInvariant();

            string[] allowed = { ".pdf", ".doc", ".docx", ".png", ".jpg", ".jpeg" };
            if (Array.IndexOf(allowed, extension) < 0)
            {
                throw new InvalidOperationException("Annotated file must be PDF, DOC, DOCX, PNG, or JPG.");
            }

            int maxBytes = 10 * 1024 * 1024;
            if (upload.PostedFile.ContentLength > maxBytes)
            {
                throw new InvalidOperationException("Annotated file is larger than 10 MB.");
            }

            string folder = Server.MapPath("~/uploads/feedback");
            Directory.CreateDirectory(folder);

            string baseName = Path.GetFileNameWithoutExtension(upload.FileName);
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                baseName = baseName.Replace(c, '-');
            }
            if (string.IsNullOrWhiteSpace(baseName)) baseName = "annotated";

            string fileName = "sub" + submissionId + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + baseName + extension;
            string physicalPath = Path.Combine(folder, fileName);
            upload.SaveAs(physicalPath);
            return "~/uploads/feedback/" + fileName;
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

        private void ShowStatus(string message, bool success)
        {
            statusMessage.Text = message;
            statusBanner.CssClass = success
                ? "mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800"
                : "mt-4 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-amber-800";
            statusBanner.Visible = true;
        }

        protected string ReviewModalId(object value)
        {
            string id = value == null || value == DBNull.Value ? "0" : value.ToString();
            return "submission-review-" + Html(id);
        }

        protected string SubmissionPreviewUrl(object value)
        {
            string url = value == null || value == DBNull.Value ? "" : value.ToString();
            return string.IsNullOrWhiteSpace(url) ? "about:blank" : ResolveUrl(url);
        }

        protected string SubmittedAtDisplay(object value)
        {
            if (value == null || value == DBNull.Value) return "";
            return " · " + Html(Convert.ToDateTime(value).ToString("d MMM, HH:mm", CultureInfo.InvariantCulture));
        }

        protected string SubmittedAtText(object value)
        {
            if (value == null || value == DBNull.Value) return "without a submission date";
            return Convert.ToDateTime(value).ToString("d MMM yyyy, HH:mm", CultureInfo.InvariantCulture);
        }

        protected string AnnotatedReviewLink(object value)
        {
            string url = value == null || value == DBNull.Value ? "" : value.ToString();
            if (string.IsNullOrWhiteSpace(url)) return "";

            return "<a href=\"" + Html(ResolveUrl(url)) + "\" target=\"_blank\" rel=\"noopener\" " +
                "class=\"mt-3 inline-flex items-center gap-1.5 text-[#a01020] hover:text-[#e0162b]\" " +
                "style=\"font-size:12.5px;font-weight:700\"><i data-lucide=\"file-check\" class=\"h-4 w-4\"></i>Open attached feedback file</a>";
        }
    }
}
