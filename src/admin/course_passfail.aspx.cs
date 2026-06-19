using System;
using System.Linq;
using System.Web.Script.Serialization;
using src.services.admin;

namespace src.admin
{
    public partial class course_passfail : src.security.AdminPage
    {
        private readonly AdminPortalService service = new AdminPortalService();

        protected string PassFailJson { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var courses = service.GetCourseMetrics();
            var payload = courses.Select(c => new
            {
                code = c.Code,
                title = c.Title,
                prog = c.Programme,
                sem = c.Semester,
                lecturer = c.Lecturer,
                enrolled = c.Enrolled,
                passed = c.Passed,
                failed = c.Failed,
                passRate = c.PassRate,
                avgMarks = c.AverageMarks,
                students = service.GetCourseStudents(c.Code).Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    prog = s.Programme,
                    sem = s.Semester,
                    marks = s.Marks,
                    grade = s.Grade,
                    status = s.Grade == "F" ? "Fail" : "Pass"
                }).ToList()
            }).ToList();

            PassFailJson = new JavaScriptSerializer().Serialize(payload);
        }
    }
}
