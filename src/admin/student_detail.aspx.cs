using System;
using System.Web;
using src.services.admin;

namespace src.admin
{
    public partial class student_detail : src.security.AdminPage
    {
        private readonly AdminPortalService service = new AdminPortalService();

        protected AdminStudentDetailSummary Student { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Student = service.GetStudentDetail(Request.QueryString["id"]);
        }

        protected string Html(string value) { return HttpUtility.HtmlEncode(value ?? ""); }
        protected string Initials()
        {
            if (Student == null || string.IsNullOrWhiteSpace(Student.Name)) return "ST";
            var parts = Student.Name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpperInvariant();
            return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpperInvariant();
        }
        protected string DecimalNumber(decimal value) { return value.ToString("0.00"); }
        protected string Percent(decimal value) { return value.ToString("0.0") + "%"; }
    }
}
