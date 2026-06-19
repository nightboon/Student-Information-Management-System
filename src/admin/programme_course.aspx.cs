using System;
using System.Text;
using System.Web;
using System.Web.Services;
using src.services.admin;

namespace src.admin
{
    public partial class programme_course : src.security.AdminPage
    {
        private readonly AdminPortalService service = new AdminPortalService();

        protected string ProgrammeRowsHtml { get; private set; }
        protected string CourseRowsHtml { get; private set; }
        protected string AssignmentRowsHtml { get; private set; }
        protected string ProgrammeOptionsHtml { get; private set; }
        protected string EducationLevelOptionsHtml { get; private set; }
        protected string ProgrammeStatusOptionsHtml { get; private set; }
        protected string CourseStatusOptionsHtml { get; private set; }
        protected string SemesterOptionsHtml { get; private set; }
        protected string LecturerOptionsHtml { get; private set; }
        protected string ProgrammeStatusFilterOptionsHtml { get; private set; }
        protected string CourseStatusFilterOptionsHtml { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ProgrammeRowsHtml = BuildProgrammeRows();
            CourseRowsHtml = BuildCourseRows();
            AssignmentRowsHtml = BuildAssignmentRows();
            var lookups = service.GetLookups();
            ProgrammeOptionsHtml = AdminPortalService.RenderOptions(lookups.Programmes, null);
            EducationLevelOptionsHtml = AdminPortalService.RenderOptions(lookups.EducationLevels, null);
            ProgrammeStatusOptionsHtml = AdminPortalService.RenderOptions(lookups.ProgrammeStatuses, null);
            CourseStatusOptionsHtml = AdminPortalService.RenderOptions(lookups.CourseStatuses, null);
            SemesterOptionsHtml = AdminPortalService.RenderOptions(lookups.Semesters, null);
            LecturerOptionsHtml = AdminPortalService.RenderOptions(lookups.Lecturers, "Select lecturer...");
            ProgrammeStatusFilterOptionsHtml = AdminPortalService.RenderOptions(lookups.ProgrammeStatuses, "All statuses");
            CourseStatusFilterOptionsHtml = AdminPortalService.RenderOptions(lookups.CourseStatuses, "All statuses");
        }

        private string BuildProgrammeRows()
        {
            var html = new StringBuilder();
            foreach (var p in service.GetProgrammes())
            {
                html.Append("<tr data-row data-code=\"").Append(Attr(p.Code)).Append("\" data-name=\"").Append(Attr(p.Name)).Append("\" data-level=\"").Append(Attr(p.Level)).Append("\" data-duration=\"").Append(Attr(p.Duration)).Append("\" data-semesters=\"").Append(p.Semesters).Append("\" data-search=\"").Append(Attr((p.Code + " " + p.Name).ToLowerInvariant())).Append("\" data-status=\"").Append(Attr(p.Status)).Append("\" class=\"border-b border-slate-100 hover:bg-slate-50/60\">");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"text-slate-900 font-medium\">").Append(Html(p.Code)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(p.Name)).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(p.Level)).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(p.Duration)).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-right\" style=\"font-size:12.5px\">").Append(p.Semesters).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-right\" style=\"font-size:12.5px\">").Append(p.CourseCount).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-right\" style=\"font-size:12.5px\">").Append(p.StudentCount).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 ").Append(StatusBadge(p.Status)).Append("\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(p.Status)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-right\" style=\"font-size:12.5px\"><div class=\"flex items-center justify-end gap-1\"><button type=\"button\" data-admin-edit-programme data-modal-open=\"prog-modal\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50\"><i data-lucide=\"pencil\" class=\"h-3.5 w-3.5\"></i></button><button type=\"button\" data-admin-delete-programme data-code=\"").Append(Attr(p.Code)).Append("\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-[#e0162b] hover:bg-[#e0162b]/5\"><i data-lucide=\"trash-2\" class=\"h-3.5 w-3.5\"></i></button></div></td></tr>");
            }
            html.Append("<tr data-table-empty style=\"display:none\"><td colspan=\"9\" class=\"px-6 py-12 text-center text-slate-400\" style=\"font-size:13px\">No programmes match your filters.</td></tr>");
            return html.ToString();
        }

        private string BuildCourseRows()
        {
            var html = new StringBuilder();
            foreach (var c in service.GetCourseCatalog())
            {
                html.Append("<tr data-row data-code=\"").Append(Attr(c.Code)).Append("\" data-name=\"").Append(Attr(c.Name)).Append("\" data-programme=\"").Append(Attr(c.Programme)).Append("\" data-credit-hours=\"").Append(c.CreditHours).Append("\" data-prerequisites=\"").Append(Attr(c.Prerequisites)).Append("\" data-search=\"").Append(Attr((c.Code + " " + c.Name).ToLowerInvariant())).Append("\" data-status=\"").Append(Attr(c.Status)).Append("\" class=\"border-b border-slate-100 hover:bg-slate-50/60\">");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"text-slate-900 font-medium\">").Append(Html(c.Code)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(c.Name)).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(c.Programme)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-right\" style=\"font-size:12.5px\">").Append(c.CreditHours).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(c.Prerequisites)).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(c.Lecturer)).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 ").Append(StatusBadge(c.Status)).Append("\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(c.Status)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-right\" style=\"font-size:12.5px\"><div class=\"flex items-center justify-end gap-1\"><button type=\"button\" data-admin-edit-course data-modal-open=\"course-modal\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50\"><i data-lucide=\"pencil\" class=\"h-3.5 w-3.5\"></i></button><button type=\"button\" data-admin-delete-course data-code=\"").Append(Attr(c.Code)).Append("\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-[#e0162b] hover:bg-[#e0162b]/5\"><i data-lucide=\"trash-2\" class=\"h-3.5 w-3.5\"></i></button></div></td></tr>");
            }
            html.Append("<tr data-table-empty style=\"display:none\"><td colspan=\"8\" class=\"px-6 py-12 text-center text-slate-400\" style=\"font-size:13px\">No courses match your filters.</td></tr>");
            return html.ToString();
        }

        private string BuildAssignmentRows()
        {
            var html = new StringBuilder();
            foreach (var c in service.GetCourseMetrics())
            {
                html.Append("<tr data-row data-offer-id=\"").Append(c.OfferId).Append("\" data-programme=\"").Append(Attr(c.Programme)).Append("\" data-semester=\"").Append(Attr(c.OfferingSemester)).Append("\" data-course-code=\"").Append(Attr(c.Code)).Append("\" data-title=\"").Append(Attr(c.Title)).Append("\" data-credit=\"").Append(c.CreditHours).Append("\" data-lecturer=\"").Append(Attr(c.Lecturer)).Append("\" data-search=\"").Append(Attr((c.Programme + " " + c.Code + " " + c.Title + " " + c.Lecturer).ToLowerInvariant())).Append("\" class=\"border-b border-slate-100 hover:bg-slate-50/60\">");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"inline-flex items-center gap-1 rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200\" style=\"font-size:11.5px;font-weight:600\">").Append(Html(c.Programme)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(c.OfferingSemester)).Append("</td>");
                html.Append("<td class=\"px-6 py-3\" style=\"font-size:12.5px\"><span class=\"text-slate-900 font-medium\">").Append(Html(c.Code)).Append("</span></td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(c.Title)).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700 text-right\" style=\"font-size:12.5px\">").Append(c.CreditHours).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-slate-700\" style=\"font-size:12.5px\">").Append(Html(c.Lecturer)).Append("</td>");
                html.Append("<td class=\"px-6 py-3 text-right\" style=\"font-size:12.5px\"><div class=\"flex items-center justify-end gap-1\"><button type=\"button\" data-admin-edit-assignment data-modal-open=\"assign-modal\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50\"><i data-lucide=\"pencil\" class=\"h-3.5 w-3.5\"></i></button><button type=\"button\" data-admin-delete-assignment data-offer-id=\"").Append(c.OfferId).Append("\" class=\"inline-flex h-7 w-7 items-center justify-center rounded-md border border-slate-200 bg-white text-[#e0162b] hover:bg-[#e0162b]/5\"><i data-lucide=\"trash-2\" class=\"h-3.5 w-3.5\"></i></button></div></td></tr>");
            }
            return html.ToString();
        }

        private static string StatusBadge(string status)
        {
            return status == "Active" ? "bg-emerald-50 text-emerald-700 border-emerald-100" : "bg-slate-100 text-slate-600 border-slate-200";
        }

        private static string Html(string value) { return HttpUtility.HtmlEncode(value ?? ""); }
        private static string Attr(string value) { return HttpUtility.HtmlAttributeEncode(value ?? ""); }

        [WebMethod(EnableSession = true)]
        public static object SaveProgramme(AdminProgrammeSaveRequest request)
        {
            EnsureAdmin();
            new AdminPortalService().SaveProgramme(request);
            return new { ok = true };
        }

        [WebMethod(EnableSession = true)]
        public static object SaveCourse(AdminCourseSaveRequest request)
        {
            EnsureAdmin();
            new AdminPortalService().SaveCourse(request);
            return new { ok = true };
        }

        [WebMethod(EnableSession = true)]
        public static object DeleteProgramme(string code)
        {
            EnsureAdmin();
            new AdminPortalService().DeleteProgramme(code);
            return new { ok = true };
        }

        [WebMethod(EnableSession = true)]
        public static object DeleteCourse(string code)
        {
            EnsureAdmin();
            new AdminPortalService().DeleteCourse(code);
            return new { ok = true };
        }

        [WebMethod(EnableSession = true)]
        public static object SaveCourseAssignment(AdminCourseAssignmentSaveRequest request)
        {
            EnsureAdmin();
            new AdminPortalService().SaveCourseAssignment(request);
            return new { ok = true };
        }

        [WebMethod(EnableSession = true)]
        public static object DeleteCourseAssignment(int offerId)
        {
            EnsureAdmin();
            new AdminPortalService().DeleteCourseAssignment(offerId);
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
