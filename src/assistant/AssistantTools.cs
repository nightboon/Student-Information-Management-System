using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using src.services;

namespace src.assistant
{
    public static class AssistantTools
    {
        private static readonly JavaScriptSerializer Json =
            new JavaScriptSerializer { MaxJsonLength = 10000000 };

        public static List<object> DefinitionsFor(string role)
        {
            if (IsLecturer(role))
            {
                return new List<object>
                {
                    Tool("get_my_courses", "Get the courses this lecturer is teaching this semester."),
                    Tool("get_at_risk_students", "Get students flagged as at-risk across this lecturer's courses.")
                };
            }

            return new List<object>
            {
                Tool("get_today_classes", "Get the student's classes scheduled for today."),
                Tool("get_weekly_timetable", "Get the student's full weekly class timetable."),
                Tool("get_current_courses", "Get the list of courses the student is enrolled in."),
                Tool("get_grades", "Get the student's cumulative CGPA and total credits earned."),
                Tool("get_attendance", "Get the student's current-semester attendance summary."),
                Tool("get_assignments_due", "Get the student's assignments due within the next 7 days."),
                Tool("get_announcements", "Get the latest announcements relevant to the student's courses.")
            };
        }

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
            var user = new UserContext { UserId = userId, Role = "STUDENT" };

            switch (toolName)
            {
                case "get_today_classes":
                    var todayName = System.DateTime.Today.DayOfWeek.ToString();
                    var today = StudentPortalService.GetTimetablePage(user);
                    return Serialize((today == null ? new List<StudentClassSession>() : today.Sessions)
                        .Where(c => string.Equals(c.DayOfWeek, todayName, System.StringComparison.OrdinalIgnoreCase))
                        .Select(c => new
                        {
                            course = c.CourseCode + " " + c.CourseName,
                            time = c.StartTime.ToString(@"hh\:mm") + "-" + c.EndTime.ToString(@"hh\:mm"),
                            venue = c.Venue,
                            lecturer = c.LecturerName
                        }));

                case "get_weekly_timetable":
                    var week = StudentPortalService.GetTimetablePage(user);
                    return Serialize((week == null ? new List<StudentClassSession>() : week.Sessions).Select(c => new
                    {
                        day = c.DayOfWeek,
                        course = c.CourseCode + " " + c.CourseName,
                        time = c.StartTime.ToString(@"hh\:mm") + "-" + c.EndTime.ToString(@"hh\:mm"),
                        venue = c.Venue,
                        lecturer = c.LecturerName
                    }));

                case "get_current_courses":
                    return Serialize(StudentPortalService.GetCourses(user).Select(c => new
                    {
                        code = c.CourseCode,
                        name = c.CourseName,
                        creditHours = c.CreditHours,
                        lecturer = c.LecturerName,
                        semester = c.SemesterName
                    }));

                case "get_grades":
                    var grades = StudentPortalService.GetGradePage(user);
                    return Serialize(new
                    {
                        cgpa = grades == null ? null : grades.Cgpa,
                        creditsEarned = grades == null ? 0 : grades.CreditsEarned
                    });

                case "get_attendance":
                    var attendance = StudentPortalService.GetAttendancePage(user);
                    var courses = attendance == null ? new List<StudentAttendanceCourse>() : attendance.Courses.Where(c => c.IsCurrent).ToList();
                    var present = courses.Sum(c => c.PresentCount);
                    var late = courses.Sum(c => c.LateCount);
                    var absent = courses.Sum(c => c.AbsentCount);
                    var total = courses.Sum(c => c.TotalCount);
                    return Serialize(new
                    {
                        attendanceRate = total == 0 ? null : (decimal?)System.Math.Round((decimal)present / total, 4),
                        present = present,
                        late = late,
                        absent = absent,
                        total = total
                    });

                case "get_assignments_due":
                    return Serialize(StudentPortalService.GetAssignments(user, null)
                        .Where(x => !x.HasSubmission && x.DueDate.Date <= System.DateTime.Today.AddDays(7))
                        .Select(x => new
                        {
                            title = x.Title,
                            course = x.CourseCode,
                            due = x.DueDate.ToString("yyyy-MM-dd")
                        }));

                case "get_announcements":
                    return Serialize(StudentPortalService.GetNotifications(user, new HashSet<int>()).Take(10).Select(x => new
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
            var user = new UserContext { UserId = userId, Role = "LECTURER" };
            switch (toolName)
            {
                case "get_my_courses":
                    return Serialize(LecturerPortalService.GetCourses(user).Select(c => new
                    {
                        code = c.CourseCode,
                        name = c.CourseName,
                        creditHours = c.CreditHours,
                        semester = c.SemesterName,
                        enrolled = c.EnrolledCount,
                        status = c.Status
                    }));

                case "get_at_risk_students":
                    return Serialize(LecturerPortalService.GetAtRisk(user).Select(s => new
                    {
                        student = s.StudentName,
                        studentNo = s.StudentNo,
                        course = s.CourseCode + " " + s.CourseName,
                        riskLevel = s.RiskLevel,
                        reason = s.RiskReason,
                        attendance = s.AttendancePercent,
                        currentMark = s.CurrentMark
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
