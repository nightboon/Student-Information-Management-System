using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using src.services;
using static src.services.StudentPortalFormat;

namespace src.lecturer
{
    public class LecturerAttendanceClassSession
    {
        public int TimetableId { get; set; }
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Venue { get; set; }
        public string Color { get; set; }
    }

    public partial class lecturer_attendance : src.security.LecturerPage
    {
        private LecturerProfile _lecturer;
        private StudentRegistrationTerm _term;
        private List<LecturerAttendanceClassSession> _weekly = new List<LecturerAttendanceClassSession>();
        private List<LecturerCourseCard> _courses = new List<LecturerCourseCard>();
        private int _coursesTeaching;
        private int _studentsTaught;

        protected string TimetablePayloadJson { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            var user = UserContextFactory.FromSession(Session);
            _lecturer = LecturerPortalService.GetProfile(user);
            _term = AcademicTermReader.GetCurrentTerm();
            TimetablePayloadJson = "{}";

            if (_lecturer == null) return;

            var currentLabel = TermLabel(_term);
            _courses = LecturerPortalService.GetCourses(user)
                .FindAll(c => string.IsNullOrEmpty(currentLabel) || string.Equals(c.SemesterName, currentLabel, StringComparison.OrdinalIgnoreCase));

            // Map facade LecturerClassSession -> local LecturerAttendanceClassSession for FullCalendar
            _weekly = LecturerPortalService.GetClassSessions(user)
                .Select(s => new LecturerAttendanceClassSession
                {
                    TimetableId = s.TimetableId,
                    OfferingId = s.OfferingId,
                    CourseCode = s.CourseCode,
                    CourseName = s.CourseName,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Venue = s.Venue,
                    Color = s.Color
                })
                .ToList();

            _coursesTeaching = _courses.Count;
            _studentsTaught = _courses.Sum(c => c.EnrolledCount);

            coursesRepeater.DataSource = _courses;
            coursesRepeater.DataBind();

            TimetablePayloadJson = BuildPayloadJson(_weekly);
        }

        protected string DepartmentDisplay
        {
            get
            {
                return _lecturer != null && !string.IsNullOrEmpty(_lecturer.DepartmentId)
                    ? _lecturer.DepartmentId : "-";
            }
        }

        protected int CoursesTeachingCount
        {
            get { return _coursesTeaching; }
        }

        protected string TotalStudentsDisplay
        {
            get { return _studentsTaught + (_studentsTaught == 1 ? " student" : " students"); }
        }

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

        protected string SemesterDisplay
        {
            get { return TermLabel(_term); }
        }

        protected int CoursesThisSemesterCount
        {
            get { return _courses.Count; }
        }

        protected string TakeAttendanceUrl
        {
            get { return ResolveUrl("~/lecturer/lecturer_take_attendance.aspx"); }
        }

        protected string CourseIconStyle(object color)
        {
            var safe = SafeColor(color);
            return "background-color:" + safe + "15;color:" + safe;
        }

        protected string EnrolledLabel(int count)
        {
            return count + (count == 1 ? " student enrolled" : " students enrolled");
        }

        private string BuildPayloadJson(List<LecturerAttendanceClassSession> sessions)
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
                    startRecur = _term != null ? FormatDate(_term.StartDate) : (string)null,
                    endRecur = _term != null ? FormatDate(_term.EndDate.AddDays(1)) : (string)null,
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

        private DateTime InitialWeekDate()
        {
            var today = DateTime.Today;
            if (_term == null) return today;
            if (today < _term.StartDate.Date) return _term.StartDate.Date;
            if (today > _term.EndDate.Date) return _term.EndDate.Date;
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

        private static TimeSpan GetEarliestStart(List<LecturerAttendanceClassSession> sessions)
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

        private static TimeSpan GetLatestEnd(List<LecturerAttendanceClassSession> sessions)
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

        private static bool HasWeekendClasses(List<LecturerAttendanceClassSession> sessions)
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
