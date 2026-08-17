using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Linq;
using src.services;

namespace student_information_management_system
{
    public partial class lecturer_materials : src.security.LecturerPage
    {
        private LecturerProfile _lecturer;
        private List<LecturerMaterialRow> _materials = new List<LecturerMaterialRow>();
        private int? _offeringFilter;
        private CourseDashboardStats _courseStats;
        private List<StudentCourseModule> _courseModules = new List<StudentCourseModule>();
        private List<LecturerMaterialRow> _courseAssignments = new List<LecturerMaterialRow>();

        protected void Page_Load(object sender, EventArgs e)
        {
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

            if (_offeringFilter.HasValue)
            {
                _courseStats = LecturerPortalService.GetCourseStats(user, _offeringFilter.Value);
                if (_courseStats == null)
                {
                    Response.Redirect("~/lecturer/lecturer_courses.aspx");
                    return;
                }

                _courseModules = LecturerPortalService.GetCourseModules(user, _offeringFilter.Value);
                courseModulesRepeater.DataSource = _courseModules;
                courseModulesRepeater.DataBind();
                courseModulesEmptyPanel.Visible = _courseModules.Count == 0;
                _courseAssignments = LecturerPortalService.GetMaterials(user, _offeringFilter.Value)
                    .Where(material => !string.Equals(material.MaterialType, "Lecture Notes", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(material => material.DueDate ?? DateTime.MaxValue)
                    .ThenBy(material => material.Title)
                    .ToList();
                courseAssignmentsRepeater.DataSource = _courseAssignments;
                courseAssignmentsRepeater.DataBind();
                courseAssignmentsEmptyPanel.Visible = _courseAssignments.Count == 0;
                courseModulesPanel.Visible = true;
                materialsManagerPanel.Visible = false;
                return;
            }

            if (!IsPostBack)
            {
                var courses = LecturerPortalService.GetCourses(user);
                var sessions = AcademicTermReader.GetSessionOptions();
                courseSelect.Items.Clear();
                courseSelect.Items.Add(new ListItem("Choose semester first", ""));
                courseFilterSelect.Items.Clear();
                courseFilterSelect.Items.Add(new ListItem("All courses", "all"));
                foreach (var course in courses)
                {
                    var uploadCourse = new ListItem(course.CourseCode + " - " + course.CourseName, course.OfferingId.ToString(CultureInfo.InvariantCulture));
                    uploadCourse.Attributes["data-year"] = course.AcademicYear;
                    uploadCourse.Attributes["data-semester"] = course.Semester;
                    courseSelect.Items.Add(uploadCourse);

                    var filterCourse = new ListItem(course.CourseCode + " - " + course.CourseName, course.OfferingId.ToString(CultureInfo.InvariantCulture));
                    filterCourse.Attributes["data-year"] = course.AcademicYear;
                    filterCourse.Attributes["data-semester"] = course.Semester;
                    courseFilterSelect.Items.Add(filterCourse);
                }

                var filterYears = sessions.Select(term => term.AcademicYear)
                    .Concat(courses.Select(course => course.AcademicYear))
                    .Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().ToList();
                var uploadYears = courses.Select(course => course.AcademicYear)
                    .Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().ToList();
                var semesters = sessions.Select(term => term.Semester)
                    .Concat(courses.Select(course => course.Semester))
                    .Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().ToList();

                yearFilterSelect.Items.Clear();
                yearFilterSelect.Items.Add(new ListItem("All years", "all"));
                uploadYearSelect.Items.Clear();
                uploadYearSelect.Items.Add(new ListItem("Choose academic year", ""));
                foreach (string year in filterYears)
                    yearFilterSelect.Items.Add(new ListItem(StudentPortalFormat.AcademicYearLabel(year), year));
                foreach (string year in uploadYears)
                    uploadYearSelect.Items.Add(new ListItem(StudentPortalFormat.AcademicYearLabel(year), year));

                semesterFilterSelect.Items.Clear();
                semesterFilterSelect.Items.Add(new ListItem("All semesters", "all"));
                foreach (string semester in semesters)
                    semesterFilterSelect.Items.Add(new ListItem(StudentPortalFormat.SemesterLabel(semester), semester));

                uploadSemesterSelect.Items.Clear();
                uploadSemesterSelect.Items.Add(new ListItem("Choose academic year first", ""));

                materialTypeSelect.Items.Clear();
                materialTypeSelect.Items.Add(new ListItem("Assignment", "Assignment"));
                materialTypeSelect.Items.Add(new ListItem("Lecture Notes", "Lecture Notes"));
                materialTypeSelect.Items.Add(new ListItem("Quiz", "Quiz"));
                materialTypeSelect.Items.Add(new ListItem("Test", "Test"));
                materialTypeSelect.Items.Add(new ListItem("Viva", "Viva"));

                weekSelect.Items.Clear();
                for (int week = 1; week <= 14; week++)
                    weekSelect.Items.Add(new ListItem("Week " + week, week.ToString(CultureInfo.InvariantCulture)));

                if (_offeringFilter.HasValue)
                {
                    var value = _offeringFilter.Value.ToString(CultureInfo.InvariantCulture);
                    if (courseSelect.Items.FindByValue(value) != null)
                        courseSelect.SelectedValue = value;
                }
            }
            dueDateInput.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (!IsPostBack && string.IsNullOrWhiteSpace(dueTimeInput.Text))
                dueTimeInput.Text = "23:59";
            weightInput.Attributes["min"] = "0";
            weightInput.Attributes["max"] = "100";
            weightInput.Attributes["step"] = "0.01";
            LoadRows();
        }

        protected void PublishMaterial_Click(object sender, EventArgs e)
        {
            int offeringId;
            int.TryParse(courseSelect.SelectedValue, out offeringId);
            string materialType = materialTypeSelect.SelectedValue;
            bool isLectureNotes = string.Equals(materialType, "Lecture Notes", StringComparison.OrdinalIgnoreCase);
            bool isQuiz = string.Equals(materialType, "Quiz", StringComparison.OrdinalIgnoreCase);
            bool isViva = string.Equals(materialType, "Viva", StringComparison.OrdinalIgnoreCase);
            bool requiresAssessmentDetails =
                string.Equals(materialType, "Assignment", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(materialType, "Quiz", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(materialType, "Test", StringComparison.OrdinalIgnoreCase) ||
                isViva;
            bool requiresPositiveWeight =
                string.Equals(materialType, "Assignment", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(materialType, "Test", StringComparison.OrdinalIgnoreCase) ||
                isViva;
            int selectedWeek;
            int? weekNumber = isLectureNotes &&
                int.TryParse(weekSelect.SelectedValue, out selectedWeek) &&
                selectedWeek >= 1 && selectedWeek <= 14
                    ? (int?)selectedWeek
                    : null;

            DateTime? parsedDueDate = ParseDueDateTime(dueDateInput.Text, dueTimeInput.Text);

            decimal weight;
            decimal? parsedWeight = decimal.TryParse(weightInput.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out weight)
                ? (decimal?)weight
                : null;

            if (isLectureNotes)
            {
                parsedDueDate = null;
                parsedWeight = null;
                if (!weekNumber.HasValue)
                {
                    ShowStatus("Please choose a week between Week 1 and Week 14 for lecture notes.", false);
                    return;
                }
            }

            if (offeringId <= 0)
            {
                ShowStatus("No course is available for publishing materials.", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(titleInput.Text))
            {
                ShowStatus("Title is required.", false);
                return;
            }

            if (requiresAssessmentDetails && !parsedDueDate.HasValue)
            {
                ShowStatus("Due date and due time are required for assessments.", false);
                return;
            }

            if (requiresAssessmentDetails && !parsedWeight.HasValue)
            {
                ShowStatus("Course weight is required for assessments.", false);
                return;
            }
            if (requiresPositiveWeight && parsedWeight.Value <= 0m)
            {
                ShowStatus("Course weight for assessments must be greater than 0%.", false);
                return;
            }

            Uri quizUrl;
            string description = (descriptionInput.Text ?? "").Trim();
            bool validQuizUrl = IsGoogleFormsUrl(description, out quizUrl);

            if (isQuiz && !validQuizUrl)
            {
                ShowStatus("Quiz links must use Google Forms. Please paste a forms.gle or docs.google.com/forms sharing link.", false);
                return;
            }

            string submissionMode = string.Equals(materialType, "Assignment", StringComparison.OrdinalIgnoreCase)
                ? "FILE"
                : isViva
                    ? (Request.Form["assessmentMode"] ?? "").Trim().ToUpperInvariant()
                    : "MANUAL";
            if (isViva && submissionMode != "LINK" && submissionMode != "MANUAL")
            {
                ShowStatus("Choose whether this Viva uses a Google Drive video link or lecturer-entered marks.", false);
                return;
            }

            if (!isQuiz && !materialFileInput.HasFile)
            {
                ShowStatus("Please choose a file to upload.", false);
                return;
            }

            if (parsedDueDate.HasValue && parsedDueDate.Value < DateTime.Now)
            {
                ShowStatus("Due date and time cannot be in the past.", false);
                return;
            }

            if (parsedWeight.HasValue && (parsedWeight.Value < 0m || parsedWeight.Value > 100m))
            {
                ShowStatus("Course weight must be between 0 and 100.", false);
                return;
            }

            var user = UserContextFactory.FromSession(Session);
            decimal currentWeight = LecturerPortalService.GetMaterialWeightTotal(user, offeringId);
            if (requiresAssessmentDetails && currentWeight >= 100m)
            {
                ShowStatus(
                    "This course has already reached 100% course weight. Assessments cannot be added until the course weight is below 100%.",
                    false);
                return;
            }
            if (parsedWeight.HasValue && currentWeight + parsedWeight.Value > 100m)
            {
                decimal remaining = Math.Max(0m, 100m - currentWeight);
                ShowStatus(
                    "This course already uses " + currentWeight.ToString("0.##", CultureInfo.InvariantCulture) +
                    "% weight. You can add at most " + remaining.ToString("0.##", CultureInfo.InvariantCulture) + "%.",
                    false);
                return;
            }

            string fileUrl = isQuiz ? description : "";
            string fileType = isQuiz ? "link" : "";
            int fileSizeBytes = 0;
            if (!isQuiz)
            {
                try
                {
                    fileUrl = SaveMaterialFile(materialFileInput, offeringId, out fileType, out fileSizeBytes);
                }
                catch (InvalidOperationException ex)
                {
                    ShowStatus(ex.Message, false);
                    return;
                }
            }

            int added = LecturerPortalService.AddMaterial(user, new LecturerMaterialInput
            {
                OfferingId = offeringId,
                Title = titleInput.Text,
                Description = description,
                MaterialType = materialType,
                SubmissionMode = submissionMode,
                Week = weekNumber,
                DueDate = parsedDueDate,
                Weight = parsedWeight,
                FileUrl = fileUrl,
                FileType = fileType,
                FileSizeBytes = fileSizeBytes,
                UploadedAt = DateTime.Now
            });

            if (added == -1)
            {
                ShowStatus("This material would make the course weight exceed 100%. Delete or reduce another weighted material first.", false);
                TryDeleteSavedFile(fileUrl);
                return;
            }

            if (added == 0)
            {
                ShowStatus("Material could not be published for this course.", false);
                TryDeleteSavedFile(fileUrl);
                return;
            }

            titleInput.Text = "";
            descriptionInput.Text = "";
            dueDateInput.Text = "";
            dueTimeInput.Text = "23:59";
            weightInput.Text = "";
            ShowStatus("Material published for enrolled students.", true);
            LoadRows();
        }

        protected void MaterialsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "DeleteMaterial") return;

            int materialId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out materialId)) return;

            bool deleted = LecturerPortalService.DeleteMaterial(UserContextFactory.FromSession(Session), materialId);
            ShowStatus(deleted ? "Material deleted." : "Material could not be deleted.", deleted);
            LoadRows();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object DeleteMaterial(int materialId)
        {
            var context = HttpContext.Current;
            var user = context == null ? null : UserContextFactory.FromSession(context.Session);
            if (user == null || !user.IsLecturer)
                return new { success = false, message = "Your lecturer session has expired." };

            bool deleted = LecturerPortalService.DeleteMaterial(user, materialId);
            return new
            {
                success = deleted,
                message = deleted ? "Material deleted." : "Material could not be deleted."
            };
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object UpdateMaterialWeight(int materialId, decimal weight)
        {
            var context = HttpContext.Current;
            var user = context == null ? null : UserContextFactory.FromSession(context.Session);
            if (user == null || !user.IsLecturer)
                return new { success = false, message = "Your lecturer session has expired." };

            var result = LecturerPortalService.UpdateMaterialWeight(user, materialId, weight);
            return new
            {
                success = result.Success,
                message = result.Message,
                weight = result.Weight,
                courseTotal = result.CourseTotal
            };
        }

        private string SaveMaterialFile(FileUpload upload, int offeringId, out string fileType, out int fileSizeBytes)
        {
            string extension = Path.GetExtension(upload.FileName);
            if (string.IsNullOrWhiteSpace(extension))
                throw new InvalidOperationException("Uploaded file needs a file extension.");

            extension = extension.ToLowerInvariant();
            string[] allowed = { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".zip", ".sql", ".txt", ".png", ".jpg", ".jpeg", ".mp4" };
            if (Array.IndexOf(allowed, extension) < 0)
                throw new InvalidOperationException("File type is not supported for course materials.");

            int maxBytes = 25 * 1024 * 1024;
            fileSizeBytes = upload.PostedFile.ContentLength;
            if (fileSizeBytes > maxBytes)
                throw new InvalidOperationException("Material file is larger than 25 MB.");

            string folder = Server.MapPath("~/uploads/materials");
            Directory.CreateDirectory(folder);

            string baseName = Path.GetFileNameWithoutExtension(upload.FileName);
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                baseName = baseName.Replace(c, '-');
            }
            if (string.IsNullOrWhiteSpace(baseName)) baseName = "material";

            string fileName = "off" + offeringId + "-" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + "-" + baseName + extension;
            upload.SaveAs(Path.Combine(folder, fileName));

            fileType = extension.TrimStart('.');
            return "~/uploads/materials/" + fileName;
        }

        private void TryDeleteSavedFile(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl) || !fileUrl.StartsWith("~/uploads/materials/", StringComparison.OrdinalIgnoreCase))
                return;

            string physicalPath = Server.MapPath(fileUrl);
            if (File.Exists(physicalPath))
                File.Delete(physicalPath);
        }

        private static bool IsGoogleFormsUrl(string value, out Uri uri)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                return false;

            string host = uri.Host.TrimEnd('.').ToLowerInvariant();
            if (host == "forms.gle") return true;
            return host == "docs.google.com" &&
                uri.AbsolutePath.StartsWith("/forms/", StringComparison.OrdinalIgnoreCase);
        }

        private void LoadRows()
        {
            var user = UserContextFactory.FromSession(Session);
            _materials = LecturerPortalService.GetMaterials(user, _offeringFilter);
            materialsRepeater.DataSource = _materials;
            materialsRepeater.DataBind();
            emptyPanel.Visible = _materials.Count == 0;
        }

        protected string Html(object value)
        {
            return HttpUtility.HtmlEncode(value == null ? "" : value.ToString());
        }

        protected string UploadedLabel(object value)
        {
            return ((DateTime)value).ToString("d MMM yyyy", CultureInfo.InvariantCulture);
        }

        protected string WeekLabel(object materialType, object week)
        {
            if (!string.Equals(Convert.ToString(materialType), "Lecture Notes", StringComparison.OrdinalIgnoreCase) ||
                week == null || week == DBNull.Value)
                return "";
            return "<span class=\"rounded bg-blue-50 px-1.5 py-0.5 text-blue-700\" style=\"font-size:11px;font-weight:700\">Week " +
                Convert.ToInt32(week).ToString(CultureInfo.InvariantCulture) + "</span>";
        }

        protected int CountByType(string materialType)
        {
            if (string.IsNullOrWhiteSpace(materialType)) return _materials.Count;

            int count = 0;
            foreach (var material in _materials)
            {
                if (string.Equals(material.MaterialType, materialType, StringComparison.OrdinalIgnoreCase))
                    count++;
            }
            return count;
        }

        protected string FileMeta(object fileType, object size)
        {
            return fileType == null || string.IsNullOrWhiteSpace(fileType.ToString())
                ? "RESOURCE"
                : fileType.ToString().ToUpperInvariant();
        }

        protected string MaterialIcon(object materialType)
        {
            string type = materialType == null ? "" : materialType.ToString().ToLowerInvariant();
            if (type == "assignment") return "clipboard-check";
            if (type == "quiz") return "circle-help";
            if (type == "test") return "clipboard-list";
            if (type == "viva") return "presentation";
            return "book-open";
        }

        protected string TypeBadgeClass(object materialType)
        {
            string type = materialType == null ? "" : materialType.ToString().ToLowerInvariant();
            if (type == "assignment") return "bg-emerald-50 text-emerald-700";
            if (type == "quiz") return "bg-blue-50 text-blue-700";
            if (type == "test") return "bg-amber-50 text-amber-700";
            if (type == "viva") return "bg-purple-50 text-purple-700";
            return "bg-[#e0162b]/10 text-[#a01020]";
        }

        protected string DueDateLabel(object value)
        {
            if (value == null || value == DBNull.Value) return "No due date";
            DateTime due = Convert.ToDateTime(value);
            return due.ToString("d MMM yyyy 'at' h:mm", CultureInfo.InvariantCulture) +
                (due.Hour < 12 ? " a.m." : " p.m.");
        }

        private static DateTime? ParseDueDateTime(string dateText, string timeText)
        {
            DateTime date;
            TimeSpan time;
            if (!DateTime.TryParseExact(dateText, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out date)
                || !TimeSpan.TryParseExact(timeText, @"hh\:mm", CultureInfo.InvariantCulture, out time))
                return null;

            return date.Date.Add(time);
        }

        protected string WeightLabel(object value)
        {
            if (value == null || value == DBNull.Value) return "-";
            return Convert.ToDecimal(value).ToString("0.##", CultureInfo.InvariantCulture) + "%";
        }

        protected string WeightInputValue(object value)
        {
            return value == null || value == DBNull.Value
                ? ""
                : Convert.ToDecimal(value).ToString("0.##", CultureInfo.InvariantCulture);
        }

        protected string CourseWeightLabel(object value)
        {
            if (value == null || value == DBNull.Value) return "Unweighted";
            return Convert.ToDecimal(value).ToString("0.##", CultureInfo.InvariantCulture) + "%";
        }

        protected string MaterialPreviewUrl(object value)
        {
            string id = value == null || value == DBNull.Value ? "" : value.ToString();
            return string.IsNullOrWhiteSpace(id)
                ? ""
                : ResolveUrl("~/shared/material_preview.aspx?id=" + HttpUtility.UrlEncode(id));
        }

        protected void CourseModulesRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "SaveModule" || !_offeringFilter.HasValue) return;

            var titleInput = e.Item.FindControl("moduleTitleInput") as TextBox;
            var descriptionInput = e.Item.FindControl("moduleDescriptionInput") as TextBox;
            string postedTitle = titleInput == null ? null : Request.Form[titleInput.UniqueID];
            string postedDescription = descriptionInput == null ? null : Request.Form[descriptionInput.UniqueID];
            string title = (postedTitle ?? (titleInput == null ? "" : titleInput.Text)).Trim();
            string description = (postedDescription ?? (descriptionInput == null ? "" : descriptionInput.Text)).Trim();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                ShowCourseModuleStatus("Enter both a week title and description.", false);
                return;
            }

            bool updated = LecturerPortalService.UpdateModule(
                UserContextFactory.FromSession(Session),
                _offeringFilter.Value,
                Convert.ToString(e.CommandArgument),
                title,
                description);

            ShowCourseModuleStatus(updated ? "Week details updated." : "The week details could not be updated.", updated);
            if (updated)
            {
                _courseModules = LecturerPortalService.GetCourseModules(
                    UserContextFactory.FromSession(Session), _offeringFilter.Value);
                courseModulesRepeater.DataSource = _courseModules;
                courseModulesRepeater.DataBind();
            }
        }

        private void ShowCourseModuleStatus(string message, bool success)
        {
            courseModuleStatusMessage.Text = Server.HtmlEncode(message);
            courseModuleStatusPanel.CssClass = success
                ? "mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800"
                : "mt-4 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-amber-800";
            courseModuleStatusPanel.Visible = true;
        }

        protected int CourseModuleCount { get { return _courseModules.Count; } }
        protected int CourseAssignmentCount { get { return _courseAssignments.Count; } }
        protected bool ShowAssignmentsTab
        {
            get { return string.Equals(Request.QueryString["tab"], "assignments", StringComparison.OrdinalIgnoreCase); }
        }

        protected string CourseAccentColor
        {
            get
            {
                string color = _courseStats == null ? null : _courseStats.Color;
                return !string.IsNullOrEmpty(color) && System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$")
                    ? color
                    : "#10b981";
            }
        }

        protected string CourseDashboardUrl
        {
            get
            {
                return ResolveUrl("~/lecturer/lecturer_course_dashboard.aspx?offering=" +
                    _offeringFilter.GetValueOrDefault().ToString(CultureInfo.InvariantCulture));
            }
        }

        protected string CourseFileSize(object bytes)
        {
            if (bytes == null || bytes is DBNull) return "";
            int value = Convert.ToInt32(bytes);
            if (value >= 1048576) return Math.Round(value / 1048576.0, 1) + " MB";
            if (value >= 1024) return Math.Round(value / 1024.0, 0) + " KB";
            return value + " B";
        }

        protected string CourseFileIcon(string fileType)
        {
            switch ((fileType ?? "").ToLowerInvariant())
            {
                case "video": return "play-circle";
                case "pdf": return "file-text";
                case "docx": return "file-text";
                default: return "paperclip";
            }
        }

        private void ShowStatus(string message, bool success)
        {
            statusMessage.Text = message;
            statusBanner.CssClass = success
                ? "mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800"
                : "mt-4 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-amber-800";
            statusBanner.Visible = true;
        }
    }
}
