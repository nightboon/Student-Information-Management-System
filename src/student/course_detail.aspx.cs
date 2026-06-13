using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using src.services;

namespace src.student
{
    public partial class course_detail : src.security.StudentPage
    {
        protected new CourseHeader Header;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();
            Page.Form.Enctype = "multipart/form-data";

            if (Session["user_id"] == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }
            int userId = (int)Session["user_id"];

            int offeringId;
            if (!int.TryParse(Request.QueryString["offering"], out offeringId))
            {
                Response.Redirect("~/student/courses.aspx");
                return;
            }

            Header = CourseDetailService.GetHeader(offeringId, userId);
            if (Header == null)   // not enrolled / no such offering
            {
                Response.Redirect("~/student/courses.aspx");
                return;
            }

            outcomesRepeater.DataSource = CourseDetailService.GetLearningOutcomes(Header.CourseId);
            outcomesRepeater.DataBind();

            modulesRepeater.DataSource = ModuleService.GetModules(offeringId);
            modulesRepeater.DataBind();

            announcementsRepeater.DataSource = AnnouncementService.GetByOffering(offeringId);
            announcementsRepeater.DataBind();

            assignmentsRepeater.DataSource = AssignmentService.GetByOffering(offeringId, userId);
            assignmentsRepeater.DataBind();

            _gradebook = AssessmentService.GetGradebook(offeringId, userId);
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

            var upload = e.Item.FindControl("submissionInput") as FileUpload;
            if (upload == null || !upload.HasFile)
            {
                assignmentStatusMessage.Text = "Choose a file before submitting.";
                assignmentStatusPanel.CssClass = "mb-4 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-amber-800";
                assignmentStatusPanel.Visible = true;
                return;
            }

            string fileUrl;
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

            bool saved = AssignmentService.SaveSubmission((int)Session["user_id"], assignmentId, fileUrl);
            assignmentStatusMessage.Text = saved ? "Assignment submitted." : "Unable to submit this assignment.";
            assignmentStatusPanel.CssClass = saved
                ? "mb-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800"
                : "mb-4 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-amber-800";
            assignmentStatusPanel.Visible = true;

            int offeringId;
            if (int.TryParse(Request.QueryString["offering"], out offeringId))
            {
                assignmentsRepeater.DataSource = AssignmentService.GetByOffering(offeringId, (int)Session["user_id"]);
                assignmentsRepeater.DataBind();
            }
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

        private Gradebook _gradebook;

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

        /// <summary>Due/status line for an assignment card.</summary>
        protected string DueLabel(string status, DateTime due)
        {
            if (status == "MARKED") return "Graded · " + due.ToString("d MMM");
            if (status == "SUBMITTED") return "Submitted · " + due.ToString("d MMM");
            int days = (int)(due.Date - DateTime.Today).TotalDays;
            if (days < 0) return "Overdue · " + due.ToString("d MMM");
            if (days == 0) return "Today · 11:59 PM";
            if (days == 1) return "Tomorrow · 11:59 PM";
            return "In " + days + " days";
        }

        protected Gradebook Book { get { return _gradebook; } }

        /// <summary>Donut stroke-dashoffset for a 0-100 percentage over circumference 301.6.</summary>
        protected string DonutOffset(decimal? pct)
        {
            decimal p = pct ?? 0m;
            return Math.Round(301.6m * (1 - p / 100m), 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
