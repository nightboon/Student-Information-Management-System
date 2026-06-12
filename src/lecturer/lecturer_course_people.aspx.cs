using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using src.services;

namespace src.lecturer
{
    public partial class lecturer_course_people : src.security.LecturerPage
    {
        private int _offeringId;
        private LecturerCourse _course;
        private List<EnrolledStudentRow> _students = new List<EnrolledStudentRow>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            var lecturer = LecturerService.GetByUserId((int)Session["user_id"]);
            if (lecturer == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            if (!int.TryParse(Request.QueryString["offering"], out _offeringId))
            {
                Response.Redirect("~/lecturer/lecturer_courses.aspx");
                return;
            }

            foreach (var course in LecturerCourseService.GetCourses(lecturer.UserId))
            {
                if (course.OfferingId == _offeringId)
                {
                    _course = course;
                    break;
                }
            }

            if (_course == null)
            {
                Response.Redirect("~/lecturer/lecturer_courses.aspx");
                return;
            }

            _students = LecturerCourseService.GetEnrolledStudents(_offeringId, lecturer.LecturerId);
            studentsRepeater.DataSource = _students;
            studentsRepeater.DataBind();
            emptyPanel.Visible = _students.Count == 0;
        }

        protected string CourseCode { get { return _course != null ? _course.CourseCode : ""; } }
        protected string CourseName { get { return _course != null ? _course.CourseName : ""; } }

        protected string EnrolledLabel
        {
            get
            {
                int count = _students.Count;
                return count + (count == 1 ? " student enrolled" : " students enrolled");
            }
        }

        protected string BackUrl
        {
            get { return ResolveUrl("~/lecturer/lecturer_course_dashboard.aspx") + "?offering=" + _offeringId.ToString(CultureInfo.InvariantCulture); }
        }

        protected string Html(object value)
        {
            return HttpUtility.HtmlEncode(value == null ? "" : value.ToString());
        }
    }
}
