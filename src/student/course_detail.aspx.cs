using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using src.services;

namespace src.student
{
    public partial class course_detail : src.security.StudentPage
    {
        protected new StudentCourseHeader Header;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();
            Page.Form.Enctype = "multipart/form-data";

            if (Session["user_id"] == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }
            var user = UserContextFactory.FromSession(Session);

            int offeringId;
            if (!int.TryParse(Request.QueryString["offering"], out offeringId))
            {
                Response.Redirect("~/student/courses.aspx");
                return;
            }

            Header = StudentPortalService.GetCourseHeader(user, offeringId);
            if (Header == null)   // not enrolled / no such offering
            {
                Response.Redirect("~/student/courses.aspx");
                return;
            }

            outcomesRepeater.DataSource = StudentPortalService.GetLearningOutcomes(user, Header.CourseId);
            outcomesRepeater.DataBind();

            modulesRepeater.DataSource = StudentPortalService.GetCourseModules(user, offeringId);
            modulesRepeater.DataBind();

            announcementsRepeater.DataSource = StudentPortalService.GetAnnouncements(user, offeringId);
            announcementsRepeater.DataBind();

            assignmentsRepeater.DataSource = StudentPortalService.GetAssignments(user, offeringId);
            assignmentsRepeater.DataBind();

            _gradebook = StudentPortalService.GetGradebook(user, offeringId);
            assessmentsRepeater.DataSource = _gradebook.Items;
            assessmentsRepeater.DataBind();
            barsRepeater.DataSource = _gradebook.Items;
            barsRepeater.DataBind();
        }

        protected void assignmentsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "SubmitAssignment") return;

            int assignmentId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out assignmentId)) return;

            var user = UserContextFactory.FromSession(Session);
            var assignment = StudentPortalService.GetAssignments(user, null)
                .Find(a => a.AssignmentId == assignmentId);
            if (assignment == null || !assignment.RequiresSubmission || !assignment.CanSubmit)
            {
                assignmentStatusMessage.Text = "The submission window for this assignment is closed.";
                assignmentStatusPanel.CssClass = "mb-4 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-amber-800";
                assignmentStatusPanel.Visible = true;
                return;
            }

            string fileUrl;
            if (assignment.IsLinkSubmission)
            {
                var linkInput = e.Item.FindControl("submissionLinkInput") as TextBox;
                string postedLink = linkInput == null ? "" : linkInput.Text;
                if (linkInput != null && string.IsNullOrWhiteSpace(postedLink))
                    postedLink = Request.Form[linkInput.UniqueID] ?? "";
                Uri driveUrl;
                if (linkInput == null || !IsGoogleDriveUrl(postedLink, out driveUrl))
                {
                    assignmentStatusMessage.Text =
                        "Viva video submissions must use a valid drive.google.com sharing link.";
                    assignmentStatusPanel.CssClass = "mb-4 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-amber-800";
                    assignmentStatusPanel.Visible = true;
                    return;
                }
                fileUrl = driveUrl.AbsoluteUri;
            }
            else
            {
                var upload = e.Item.FindControl("submissionInput") as FileUpload;
                if (upload == null || !upload.HasFile)
                {
                    assignmentStatusMessage.Text = "Choose a file before submitting.";
                    assignmentStatusPanel.CssClass = "mb-4 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-amber-800";
                    assignmentStatusPanel.Visible = true;
                    return;
                }

                try
                {
                    fileUrl = SaveUploadedSubmission(upload, assignmentId);
                }
                catch (InvalidOperationException ex)
                {
                    assignmentStatusMessage.Text = ex.Message;
                    assignmentStatusPanel.CssClass = "mb-4 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-amber-800";
                    assignmentStatusPanel.Visible = true;
                    return;
                }
            }

            bool saved = StudentPortalService.SaveSubmission(user, assignmentId, fileUrl);
            assignmentStatusMessage.Text = saved ? "Submission received." : "Unable to save this submission.";
            assignmentStatusPanel.CssClass = saved
                ? "mb-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800"
                : "mb-4 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-amber-800";
            assignmentStatusPanel.Visible = true;

            int offeringId;
            if (int.TryParse(Request.QueryString["offering"], out offeringId))
            {
                assignmentsRepeater.DataSource = StudentPortalService.GetAssignments(user, offeringId);
                assignmentsRepeater.DataBind();
            }
        }

        protected bool HasLecturerFeedback(object dataItem)
        {
            var assignment = dataItem as StudentCourseAssignment;
            return assignment != null &&
                (!string.IsNullOrWhiteSpace(assignment.Feedback) ||
                 !string.IsNullOrWhiteSpace(assignment.AnnotatedFileUrl));
        }

        protected string FeedbackFileUrl(object value)
        {
            string url = Convert.ToString(value);
            return string.IsNullOrWhiteSpace(url) ? "" : ResolveUrl(url);
        }

        protected string SubmissionUrl(object value)
        {
            string url = Convert.ToString(value);
            Uri absolute;
            return Uri.TryCreate(url, UriKind.Absolute, out absolute) ? url : ResolveUrl(url);
        }

        protected bool IsQuiz(object dataItem)
        {
            var assignment = dataItem as StudentCourseAssignment;
            return assignment != null &&
                string.Equals(assignment.AssignmentType, "Quiz", StringComparison.OrdinalIgnoreCase);
        }

        private string SaveUploadedSubmission(FileUpload upload, int assignmentId)
        {
            string extension = Path.GetExtension(upload.FileName);
            if (string.IsNullOrWhiteSpace(extension)) extension = ".dat";
            extension = extension.ToLowerInvariant();

            string[] allowed = { ".pdf", ".doc", ".docx", ".zip", ".png", ".jpg", ".jpeg", ".txt" };
            if (Array.IndexOf(allowed, extension) < 0) extension = ".dat";

            int maxBytes = 10 * 1024 * 1024;
            if (upload.PostedFile.ContentLength > maxBytes)
            {
                throw new InvalidOperationException("Submission file is larger than 10 MB.");
            }

            string folder = Server.MapPath("~/uploads/submissions");
            Directory.CreateDirectory(folder);

            string baseName = Path.GetFileNameWithoutExtension(upload.FileName);
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                baseName = baseName.Replace(c, '-');
            }
            if (string.IsNullOrWhiteSpace(baseName)) baseName = "submission";

            string fileName = "a" + assignmentId + "-u" + Session["user_id"] + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + baseName + extension;
            string physicalPath = Path.Combine(folder, fileName);
            upload.SaveAs(physicalPath);
            return "~/uploads/submissions/" + fileName;
        }

        private static bool IsGoogleDriveUrl(string value, out Uri uri)
        {
            if (!Uri.TryCreate((value ?? "").Trim(), UriKind.Absolute, out uri) ||
                uri.Scheme != Uri.UriSchemeHttps)
                return false;

            return string.Equals(uri.Host.TrimEnd('.'), "drive.google.com", StringComparison.OrdinalIgnoreCase) &&
                uri.AbsolutePath.StartsWith("/file/d/", StringComparison.OrdinalIgnoreCase) &&
                uri.AbsolutePath.Length > "/file/d/".Length;
        }

        private StudentGradebook _gradebook;

        /// <summary>Course accent color, or a neutral slate fallback for null/malformed values.</summary>
        protected string AccentColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return "#64748b";
            return System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$")
                ? color : "#64748b";
        }

        /// <summary>Up to two uppercase initials from a full name (e.g. "Dr. Sarah Tan" -> "ST").</summary>
        protected string Initials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "?";
            var parts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string letters = "";
            for (int i = parts.Length - 1; i >= 0 && letters.Length < 2; i--)
            {
                char c = parts[i][0];
                if (char.IsLetter(c)) letters = char.ToUpperInvariant(c) + letters;
            }
            return letters.Length == 0 ? "?" : letters;
        }

        /// <summary>Human file size ("3.4 MB"); empty for null (e.g. videos).</summary>
        protected string FileSize(object bytes)
        {
            if (bytes == null || bytes is DBNull) return "";
            int b = (int)bytes;
            if (b >= 1048576) return Math.Round(b / 1048576.0, 1) + " MB";
            if (b >= 1024) return Math.Round(b / 1024.0, 0) + " KB";
            return b + " B";
        }

        /// <summary>lucide icon name for a material file type.</summary>
        protected string FileIcon(string fileType)
        {
            switch ((fileType ?? "").ToLowerInvariant())
            {
                case "video": return "play-circle";
                case "pdf": return "file-text";
                case "docx": return "file-text";
                default: return "paperclip";
            }
        }

        protected string MaterialPreviewUrl(object materialId)
        {
            return ResolveUrl("~/shared/material_preview.aspx?id=" + HttpUtility.UrlEncode(Convert.ToString(materialId)));
        }

        /// <summary>Due/status line for an assignment card.</summary>
        protected string DueLabel(string status, DateTime due)
        {
            string deadline = due.ToString("d MMM yyyy 'at' h:mm", CultureInfo.InvariantCulture) +
                (due.Hour < 12 ? " a.m." : " p.m.");
            if (status == "MARKED") return "Graded · Due " + deadline;
            if (status == "EXTENDED") return "Late submission allowed until " + deadline;
            if (status == "NO UPLOAD REQUIRED") return "No file submission required";
            if (status == "SUBMITTED") return "Submitted · Due " + deadline;
            int days = (int)(due.Date - DateTime.Today).TotalDays;
            if (due < DateTime.Now) return "Overdue · " + deadline;
            if (days == 0) return "Today · " + due.ToString("h:mm", CultureInfo.InvariantCulture) +
                (due.Hour < 12 ? " a.m." : " p.m.");
            if (days == 1) return "Tomorrow · " + due.ToString("h:mm", CultureInfo.InvariantCulture) +
                (due.Hour < 12 ? " a.m." : " p.m.");
            return "Due " + deadline;
        }

        protected StudentGradebook Book { get { return _gradebook; } }

        /// <summary>Donut stroke-dashoffset for a 0-100 percentage over circumference 301.6.</summary>
        protected string DonutOffset(decimal? pct)
        {
            decimal p = pct ?? 0m;
            return Math.Round(301.6m * (1 - p / 100m), 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
