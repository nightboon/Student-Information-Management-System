using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using src.services;

namespace src.lecturer
{
    public partial class lecturer_courses : src.security.LecturerPage
    {
        private List<LecturerCourse> _courses;

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

            int userId = (int)Session["user_id"];
            _courses = LecturerCourseService.GetCourses(userId);

            coursesRepeater.DataSource = _courses;
            coursesRepeater.DataBind();

            // Filter tabs are data-driven: one per distinct semester the lecturer
            // teaches in, newest first (the service already orders by start_date).
            semesterRepeater.DataSource = _courses
                .Select(c => c.SemesterName)
                .Distinct()
                .ToList();
            semesterRepeater.DataBind();
        }

        /// <summary>Total number of courses the lecturer teaches (all semesters).</summary>
        protected int CourseCount
        {
            get { return _courses != null ? _courses.Count : 0; }
        }

        /// <summary>
        /// Course accent color from the DB. Returns a neutral slate fallback when
        /// unset or when the stored value is not a plain hex color, so a malformed
        /// value can never break out of the inline style attribute it is written into.
        /// </summary>
        protected string AccentColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return "#64748b";
            // Require a 6-digit hex (#rrggbb): the icon tint appends an alpha suffix
            // ("...15"), which only forms valid CSS for a 6-digit color.
            return System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$")
                ? color : "#64748b";
        }

        /// <summary>Tailwind classes for the teaching-status badge.</summary>
        protected string StatusBadgeClass(string status)
        {
            switch (status)
            {
                case "Completed": return "bg-emerald-50 text-emerald-700";
                case "Upcoming": return "bg-blue-50 text-blue-700";
                default: return "bg-[#e0162b]/10 text-[#a01020]"; // In progress
            }
        }

        /// <summary>Lucide icon name for the teaching-status badge.</summary>
        protected string StatusIcon(string status)
        {
            switch (status)
            {
                case "Completed": return "check-circle-2";
                case "Upcoming": return "clock";
                default: return "circle-dot"; // In progress
            }
        }

        /// <summary>Lowercased "code name" string used by the client-side search filter.</summary>
        protected string SearchKey(string code, string name)
        {
            return ((code ?? "") + " " + (name ?? "")).ToLowerInvariant();
        }

        /// <summary>Course dashboard URL for an offering, used by the card link.</summary>
        protected string CourseUrl(object offeringId)
        {
            return ResolveUrl("~/lecturer/lecturer_course_dashboard.aspx") + "?offering=" + offeringId;
        }
    }
}
