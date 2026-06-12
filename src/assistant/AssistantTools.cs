using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using src.services;

namespace src.assistant
{
    /// <summary>
    /// The assistant's tool registry. Two responsibilities:
    /// (A) <see cref="Definitions"/> describes the read-only tools the model may
    ///     call; (B) <see cref="Execute"/> runs the matching service method.
    ///
    /// Every tool is parameterless and scoped to a single student: the userId is
    /// supplied by the server from the session, never by the model. The model can
    /// only choose WHICH tool to run, so it can neither reach other students' data
    /// nor influence the underlying (parameterized) SQL.
    /// </summary>
    public static class AssistantTools
    {
        private static readonly JavaScriptSerializer Json =
            new JavaScriptSerializer { MaxJsonLength = 10_000_000 };

        /// <summary>Tool schemas in OpenAI "tools" format, sent on every request.</summary>
        public static List<object> Definitions()
        {
            return new List<object>
            {
                Tool("get_today_classes",    "Get the student's classes scheduled for today (time, course, venue, lecturer)."),
                Tool("get_weekly_timetable", "Get the student's full weekly class timetable for the current semester."),
                Tool("get_current_courses",  "Get the list of courses the student is enrolled in this semester."),
                Tool("get_grades",           "Get the student's cumulative CGPA and total credits earned."),
                Tool("get_attendance",       "Get the student's current-semester attendance rate and present/late/absent counts."),
                Tool("get_assignments_due",  "Get the student's assignments due within the next 7 days."),
                Tool("get_announcements",    "Get the latest announcements relevant to the student's courses."),
            };
        }

        /// <summary>
        /// Runs the named tool for the given student and returns a compact JSON
        /// string to feed back to the model. Unknown tool names return an error
        /// object rather than throwing.
        /// </summary>
        public static string Execute(string toolName, int userId)
        {
            switch (toolName)
            {
                case "get_today_classes":
                    return Serialize(TimetableService.GetTodayClasses(userId).Select(c => new
                    {
                        course = c.CourseCode + " " + c.CourseName,
                        time = c.StartTime.ToString(@"hh\:mm") + "-" + c.EndTime.ToString(@"hh\:mm"),
                        venue = c.Venue,
                        lecturer = c.LecturerName
                    }));

                case "get_weekly_timetable":
                    return Serialize(TimetableService.GetWeeklyTimetable(userId).Select(c => new
                    {
                        day = c.DayOfWeek,
                        course = c.CourseCode + " " + c.CourseName,
                        time = c.StartTime.ToString(@"hh\:mm") + "-" + c.EndTime.ToString(@"hh\:mm"),
                        venue = c.Venue,
                        lecturer = c.LecturerName
                    }));

                case "get_current_courses":
                    return Serialize(EnrolmentService.GetCurrentCourses(userId).Select(c => new
                    {
                        code = c.CourseCode,
                        name = c.CourseName,
                        creditHours = c.CreditHours,
                        lecturer = c.LecturerName,
                        status = c.Status
                    }));

                case "get_grades":
                    var g = GradeService.GetSummary(userId);
                    return Serialize(new { cgpa = g.Cgpa, creditsEarned = g.CreditsEarned });

                case "get_attendance":
                    var a = AttendanceService.GetAttendancePage(userId);
                    return Serialize(new
                    {
                        attendanceRate = a.AttendanceRate,
                        present = a.PresentCount,
                        late = a.LateCount,
                        absent = a.AbsentCount,
                        total = a.TotalCount
                    });

                case "get_assignments_due":
                    return Serialize(AssignmentService.GetDueThisWeek(userId).Select(x => new
                    {
                        title = x.Title,
                        course = x.CourseCode + " " + x.CourseName,
                        due = x.DueDate.ToString("yyyy-MM-dd")
                    }));

                case "get_announcements":
                    return Serialize(AnnouncementService.GetForStudent(userId).Select(x => new
                    {
                        title = x.Title,
                        content = x.Content,
                        date = x.CreatedAt.ToString("yyyy-MM-dd")
                    }));

                default:
                    return "{\"error\":\"unknown tool\"}";
            }
        }

        private static object Tool(string name, string description)
        {
            return new Dictionary<string, object>
            {
                ["type"] = "function",
                ["function"] = new Dictionary<string, object>
                {
                    ["name"] = name,
                    ["description"] = description,
                    ["parameters"] = new Dictionary<string, object>
                    {
                        ["type"] = "object",
                        ["properties"] = new Dictionary<string, object>()
                    }
                }
            };
        }

        private static string Serialize(object value)
        {
            return Json.Serialize(value);
        }
    }
}
