using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using src.db;
using src.services;

namespace src.student
{
    public partial class student_dashboard : src.security.StudentPage
    {
        private Student _student;
        private Semester _semester;

        protected string Greeting
        {
            get
            {
                int hour = DateTime.Now.Hour;
                string greeting_msg = "Good";

                if (hour >= 5 && hour < 12)
                {
                    greeting_msg += " Morning";
                }else if(hour >= 12 && hour < 17)
                {
                    greeting_msg += " Afternoon";
                }
                else if (hour >= 17 && hour < 21)
                {
                    greeting_msg += " Evening";
                }
                else
                {
                    greeting_msg += " Night";
                }

                return greeting_msg;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["user_id"] == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            _student = StudentService.GetByUserId((int)Session["user_id"]);
            _semester = SemesterService.GetCurrent();
            if (_student != null)
            {
                coursesRepeater.DataSource = _student.CurrentCourses;
                coursesRepeater.DataBind();
                scheduleRepeater.DataSource = _student.TodayClasses;
                scheduleRepeater.DataBind();
                assignmentsRepeater.DataSource = _student.AssignmentsDueThisWeek;
                assignmentsRepeater.DataBind();
                announcementsRepeater.DataSource = _student.Announcements;
                announcementsRepeater.DataBind();
            }
        }

        protected string CurrentDateLabel
        {
            // e.g. "Sunday, 24 May 2026" (day-first, invariant so it doesn't shift with server culture).
            get { return DateTime.Now.ToString("dddd, d MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture); }
        }

        protected int SemesterWeek
        {
            get { return _semester != null ? _semester.CurrentWeek : 1; }
        }

        protected string SemesterNumber
        {
            get { return _student != null ? _student.CurrentSemesterNo.ToString(System.Globalization.CultureInfo.InvariantCulture) : ""; }
        }

        protected string GetUserName
        {

            get { return _student != null ? _student.FullName : "Student"; }
        }

        protected int TodayClassCount
        {
            // Today's classes come from the student profile loaded in Page_Load.
            get { return _student != null && _student.TodayClasses != null ? _student.TodayClasses.Count : 0; }
        }

        protected int AssignmentDueCount
        {
            // Assignments due this week, from the student profile loaded in Page_Load.
            get { return _student != null && _student.AssignmentsDueThisWeek != null ? _student.AssignmentsDueThisWeek.Count : 0; }
        }

        protected int PendingTaskCount
        {
            // Current-semester assignments the student has not yet submitted.
            get { return _student != null ? _student.PendingTaskCount : 0; }
        }

        protected string CgpaDisplay
        {
            // 2-decimal CGPA, or an em dash when the student has no published grades.
            get
            {
                return _student != null && _student.Cgpa.HasValue
                    ? _student.Cgpa.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                    : "—";
            }
        }

        protected string AttendanceDisplay
        {
            // Whole-percent attendance, or an em dash when there are no records.
            get
            {
                return _student != null && _student.AttendanceRate.HasValue
                    ? System.Math.Round(_student.AttendanceRate.Value * 100).ToString("0", System.Globalization.CultureInfo.InvariantCulture) + "%"
                    : "—";
            }
        }

        protected int CreditsEarnedValue
        {
            get { return _student != null ? _student.CreditsEarned : 0; }
        }

        protected string ClassColor(string color)
        {
            // Course color from the DB; falls back to neutral slate when unset.
            return string.IsNullOrEmpty(color) ? "#64748b" : color;
        }

        protected string FormatTimeRange(TimeSpan start, TimeSpan end)
        {
            // en dash (–) between the two HH:mm times, matching the markup.
            return start.ToString(@"hh\:mm") + " – " + end.ToString(@"hh\:mm");
        }

        protected bool IsLiveNow(TimeSpan start, TimeSpan end)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            return now >= start && now < end;
        }

        protected string TodayScheduleSubtitle
        {
            // e.g. "4 classes · 6h 30m total", or a friendly note when empty.
            get
            {
                if (_student == null || _student.TodayClasses == null || _student.TodayClasses.Count == 0)
                {
                    return "No classes today";
                }

                int count = _student.TodayClasses.Count;
                TimeSpan total = TimeSpan.Zero;
                foreach (var session in _student.TodayClasses)
                {
                    total += session.EndTime - session.StartTime;
                }

                string duration = (int)total.TotalHours + "h " + total.Minutes + "m";
                return count + (count == 1 ? " class" : " classes") + " · " + duration + " total";
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

        protected int AnnouncementCount
        {
            get { return _student != null && _student.Announcements != null ? _student.Announcements.Count : 0; }
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
            return when.ToString("d MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }

    }
}