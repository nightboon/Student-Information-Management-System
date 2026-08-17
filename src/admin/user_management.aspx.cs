using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using src.services;
using src.services.admin;
using src.services.email;

namespace src.admin
{
    public partial class user_management : src.security.AdminPage
    {
        private readonly AdminPortalService service = new AdminPortalService();

        protected int TotalUsers { get; private set; }
        protected int StudentUsers { get; private set; }
        protected int LecturerUsers { get; private set; }
        protected int ActiveUsers { get; private set; }
        protected string UserRowsHtml { get; private set; }
        protected string ProgrammeOptionsHtml { get; private set; }
        protected string ProgrammeDepartmentOptionsHtml { get; private set; }
        protected string ProgrammeOnlyOptionsHtml { get; private set; }
        protected string DepartmentOnlyOptionsHtml { get; private set; }
        protected string RoleOptionsHtml { get; private set; }
        protected string UserStatusOptionsHtml { get; private set; }
        protected string RoleFilterOptionsHtml { get; private set; }
        protected string UserStatusFilterOptionsHtml { get; private set; }
        protected AdminUserRow FirstUser { get; private set; }
        protected StudentBulkPreview BulkPreview { get; private set; }
        protected string BulkImportMessage { get; private set; }
        protected bool ShowBulkImportModal { get; private set; }
        protected string BulkIntakeMessage { get { return BulkPreview == null ? "" : BulkPreview.IntakeMatchMessage; } }
        protected string BulkRegistrationDate { get { return BulkPreview == null ? "" : BulkPreview.RegistrationDate.ToString("dd MMM yyyy"); } }
        protected int BulkValidCount { get { return BulkPreview == null ? 0 : BulkPreview.ValidCount; } }
        protected int BulkErrorCount { get { return BulkPreview == null ? 0 : BulkPreview.ErrorCount; } }
        protected string FirstUserName { get { return Html(FirstUser.FullName); } }
        protected string FirstUserEmail { get { return Html(FirstUser.Email); } }
        protected string FirstUserPhone { get { return Html(FirstUser.Phone); } }
        protected string FirstUserProfileId { get { return Html(FirstUser.ProfileId); } }
        protected string FirstUserRole { get { return Html(FirstUser.Role); } }
        protected string FirstUserProgramme { get { return Html(FirstUser.Programme); } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            BulkPreview = Session["StudentBulkPreview"] as StudentBulkPreview;
            PopulateIntakeOptions();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Enctype = "multipart/form-data";
            BulkPreview = Session["StudentBulkPreview"] as StudentBulkPreview;
            ShowBulkImportModal = BulkPreview != null;
            ConfirmStudentImport.Enabled = BulkPreview != null && BulkPreview.ErrorCount == 0 &&
                                           !string.IsNullOrWhiteSpace(BulkPreview.IntakeId);
            if (!IsPostBack && int.TryParse(Request.QueryString["imported"], out var imported) && imported > 0)
                BulkImportMessage = imported + " student accounts were imported successfully.";
            var users = service.GetUsers();
            var lookups = service.GetLookups();
            ProgrammeOptionsHtml = AdminPortalService.RenderOptions(lookups.Programmes, "All programmes");
            ProgrammeDepartmentOptionsHtml = AdminPortalService.RenderOptions(
                lookups.Programmes.Concat(lookups.Departments), null);
            ProgrammeOnlyOptionsHtml = AdminPortalService.RenderOptions(lookups.Programmes, null);
            DepartmentOnlyOptionsHtml = AdminPortalService.RenderOptions(lookups.Departments, null);
            RoleOptionsHtml = AdminPortalService.RenderOptions(lookups.UserRoles, null);
            UserStatusOptionsHtml = AdminPortalService.RenderOptions(lookups.UserStatuses, null);
            RoleFilterOptionsHtml = AdminPortalService.RenderOptions(lookups.UserRoles, "All roles");
            UserStatusFilterOptionsHtml = AdminPortalService.RenderOptions(lookups.UserStatuses, "All statuses");
            FirstUser = users.FirstOrDefault() ?? new AdminUserRow();
            TotalUsers = users.Count;
            StudentUsers = users.Count(u => u.Role == "Student");
            LecturerUsers = users.Count(u => u.Role == "Lecturer");
            ActiveUsers = users.Count(u => u.Status == "Active");
            UserRowsHtml = BuildUserRows(users);
        }

        protected void UploadStudentFile_Click(object sender, EventArgs e)
        {
            ShowBulkImportModal = true;
            try
            {
                if (!StudentImportFile.HasFile) throw new ArgumentException("Choose an Excel .xlsx file.");
                if (!StudentImportFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Only .xlsx Excel files are supported.");
                if (StudentImportFile.PostedFile.ContentLength > 5 * 1024 * 1024)
                    throw new ArgumentException("The workbook must be smaller than 5 MB.");

                BulkPreview = new StudentBulkImportService().Prepare(
                    StudentImportFile.PostedFile.InputStream, DateTime.Now);
                Session["StudentBulkPreview"] = BulkPreview;
                PopulateIntakeOptions();
                ConfirmStudentImport.Enabled = BulkPreview.ErrorCount == 0 &&
                                               !string.IsNullOrWhiteSpace(BulkPreview.IntakeId);
            }
            catch (Exception ex)
            {
                BulkPreview = null;
                Session.Remove("StudentBulkPreview");
                BulkImportMessage = ex.Message;
            }
        }

        private void PopulateIntakeOptions()
        {
            if (BulkPreview == null || IntakeOverride == null) return;
            var selected = IntakeOverride.SelectedValue;
            IntakeOverride.Items.Clear();
            foreach (var intake in BulkPreview.IntakeOptions)
                IntakeOverride.Items.Add(new System.Web.UI.WebControls.ListItem(
                    intake.Name + " (" + intake.Id + ")", intake.Id));
            var preferred = string.IsNullOrWhiteSpace(selected) ? BulkPreview.IntakeId : selected;
            if (IntakeOverride.Items.FindByValue(preferred) != null)
                IntakeOverride.SelectedValue = preferred;
        }

        protected void ConfirmStudentImport_Click(object sender, EventArgs e)
        {
            ShowBulkImportModal = true;
            BulkPreview = Session["StudentBulkPreview"] as StudentBulkPreview;
            try
            {
                if (BulkPreview == null) throw new ArgumentException("Upload and validate the workbook first.");
                var result = new StudentBulkImportService().Import(BulkPreview.Rows, IntakeOverride.SelectedValue);
                foreach (var account in result.Created)
                {
                    var copy = account;
                    Task.Run(() => EmailService.SendNewAccount(
                        copy.PersonalEmail, copy.FullName, copy.InstitutionalEmail,
                        copy.TemporaryPassword, "Programme", copy.ProgrammeId,
                        "Imported in intake " + result.IntakeId + "."));
                }
                Session.Remove("StudentBulkPreview");
                Response.Redirect("user_management.aspx?imported=" + result.Created.Count, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                BulkImportMessage = ex.Message;
            }
        }

        protected string RenderBulkPreviewRows()
        {
            if (BulkPreview == null) return "";
            var html = new StringBuilder();
            foreach (var row in BulkPreview.Rows)
            {
                html.Append("<tr class=\"border-b border-slate-100\">")
                    .Append("<td class=\"px-3 py-2 text-slate-400\">").Append(row.RowNumber).Append("</td>")
                    .Append("<td class=\"px-3 py-2 font-medium text-slate-800\">").Append(Html(row.FullName)).Append("</td>")
                    .Append("<td class=\"px-3 py-2 text-slate-600\">").Append(Html(row.InstitutionalEmail)).Append("</td>")
                    .Append("<td class=\"px-3 py-2 text-slate-600\">").Append(Html(row.ProgrammeCode)).Append("</td>")
                    .Append("<td class=\"px-3 py-2\"><span class=\"inline-flex rounded-full px-2 py-0.5 ")
                    .Append(row.IsValid ? "bg-emerald-50 text-emerald-700" : "bg-red-50 text-red-700")
                    .Append("\">").Append(row.IsValid ? "Ready" : Html(string.Join(" ", row.Errors)))
                    .Append("</span></td></tr>");
            }
            return html.ToString();
        }

        protected string Number(int value)
        {
            return value.ToString("N0");
        }

        private static string BuildUserRows(IEnumerable<AdminUserRow> users)
        {
            var html = new StringBuilder();
            foreach (var user in users)
            {
                var search = (user.ProfileId + " " + user.FullName + " " + user.Email).ToLowerInvariant();
                var roleClass = user.Role == "Lecturer"
                    ? "bg-sky-50 text-sky-700 border-sky-100"
                    : "bg-slate-100 text-slate-600 border-slate-200";
                var statusClass = user.Status == "Active"
                    ? "bg-emerald-50 text-emerald-700 border-emerald-100"
                    : user.Status == "Pending"
                        ? "bg-amber-50 text-amber-700 border-amber-100"
                        : "bg-slate-100 text-slate-600 border-slate-200";
                var created = user.CreatedAt.HasValue ? user.CreatedAt.Value.ToString("dd MMM yyyy") : "-";

                html.Append("<tr data-row data-user-id=\"").Append(user.UserId).Append("\" data-profile-id=\"").Append(Attr(user.ProfileId)).Append("\" data-full-name=\"").Append(Attr(user.FullName)).Append("\" data-email=\"").Append(Attr(user.Email)).Append("\" data-phone=\"").Append(Attr(user.Phone)).Append("\" data-search=\"").Append(Attr(search)).Append("\" data-role=\"").Append(Attr(user.Role)).Append("\" data-status=\"").Append(Attr(user.Status)).Append("\" data-programme=\"").Append(Attr(user.Programme)).Append("\" class=\"border-b border-slate-100 hover:bg-slate-50/60 transition-colors\">");
                html.Append("<td class=\"px-6 py-3 text-slate-500\" style=\"font-size:12.5px\">").Append(Html(user.ProfileId)).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"text-slate-900 font-medium\">").Append(Html(user.FullName)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(user.Email)).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(user.Phone)).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 ").Append(roleClass).Append("\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(user.Role)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 ").Append(statusClass).Append("\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(user.Status)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(created).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-right\" style=\"font-size:12.5px\"><div class=\"flex items-center justify-end gap-1\">");
                html.Append("<button type=\"button\" data-admin-view-user data-drawer-open=\"view-user\" title=\"View\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50\"><i data-lucide=\"eye\" class=\"h-3.5 w-3.5\"></i></button>");
                html.Append("<button type=\"button\" data-admin-edit-user data-modal-open=\"edit-user\" title=\"Edit\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50\"><i data-lucide=\"pencil\" class=\"h-3.5 w-3.5\"></i></button>");
                html.Append("<button type=\"button\" data-admin-reset-password title=\"Reset password\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50\"><i data-lucide=\"key-round\" class=\"h-3.5 w-3.5\"></i></button>");
                html.Append("<button type=\"button\" data-admin-status data-user-id=\"").Append(user.UserId).Append("\" data-next-status=\"").Append(user.Status == "Active" ? "INACTIVE" : "ACTIVE").Append("\" data-user-name=\"").Append(Attr(user.FullName)).Append("\" title=\"").Append(user.Status == "Active" ? "Deactivate" : "Activate").Append("\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50\"><i data-lucide=\"power\" class=\"h-3.5 w-3.5\"></i></button>");
                html.Append("</div></td></tr>");
            }

            html.Append("<tr data-table-empty style=\"display:none\"><td colspan=\"8\" class=\"px-6 py-16 text-center text-slate-500\" style=\"font-size:13px\">No users match your filters.</td></tr>");
            return html.ToString();
        }

        private static string Html(string value) { return HttpUtility.HtmlEncode(value ?? ""); }
        private static string Attr(string value) { return HttpUtility.HtmlAttributeEncode(value ?? ""); }

        [WebMethod(EnableSession = true)]
        public static object CreateUser(AdminUserSaveRequest request)
        {
            try
            {
                EnsureAdmin();
                var id = new AdminPortalService().CreateUser(request);
                return new { ok = true, userId = id };
            }
            catch (Exception ex)
            {
                return new { ok = false, message = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object SetUserStatus(int userId, string status)
        {
            return RunAdminAction(() => new AdminPortalService().SetUserStatus(userId, status));
        }

        [WebMethod(EnableSession = true)]
        public static object UpdateUser(AdminUserSaveRequest request)
        {
            return RunAdminAction(() => new AdminPortalService().UpdateUser(request));
        }

        private static object RunAdminAction(Action action)
        {
            try
            {
                EnsureAdmin();
                action();
                return new { ok = true };
            }
            catch (Exception ex)
            {
                return new { ok = false, message = ex.Message };
            }
        }

        // Emails the selected user a password reset link, reusing the same
        // token + email services as the public "Forgot password" flow.
        [WebMethod(EnableSession = true)]
        public static object ResetUserPassword(string email)
        {
            EnsureAdmin();
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Missing user email.");
            email = email.Trim();

            var issued = PasswordResetService.CreateToken(email);
            if (!issued.Found) throw new ArgumentException("No account found for this user.");

            // Apply ToAbsolute to the path only (it rejects a query string), then append the token.
            var resetPath = VirtualPathUtility.ToAbsolute("~/login/reset_password.aspx");
            var resetUrl = new Uri(HttpContext.Current.Request.Url, resetPath).ToString()
                + "?token=" + issued.RawToken;

            var sent = EmailService.SendPasswordReset(email, issued.Username, resetUrl);
            if (!sent.Success) throw new ApplicationException("Could not send the reset email.");
            return new { ok = true };
        }

        private static void EnsureAdmin()
        {
            var ctx = HttpContext.Current;
            if (ctx == null || ctx.Session == null || ctx.Session["user_id"] == null ||
                !string.Equals(ctx.Session["role"] as string, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpException(403, "Admin session required.");
            }
        }

    }
}
