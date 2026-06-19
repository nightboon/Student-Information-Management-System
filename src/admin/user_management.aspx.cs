using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using src.services.admin;

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
        protected string RoleOptionsHtml { get; private set; }
        protected string UserStatusOptionsHtml { get; private set; }
        protected string RoleFilterOptionsHtml { get; private set; }
        protected string UserStatusFilterOptionsHtml { get; private set; }
        protected AdminUserRow FirstUser { get; private set; }
        protected string FirstUserName { get { return Html(FirstUser.FullName); } }
        protected string FirstUserEmail { get { return Html(FirstUser.Email); } }
        protected string FirstUserPhone { get { return Html(FirstUser.Phone); } }
        protected string FirstUserProfileId { get { return Html(FirstUser.ProfileId); } }
        protected string FirstUserRole { get { return Html(FirstUser.Role); } }
        protected string FirstUserProgramme { get { return Html(FirstUser.Programme); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            var users = service.GetUsers();
            var lookups = service.GetLookups();
            ProgrammeOptionsHtml = AdminPortalService.RenderOptions(lookups.Programmes, "All programmes");
            ProgrammeDepartmentOptionsHtml = AdminPortalService.RenderOptions(
                lookups.Programmes.Concat(lookups.Departments), null);
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
                html.Append("<button type=\"button\" data-toast=\"Password reset email is not configured\" data-toast-type=\"info\" title=\"Reset password\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50\"><i data-lucide=\"key-round\" class=\"h-3.5 w-3.5\"></i></button>");
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
            EnsureAdmin();
            var id = new AdminPortalService().CreateUser(request);
            return new { ok = true, userId = id };
        }

        [WebMethod(EnableSession = true)]
        public static object SetUserStatus(int userId, string status)
        {
            EnsureAdmin();
            new AdminPortalService().SetUserStatus(userId, status);
            return new { ok = true };
        }

        [WebMethod(EnableSession = true)]
        public static object UpdateUser(AdminUserSaveRequest request)
        {
            EnsureAdmin();
            new AdminPortalService().UpdateUser(request);
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
