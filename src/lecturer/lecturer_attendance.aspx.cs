using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Script.Serialization;
using src.services;

namespace src.lecturer
{
    public partial class lecturer_attendance : src.security.LecturerPage
    {
        private Lecturer _lecturer;
        private Semester _semester;
        private List<ClassSession> _weekly = new List<ClassSession>();
        private List<LecturerCourse> _courses = new List<LecturerCourse>();
        private int _coursesTeaching;
        private int _studentsTaught;

        /// <summary>FullCalendar payload (events + view bounds) for the weekly schedule.</summary>
        protected string TimetablePayloadJson { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] != null)
            {
                int userId = (int)Session["user_id"];
                _lecturer = LecturerService.GetByUserId(userId);
            }
            _semester = SemesterService.GetCurrent();
            TimetablePayloadJson = "{}";

            if (_lecturer != null)
            {
                int lecturerId = _lecturer.LecturerId;

                _weekly = LecturerService.GetWeeklyClasses(lecturerId);
                _coursesTeaching = LecturerService.CountActiveCourses(lecturerId);
                _studentsTaught = LecturerService.CountStudentsTaught(lecturerId);

                // Courses teaching this semester: the lecturer's current-semester
                // offerings (the service already orders newest semester first).
                _courses = LecturerCourseService.GetCourses(_lecturer.UserId)
                    .FindAll(c => _semester != null && c.SemesterName == _semester.Name);

                coursesRepeater.DataSource = _courses;
                coursesRepeater.DataBind();

                TimetablePayloadJson = BuildPayloadJson(_weekly);
            }
        }

        // ----- Header / stat cards -----

        /// <summary>The lecturer's department, e.g. "School of Computing".</summary>
        protected string DepartmentDisplay
        {
            get
            {
                return _lecturer != null && !string.IsNullOrEmpty(_lecturer.Department)
                    ? _lecturer.Department : "—";
            }
        }

        /// <summary>Number of course offerings taught this semester.</summary>
        protected int CoursesTeachingCount
        {
            get { return _coursesTeaching; }
        }

        /// <summary>Distinct students taught this semester, e.g. "205 students".</summary>
        protected string TotalStudentsDisplay
        {
            get { return _studentsTaught + (_studentsTaught == 1 ? " student" : " students"); }
        }

        /// <summary>Total weekly contact hours across the schedule, e.g. "22 hrs".</summary>
        protected string WeeklyHoursDisplay
        {
            get
            {
                decimal hours = 0m;
                foreach (var session in _weekly)
                {
                    hours += (decimal)(session.EndTime - session.StartTime).TotalHours;
                }
                if (hours == 1m) return "1 hr";
                return hours.ToString("0.#", CultureInfo.InvariantCulture) + " hrs";
            }
        }

        /// <summary>Active semester name for the subtitle (empty when none configured).</summary>
        protected string SemesterDisplay
        {
            get { return _semester != null ? _semester.Name : ""; }
        }

        /// <summary>Number of courses listed in the "teaching this semester" strip.</summary>
        protected int CoursesThisSemesterCount
        {
            get { return _courses.Count; }
        }

        /// <summary>App-resolved URL of the take-attendance page, handed to the calendar JS.</summary>
        protected string TakeAttendanceUrl
        {
            get { return ResolveUrl("~/lecturer/lecturer_take_attendance.aspx"); }
        }

        // ----- Course cards -----

        /// <summary>Tinted icon style ("background:...15;color:...") for a course card.</summary>
        protected string CourseIconStyle(object color)
        {
            var safe = SafeColor(color);
            return "background-color:" + safe + "15;color:" + safe;
        }

        /// <summary>"60 students enrolled" / "1 student enrolled".</summary>
        protected string EnrolledLabel(int count)
        {
            return count + (count == 1 ? " student enrolled" : " students enrolled");
        }

        // ----- FullCalendar payload -----

        private string BuildPayloadJson(List<ClassSession> sessions)
        {
            var events = new List<object>();
            foreach (var session in sessions)
            {
                var color = SafeColor(session.Color);
                events.Add(new
                {
                    id = session.TimetableId.ToString(CultureInfo.InvariantCulture),
                    title = session.CourseName,
                    daysOfWeek = new[] { ToFullCalendarDay(session.DayOfWeek) },
                    startTime = FormatCalendarTime(session.StartTime),
                    endTime = FormatCalendarTime(session.EndTime),
                    startRecur = _semester != null ? FormatDate(_semester.StartDate) : (string)null,
                    endRecur = _semester != null ? FormatDate(_semester.EndDate.AddDays(1)) : (string)null,
                    backgroundColor = color + "18",
                    borderColor = color + "40",
                    textColor = "#0f172a",
                    extendedProps = new
                    {
                        code = session.CourseCode,
                        type = "Class",
                        room = session.Venue,
                        offeringId = session.OfferingId,
                        color = color
                    }
                });
            }

            var payload = new
            {
                initialDate = FormatDate(InitialWeekDate()),
                slotMinTime = FormatCalendarTime(GetEarliestStart(sessions)),
                slotMaxTime = FormatCalendarTime(GetLatestEnd(sessions)),
                weekends = HasWeekendClasses(sessions),
                takeAttendanceUrl = TakeAttendanceUrl,
                events = events
            };

            return new JavaScriptSerializer().Serialize(payload).Replace("</", "<\\/");
        }

        // The week the calendar opens on: today when it falls inside the semester,
        // otherwise the nearest semester boundary so the recurring classes (which
        // only render between the semester start and end dates) are always visible.
        private DateTime InitialWeekDate()
        {
            var today = DateTime.Today;
            if (_semester == null) return today;
            if (today < _semester.StartDate) return _semester.StartDate;
            if (today > _semester.EndDate) return _semester.EndDate;
            return today;
        }

        private static int ToFullCalendarDay(string dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case "Sunday": return 0;
                case "Monday": return 1;
                case "Tuesday": return 2;
                case "Wednesday": return 3;
                case "Thursday": return 4;
                case "Friday": return 5;
                case "Saturday": return 6;
                default: return 1;
            }
        }

        private static TimeSpan GetEarliestStart(List<ClassSession> sessions)
        {
            var earliest = new TimeSpan(8, 0, 0);
            bool any = false;
            foreach (var session in sessions)
            {
                if (!any || session.StartTime < earliest)
                {
                    earliest = session.StartTime;
                    any = true;
                }
            }
            return any ? earliest : new TimeSpan(8, 0, 0);
        }

        private static TimeSpan GetLatestEnd(List<ClassSession> sessions)
        {
            var latest = new TimeSpan(18, 0, 0);
            bool any = false;
            foreach (var session in sessions)
            {
                if (!any || session.EndTime > latest)
                {
                    latest = session.EndTime;
                    any = true;
                }
            }
            return any ? latest : new TimeSpan(18, 0, 0);
        }

        private static bool HasWeekendClasses(List<ClassSession> sessions)
        {
            foreach (var session in sessions)
            {
                if (session.DayOfWeek == "Saturday" || session.DayOfWeek == "Sunday") return true;
            }
            return false;
        }

        private static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static string FormatCalendarTime(TimeSpan time)
        {
            return time.ToString(@"hh\:mm\:ss");
        }

        private static string SafeColor(object value)
        {
            var color = value == null ? null : value.ToString();
            if (string.IsNullOrWhiteSpace(color) || color.Length != 7 || color[0] != '#')
            {
                return "#e0162b";
            }
            for (var i = 1; i < color.Length; i++)
            {
                if (!Uri.IsHexDigit(color[i])) return "#e0162b";
            }
            return color;
        }
    }
}
