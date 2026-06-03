using System;
using System.Web.UI;
using src.services;

namespace student_information_management_system
{
    public partial class lecturer_dashboard : src.security.LecturerPage
    {
        private Lecturer _lecturer;
        private Semester _semester;
        private int _classesToday;
        private int _submissionsToReview;

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
                _classesToday = LecturerService.CountClassesOn(_lecturer.LecturerId, DateTime.Now.DayOfWeek);
                _submissionsToReview = LecturerService.CountSubmissionsToReview(_lecturer.LecturerId);
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
            get { return _classesToday + (_classesToday == 1 ? " class" : " classes"); }
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
            get { return DateTime.Now.ToString("dddd, d MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture); }
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
    }
}
