using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class LecturerDashboardService
    {
        public static LecturerDashboardData GetDashboard(UserContext user)
        {
            if (user == null || !user.IsLecturer) return null;

            var profile = LecturerProfileReader.GetProfile(user);
            if (profile == null) return null;

            var current = AcademicTermReader.GetCurrentTerm();
            var currentLabel = TermLabel(current);
            var allCourses = LecturerCourseReader.GetCourses(user);
            var currentCourses = allCourses
                .Where(c => string.IsNullOrEmpty(currentLabel) || IsSameTerm(c.SemesterName, currentLabel))
                .ToList();

            var todayName = DateTime.Today.DayOfWeek.ToString();
            var todayClasses = LecturerTimetableReader.GetClassSessions(user)
                .Where(s => string.Equals(s.DayOfWeek, todayName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.StartTime)
                .ToList();

            var toGrade = GetGradingItems(user);
            var announcements = LecturerAnnouncementReader.GetAnnouncements(user, null).Take(5).ToList();

            return new LecturerDashboardData
            {
                Profile = profile,
                CurrentTerm = current,
                Courses = currentCourses,
                TodayClasses = todayClasses,
                ToGrade = toGrade,
                Announcements = announcements,
                ActiveCourses = currentCourses.Count,
                StudentsTaught = currentCourses.Sum(c => c.EnrolledCount),
                SubmissionsToReview = toGrade.Sum(g => g.PendingCount),
                AttendanceRate = AttendanceRate(user)
            };
        }

        private static List<LecturerGradingItem> GetGradingItems(UserContext user)
        {
            string sql =
                "SELECT a.title, a.due_date, c.course_code, " +
                "(SELECT COUNT(*) FROM SUBMISSIONS sub WHERE sub.assignment_id = a.assignment_id " +
                " AND sub.marks_obtained IS NULL) AS pending_count " +
                "FROM ASSIGNMENTS a " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = a.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "WHERE " + ServiceAccess.VisibleOfferScope("co");

            var list = new List<LecturerGradingItem>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var pending = IntValue(reader["pending_count"]);
                        if (pending == 0) continue;
                        list.Add(new LecturerGradingItem
                        {
                            Title = Text(reader["title"]),
                            CourseCode = Text(reader["course_code"]),
                            DueDate = DateValue(reader["due_date"]) ?? DateTime.Today,
                            PendingCount = pending
                        });
                    }
                }
            }
            return list.OrderBy(g => g.DueDate).ToList();
        }

        private static decimal? AttendanceRate(UserContext user)
        {
            string sql =
                "SELECT ar.status FROM ATTENDANCE_SESSIONS ats " +
                "JOIN ATTENDANCE_RECORDS ar ON ar.session_id = ats.session_id " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = ats.offer_id " +
                "WHERE " + ServiceAccess.VisibleOfferScope("co");
            int total = 0, present = 0;
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        total++;
                        if (string.Equals(Text(reader["status"]), "PRESENT", StringComparison.OrdinalIgnoreCase)) present++;
                    }
                }
            }
            return total == 0 ? (decimal?)null : Math.Round((decimal)present / total, 4);
        }
    }
}
