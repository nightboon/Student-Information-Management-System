using System;
using System.Web;
using System.Web.UI;
using src.services;

namespace src.lecturer
{
    public partial class lecturer_account : src.security.LecturerPage
    {
        private LecturerProfile _lecturer;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["user_id"] == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            var user = UserContextFactory.FromSession(Session);
            _lecturer = LecturerPortalService.GetProfile(user);
            if (_lecturer == null)
            {
                // Session has no lecturer profile — nothing to show here.
                Response.Redirect("~/shared/login.aspx");
                return;
            }
        }

        protected string FullName
        {
            get { return _lecturer != null ? _lecturer.FullName : ""; }
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

        protected string Department
        {
            get { return _lecturer != null ? _lecturer.DepartmentId : ""; }
        }

        protected string LecturerIdLabel
        {
            get { return _lecturer != null ? _lecturer.LecturerId : ""; }
        }

        protected string Email
        {
            get { return _lecturer != null ? _lecturer.Email : ""; }
        }

        protected string Phone
        {
            get { return _lecturer != null ? _lecturer.Phone : ""; }
        }

        protected string MailingAddress
        {
            get { return _lecturer != null ? _lecturer.MailingAddress : ""; }
        }

        protected string RoleBadge
        {
            get { return (Session["role"] as string ?? "LECTURER").ToUpperInvariant(); }
        }

        // App-resolved URL of the profile image, or "" when none is set.
        protected string IconUrl
        {
            get
            {
                if (_lecturer == null || string.IsNullOrEmpty(_lecturer.IconPath)) return "";
                return ResolveUrl("~/" + _lecturer.IconPath.TrimStart('/'));
            }
        }
    }
}
