using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using src.services;

namespace src.student
{
    public partial class courses : src.security.StudentPage
    {
        private List<StudentCourseCard> _courses = new List<StudentCourseCard>();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (Session["user_id"] == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }

            var user = UserContextFactory.FromSession(Session);
            _courses = StudentPortalService.GetCourses(user) ?? new List<StudentCourseCard>();

            coursesRepeater.DataSource = _courses
                .OrderByDescending(c => c.IsCurrent)
                .ThenBy(c => c.CourseCode)
                .ToList();
            coursesRepeater.DataBind();
        }

        protected int EnrolledCount
        {
            get { return _courses.Count; }
        }

        protected bool IsCurrent(string semesterName)
        {
            return _courses.Any(c => c.IsCurrent
                && string.Equals(c.SemesterName, semesterName, StringComparison.OrdinalIgnoreCase));
        }

        protected string SearchKey(string courseCode, string courseName, string lecturerName)
        {
            return (courseCode + " " + courseName + " " + lecturerName).ToLowerInvariant();
        }

        protected string AccentColor(string color)
        {
            if (string.IsNullOrEmpty(color) || color.Length != 7 || color[0] != '#') return "#64748b";
            for (int i = 1; i < color.Length; i++)
            {
                if (!Uri.IsHexDigit(color[i])) return "#64748b";
            }
            return color;
        }
    }
}
