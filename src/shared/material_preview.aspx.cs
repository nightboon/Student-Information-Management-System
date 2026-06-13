using System;
using System.Web;
using src.services;

namespace src.shared
{
    public partial class material_preview : src.security.SecurePage
    {
        protected LecturerMaterialRow Material;
        protected string BackUrl = "~/shared/notifications.aspx";

        protected void Page_PreInit(object sender, EventArgs e)
        {
            string role = Session["role"] == null ? "" : Session["role"].ToString();
            MasterPageFile = string.Equals(role, "Lecturer", StringComparison.OrdinalIgnoreCase)
                ? "~/shared/LecturerLayout.master"
                : "~/shared/DashboardLayout.master";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int materialId;
            if (!int.TryParse(Request.QueryString["id"], out materialId))
            {
                ShowMissing();
                return;
            }

            int userId = Session["user_id"] == null ? 0 : (int)Session["user_id"];
            string role = Session["role"] == null ? "" : Session["role"].ToString();
            Material = LecturerPortalService.GetMaterialForPreview(materialId, userId, role);
            BackUrl = string.Equals(role, "Lecturer", StringComparison.OrdinalIgnoreCase)
                ? ResolveUrl("~/lecturer/lecturer_materials.aspx")
                : ResolveUrl("~/student/courses.aspx");

            if (Material == null)
            {
                ShowMissing();
                return;
            }

            string fileUrl = ResolveUrl(Material.FileUrl);
            downloadLink.NavigateUrl = fileUrl;
            downloadLink.Attributes["download"] = "";
            fileTypeLabel.Text = Html((Material.FileType ?? "file").ToUpperInvariant());
            if (CanPreviewInline(Material.FileType))
            {
                previewFrame.Attributes["src"] = fileUrl;
                previewFrame.Visible = true;
                limitedPreviewPanel.Visible = false;
            }
            else
            {
                previewFrame.Visible = false;
                previewFrame.Attributes.Remove("src");
                limitedPreviewPanel.Visible = true;
            }
            previewPanel.Visible = true;
        }

        private void ShowMissing()
        {
            missingPanel.Visible = true;
            previewPanel.Visible = false;
            downloadLink.Visible = false;
        }

        private static bool CanPreviewInline(string fileType)
        {
            string type = (fileType ?? "").ToLowerInvariant();
            return type == "pdf" || type == "png" || type == "jpg" || type == "jpeg" || type == "gif" || type == "txt" || type == "sql" || type == "mp4";
        }

        protected string Html(object value)
        {
            return HttpUtility.HtmlEncode(value == null ? "" : value.ToString());
        }
    }
}
