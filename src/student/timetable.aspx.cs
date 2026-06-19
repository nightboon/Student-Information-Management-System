using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using src.services;

namespace src.student
{
    public partial class timetable : src.security.StudentPage
    {
        protected string SemesterDisplay { get; private set; }
        protected string ProgrammeName { get; private set; }
        protected string CourseCountDisplay { get; private set; }
        protected string TotalCreditHoursDisplay { get; private set; }
        protected string WeeklyContactDisplay { get; private set; }
        protected string ScheduleRangeDisplay { get; private set; }
        protected string TimetablePayloadJson { get; private set; }

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

            LoadTimetable((int)Session["user_id"]);
        }

        protected string CourseIconStyle(object color)
        {
            var safeColor = SafeColor(color);
            return "background-color:" + safeColor + "15;color:" + safeColor;
        }

        private void LoadTimetable(int userId)
        {
            var user = UserContextFactory.FromSession(Session);
            var timetable = StudentPortalService.GetTimetablePage(user);
            if (timetable == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }

            SemesterDisplay = FormatSemester(timetable);
            ProgrammeName = timetable.ProgrammeName;
            CourseCountDisplay = timetable.CourseCount.ToString(CultureInfo.InvariantCulture);
            TotalCreditHoursDisplay = FormatCredits(timetable.TotalCreditHours);
            WeeklyContactDisplay = FormatHours(timetable.WeeklyContactHours);
            ScheduleRangeDisplay = FormatScheduleRange(timetable.Sessions);
            TimetablePayloadJson = BuildTimetablePayloadJson(timetable);

            coursesRepeater.DataSource = timetable.Courses;
            coursesRepeater.DataBind();
            emptyCoursesPanel.Visible = timetable.Courses.Count == 0;
        }

        private static string BuildTimetablePayloadJson(StudentTimetablePage timetable)
        {
            var events = new List<object>();
            foreach (var session in timetable.Sessions)
            {
                var color = SafeColor(session.Color);
                events.Add(new
                {
                    id = session.TimetableId.ToString(CultureInfo.InvariantCulture),
                    title = session.CourseName,
                    daysOfWeek = new[] { ToFullCalendarDay(session.DayOfWeek) },
                    startTime = FormatCalendarTime(session.StartTime),
                    endTime = FormatCalendarTime(session.EndTime),
                    startRecur = FormatDate(timetable.SemesterStartDate),
                    endRecur = FormatDate(timetable.SemesterEndDate.AddDays(1)),
                    backgroundColor = color + "18",
                    borderColor = color + "40",
                    textColor = "#0f172a",
                    extendedProps = new
                    {
                        code = session.CourseCode,
                        type = "Class",
                        room = session.Venue,
                        lecturer = session.LecturerName,
                        color = color
                    }
                });
            }

            var payload = new
            {
                initialDate = FormatDate(timetable.SemesterStartDate),
                slotMinTime = FormatCalendarTime(GetEarliestStart(timetable.Sessions)),
                slotMaxTime = FormatCalendarTime(GetLatestEnd(timetable.Sessions)),
                weekends = HasWeekendClasses(timetable.Sessions),
                events = events
            };

            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(payload).Replace("</", "<\\/");
        }

        private static string FormatSemester(StudentTimetablePage timetable)
        {
            if (timetable.CurrentSemesterNo <= 0)
            {
                return timetable.SemesterName;
            }

            return "Semester " + timetable.CurrentSemesterNo.ToString(CultureInfo.InvariantCulture) +
                " - " + timetable.SemesterName;
        }

        private static string FormatCredits(int credits)
        {
            return credits.ToString(CultureInfo.InvariantCulture) + (credits == 1 ? " credit" : " credits");
        }

        private static string FormatHours(decimal hours)
        {
            if (hours == 1m)
            {
                return "1 hr";
            }

            return hours.ToString("0.#", CultureInfo.InvariantCulture) + " hrs";
        }

        private static string FormatScheduleRange(List<StudentClassSession> sessions)
        {
            if (sessions.Count == 0)
            {
                return "No scheduled classes yet";
            }

            var orderedDays = new List<string>();
            foreach (var session in sessions)
            {
                if (!orderedDays.Contains(session.DayOfWeek))
                {
                    orderedDays.Add(session.DayOfWeek);
                }
            }
            orderedDays.Sort((left, right) => DayOrder(left).CompareTo(DayOrder(right)));

            var dayText = string.Join(", ", orderedDays.ConvertAll(ShortDayName));
            return dayText + " - " + FormatDisplayTime(GetEarliestStart(sessions)) +
                " - " + FormatDisplayTime(GetLatestEnd(sessions));
        }

        private static TimeSpan GetEarliestStart(List<StudentClassSession> sessions)
        {
            if (sessions.Count == 0)
            {
                return new TimeSpan(8, 0, 0);
            }

            var earliest = sessions[0].StartTime;
            foreach (var session in sessions)
            {
                if (session.StartTime < earliest)
                {
                    earliest = session.StartTime;
                }
            }

            return earliest;
        }

        private static TimeSpan GetLatestEnd(List<StudentClassSession> sessions)
        {
            if (sessions.Count == 0)
            {
                return new TimeSpan(18, 0, 0);
            }

            var latest = sessions[0].EndTime;
            foreach (var session in sessions)
            {
                if (session.EndTime > latest)
                {
                    latest = session.EndTime;
                }
            }

            return latest;
        }

        private static bool HasWeekendClasses(List<StudentClassSession> sessions)
        {
            foreach (var session in sessions)
            {
                if (session.DayOfWeek == "Saturday" || session.DayOfWeek == "Sunday")
                {
                    return true;
                }
            }

            return false;
        }

        private static string FormatDisplayTime(TimeSpan time)
        {
            return DateTime.Today.Add(time).ToString("h:mm tt", CultureInfo.InvariantCulture).ToLowerInvariant();
        }

        private static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static string FormatCalendarTime(TimeSpan time)
        {
            return time.ToString(@"hh\:mm\:ss");
        }

        private static int ToFullCalendarDay(string dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case "Sunday":
                    return 0;
                case "Monday":
                    return 1;
                case "Tuesday":
                    return 2;
                case "Wednesday":
                    return 3;
                case "Thursday":
                    return 4;
                case "Friday":
                    return 5;
                case "Saturday":
                    return 6;
                default:
                    return 1;
            }
        }

        private static int DayOrder(string dayOfWeek)
        {
            return ToFullCalendarDay(dayOfWeek) == 0 ? 7 : ToFullCalendarDay(dayOfWeek);
        }

        private static string ShortDayName(string dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case "Monday":
                    return "Mon";
                case "Tuesday":
                    return "Tue";
                case "Wednesday":
                    return "Wed";
                case "Thursday":
                    return "Thu";
                case "Friday":
                    return "Fri";
                case "Saturday":
                    return "Sat";
                case "Sunday":
                    return "Sun";
                default:
                    return dayOfWeek;
            }
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
                if (!Uri.IsHexDigit(color[i]))
                {
                    return "#e0162b";
                }
            }

            return color;
        }
    }
}
