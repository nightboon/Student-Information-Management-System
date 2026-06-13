using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using src.services;

namespace src.assistant
{
    /// <summary>
    /// The assistant's tool registry. Two responsibilities:
    /// (A) <see cref="DefinitionsFor"/> describes the read-only tools the model may
    ///     call for a given role; (B) <see cref="Execute"/> runs the matching
    ///     service method. Students and lecturers get different tool sets.
    ///
    /// Every tool is parameterless and scoped to a single user: the userId is
    /// supplied by the server from the session, never by the model. The model can
    /// only choose WHICH tool to run, so it can neither reach other users' data
    /// nor influence the underlying (parameterized) SQL.
    /// </summary>
    public static class AssistantTools
    {
        private static readonly JavaScriptSerializer Json =
            new JavaScriptSerializer { MaxJsonLength = 10_000_000 };

        /// <summary>
        /// Tool schemas in OpenAI "tools" format for the given role, sent on every
        /// request. Lecturers get teaching tools; everyone else gets student tools.
        /// </summary>
        public static List<object> DefinitionsFor(string role)
        {
            if (IsLecturer(role))
            {
                return new List<object>
                {
                    Tool("get_my_courses",        "Get the courses this lecturer is teaching this semester (code, name, credit hours, semester, enrolled count)."),
                    Tool("get_at_risk_students",  "Get students flagged as at-risk across this lecturer's courses (name, course, risk level, reason, missing submissions)."),
                };
            }

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
        /// Runs the named tool for the given user and role, returning a compact JSON
        /// string to feed back to the model. Unknown tool names return an error
        /// object rather than throwing. The userId is supplied by the server from
        /// the session, never by the model.
        /// </summary>
        public static string Execute(string toolName, int userId, string role)
        {
            return IsLecturer(role)
                ? ExecuteLecturer(toolName, userId)
                : ExecuteStudent(toolName, userId);
        }

        private static bool IsLecturer(string role)
        {
            return string.Equals(role, "LECTURER", System.StringComparison.OrdinalIgnoreCase);
        }

        private static string ExecuteStudent(string toolName, int userId)
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

        private static string ExecuteLecturer(string toolName, int userId)
        {
            switch (toolName)
            {
                case "get_my_courses":
                    return Serialize(LecturerCourseService.GetCourses(userId).Select(c => new
                    {
                        code = c.CourseCode,
                        name = c.CourseName,
                        creditHours = c.CreditHours,
                        semester = c.SemesterName,
                        enrolled = c.EnrolledCount,
                        status = c.Status
                    }));

                case "get_at_risk_students":
                    var lecturer = LecturerService.GetByUserId(userId);
                    if (lecturer == null) return "{\"error\":\"not a lecturer\"}";
                    return Serialize(LecturerPortalService.GetAtRiskStudents(lecturer.LecturerId).Select(s => new
                    {
                        student = s.StudentName,
                        studentNo = s.StudentNo,
                        course = s.CourseCode + " " + s.CourseName,
                        riskLevel = s.RiskLevel,
                        reason = s.RiskReason,
                        missingSubmissions = s.MissingSubmissions
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
