using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using src.services;

namespace student_information_management_system
{
    public partial class lecturer_materials : src.security.LecturerPage
    {
        private Lecturer _lecturer;
        private List<LecturerMaterialRow> _materials = new List<LecturerMaterialRow>();
        private int? _offeringFilter;

        protected void Page_Load(object sender, EventArgs e)
        {
            _lecturer = Session["user_id"] != null ? LecturerService.GetByUserId((int)Session["user_id"]) : null;
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
                courseSelect.DataSource = LecturerCourseService.GetCourses(_lecturer.UserId);
                courseSelect.DataTextField = "CourseCode";
                courseSelect.DataValueField = "OfferingId";
                courseSelect.DataBind();
                if (_offeringFilter.HasValue)
                {
                    var value = _offeringFilter.Value.ToString();
                    if (courseSelect.Items.FindByValue(value) != null)
                        courseSelect.SelectedValue = value;
                }
            }
            LoadRows();
        }

        protected void PublishMaterial_Click(object sender, EventArgs e)
        {
            int offeringId;
            int week;
            int sizeBytes;
            int.TryParse(courseSelect.SelectedValue, out offeringId);
            int? parsedWeek = int.TryParse(weekInput.Text, out week) ? (int?)week : null;
            int? parsedSize = int.TryParse(sizeInput.Text, out sizeBytes) ? (int?)sizeBytes : null;
            LecturerPortalService.AddMaterial(_lecturer.LecturerId, offeringId, titleInput.Text, descriptionInput.Text, parsedWeek, fileTypeInput.Text, parsedSize);

            titleInput.Text = "";
            descriptionInput.Text = "";
            weekInput.Text = "";
            fileTypeInput.Text = "";
            sizeInput.Text = "";
            statusMessage.Text = "Material published for enrolled students.";
            statusBanner.Visible = true;
            LoadRows();
        }

        private void LoadRows()
        {
            _materials = LecturerPortalService.GetMaterials(_lecturer.LecturerId, _offeringFilter);
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

        protected string FileMeta(object fileType, object size)
        {
            string type = fileType == null || string.IsNullOrWhiteSpace(fileType.ToString()) ? "resource" : fileType.ToString().ToUpperInvariant();
            if (size == null || size == DBNull.Value) return type;
            decimal mb = Convert.ToInt32(size) / 1048576m;
            return type + " - " + mb.ToString("0.#", CultureInfo.InvariantCulture) + " MB";
        }

        protected string FileIcon(object fileType)
        {
            string type = fileType == null ? "" : fileType.ToString().ToLowerInvariant();
            if (type == "sql" || type == "js" || type == "cs") return "file-code";
            if (type == "video" || type == "mp4") return "video";
            if (type == "pptx") return "presentation";
            return "file-text";
        }
    }
}
