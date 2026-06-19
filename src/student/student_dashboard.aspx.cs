using System;
using System.Globalization;
using System.Linq;
using System.Web;
using src.services;

namespace src.student
{
    public partial class student_dashboard : src.security.StudentPage
    {
        private StudentDashboardData _dashboard;

        protected string Greeting
        {
            get
            {
                int hour = DateTime.Now.Hour;
                if (hour >= 5 && hour < 12) return "Good Morning";
                if (hour >= 12 && hour < 17) return "Good Afternoon";
                if (hour >= 17 && hour < 21) return "Good Evening";
                return "Good Night";
            }
        }

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

            var user = UserContextFactory.FromSession(Session);
            _dashboard = StudentPortalService.GetDashboard(user, ReadNotificationIds());
            if (_dashboard == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }

            coursesRepeater.DataSource = _dashboard.Courses.Where(c => c.IsCurrent).Take(4).ToList();
            coursesRepeater.DataBind();

            scheduleRepeater.DataSource = _dashboard.TodayClasses;
            scheduleRepeater.DataBind();

            assignmentsRepeater.DataSource = _dashboard.AssignmentsDueThisWeek;
            assignmentsRepeater.DataBind();

            announcementsRepeater.DataSource = _dashboard.Announcements;
            announcementsRepeater.DataBind();
        }

        protected string CurrentDateLabel
        {
            get { return DateTime.Now.ToString("dddd, d MMMM yyyy", CultureInfo.InvariantCulture); }
        }

        protected int SemesterWeek
        {
            get
            {
                if (_dashboard == null || _dashboard.CurrentTerm == null) return 1;
                int week = ((DateTime.Today - _dashboard.CurrentTerm.StartDate.Date).Days / 7) + 1;
                return Math.Max(1, week);
            }
        }

        protected string SemesterNumber
        {
            get
            {
                return _dashboard != null && _dashboard.Account != null
                    ? Math.Max(1, _dashboard.Account.CurrentSemesterNo).ToString(CultureInfo.InvariantCulture)
                    : "";
            }
        }

        protected string GetUserName
        {
            get { return _dashboard != null && _dashboard.Account != null ? _dashboard.Account.FullName : "Student"; }
        }

        protected int TodayClassCount
        {
            get { return _dashboard != null && _dashboard.TodayClasses != null ? _dashboard.TodayClasses.Count : 0; }
        }

        protected int AssignmentDueCount
        {
            get { return _dashboard != null && _dashboard.AssignmentsDueThisWeek != null ? _dashboard.AssignmentsDueThisWeek.Count : 0; }
        }

        protected int PendingTaskCount
        {
            get { return _dashboard != null ? _dashboard.PendingTaskCount : 0; }
        }

        protected string CgpaDisplay
        {
            get { return _dashboard != null && _dashboard.Cgpa.HasValue ? _dashboard.Cgpa.Value.ToString("0.00", CultureInfo.InvariantCulture) : "-"; }
        }

        protected string AttendanceDisplay
        {
            get
            {
                return _dashboard != null && _dashboard.AttendanceRate.HasValue
                    ? Math.Round(_dashboard.AttendanceRate.Value * 100m).ToString("0", CultureInfo.InvariantCulture) + "%"
                    : "-";
            }
        }

        protected int CreditsEarnedValue
        {
            get { return _dashboard != null ? _dashboard.CreditsEarned : 0; }
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

        protected string TodayScheduleSubtitle
        {
            get
            {
                if (_dashboard == null || _dashboard.TodayClasses == null || _dashboard.TodayClasses.Count == 0)
                {
                    return "No classes today";
                }

                TimeSpan total = TimeSpan.Zero;
                foreach (var session in _dashboard.TodayClasses)
                {
                    total += session.EndTime - session.StartTime;
                }

                int count = _dashboard.TodayClasses.Count;
                string duration = (int)total.TotalHours + "h " + total.Minutes + "m";
                return count + (count == 1 ? " class" : " classes") + " - " + duration + " total";
            }
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
            return (due.Date - DateTime.Today).Days <= 1 ? "alert-circle" : "check-circle-2";
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
            return (due.Date - DateTime.Today).Days <= 1 ? "text-[#e0162b] font-semibold" : "text-slate-500";
        }

        protected int AnnouncementCount
        {
            get { return _dashboard != null && _dashboard.Announcements != null ? _dashboard.Announcements.Count : 0; }
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

        private System.Collections.Generic.ISet<int> ReadNotificationIds()
        {
            var ids = Session["student_notification_read_ids"] as System.Collections.Generic.ISet<int>;
            return ids ?? new System.Collections.Generic.HashSet<int>();
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
