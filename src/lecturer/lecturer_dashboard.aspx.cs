using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using src.services;

namespace student_information_management_system
{
    public partial class lecturer_dashboard : src.security.LecturerPage
    {
        private Lecturer _lecturer;
        private Semester _semester;
        private List<ClassSession> _todayClasses = new List<ClassSession>();
        private List<GradingItem> _toGrade = new List<GradingItem>();
        private List<LecturerCourse> _courses = new List<LecturerCourse>();
        private List<Announcement> _announcements = new List<Announcement>();
        private int _submissionsToReview;
        private int _activeCourses;
        private int _studentsTaught;
        private decimal? _attendanceRate;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] != null)
            {
                int userId = (int)Session["user_id"];
                _lecturer = LecturerService.GetByUserId(userId);
            }
            _semester = SemesterService.GetCurrent();

            if (_lecturer != null)
            {
                int lecturerId = _lecturer.LecturerId;

                _todayClasses = LecturerService.GetTodayClasses(lecturerId);
                _toGrade = LecturerService.GetAssignmentsToGrade(lecturerId);
                _announcements = LecturerService.GetAnnouncements(lecturerId);
                _submissionsToReview = LecturerService.CountSubmissionsToReview(lecturerId);
                _activeCourses = LecturerService.CountActiveCourses(lecturerId);
                _studentsTaught = LecturerService.CountStudentsTaught(lecturerId);
                _attendanceRate = LecturerService.GetCurrentSemesterAttendanceRate(lecturerId);

                // My Courses: the lecturer's current-semester offerings (newest first
                // already), capped to keep the dashboard card compact.
                _courses = LecturerCourseService.GetCourses(_lecturer.UserId)
                    .Where(c => _semester != null && c.SemesterName == _semester.Name)
                    .Take(4)
                    .ToList();

                scheduleRepeater.DataSource = _todayClasses;
                scheduleRepeater.DataBind();
                gradeRepeater.DataSource = _toGrade;
                gradeRepeater.DataBind();
                coursesRepeater.DataSource = _courses;
                coursesRepeater.DataBind();
                announcementsRepeater.DataSource = _announcements;
                announcementsRepeater.DataBind();
            }
        }

        /// <summary>Time-of-day greeting: morning before noon, afternoon before 6pm, otherwise evening.</summary>
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

        /// <summary>"4 classes" / "1 class" — classes the lecturer teaches today.</summary>
        protected string ClassesTodayLabel
        {
            get { return _todayClasses.Count + (_todayClasses.Count == 1 ? " class" : " classes"); }
        }

        /// <summary>"12 submissions" / "1 submission" — ungraded submissions awaiting review.</summary>
        protected string SubmissionsToReviewLabel
        {
            get { return _submissionsToReview + (_submissionsToReview == 1 ? " submission" : " submissions"); }
        }

        /// <summary>
        /// The signed-in lecturer's display name, falling back to the login
        /// username and then a generic label when the profile can't be resolved.
        /// </summary>
        protected string LecturerName
        {
            get
            {
                if (_lecturer != null && !string.IsNullOrEmpty(_lecturer.FullName))
                    return _lecturer.FullName;
                return Session["username"] as string ?? "Lecturer";
            }
        }

        /// <summary>Today's date, e.g. "Sunday, 24 May 2026" (day-first, culture-invariant).</summary>
        protected string CurrentDateLabel
        {
            get { return DateTime.Now.ToString("dddd, d MMMM yyyy", CultureInfo.InvariantCulture); }
        }

        /// <summary>Current teaching week of the active semester (1 when none is configured).</summary>
        protected int SemesterWeek
        {
            get { return _semester != null ? _semester.CurrentWeek : 1; }
        }

        /// <summary>Name of the active semester, e.g. "2026-S2" (empty when none is configured).</summary>
        protected string SemesterName
        {
            get { return _semester != null ? _semester.Name : ""; }
        }

        // ----- Stat cards -----

        /// <summary>Active courses this semester.</summary>
        protected int ActiveCoursesCount
        {
            get { return _activeCourses; }
        }

        /// <summary>Whole-percent attendance across this semester's offerings, or an em dash when none.</summary>
        protected string AttendanceDisplay
        {
            get
            {
                return _attendanceRate.HasValue
                    ? Math.Round(_attendanceRate.Value * 100).ToString("0", CultureInfo.InvariantCulture) + "%"
                    : "—";
            }
        }

        /// <summary>Distinct students taught this semester.</summary>
        protected int StudentsTaughtCount
        {
            get { return _studentsTaught; }
        }

        /// <summary>Submissions awaiting grading.</summary>
        protected int PendingGradingCount
        {
            get { return _submissionsToReview; }
        }

        // ----- Today's Schedule -----

        protected int TodayClassCount
        {
            get { return _todayClasses.Count; }
        }

        /// <summary>e.g. "4 classes · 6h 30m total", or a friendly note when empty.</summary>
        protected string TodayScheduleSubtitle
        {
            get
            {
                if (_todayClasses.Count == 0) return "No classes today";

                TimeSpan total = TimeSpan.Zero;
                foreach (var session in _todayClasses)
                {
                    total += session.EndTime - session.StartTime;
                }

                string duration = (int)total.TotalHours + "h " + total.Minutes + "m";
                return ClassesTodayLabel + " · " + duration + " total";
            }
        }

        /// <summary>Course color from the DB; falls back to neutral slate when unset or malformed.</summary>
        protected string ClassColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return "#64748b";
            return System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$")
                ? color : "#64748b";
        }

        /// <summary>en dash (–) between the two HH:mm times, matching the markup.</summary>
        protected string FormatTimeRange(TimeSpan start, TimeSpan end)
        {
            return start.ToString(@"hh\:mm") + " – " + end.ToString(@"hh\:mm");
        }

        protected bool IsLiveNow(TimeSpan start, TimeSpan end)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            return now >= start && now < end;
        }

        // ----- To Grade -----

        protected int ToGradeCount
        {
            get { return _toGrade.Count; }
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

        // ----- My Courses -----

        protected int MyCoursesCount
        {
            get { return _courses.Count; }
        }

        /// <summary>e.g. "2026-S2", the active semester name for the My Courses subtitle.</summary>
        protected string MyCoursesSubtitle
        {
            get { return _semester != null ? _semester.Name : ""; }
        }

        /// <summary>Course accent color, validated to a 6-digit hex so the "15" alpha suffix stays valid CSS.</summary>
        protected string AccentColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return "#64748b";
            return System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$")
                ? color : "#64748b";
        }

        /// <summary>"60 students enrolled" / "1 student enrolled".</summary>
        protected string EnrolledLabel(int count)
        {
            return count + (count == 1 ? " student enrolled" : " students enrolled");
        }

        // ----- Announcements -----

        protected int AnnouncementCount
        {
            get { return _announcements.Count; }
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
    }
}
