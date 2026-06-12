using System;
using System.Web.UI;
using src.services;

namespace src.controls
{
    public partial class admin_topbar : UserControl
    {
        private UserProfile _profile;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] == null) return;
            _profile = UserAccountService.GetProfile((int)Session["user_id"]);
        }

        protected string DisplayName
        {
            get { return _profile != null ? _profile.Username : (Session["username"] as string ?? "Administrator"); }
        }

        protected string RoleLabel
        {
            get { return "Administrator"; }
        }

        // App-resolved URL of the profile image, or "" when none is set (the
        // markup then falls back to the initials avatar).
        protected string IconUrl
        {
            get
            {
                if (_profile == null || string.IsNullOrEmpty(_profile.IconPath)) return "";
                return ResolveUrl("~/" + _profile.IconPath.TrimStart('/'));
            }
        }

        // First letters of the first two words of the name, e.g. "Admin One" -> "AO".
        protected string Initials
        {
            get
            {
                var name = DisplayName;
                if (string.IsNullOrEmpty(name)) return "";
                var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string initials = parts[0].Substring(0, 1);
                if (parts.Length > 1) initials += parts[1].Substring(0, 1);
                return initials.ToUpperInvariant();
            }
        }
    }
}
