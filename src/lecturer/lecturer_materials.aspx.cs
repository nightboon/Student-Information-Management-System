using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using src.services;

namespace student_information_management_system
{
    public partial class lecturer_materials : src.security.LecturerPage
    {
        private LecturerProfile _lecturer;
        private List<LecturerMaterialRow> _materials = new List<LecturerMaterialRow>();
        private int? _offeringFilter;

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

            if (!IsPostBack)
            {
                var courses = LecturerPortalService.GetCourses(user);
                courseSelect.DataSource = courses;
                courseSelect.DataTextField = "CourseCode";
                courseSelect.DataValueField = "OfferingId";
                courseSelect.DataBind();

                courseFilterSelect.Items.Clear();
                courseFilterSelect.Items.Add(new ListItem("All courses", "all"));
                foreach (var course in courses)
                {
                    courseFilterSelect.Items.Add(new ListItem(course.CourseCode, course.CourseCode));
                }

                materialTypeSelect.Items.Clear();
                materialTypeSelect.Items.Add(new ListItem("Assignment", "Assignment"));
                materialTypeSelect.Items.Add(new ListItem("Lecture Notes", "Lecture Notes"));
                materialTypeSelect.Items.Add(new ListItem("Quiz", "Quiz"));
                materialTypeSelect.Items.Add(new ListItem("Test", "Test"));

                if (_offeringFilter.HasValue)
                {
                    var value = _offeringFilter.Value.ToString(CultureInfo.InvariantCulture);
                    if (courseSelect.Items.FindByValue(value) != null)
                        courseSelect.SelectedValue = value;
                }
            }
            dueDateInput.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            weightInput.Attributes["min"] = "0";
            weightInput.Attributes["max"] = "100";
            weightInput.Attributes["step"] = "0.01";
            LoadRows();
        }

        protected void PublishMaterial_Click(object sender, EventArgs e)
        {
            int offeringId;
            int.TryParse(courseSelect.SelectedValue, out offeringId);

            DateTime dueDate;
            DateTime? parsedDueDate = DateTime.TryParse(dueDateInput.Text, out dueDate) ? (DateTime?)dueDate : null;

            decimal weight;
            decimal? parsedWeight = decimal.TryParse(weightInput.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out weight)
                ? (decimal?)weight
                : null;

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

            if (!materialFileInput.HasFile)
            {
                ShowStatus("Please choose a file to upload.", false);
                return;
            }

            if (parsedDueDate.HasValue && parsedDueDate.Value.Date < DateTime.Today)
            {
                ShowStatus("Due date cannot be before today.", false);
                return;
            }

            if (parsedWeight.HasValue && (parsedWeight.Value < 0m || parsedWeight.Value > 100m))
            {
                ShowStatus("Course weight must be between 0 and 100.", false);
                return;
            }

            string fileUrl;
            string fileType;
            int fileSizeBytes;
            try
            {
                fileUrl = SaveMaterialFile(materialFileInput, offeringId, out fileType, out fileSizeBytes);
            }
            catch (InvalidOperationException ex)
            {
                ShowStatus(ex.Message, false);
                return;
            }

            var user = UserContextFactory.FromSession(Session);
            int added = LecturerPortalService.AddMaterial(user, new LecturerMaterialInput
            {
                OfferingId = offeringId,
                Title = titleInput.Text,
                FileUrl = fileUrl,
                UploadedAt = DateTime.Now
            });

            if (added == 0)
            {
                ShowStatus("Material could not be published for this course.", false);
                return;
            }

            titleInput.Text = "";
            descriptionInput.Text = "";
            dueDateInput.Text = "";
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
            return "book-open";
        }

        protected string TypeBadgeClass(object materialType)
        {
            string type = materialType == null ? "" : materialType.ToString().ToLowerInvariant();
            if (type == "assignment") return "bg-emerald-50 text-emerald-700";
            if (type == "quiz") return "bg-blue-50 text-blue-700";
            if (type == "test") return "bg-amber-50 text-amber-700";
            return "bg-[#e0162b]/10 text-[#a01020]";
        }

        protected string DueDateLabel(object value)
        {
            if (value == null || value == DBNull.Value) return "No due date";
            return Convert.ToDateTime(value).ToString("d MMM yyyy", CultureInfo.InvariantCulture);
        }

        protected string WeightLabel(object value)
        {
            if (value == null || value == DBNull.Value) return "-";
            return Convert.ToDecimal(value).ToString("0.##", CultureInfo.InvariantCulture) + "%";
        }

        protected string MaterialPreviewUrl(object value)
        {
            string id = value == null || value == DBNull.Value ? "" : value.ToString();
            if (string.IsNullOrWhiteSpace(id)) return "";
            return ResolveUrl("~/shared/material_preview.aspx?id=" + HttpUtility.UrlEncode(id));
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
