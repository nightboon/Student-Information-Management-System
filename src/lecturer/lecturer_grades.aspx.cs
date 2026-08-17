using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;
using src.services;

namespace student_information_management_system
{
    public partial class lecturer_grades : src.security.LecturerPage
    {
        private const int MaximumAnnotationDraftLength = 2 * 1024 * 1024;

        private LecturerProfile _lecturer;
        private List<LecturerAssessmentOption> _assessments = new List<LecturerAssessmentOption>();
        private List<LecturerGradeRow> _rows = new List<LecturerGradeRow>();
        private int _assessmentId;
        private int? _offeringFilter;
        private bool _hasInvalidMarks;

        protected bool IsCourseScoped { get { return _offeringFilter.HasValue; } }
        protected int SelectedOfferingId { get { return _offeringFilter.GetValueOrDefault(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();
            Page.Form.Enctype = "multipart/form-data";

            var user = UserContextFactory.FromSession(Session);
            _lecturer = LecturerPortalService.GetProfile(user);
            if (_lecturer == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            int offeringId;
            if (int.TryParse(Request.QueryString["offering"], out offeringId) && offeringId > 0)
                _offeringFilter = offeringId;

            _assessments = _offeringFilter.HasValue
                ? LecturerPortalService.GetAssessments(user, _offeringFilter.Value)
                : LecturerPortalService.GetAssessments(user);
            if (!IsPostBack)
            {
                assessmentSelect.DataSource = _assessments;
                assessmentSelect.DataTextField = "Label";
                assessmentSelect.DataValueField = "AssessmentId";
                assessmentSelect.DataBind();
                int requestedAssessmentId;
                string requestedValue = int.TryParse(Request.QueryString["assessment"], out requestedAssessmentId)
                    ? requestedAssessmentId.ToString(CultureInfo.InvariantCulture)
                    : "";
                if (assessmentSelect.Items.FindByValue(requestedValue) != null)
                    assessmentSelect.SelectedValue = requestedValue;
                else if (_assessments.Count > 0)
                    assessmentSelect.SelectedIndex = 0;
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
            var marksBySubmission = ReadSubmittedMarks();
            if (_hasInvalidMarks)
            {
                ShowStatus("Marks must be between 0 and 100.", false);
                return;
            }

            LecturerPortalService.SaveGradeMarks(UserContextFactory.FromSession(Session), _assessmentId, marksBySubmission);
            ShowStatus("Marks saved as draft.", true);
            LoadRows();
        }

        protected void Publish_Click(object sender, EventArgs e)
        {
            var marksBySubmission = ReadSubmittedMarks();
            if (_hasInvalidMarks)
            {
                ShowStatus("Marks must be between 0 and 100.", false);
                return;
            }

            var user = UserContextFactory.FromSession(Session);
            LecturerPortalService.SaveGradeMarks(user, _assessmentId, marksBySubmission);
            var emailResult = LecturerPortalService.PublishGrades(user, _assessmentId);
            if (!emailResult.Success)
            {
                ShowStatus("Marks, feedback, and final course grades were published, but one or more grade emails could not be sent. Please try again later.", false);
                LoadRows();
                return;
            }

            string emailMessage = emailResult.SentCount > 0
                ? " Grade email sent to " + emailResult.SentCount.ToString(CultureInfo.InvariantCulture) + " student" + (emailResult.SentCount == 1 ? "." : "s.")
                : "";
            ShowStatus("Marks, feedback, and final course grades published to students." + emailMessage, true);
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

            var extensionDate = e.Item.FindControl("extensionDateInput") as TextBox;
            var extensionTime = e.Item.FindControl("extensionTimeInput") as TextBox;
            if (extensionDate != null)
            {
                extensionDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (string.IsNullOrWhiteSpace(extensionDate.Text))
                    extensionDate.Text = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            if (extensionTime != null && string.IsNullOrWhiteSpace(extensionTime.Text))
                extensionTime.Text = "23:59";
        }

        protected void gradeRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int submissionId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out submissionId) || submissionId <= 0)
            {
                ShowStatus("The submission could not be identified.", false);
                return;
            }

            if (e.CommandName == "GrantExtension")
            {
                var dateInput = e.Item.FindControl("extensionDateInput") as TextBox;
                var timeInput = e.Item.FindControl("extensionTimeInput") as TextBox;
                DateTime date;
                TimeSpan time;
                if (dateInput == null || timeInput == null ||
                    !DateTime.TryParseExact(dateInput.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ||
                    !TimeSpan.TryParse(timeInput.Text, CultureInfo.InvariantCulture, out time))
                {
                    ShowStatus("Choose a valid extension date and time.", false);
                    LoadRows();
                    return;
                }
                var deadline = date.Date.Add(time);
                var now = DateTime.Now;
                var earliestDeadline = new DateTime(
                    now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddMinutes(1);
                if (deadline < earliestDeadline)
                {
                    ShowStatus(
                        "Choose a deadline of at least " +
                        earliestDeadline.ToString("d MMM yyyy, HH:mm", CultureInfo.InvariantCulture) + ".",
                        false);
                    LoadRows();
                    return;
                }
                bool granted = LecturerPortalService.GrantSubmissionExtension(
                    UserContextFactory.FromSession(Session), submissionId, deadline);
                ShowStatus(granted
                    ? "One-time late-submission deadline granted until " + deadline.ToString("d MMM yyyy, HH:mm", CultureInfo.InvariantCulture) + "."
                    : "The extension could not be granted. It may already have been used or the deadline is invalid.", granted);
                LoadRows();
                return;
            }

            if (e.CommandName != "SaveReview") return;

            var feedbackInput = e.Item.FindControl("reviewFeedbackInput") as TextBox;
            var annotatedUpload = e.Item.FindControl("annotatedFileInput") as FileUpload;
            string feedback = feedbackInput == null ? "" : feedbackInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(feedback))
            {
                ShowStatus("Feedback is required before saving.", false);
                return;
            }

            string annotatedFileUrl = "";

            if (annotatedUpload != null && annotatedUpload.HasFile)
            {
                try
                {
                    annotatedFileUrl = SaveAnnotatedFile(annotatedUpload, submissionId);
                }
                catch (InvalidOperationException ex)
                {
                    ShowStatus(ex.Message, false);
                    return;
                }
            }

            bool saved = LecturerPortalService.SaveSubmissionReview(
                UserContextFactory.FromSession(Session),
                submissionId,
                feedback,
                annotatedFileUrl);

            if (!saved && !string.IsNullOrWhiteSpace(annotatedFileUrl))
                TryDeleteAnnotatedFile(annotatedFileUrl);

            ShowStatus(saved ? "Submission feedback saved as draft." : "Submission feedback could not be saved.", saved);
            LoadRows();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object LoadAnnotationDraft(int submissionId)
        {
            var context = HttpContext.Current;
            var user = context == null ? null : UserContextFactory.FromSession(context.Session);
            if (user == null || !user.IsLecturer)
                return new { success = false, message = "Your lecturer session has expired.", annotationsJson = "" };

            string annotationsJson = LecturerPortalService.GetAnnotationDraft(user, submissionId);
            return new { success = true, message = "", annotationsJson = annotationsJson };
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object SaveAnnotationDraft(int submissionId, string annotationsJson)
        {
            var context = HttpContext.Current;
            var user = context == null ? null : UserContextFactory.FromSession(context.Session);
            if (user == null || !user.IsLecturer)
                return new { success = false, message = "Your lecturer session has expired." };

            annotationsJson = annotationsJson ?? "[]";
            if (annotationsJson.Length > MaximumAnnotationDraftLength)
                return new { success = false, message = "This annotation draft is too large to save." };

            bool saved = LecturerPortalService.SaveAnnotationDraft(user, submissionId, annotationsJson);
            return new
            {
                success = saved,
                message = saved ? "Annotation progress saved." : "Annotation progress could not be saved."
            };
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object ExpireSubmissionExtension(int submissionId)
        {
            var context = HttpContext.Current;
            var user = context == null ? null : UserContextFactory.FromSession(context.Session);
            if (user == null || !user.IsLecturer)
                return new { success = false, message = "Your lecturer session has expired." };

            bool expired = LecturerPortalService.ExpireSubmissionExtension(user, submissionId);
            return new
            {
                success = expired,
                message = expired ? "" : "The extension could not be expired."
            };
        }

        private string SaveAnnotatedFile(FileUpload upload, int submissionId)
        {
            string extension = Path.GetExtension(upload.FileName);
            extension = string.IsNullOrWhiteSpace(extension) ? "" : extension.ToLowerInvariant();
            string[] allowed = { ".pdf", ".png", ".jpg", ".jpeg" };
            if (Array.IndexOf(allowed, extension) < 0)
                throw new InvalidOperationException("Annotated files must be PDF, PNG, JPG, or JPEG.");
            if (upload.PostedFile.ContentLength > 10 * 1024 * 1024)
                throw new InvalidOperationException("Annotated file is larger than 10 MB.");

            string folder = Server.MapPath("~/uploads/feedback");
            Directory.CreateDirectory(folder);
            string baseName = Path.GetFileNameWithoutExtension(upload.FileName);
            foreach (char c in Path.GetInvalidFileNameChars()) baseName = baseName.Replace(c, '-');
            if (string.IsNullOrWhiteSpace(baseName)) baseName = "annotated";

            string fileName = "submission-" + submissionId + "-" +
                DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + "-" + baseName + extension;
            upload.SaveAs(Path.Combine(folder, fileName));
            return "~/uploads/feedback/" + fileName;
        }

        private void TryDeleteAnnotatedFile(string url)
        {
            if (!url.StartsWith("~/uploads/feedback/", StringComparison.OrdinalIgnoreCase)) return;
            string path = Server.MapPath(url);
            if (File.Exists(path)) File.Delete(path);
        }

        private void LoadRows()
        {
            _rows = _assessmentId > 0
                ? LecturerPortalService.GetGradeRows(UserContextFactory.FromSession(Session), _assessmentId)
                : new List<LecturerGradeRow>();
            gradeRepeater.DataSource = _rows;
            gradeRepeater.DataBind();
            emptyPanel.Visible = _rows.Count == 0;
        }

        private IDictionary<int, decimal?> ReadSubmittedMarks()
        {
            var marksBySubmission = new Dictionary<int, decimal?>();
            foreach (RepeaterItem item in gradeRepeater.Items)
            {
                var submissionHidden = item.FindControl("submissionId") as HiddenField;
                var input = item.FindControl("marksInput") as TextBox;

                int submissionId;
                if (submissionHidden == null || input == null
                    || !input.Enabled
                    || !int.TryParse(submissionHidden.Value, out submissionId)
                    || submissionId == 0)
                    continue;

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

                marksBySubmission[submissionId] = mark;
            }
            return marksBySubmission;
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

        protected string SubmissionStatusClass(object isMissing)
        {
            return Convert.ToBoolean(isMissing)
                ? "text-[#e0162b] font-semibold"
                : "text-slate-400";
        }

        protected string MarkStatusClass(object statusValue)
        {
            string status = Convert.ToString(statusValue);
            if (status.StartsWith("Missing", StringComparison.OrdinalIgnoreCase)) return "text-[#e0162b]";
            if (status.Equals("Published", StringComparison.OrdinalIgnoreCase)) return "text-emerald-700";
            if (status.Equals("Ready to publish", StringComparison.OrdinalIgnoreCase)) return "text-blue-700";
            return "text-amber-700";
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

        protected string ExtensionDialogId(object value)
        {
            string id = value == null || value == DBNull.Value ? "0" : value.ToString();
            return "submission-extension-" + Html(id);
        }

        protected string ExtensionDeadlineValue(object value)
        {
            if (value == null || value == DBNull.Value) return "";
            return Convert.ToDateTime(value).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
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
