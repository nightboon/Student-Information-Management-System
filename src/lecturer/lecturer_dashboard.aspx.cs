using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using src.services;
using static src.services.StudentPortalFormat;

namespace student_information_management_system
{
    public partial class lecturer_dashboard : src.security.LecturerPage
    {
        private LecturerDashboardData _data;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            var user = UserContextFactory.FromSession(Session);
            _data = LecturerPortalService.GetDashboard(user);

            if (_data == null) return;

            scheduleRepeater.DataSource = _data.TodayClasses;
            scheduleRepeater.DataBind();
            gradeRepeater.DataSource = _data.ToGrade;
            gradeRepeater.DataBind();
            coursesRepeater.DataSource = _data.Courses.Take(4).ToList();
            coursesRepeater.DataBind();
            announcementsRepeater.DataSource = _data.Announcements;
            announcementsRepeater.DataBind();
        }

        protected string Greeting
        {
            get
            {
                int hour = DateTime.Now.Hour;
                if (hour < 12) return "Good morning";
                if (hour < 18) return "Good afternoon";
                return "Good evening";
            }
        }

        protected string ClassesTodayLabel
        {
            get
            {
                int count = _data != null ? _data.TodayClasses.Count : 0;
                return count + (count == 1 ? " class" : " classes");
            }
        }

        protected string SubmissionsToReviewLabel
        {
            get
            {
                int count = _data != null ? _data.SubmissionsToReview : 0;
                return count + (count == 1 ? " submission" : " submissions");
            }
        }

        protected string LecturerName
        {
            get
            {
                if (_data != null && _data.Profile != null && !string.IsNullOrEmpty(_data.Profile.FullName))
                    return _data.Profile.FullName;
                return Session["username"] as string ?? "Lecturer";
            }
        }

        protected string CurrentDateLabel
        {
            get { return DateTime.Now.ToString("dddd, d MMMM yyyy", CultureInfo.InvariantCulture); }
        }

        protected int SemesterWeek
        {
            get
            {
                if (_data == null || _data.CurrentTerm == null) return 1;
                return Math.Max(1, ((DateTime.Today - _data.CurrentTerm.StartDate.Date).Days / 7) + 1);
            }
        }

        protected string SemesterName
        {
            get { return _data != null ? TermLabel(_data.CurrentTerm) : ""; }
        }

        protected int ActiveCoursesCount
        {
            get { return _data != null ? _data.ActiveCourses : 0; }
        }

        protected string AttendanceDisplay
        {
            get
            {
                if (_data == null || !_data.AttendanceRate.HasValue) return "-";
                return Math.Round(_data.AttendanceRate.Value * 100).ToString("0", CultureInfo.InvariantCulture) + "%";
            }
        }

        protected int StudentsTaughtCount
        {
            get { return _data != null ? _data.StudentsTaught : 0; }
        }

        protected int PendingGradingCount
        {
            get { return _data != null ? _data.SubmissionsToReview : 0; }
        }

        protected int TodayClassCount
        {
            get { return _data != null ? _data.TodayClasses.Count : 0; }
        }

        protected string TodayScheduleSubtitle
        {
            get
            {
                if (_data == null || _data.TodayClasses.Count == 0) return "No classes today";

                TimeSpan total = TimeSpan.Zero;
                foreach (var session in _data.TodayClasses)
                {
                    total += session.EndTime - session.StartTime;
                }

                string duration = (int)total.TotalHours + "h " + total.Minutes + "m";
                return ClassesTodayLabel + " - " + duration + " total";
            }
        }

        protected string ClassColor(string color)
        {
            return SafeColor(color);
        }

        protected string FormatTimeRange(TimeSpan start, TimeSpan end)
        {
            return start.ToString(@"hh\:mm") + " - " + end.ToString(@"hh\:mm");
        }

        protected bool IsLiveNow(TimeSpan start, TimeSpan end)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            return now >= start && now < end;
        }

        protected int ToGradeCount
        {
            get { return _data != null ? _data.ToGrade.Count : 0; }
        }

        protected string FormatRelativeDue(DateTime due)
        {
            int days = (due.Date - DateTime.Today).Days;
            if (days < 0) return "Overdue";
            if (days == 0) return "Due today";
            if (days == 1) return "Tomorrow";
            return "In " + days + " days";
        }

        protected string DueIcon(DateTime due)
        {
            int days = (due.Date - DateTime.Today).Days;
            return days <= 1 ? "alert-circle" : "check-circle-2";
        }

        protected string DueBadgeClass(DateTime due)
        {
            int days = (due.Date - DateTime.Today).Days;
            if (days <= 1) return "bg-[#e0162b]/10 text-[#e0162b]";
            if (days <= 3) return "bg-amber-50 text-amber-600";
            return "bg-emerald-50 text-emerald-600";
        }

        protected string DueTextClass(DateTime due)
        {
            int days = (due.Date - DateTime.Today).Days;
            return days <= 1 ? "text-[#e0162b] font-semibold" : "text-slate-500";
        }

        protected int MyCoursesCount
        {
            get { return _data != null ? Math.Min(_data.Courses.Count, 4) : 0; }
        }

        protected string MyCoursesSubtitle
        {
            get { return _data != null ? TermLabel(_data.CurrentTerm) : ""; }
        }

        protected string AccentColor(string color)
        {
            return SafeColor(color);
        }

        protected string EnrolledLabel(int count)
        {
            return count + (count == 1 ? " student enrolled" : " students enrolled");
        }

        protected int AnnouncementCount
        {
            get { return _data != null ? _data.Announcements.Count : 0; }
        }

        protected string FormatRelativeTime(DateTime when)
        {
            TimeSpan ago = DateTime.Now - when;
            if (ago.TotalMinutes < 1) return "Just now";
            if (ago.TotalMinutes < 60) return (int)ago.TotalMinutes + "m ago";
            if (ago.TotalHours < 24) return (int)ago.TotalHours + "h ago";

            int days = (int)ago.TotalDays;
            if (days == 1) return "Yesterday";
            if (days < 7) return days + " days ago";
            return when.ToString("d MMM yyyy", CultureInfo.InvariantCulture);
        }

        private static string SafeColor(string color)
        {
            if (string.IsNullOrEmpty(color) || color.Length != 7 || color[0] != '#') return "#64748b";
            for (int i = 1; i < color.Length; i++)
            {
                if (!Uri.IsHexDigit(color[i])) return "#64748b";
            }
            return color;
        }
    }
}
