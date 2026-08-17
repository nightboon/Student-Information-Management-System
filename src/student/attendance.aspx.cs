using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using src.services;

namespace src.student
{
    public partial class attendance : src.security.StudentPage
    {
        private int _selectedOfferingId;

        protected string OverallRateDisplay { get; private set; }
        protected string OverallSubtext { get; private set; }
        protected string PresentCountDisplay { get; private set; }
        protected string LateCountDisplay { get; private set; }
        protected string AbsentCountDisplay { get; private set; }
        protected string DetailCourseCode { get; private set; }
        protected string DetailCourseName { get; private set; }
        protected string DetailLecturerName { get; private set; }
        protected string DetailCourseColorStyle { get; private set; }
        protected string DetailPresentDisplay { get; private set; }
        protected string DetailLateDisplay { get; private set; }
        protected string DetailAbsentDisplay { get; private set; }
        protected string SessionsFooterDisplay { get; private set; }
        protected string SemesterOptionsHtml { get; private set; }
        protected string AttendancePayloadJson { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["user_id"] == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }

            BindAttendancePage((int)Session["user_id"]);
        }

        protected string CourseCardClass(object dataItem)
        {
            var course = dataItem as StudentAttendanceCourse;
            bool selected = course != null && course.OfferingId == _selectedOfferingId;
            return selected
                ? "text-left rounded-lg border bg-white p-5 border-slate-200 cursor-pointer"
                : "text-left rounded-lg border bg-white p-5 border-slate-200 cursor-pointer";
        }

        protected string CourseColorStyle(object color)
        {
            return "background-color:" + SafeColor(color);
        }

        protected string CourseRateDisplay(object dataItem)
        {
            var course = dataItem as StudentAttendanceCourse;
            return course == null ? "N/A" : FormatRate(course.AttendanceRate);
        }

        protected string CourseRatioDisplay(object dataItem)
        {
            var course = dataItem as StudentAttendanceCourse;
            if (course == null) return "0 / 0";
            return (course.PresentCount + course.LateCount) + " / " + course.TotalCount;
        }

        protected string CourseBarStyle(object dataItem)
        {
            var course = dataItem as StudentAttendanceCourse;
            decimal width = course == null || !course.AttendanceRate.HasValue
                ? 0m
                : course.AttendanceRate.Value * 100m;
            return "width:" + Math.Max(0m, Math.Min(100m, width)).ToString("0.#") + "%;background-color:#e0162b";
        }

        protected string SessionDateDisplay(object dataItem)
        {
            var session = dataItem as StudentAttendanceSession;
            return session == null ? "" : session.AttendanceDate.ToString("d MMM yyyy");
        }

        protected string SessionDayDisplay(object dataItem)
        {
            var session = dataItem as StudentAttendanceSession;
            return session == null ? "" : session.AttendanceDate.ToString("ddd");
        }

        protected string SessionTimeDisplay(object dataItem)
        {
            var session = dataItem as StudentAttendanceSession;
            if (session == null || !session.StartTime.HasValue || !session.EndTime.HasValue)
            {
                return "TBA";
            }

            return FormatTime(session.StartTime.Value) + " - " + FormatTime(session.EndTime.Value);
        }

        protected string SessionTypeDisplay(object dataItem)
        {
            var session = dataItem as StudentAttendanceSession;
            return session == null || string.IsNullOrWhiteSpace(session.SessionType)
                ? "Class"
                : session.SessionType;
        }

        protected string SessionVenueDisplay(object dataItem)
        {
            var session = dataItem as StudentAttendanceSession;
            return session == null || string.IsNullOrWhiteSpace(session.Venue)
                ? "TBA"
                : session.Venue;
        }

        protected string StatusBadgeClass(object statusValue)
        {
            string status = (statusValue == null ? "" : statusValue.ToString()).ToUpperInvariant();
            switch (status)
            {
                case "PRESENT":
                    return "inline-flex items-center gap-1 rounded border bg-emerald-50 text-emerald-700 border-emerald-100 px-1.5 py-0.5";
                case "LATE":
                    return "inline-flex items-center gap-1 rounded border bg-sky-50 text-sky-700 border-sky-100 px-1.5 py-0.5";
                case "ABSENT":
                    return "inline-flex items-center gap-1 rounded border bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20 px-1.5 py-0.5";
                default:
                    return "inline-flex items-center gap-1 rounded border bg-slate-50 text-slate-700 border-slate-200 px-1.5 py-0.5";
            }
        }

        protected string StatusIcon(object statusValue)
        {
            string status = (statusValue == null ? "" : statusValue.ToString()).ToUpperInvariant();
            switch (status)
            {
                case "PRESENT": return "check-circle-2";
                case "LATE": return "clock";
                case "ABSENT": return "x-circle";
                default: return "circle";
            }
        }

        protected string StatusDisplay(object statusValue)
        {
            string status = statusValue == null ? "" : statusValue.ToString();
            return string.IsNullOrWhiteSpace(status) ? "N/A" : status.ToUpperInvariant();
        }

        protected string LecturerDisplay(object lecturer)
        {
            string name = lecturer == null ? "" : lecturer.ToString();
            return string.IsNullOrWhiteSpace(name) ? "Lecturer not assigned" : name;
        }

        private void BindAttendancePage(int userId)
        {
            var user = UserContextFactory.FromSession(Session);
            var attendance = StudentPortalService.GetAttendancePage(user);
            if (attendance == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }

            // Cards: every kept semester. JS filters to the current semester on load.
            courseRepeater.DataSource = attendance.Courses;
            courseRepeater.DataBind();

            // Hero: current-semester subset only (the initial, pre-filter view).
            var currentCourses = attendance.Courses.Where(c => c.IsCurrent).ToList();
            int hPresent = currentCourses.Sum(c => c.PresentCount);
            int hLate = currentCourses.Sum(c => c.LateCount);
            int hAbsent = currentCourses.Sum(c => c.AbsentCount);
            int hTotal = currentCourses.Sum(c => c.TotalCount);
            decimal? hRate = hTotal == 0 ? (decimal?)null : Math.Round((decimal)(hPresent + hLate) / hTotal, 4);

            OverallRateDisplay = FormatRate(hRate);
            PresentCountDisplay = hPresent.ToString();
            LateCountDisplay = hLate.ToString();
            AbsentCountDisplay = hAbsent.ToString();
            OverallSubtext = hTotal == 0
                ? "No attendance records yet"
                : (hPresent + hLate) + " present / " + hTotal + " recorded across " + currentCourses.Count + " " + Pluralize("course", currentCourses.Count);

            // Default selected course: first current-semester course, else first overall.
            var selectedCourse = currentCourses.FirstOrDefault() ?? attendance.Courses.FirstOrDefault();
            _selectedOfferingId = selectedCourse == null ? 0 : selectedCourse.OfferingId;

            if (selectedCourse != null)
            {
                DetailCourseCode = selectedCourse.CourseCode;
                DetailCourseName = selectedCourse.CourseName;
                DetailLecturerName = LecturerDisplay(selectedCourse.LecturerName);
                DetailCourseColorStyle = CourseColorStyle(selectedCourse.Color);
                DetailPresentDisplay = selectedCourse.PresentCount.ToString();
                DetailLateDisplay = selectedCourse.LateCount.ToString();
                DetailAbsentDisplay = selectedCourse.AbsentCount.ToString();
                SessionsFooterDisplay = "Showing " + selectedCourse.Sessions.Count + " of " + selectedCourse.Sessions.Count + " recorded sessions";

                sessionsRepeater.DataSource = selectedCourse.Sessions;
                sessionsRepeater.DataBind();
            }

            SemesterOptionsHtml = BuildSemesterOptions(attendance.Courses);
            AttendancePayloadJson = BuildPayloadJson(attendance.Courses, _selectedOfferingId, currentCourses);
        }

        private static string BuildSemesterOptions(List<StudentAttendanceCourse> courses)
        {
            var semesters = courses
                .GroupBy(c => c.SemesterId)
                .Select(g => new { Id = g.Key, Name = g.First().SemesterName, IsCurrent = g.First().IsCurrent })
                .ToList();

            var sb = new StringBuilder();
            foreach (var sem in semesters)
            {
                sb.Append("<option value=\"")
                  .Append(sem.Id)
                  .Append("\"")
                  .Append(sem.IsCurrent ? " selected" : "")
                  .Append(">")
                  .Append(HttpUtility.HtmlEncode(sem.Name))
                  .Append("</option>");
            }
            sb.Append("<option value=\"all\">All semesters</option>");
            return sb.ToString();
        }

        private string BuildPayloadJson(List<StudentAttendanceCourse> courses, int defaultOfferingId, List<StudentAttendanceCourse> currentCourses)
        {
            int currentSemesterId = currentCourses.Select(c => c.SemesterId).FirstOrDefault();

            var payload = new
            {
                defaultOfferingId = defaultOfferingId,
                currentSemesterId = currentSemesterId,
                courses = courses.Select(c => new
                {
                    offeringId = c.OfferingId,
                    semesterId = c.SemesterId,
                    semesterName = c.SemesterName,
                    isCurrent = c.IsCurrent,
                    code = c.CourseCode,
                    name = c.CourseName,
                    lecturer = LecturerDisplay(c.LecturerName),
                    color = SafeColor(c.Color),
                    present = c.PresentCount,
                    late = c.LateCount,
                    absent = c.AbsentCount,
                    total = c.TotalCount,
                    sessions = c.Sessions.Select(s => new
                    {
                        dateIso = s.AttendanceDate.ToString("yyyy-MM-dd"),
                        date = SessionDateDisplay(s),
                        day = SessionDayDisplay(s),
                        time = SessionTimeDisplay(s),
                        type = SessionTypeDisplay(s),
                        venue = SessionVenueDisplay(s),
                        status = (s.Status ?? "").ToUpperInvariant()
                    }).ToList()
                }).ToList()
            };

            string json = new JavaScriptSerializer().Serialize(payload);
            // Defensive: never let a value close the inline <script> early.
            return json.Replace("</", "<\\/");
        }

        private static string FormatRate(decimal? rate)
        {
            return rate.HasValue ? (rate.Value * 100m).ToString("0.#") + "%" : "N/A";
        }

        private static string FormatTime(TimeSpan time)
        {
            return DateTime.Today.Add(time).ToString("HH:mm");
        }

        private static string SafeColor(object color)
        {
            string value = color == null ? "" : color.ToString();
            if (value.Length == 7 && value[0] == '#' && value.Skip(1).All(Uri.IsHexDigit))
            {
                return value;
            }
            return "#e0162b";
        }

        private static string Pluralize(string word, int count)
        {
            return count == 1 ? word : word + "s";
        }
    }
}
