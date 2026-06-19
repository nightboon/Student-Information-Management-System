using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Script.Serialization;
using src.services;

namespace src.lecturer
{
    public partial class lecturer_take_attendance : src.security.LecturerPage
    {
        private string _lecturerId;
        private int _offeringId;
        private DateTime _date;
        private AttendanceOffering _offering;
        private List<RosterEntry> _roster = new List<RosterEntry>();

        protected void Page_Load(object sender, EventArgs e)
        {
            var user = UserContextFactory.FromSession(Session);
            var profile = LecturerPortalService.GetProfile(user);
            if (profile == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }
            _lecturerId = profile.LecturerId;

            // offering is required; date defaults to today when missing or malformed.
            if (!int.TryParse(Request.QueryString["offering"], out _offeringId))
            {
                Response.Redirect("~/lecturer/lecturer_attendance.aspx");
                return;
            }
            _date = ParseDate(Request.QueryString["date"]);

            // Authorisation: only offerings this lecturer teaches resolve here.
            _offering = LecturerPortalService.GetAttendanceOffering(user, _offeringId);
            if (_offering == null)
            {
                Response.Redirect("~/lecturer/lecturer_attendance.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadRoster();
            }
        }

        private void LoadRoster()
        {
            var user = UserContextFactory.FromSession(Session);
            _roster = LecturerPortalService.GetAttendanceRoster(user, _offeringId, _date);
            rosterRepeater.DataSource = _roster;
            rosterRepeater.DataBind();
            attendanceData.Value = BuildInitialJson(_roster);
            emptyPanel.Visible = _roster.Count == 0;
        }

        protected void SaveAttendance_Click(object sender, EventArgs e)
        {
            var statuses = ParseSubmitted(attendanceData.Value);
            int saved = LecturerPortalService.SaveAttendance(UserContextFactory.FromSession(Session), _offeringId, _date, statuses);

            // Re-read so the roster reflects exactly what is now stored.
            LoadRoster();

            savedBanner.Visible = true;
            savedMessage.Text = saved == 1
                ? "Attendance saved for 1 student."
                : "Attendance saved for " + saved.ToString(CultureInfo.InvariantCulture) + " students.";
        }

        // ----- Header -----

        protected string CourseCode { get { return _offering != null ? _offering.CourseCode : ""; } }
        protected string CourseName { get { return _offering != null ? _offering.CourseName : ""; } }

        protected string DateDisplay
        {
            get { return _date.ToString("dddd, d MMMM yyyy", CultureInfo.InvariantCulture); }
        }

        protected string EnrolledCountDisplay
        {
            get
            {
                int n = _offering != null ? _offering.EnrolledCount : 0;
                return n + (n == 1 ? " student" : " students");
            }
        }

        /// <summary>Accent color for the course icon, validated to a 6-digit hex.</summary>
        protected string AccentColor
        {
            get
            {
                var color = _offering != null ? _offering.Color : null;
                if (string.IsNullOrEmpty(color)) return "#64748b";
                return System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$")
                    ? color : "#64748b";
            }
        }

        protected string BackUrl { get { return ResolveUrl("~/lecturer/lecturer_attendance.aspx"); } }

        // ----- Roster row helpers -----

        /// <summary>"true" when the row's stored status matches this segment, for data-active styling.</summary>
        protected string SegActive(object rowStatus, string target)
        {
            var status = rowStatus == null ? "" : rowStatus.ToString();
            return string.Equals(status, target, StringComparison.OrdinalIgnoreCase) ? "true" : "false";
        }

        // ----- Plumbing -----

        private static DateTime ParseDate(string value)
        {
            DateTime parsed;
            if (!string.IsNullOrEmpty(value) &&
                DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out parsed))
            {
                return parsed.Date;
            }
            return DateTime.Today;
        }

        /// <summary>Initial {enrolmentId: status} map of statuses already recorded.</summary>
        private static string BuildInitialJson(List<RosterEntry> roster)
        {
            var map = new Dictionary<string, string>();
            foreach (var entry in roster)
            {
                if (!string.IsNullOrEmpty(entry.Status))
                {
                    map[entry.EnrolmentId.ToString(CultureInfo.InvariantCulture)] = entry.Status;
                }
            }
            return new JavaScriptSerializer().Serialize(map);
        }

        /// <summary>Parses the posted {enrolmentId: status} map into a typed dictionary.</summary>
        private static IDictionary<int, string> ParseSubmitted(string json)
        {
            var result = new Dictionary<int, string>();
            if (string.IsNullOrWhiteSpace(json)) return result;

            var raw = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json);
            if (raw == null) return result;

            foreach (var pair in raw)
            {
                int enrolmentId;
                if (int.TryParse(pair.Key, out enrolmentId) && !string.IsNullOrEmpty(pair.Value))
                {
                    result[enrolmentId] = pair.Value;
                }
            }
            return result;
        }
    }
}
