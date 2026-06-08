using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using src.services;

namespace student_information_management_system
{
    public partial class lecturer_announcements : src.security.LecturerPage
    {
        private Lecturer _lecturer;
        private List<LecturerAnnouncementRow> _announcements = new List<LecturerAnnouncementRow>();

        protected void Page_Load(object sender, EventArgs e)
        {
            _lecturer = Session["user_id"] != null ? LecturerService.GetByUserId((int)Session["user_id"]) : null;
            if (_lecturer == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                var courses = LecturerCourseService.GetCourses(_lecturer.UserId);
                courseSelect.DataSource = courses;
                courseSelect.DataTextField = "CourseCode";
                courseSelect.DataValueField = "OfferingId";
                courseSelect.DataBind();
                courseSelect.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All assigned courses", "0"));
            }
            LoadRows();
        }

        protected void PostAnnouncement_Click(object sender, EventArgs e)
        {
            int offeringId;
            int.TryParse(courseSelect.SelectedValue, out offeringId);
            LecturerPortalService.AddAnnouncement(_lecturer.LecturerId, _lecturer.UserId, offeringId, titleInput.Text, messageInput.Text, pinnedInput.Checked);
            titleInput.Text = "";
            messageInput.Text = "";
            pinnedInput.Checked = false;
            statusMessage.Text = "Announcement posted to students.";
            statusBanner.Visible = true;
            LoadRows();
        }

        private void LoadRows()
        {
            _announcements = LecturerPortalService.GetLecturerAnnouncements(_lecturer.LecturerId);
            announcementsRepeater.DataSource = _announcements;
            announcementsRepeater.DataBind();
            emptyPanel.Visible = _announcements.Count == 0;
        }

        protected string Html(object value)
        {
            return HttpUtility.HtmlEncode(value == null ? "" : value.ToString());
        }

        protected string RelativeTime(object value)
        {
            var when = (DateTime)value;
            TimeSpan ago = DateTime.Now - when;
            if (ago.TotalHours < 1) return "Just now";
            if (ago.TotalHours < 24) return ((int)ago.TotalHours).ToString(CultureInfo.InvariantCulture) + "h ago";
            if (ago.TotalDays < 2) return "Yesterday";
            return when.ToString("d MMM yyyy", CultureInfo.InvariantCulture);
        }
    }
}
