using System;
using System.Linq;
using System.Web.Script.Serialization;
using src.services.admin;

namespace src.admin
{
    public partial class course_attendance : src.security.AdminPage
    {
        private readonly AdminPortalService service = new AdminPortalService();

        protected string AttendanceJson { get; private set; }

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
                avgPct = c.AverageAttendance,
                sessionsHeld = c.SessionsHeld,
                sessions = new[] { new { present = c.Present, absent = c.Absent } },
                students = service.GetCourseStudents(c.Code).Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    prog = s.Programme,
                    present = s.Present,
                    absent = s.Absent,
                    pct = s.AttendancePercentage
                }).ToList()
            }).ToList();

            AttendanceJson = new JavaScriptSerializer().Serialize(payload);
        }
    }
}
