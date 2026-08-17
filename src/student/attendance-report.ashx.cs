using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using src.services;

namespace src.student
{
    public class AttendanceReportDownloadHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            var user = UserContextFactory.FromSession(context.Session);
            if (user == null || !user.IsStudent)
            {
                WriteError(context, 401, "Please sign in as a student to export attendance.");
                return;
            }

            var account = StudentPortalService.GetAccount(user);
            var attendance = StudentPortalService.GetAttendancePage(user);
            if (account == null || attendance == null)
            {
                WriteError(context, 404, "Attendance data is not available.");
                return;
            }

            var pdfFilters = new AttendancePdfFilters
            {
                Semester = "All semesters",
                Course = "All courses",
                Status = "All statuses"
            };

            int semesterId;
            if (int.TryParse(context.Request.QueryString["semesterId"], out semesterId) && semesterId > 0)
            {
                var semesterCourse = attendance.Courses.FirstOrDefault(course => course.SemesterId == semesterId);
                if (semesterCourse != null) pdfFilters.Semester = semesterCourse.SemesterName;
                attendance.Courses = attendance.Courses
                    .Where(course => course.SemesterId == semesterId)
                    .ToList();
            }

            int offeringId;
            if (int.TryParse(context.Request.QueryString["offeringId"], out offeringId) && offeringId > 0)
            {
                var selectedCourse = attendance.Courses.FirstOrDefault(course => course.OfferingId == offeringId);
                if (selectedCourse != null)
                {
                    pdfFilters.Course = selectedCourse.CourseCode + " - " + selectedCourse.CourseName;
                }
                attendance.Courses = attendance.Courses
                    .Where(course => course.OfferingId == offeringId)
                    .ToList();
            }

            var status = (context.Request.QueryString["status"] ?? "").Trim().ToUpperInvariant();
            if (status != "PRESENT" && status != "LATE" && status != "ABSENT" && status != "N/A")
            {
                status = "";
            }
            else
            {
                pdfFilters.Status = status == "N/A" ? "Not recorded" : status;
            }

            var dateFrom = ParseDate(context.Request.QueryString["dateFrom"]);
            var dateTo = ParseDate(context.Request.QueryString["dateTo"]);
            pdfFilters.DateFrom = dateFrom;
            pdfFilters.DateTo = dateTo;
            var hasRecordFilter = status.Length > 0 || dateFrom.HasValue || dateTo.HasValue;

            foreach (var course in attendance.Courses)
            {
                var sessions = course.Sessions == null
                    ? Enumerable.Empty<StudentAttendanceSession>()
                    : course.Sessions.AsEnumerable();

                course.Sessions = sessions.Where(session =>
                    (status.Length == 0 || string.Equals(session.Status, status, StringComparison.OrdinalIgnoreCase)) &&
                    (!dateFrom.HasValue || session.AttendanceDate.Date >= dateFrom.Value) &&
                    (!dateTo.HasValue || session.AttendanceDate.Date <= dateTo.Value))
                    .ToList();

                course.PresentCount = course.Sessions.Count(session => string.Equals(session.Status, "PRESENT", StringComparison.OrdinalIgnoreCase));
                course.LateCount = course.Sessions.Count(session => string.Equals(session.Status, "LATE", StringComparison.OrdinalIgnoreCase));
                course.AbsentCount = course.Sessions.Count(session => string.Equals(session.Status, "ABSENT", StringComparison.OrdinalIgnoreCase));
                course.TotalCount = course.PresentCount + course.LateCount + course.AbsentCount;
                course.AttendanceRate = course.TotalCount == 0
                    ? (decimal?)null
                    : Math.Round((decimal)(course.PresentCount + course.LateCount) / course.TotalCount, 4);
            }

            if (hasRecordFilter)
            {
                attendance.Courses = attendance.Courses
                    .Where(course => course.Sessions.Count > 0)
                    .ToList();
            }

            var pdf = AttendancePdfService.Create(account, attendance, DateTime.Now, pdfFilters);
            var safeId = string.IsNullOrWhiteSpace(account.StudentId) ? "student" : account.StudentId;

            context.Response.Clear();
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader(
                "Content-Disposition",
                "attachment; filename=INTI-Attendance-" + safeId + "-" +
                DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".pdf");
            context.Response.OutputStream.Write(pdf, 0, pdf.Length);
            context.ApplicationInstance.CompleteRequest();
        }

        private static void WriteError(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "text/plain";
            context.Response.Write(message);
        }

        private static DateTime? ParseDate(string value)
        {
            DateTime parsed;
            return DateTime.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsed)
                ? parsed.Date
                : (DateTime?)null;
        }
    }
}
