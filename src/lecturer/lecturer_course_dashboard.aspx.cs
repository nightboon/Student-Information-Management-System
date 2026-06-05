using System;
using System.Globalization;
using System.Linq;
using src.services;

namespace src.lecturer
{
    public partial class lecturer_course_dashboard : src.security.LecturerPage
    {
        private int _offeringId;
        private CourseDashboardStats _stats;
        private int _announcementCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }
            int userId = (int)Session["user_id"];

            if (!int.TryParse(Request.QueryString["offering"], out _offeringId))
            {
                Response.Redirect("~/lecturer/lecturer_courses.aspx");
                return;
            }

            var lecturer = LecturerService.GetByUserId(userId);
            if (lecturer == null)
            {
                Response.Redirect("~/lecturer/lecturer_courses.aspx");
                return;
            }

            // Returns null unless this lecturer teaches the offering (authorisation).
            _stats = LecturerCourseDashboardService.GetStats(_offeringId, lecturer.LecturerId);
            if (_stats == null)
            {
                Response.Redirect("~/lecturer/lecturer_courses.aspx");
                return;
            }

            // The announcements list feeds both the card count and the latest panel.
            // GetByOffering orders pinned-first, so re-sort by date for "latest".
            var announcements = AnnouncementService.GetByOffering(_offeringId);
            _announcementCount = announcements.Count;
            announcementsRepeater.DataSource = announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(3)
                .ToList();
            announcementsRepeater.DataBind();
        }

        // ----- Header -----

        protected string CourseCode { get { return _stats.CourseCode; } }
        protected string CourseName { get { return _stats.CourseName; } }
        protected string Description { get { return _stats.Description; } }
        protected string LevelLabel { get { return _stats.LevelLabel; } }
        protected int CreditHours { get { return _stats.CreditHours; } }
        protected string SemesterName { get { return _stats.SemesterName; } }

        /// <summary>Course accent color, or a neutral slate fallback for null/malformed values.</summary>
        protected string AccentColor
        {
            get
            {
                var color = _stats.Color;
                if (string.IsNullOrEmpty(color)) return "#64748b";
                return System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$")
                    ? color : "#64748b";
            }
        }

        // ----- Overview metrics -----

        protected int EnrolledCount { get { return _stats.EnrolledCount; } }
        protected int PendingGrading { get { return _stats.PendingGrading; } }

        /// <summary>Class average over published grades, e.g. "72.3%", or "—" when none.</summary>
        protected string AverageGradeDisplay
        {
            get
            {
                return _stats.AverageGrade.HasValue
                    ? _stats.AverageGrade.Value.ToString("0.#", CultureInfo.InvariantCulture) + "%"
                    : "—";
            }
        }

        /// <summary>Present-rate over recorded attendance, e.g. "92%", or "—" when none.</summary>
        protected string AttendanceDisplay
        {
            get
            {
                return _stats.AttendanceRate.HasValue
                    ? Math.Round(_stats.AttendanceRate.Value * 100).ToString("0", CultureInfo.InvariantCulture) + "%"
                    : "—";
            }
        }

        // ----- Card labels -----

        /// <summary>"28 students" / "1 student".</summary>
        protected string EnrolledLabel
        {
            get { return _stats.EnrolledCount + (_stats.EnrolledCount == 1 ? " student" : " students"); }
        }

        /// <summary>"3 announcements" / "1 announcement".</summary>
        protected string AnnouncementLabel
        {
            get { return _announcementCount + (_announcementCount == 1 ? " announcement" : " announcements"); }
        }

        /// <summary>"6 assessments" / "1 assessment".</summary>
        protected string AssessmentLabel
        {
            get { return _stats.AssessmentCount + (_stats.AssessmentCount == 1 ? " assessment" : " assessments"); }
        }

        /// <summary>"12 files" / "1 file".</summary>
        protected string MaterialLabel
        {
            get { return _stats.MaterialCount + (_stats.MaterialCount == 1 ? " file" : " files"); }
        }

        /// <summary>Number of announcements (drives the empty-state on the latest panel).</summary>
        protected int AnnouncementCount { get { return _announcementCount; } }

        // ----- Sub-page links (carry the offering id) -----

        protected string AnnouncementsUrl { get { return SubUrl("lecturer_course_announcements.aspx"); } }
        protected string PeopleUrl { get { return SubUrl("lecturer_course_people.aspx"); } }
        protected string GradesUrl { get { return SubUrl("lecturer_course_grades.aspx"); } }
        protected string MaterialsUrl { get { return SubUrl("lecturer_course_materials.aspx"); } }

        private string SubUrl(string page)
        {
            return ResolveUrl("~/lecturer/" + page) + "?offering=" + _offeringId;
        }
    }
}
