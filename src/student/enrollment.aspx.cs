using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using src.services;

namespace src.student
{
    public partial class enrollment : src.security.StudentPage
    {
        private StudentRegistrationTerm _regSemester;
        private StudentRegistrationWindow _window;
        private List<StudentOfferingOption> _offerings;
        private int _semesterNo;

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
            var page = StudentPortalService.GetEnrollmentPage(user);
            if (page == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }

            _regSemester = page.Term;
            _window = page.Window;
            _offerings = page.Offerings ?? new List<StudentOfferingOption>();
            _semesterNo = page.SemesterNo;

            offeringsRepeater.DataSource = _offerings;
            offeringsRepeater.DataBind();
        }

        // --- Header -----------------------------------------------------------

        /// <summary>"Academic Year 2026 / 2027" from the registration term start year.</summary>
        protected string AcademicYearLabel
        {
            get
            {
                if (_regSemester == null) return "";
                int y = _regSemester.StartDate.Year;
                return "Academic Year " + y + " / " + (y + 1);
            }
        }

        /// <summary>Registration term name + month, e.g. "2026-S3 (Aug 2026)".</summary>
        protected string TermLabel
        {
            get
            {
                if (_regSemester == null) return "the upcoming semester";
                return _regSemester.Name + " (" + _regSemester.StartDate.ToString("MMM yyyy") + ")";
            }
        }

        /// <summary>
        /// "Y2 &middot; Trimester 1" for the registration term. The registration
        /// term is one semester after the student's current semester number; three
        /// trimesters make a year of study. Falls back to empty when unknown.
        /// </summary>
        protected string YearAndTrimesterLabel
        {
            get
            {
                if (_semesterNo <= 0) return TermLabel;
                int regNo = _semesterNo + 1;
                int year = ((regNo - 1) / 3) + 1;
                int trimester = ((regNo - 1) % 3) + 1;
                return "Y" + year + " · Trimester " + trimester;
            }
        }

        // --- Registration window ---------------------------------------------

        protected bool RegistrationOpen { get { return _window != null && _window.IsOpen; } }

        protected int ActivePhase { get { return _window != null ? _window.ActivePhase : 1; } }

        /// <summary>"1 May - 15 Jun 2026" registration date range, or "" if unset.</summary>
        protected string RegistrationDateRange
        {
            get
            {
                if (_window == null || !_window.RegistrationStart.HasValue || !_window.RegistrationEnd.HasValue)
                    return "";
                return _window.RegistrationStart.Value.ToString("d MMM") + " - "
                     + _window.RegistrationEnd.Value.ToString("d MMM yyyy");
            }
        }

        /// <summary>"1 - 14 Sep 2026" add/drop date range, or "" if unset.</summary>
        protected string AddDropDateRange
        {
            get
            {
                if (_window == null || !_window.AddDropStart.HasValue || !_window.AddDropEnd.HasValue)
                    return "";
                return _window.AddDropStart.Value.ToString("d MMM") + " - "
                     + _window.AddDropEnd.Value.ToString("d MMM yyyy");
            }
        }

        // --- Stats ------------------------------------------------------------

        /// <summary>Courses the student is already registered for in the registration term.</summary>
        protected int AlreadyRegisteredCount
        {
            get
            {
                if (_offerings == null) return 0;
                return _offerings.Count(o => o.MyStatus == "ENROLLED" || o.MyStatus == "PENDING");
            }
        }

        /// <summary>Flat fee-per-credit rate, echoed into JS for live fee totals.</summary>
        protected decimal FeePerCredit
        {
            get
            {
                if (_offerings != null && _offerings.Count > 0) return _offerings[0].FeePerCredit;
                return 150m;
            }
        }

        // --- Row helpers ------------------------------------------------------

        /// <summary>Course accent color from the DB, validated to a 6-digit hex; slate fallback.</summary>
        protected string AccentColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return "#64748b";
            return System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$")
                ? color : "#64748b";
        }

        protected bool RowRegistered(object myStatus)
        {
            var s = myStatus as string;
            return s == "ENROLLED" || s == "PENDING";
        }

        protected bool RowFull(object myStatus, object enrolled, object capacity)
        {
            if (RowRegistered(myStatus)) return false;
            int en = Convert.ToInt32(enrolled);
            int cap = Convert.ToInt32(capacity);
            return cap > 0 && en >= cap;
        }

        protected bool RowOpen(object myStatus, object enrolled, object capacity)
        {
            return RegistrationOpen
                && !RowRegistered(myStatus)
                && !RowFull(myStatus, enrolled, capacity);
        }

        /// <summary>Green seat dot when seats remain, red when full.</summary>
        protected string SeatDotColor(object enrolled, object capacity)
        {
            int en = Convert.ToInt32(enrolled);
            int cap = Convert.ToInt32(capacity);
            return (cap > 0 && en >= cap) ? "#fecaca" : "#bbf7d0";
        }

        protected decimal RowFee(object credits)
        {
            return Convert.ToInt32(credits) * FeePerCredit;
        }

        // --- Persistence ------------------------------------------------------

        [WebMethod(EnableSession = true)]
        public static object Enrol(int[] offeringIds)
        {
            var ctx = HttpContext.Current;
            if (ctx.Session["user_id"] == null)
            {
                ctx.Response.StatusCode = 401;
                ctx.Response.SuppressContent = true;
                return null;
            }

            var user = UserContextFactory.FromSession(ctx.Session);
            int inserted = StudentPortalService.Enrol(user, offeringIds ?? new int[0]);
            return new { ok = true, inserted = inserted };
        }
    }
}
