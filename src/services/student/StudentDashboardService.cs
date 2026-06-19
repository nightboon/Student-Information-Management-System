using System;
using System.Collections.Generic;
using System.Linq;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class StudentDashboardService
    {
        public static StudentDashboardData GetDashboard(UserContext user, ISet<int> readIds)
        {
            if (!IsStudent(user)) return null;

            var account = StudentProfileReader.GetAccount(user);
            var studentId = account == null ? null : account.StudentId;
            var courses = account == null ? new List<StudentCourseCard>() : StudentCourseReader.GetCourses(user, studentId);
            var timetable = StudentTimetableReader.GetTimetablePage(user);
            var gradePage = StudentGradeReader.GetGradePage(user);
            var attendance = account == null ? null : StudentAttendanceReader.GetAttendancePage(user, studentId);
            var assignments = account == null ? new List<StudentCourseAssignment>() : StudentAssignmentReader.GetAssignments(user, studentId, null);
            var todayName = DateTime.Today.DayOfWeek.ToString();
            var todayClasses = timetable == null
                ? new List<StudentClassSession>()
                : timetable.Sessions.Where(s => string.Equals(s.DayOfWeek, todayName, StringComparison.OrdinalIgnoreCase)).OrderBy(s => s.StartTime).ToList();
            var dueSoon = assignments
                .Where(a => !a.HasSubmission && a.DueDate.Date >= DateTime.Today && a.DueDate.Date <= DateTime.Today.AddDays(7))
                .OrderBy(a => a.DueDate)
                .Select(a => new StudentDashboardAssignment { Title = a.Title, CourseCode = a.CourseCode, DueDate = a.DueDate })
                .ToList();
            var currentAttendanceCourses = attendance == null ? new List<StudentAttendanceCourse>() : attendance.Courses.Where(c => c.IsCurrent).ToList();
            var total = currentAttendanceCourses.Sum(c => c.TotalCount);
            var present = currentAttendanceCourses.Sum(c => c.PresentCount);

            return new StudentDashboardData
            {
                Account = account,
                CurrentTerm = AcademicTermReader.GetCurrentTerm(),
                Courses = courses,
                TodayClasses = todayClasses,
                AssignmentsDueThisWeek = dueSoon,
                Announcements = StudentAnnouncementReader.GetNotifications(user, readIds).Take(5).ToList(),
                Cgpa = gradePage == null ? null : gradePage.Cgpa,
                AttendanceRate = total == 0 ? (decimal?)null : Math.Round((decimal)present / total, 4),
                CreditsEarned = gradePage == null ? 0 : gradePage.CreditsEarned,
                PendingTaskCount = assignments.Count(a => !a.HasSubmission)
            };
        }
    }
}
