using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using src.services;

namespace student_information_management_system
{
    public partial class lecturer_announcements : src.security.LecturerPage
    {
        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg", ".gif", ".webp", ".txt", ".zip"
        };

        private Lecturer _lecturer;
        private List<LecturerAnnouncementRow> _announcements = new List<LecturerAnnouncementRow>();
        private LecturerAnnouncementRow _selectedAnnouncement;
        private int? _offeringFilter;
        private string _tabFilter = "all";

        protected bool IsCourseScoped { get { return _offeringFilter.HasValue; } }
        protected int SelectedOfferingId { get { return _offeringFilter.GetValueOrDefault(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Form != null)
            {
                Page.Form.Enctype = "multipart/form-data";
                Page.Form.Action = Request.RawUrl;
            }

            _lecturer = Session["user_id"] != null ? LecturerService.GetByUserId((int)Session["user_id"]) : null;
            if (_lecturer == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            int offeringId;
            if (int.TryParse(Request.QueryString["offering"], out offeringId) && offeringId > 0)
                _offeringFilter = offeringId;

            _tabFilter = (ViewState["AnnouncementTab"] as string) ?? "all";

            if (!IsPostBack)
            {
                BindCourses();
                SetComposeVisible(false);
            }

            showComposeButton.Visible = IsCourseScoped;
            composePanel.Visible = IsCourseScoped;
            LoadRows();
        }

        protected void ShowComposeButton_Click(object sender, EventArgs e)
        {
            if (!IsCourseScoped) return;
            SetComposeVisible(true);
        }

        protected void CloseComposeButton_Click(object sender, EventArgs e)
        {
            SetComposeVisible(false);
        }

        protected void PostAnnouncement_Click(object sender, EventArgs e)
        {
            if (!IsCourseScoped)
            {
                ShowError("Choose a course before posting an announcement.");
                SetComposeVisible(false);
                return;
            }

            if (string.IsNullOrWhiteSpace(titleInput.Text) || string.IsNullOrWhiteSpace(messageInput.Text))
            {
                ShowError("Add a title and message before posting.");
                SetComposeVisible(true);
                return;
            }

            int offeringId;
            int.TryParse(courseSelect.SelectedValue, out offeringId);

            string fileUrl = SaveAttachment();
            if (statusBanner.Visible && string.IsNullOrEmpty(fileUrl) && attachmentInput.HasFile)
            {
                SetComposeVisible(true);
                return;
            }

            LecturerPortalService.AddAnnouncement(_lecturer.LecturerId, _lecturer.UserId, offeringId, titleInput.Text, messageInput.Text, pinnedInput.Checked, fileUrl);
            titleInput.Text = "";
            messageInput.Text = "";
            pinnedInput.Checked = false;
            statusMessage.Text = "Announcement posted to students.";
            statusBanner.CssClass = "mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800";
            statusBanner.Visible = true;
            SetComposeVisible(false);
            LoadRows();
        }

        protected void AnnouncementsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "SelectAnnouncement") return;

            int announcementId;
            if (int.TryParse(e.CommandArgument.ToString(), out announcementId))
            {
                ViewState["SelectedAnnouncementId"] = announcementId;
                LoadRows();
            }
        }

        protected void CourseFilterSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            int offeringId;
            if (int.TryParse(courseFilterSelect.SelectedValue, out offeringId) && offeringId > 0)
            {
                Response.Redirect("~/lecturer/lecturer_announcement.aspx?offering=" + offeringId);
                return;
            }

            Response.Redirect("~/lecturer/lecturer_announcement.aspx");
        }

        protected void TabButton_Command(object sender, CommandEventArgs e)
        {
            string tab = e.CommandArgument == null ? "all" : e.CommandArgument.ToString();
            if (tab != "pinned" && tab != "files") tab = "all";

            _tabFilter = tab;
            ViewState["AnnouncementTab"] = tab;
            ViewState["SelectedAnnouncementId"] = null;
            LoadRows();
        }

        protected void PinButton_Click(object sender, EventArgs e)
        {
            if (_selectedAnnouncement == null) return;

            LecturerPortalService.SetAnnouncementPinned(_lecturer.LecturerId, _selectedAnnouncement.AnnouncementId, !_selectedAnnouncement.IsPinned);
            statusMessage.Text = _selectedAnnouncement.IsPinned ? "Announcement unpinned." : "Announcement pinned.";
            statusBanner.CssClass = "mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800";
            statusBanner.Visible = true;
            LoadRows();
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            if (_selectedAnnouncement == null) return;

            LecturerPortalService.DeleteAnnouncement(_lecturer.LecturerId, _selectedAnnouncement.AnnouncementId);
            ViewState["SelectedAnnouncementId"] = null;
            statusMessage.Text = "Announcement deleted.";
            statusBanner.CssClass = "mt-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-800";
            statusBanner.Visible = true;
            LoadRows();
        }

        private void BindCourses()
        {
            var courses = LecturerCourseService.GetCourses(_lecturer.UserId);
            var courseOptions = courses.Select(c => new
            {
                c.OfferingId,
                Label = c.CourseCode + " - " + c.CourseName
            }).ToList();

            courseSelect.DataSource = courseOptions;
            courseSelect.DataTextField = "Label";
            courseSelect.DataValueField = "OfferingId";
            courseSelect.DataBind();
            if (!_offeringFilter.HasValue)
                courseSelect.Items.Insert(0, new ListItem("All assigned courses", "0"));

            courseFilterSelect.DataSource = courseOptions;
            courseFilterSelect.DataTextField = "Label";
            courseFilterSelect.DataValueField = "OfferingId";
            courseFilterSelect.DataBind();
            courseFilterSelect.Items.Insert(0, new ListItem("All courses", "0"));

            if (_offeringFilter.HasValue)
            {
                var value = _offeringFilter.Value.ToString(CultureInfo.InvariantCulture);
                if (courseSelect.Items.FindByValue(value) != null)
                    courseSelect.SelectedValue = value;
                if (courseFilterSelect.Items.FindByValue(value) != null)
                    courseFilterSelect.SelectedValue = value;
            }
        }

        private void LoadRows()
        {
            _announcements = LecturerPortalService.GetLecturerAnnouncements(_lecturer.LecturerId, _offeringFilter);
            if (_tabFilter == "pinned")
                _announcements = _announcements.Where(a => a.IsPinned).ToList();
            else if (_tabFilter == "files")
                _announcements = _announcements.Where(a => a.HasAttachment).ToList();

            int selectedId = SelectedAnnouncementId();
            if (selectedId == 0 && _announcements.Count > 0)
            {
                selectedId = _announcements[0].AnnouncementId;
                ViewState["SelectedAnnouncementId"] = selectedId;
            }

            _selectedAnnouncement = _announcements.FirstOrDefault(a => a.AnnouncementId == selectedId);
            announcementsRepeater.DataSource = _announcements;
            announcementsRepeater.DataBind();
            emptyPanel.Visible = _announcements.Count == 0;
            detailPanel.Visible = _selectedAnnouncement != null;
            ApplyTabStyles();
        }

        private int SelectedAnnouncementId()
        {
            object value = ViewState["SelectedAnnouncementId"];
            if (value == null) return 0;
            int id;
            return int.TryParse(value.ToString(), out id) ? id : 0;
        }

        private string SaveAttachment()
        {
            if (!attachmentInput.HasFile) return null;

            string extension = Path.GetExtension(attachmentInput.FileName);
            if (!AllowedExtensions.Contains(extension))
            {
                ShowError("Attachment type is not supported.");
                return null;
            }

            const int maxBytes = 10 * 1024 * 1024;
            if (attachmentInput.PostedFile.ContentLength > maxBytes)
            {
                ShowError("Attachment must be 10 MB or smaller.");
                return null;
            }

            string folder = Server.MapPath("~/uploads/announcements");
            Directory.CreateDirectory(folder);
            string safeName = Path.GetFileNameWithoutExtension(attachmentInput.FileName);
            foreach (char invalid in Path.GetInvalidFileNameChars())
                safeName = safeName.Replace(invalid, '-');
            if (string.IsNullOrWhiteSpace(safeName)) safeName = "attachment";

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + "-" + Guid.NewGuid().ToString("N").Substring(0, 8) + "-" + safeName + extension.ToLowerInvariant();
            string physicalPath = Path.Combine(folder, fileName);
            attachmentInput.SaveAs(physicalPath);
            return "~/uploads/announcements/" + fileName;
        }

        private void ShowError(string message)
        {
            statusMessage.Text = message;
            statusBanner.CssClass = "mt-4 rounded-md border border-rose-200 bg-rose-50 px-4 py-3 text-rose-800";
            statusBanner.Visible = true;
        }

        private void SetComposeVisible(bool visible)
        {
            const string hidden = " hidden";
            if (visible)
            {
                composePanel.CssClass = composePanel.CssClass.Replace(hidden, "");
                return;
            }

            if (!composePanel.CssClass.Contains("hidden"))
                composePanel.CssClass += hidden;
        }

        private void ApplyTabStyles()
        {
            SetTabStyle(allTabButton, _tabFilter == "all");
            SetTabStyle(pinnedTabButton, _tabFilter == "pinned");
            SetTabStyle(filesTabButton, _tabFilter == "files");
        }

        private static void SetTabStyle(LinkButton button, bool active)
        {
            button.CssClass = active
                ? "rounded px-2 py-1.5 text-center bg-white text-slate-900 shadow-sm"
                : "rounded px-2 py-1.5 text-center text-slate-500 hover:text-slate-900";
        }

        protected string Html(object value)
        {
            return HttpUtility.HtmlEncode(value == null ? "" : value.ToString());
        }

        protected string SelectedClass(object value)
        {
            int id;
            int.TryParse(value.ToString(), out id);
            string selected = id == SelectedAnnouncementId() ? " bg-rose-50/60" : "";
            return selected;
        }

        protected string RelativeTime(object value)
        {
            var when = (DateTime)value;
            TimeSpan ago = DateTime.Now - when;
            if (ago.TotalMinutes < 1) return "Now";
            if (ago.TotalHours < 1) return ((int)ago.TotalMinutes).ToString(CultureInfo.InvariantCulture) + "m";
            if (ago.TotalHours < 24) return ((int)ago.TotalHours).ToString(CultureInfo.InvariantCulture) + "h";
            if (ago.TotalDays < 2) return "Yesterday";
            return when.ToString("d MMM", CultureInfo.InvariantCulture);
        }

        protected string DetailPinnedLabel
        {
            get { return _selectedAnnouncement != null && _selectedAnnouncement.IsPinned ? "PINNED" : "ANNOUNCEMENT"; }
        }

        protected string DetailTargetCourses
        {
            get { return Html(_selectedAnnouncement == null ? "" : _selectedAnnouncement.TargetCourses); }
        }

        protected string DetailTitle
        {
            get { return Html(_selectedAnnouncement == null ? "Select an announcement" : _selectedAnnouncement.Title); }
        }

        protected string DetailCreatedAt
        {
            get
            {
                return _selectedAnnouncement == null
                    ? ""
                    : Html(_selectedAnnouncement.CreatedAt.ToString("d MMM yyyy - h:mm tt", CultureInfo.InvariantCulture));
            }
        }

        protected string DetailDateOnly
        {
            get
            {
                return _selectedAnnouncement == null
                    ? ""
                    : Html(_selectedAnnouncement.CreatedAt.ToString("d MMM yyyy", CultureInfo.InvariantCulture));
            }
        }

        protected string DetailContent
        {
            get { return Html(_selectedAnnouncement == null ? "" : _selectedAnnouncement.Content); }
        }

        protected bool DetailHasAttachment
        {
            get { return _selectedAnnouncement != null && _selectedAnnouncement.HasAttachment; }
        }

        protected string DetailAttachmentUrl
        {
            get
            {
                return DetailHasAttachment
                    ? ResolveUrl(_selectedAnnouncement.FileUrl)
                    : "#";
            }
        }

        protected string DetailAttachmentLabel
        {
            get
            {
                if (!DetailHasAttachment) return "None";
                return Html(Path.GetFileName(_selectedAnnouncement.FileUrl));
            }
        }
    }
}
