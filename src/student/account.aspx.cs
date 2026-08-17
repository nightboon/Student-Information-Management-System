using System;
using System.Globalization;
using System.Web;
using System.Web.Services;
using src.services;

namespace src.shared
{
    public partial class account : src.security.StudentPage
    {
        private StudentAccountProfile _student;

        [WebMethod(EnableSession = true)]
        public static object ChangePassword(string currentPassword, string newPassword)
        {
            return AccountWebMethods.ChangePassword(currentPassword, newPassword);
        }

        [WebMethod(EnableSession = true)]
        public static object SaveProfile(string phone, string mailingAddress)
        {
            return AccountWebMethods.SaveProfile(phone, mailingAddress);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["user_id"] == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }

            var user = UserContextFactory.FromSession(Session);
            _student = StudentPortalService.GetAccount(user);
            if (_student == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }
        }

        protected string FullName
        {
            get { return _student != null ? _student.FullName : ""; }
        }

        protected string Initials
        {
            get
            {
                if (_student == null || string.IsNullOrEmpty(_student.FullName)) return "";
                var parts = _student.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string initials = parts[0].Substring(0, 1);
                if (parts.Length > 1) initials += parts[1].Substring(0, 1);
                return initials.ToUpperInvariant();
            }
        }

        protected string ProgrammeName
        {
            get { return _student != null ? _student.ProgrammeName : ""; }
        }

        protected string StudentIdLabel
        {
            get { return _student != null ? _student.StudentId : ""; }
        }

        protected string Email
        {
            get { return _student != null ? _student.Email : ""; }
        }

        protected string StatusBadge
        {
            get
            {
                return _student != null && !string.IsNullOrEmpty(_student.Status)
                    ? _student.Status.ToUpperInvariant()
                    : "";
            }
        }

        protected string IntakeLabel
        {
            get
            {
                return _student != null && _student.IntakeDate.HasValue
                    ? _student.IntakeDate.Value.ToString("MMM yyyy", CultureInfo.InvariantCulture)
                    : "-";
            }
        }

        protected string StandingLabel
        {
            get
            {
                if (_student == null) return "";
                int n = Math.Max(1, _student.CurrentSemesterNo);
                int perYear = _student.SemestersPerYear > 0 ? _student.SemestersPerYear : 3;
                int year = (n + perYear - 1) / perYear;
                return "Year " + year + " &middot; Semester " + n;
            }
        }

        protected string Phone
        {
            get { return _student != null ? _student.Phone : ""; }
        }

        protected string MailingAddress
        {
            get { return _student != null ? _student.MailingAddress : ""; }
        }

        protected string IconUrl
        {
            get
            {
                if (_student == null || string.IsNullOrEmpty(_student.IconPath)) return "";
                return ResolveUrl("~/" + _student.IconPath.TrimStart('/'));
            }
        }
    }
}
