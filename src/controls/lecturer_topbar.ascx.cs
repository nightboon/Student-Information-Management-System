using System;
using System.Web.UI;
using src.services;

namespace src.controls
{
    public partial class lecturer_topbar : UserControl
    {
        private LecturerProfile _lecturer;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] == null) return;
            _lecturer = LecturerPortalService.GetProfile(UserContextFactory.FromSession(Session));
        }

        protected string FullName
        {
            get { return _lecturer != null ? _lecturer.FullName : (Session["username"] as string ?? "Lecturer"); }
        }

        protected string Subtitle
        {
            get { return _lecturer != null ? _lecturer.DepartmentId : "Lecturer"; }
        }

        // App-resolved URL of the profile image, or "" when none is set (the
        // markup then falls back to the initials avatar).
        protected string IconUrl
        {
            get
            {
                if (_lecturer == null || string.IsNullOrEmpty(_lecturer.IconPath)) return "";
                return ResolveUrl("~/" + _lecturer.IconPath.TrimStart('/'));
            }
        }

        // First letters of the first two words of the name, e.g. "Dr. Sarah Tan" -> "DS".
        protected string Initials
        {
            get
            {
                if (_lecturer == null || string.IsNullOrEmpty(_lecturer.FullName)) return "";
                var parts = _lecturer.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string initials = parts[0].Substring(0, 1);
                if (parts.Length > 1) initials += parts[1].Substring(0, 1);
                return initials.ToUpperInvariant();
            }
        }
    }
}
