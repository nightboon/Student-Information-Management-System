using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using src.services;

namespace src.lecturer
{
    public partial class lecturer_take_attendance : src.security.LecturerPage
    {
        private AttendanceOffering _offering;
        private List<RosterEntry> _roster = new List<RosterEntry>();

        protected void Page_Load(object sender, EventArgs e)
        {
            var user = UserContextFactory.FromSession(Session);
            if (LecturerPortalService.GetProfile(user) == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                int requestedOffering;
                int.TryParse(Request.QueryString["offering"], out requestedOffering);
                BindCourses(user, requestedOffering);
                dateInput.Text = ParseDate(Request.QueryString["date"]).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                startTimeInput.Text = ParseTime(Request.QueryString["start"], new TimeSpan(9, 0, 0)).ToString(@"hh\:mm");
                endTimeInput.Text = ParseTime(Request.QueryString["end"], new TimeSpan(10, 0, 0)).ToString(@"hh\:mm");

                if (requestedOffering > 0 &&
                    courseSelect.Items.FindByValue(requestedOffering.ToString(CultureInfo.InvariantCulture)) != null)
                {
                    courseSelect.SelectedValue = requestedOffering.ToString(CultureInfo.InvariantCulture);
                }
            }

            LoadOffering(user);
            if (!IsPostBack) LoadRoster();
        }

        protected void CourseSelect_Changed(object sender, EventArgs e)
        {
            LoadOffering(UserContextFactory.FromSession(Session));
            LoadRoster();
        }

        protected void SaveAttendance_Click(object sender, EventArgs e)
        {
            DateTime date;
            TimeSpan startTime;
            TimeSpan endTime;
            if (!TryGetSessionDetails(out date, out startTime, out endTime))
            {
                LoadOffering(UserContextFactory.FromSession(Session));
                LoadRoster();
                return;
            }

            var statuses = ParseSubmitted(attendanceData.Value);
            int saved = LecturerPortalService.SaveAttendance(
                UserContextFactory.FromSession(Session),
                SelectedOfferingId,
                date,
                startTime,
                endTime,
                statuses);

            LoadOffering(UserContextFactory.FromSession(Session));
            LoadRoster();
            ShowStatus(
                saved == 1
                    ? "Attendance saved for 1 student."
                    : "Attendance saved for " + saved.ToString(CultureInfo.InvariantCulture) + " students.",
                true);
        }

        private void BindCourses(UserContext user, int requestedOfferingId)
        {
            var allCourses = LecturerPortalService.GetCourses(user);
            var courses = allCourses
                .Where(c => string.Equals(c.Status, "In progress", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (requestedOfferingId > 0)
            {
                var requested = allCourses.FirstOrDefault(c => c.OfferingId == requestedOfferingId);
                if (requested != null && courses.All(c => c.OfferingId != requestedOfferingId))
                    courses.Add(requested);
            }
            if (courses.Count == 0) courses = allCourses;

            courseSelect.DataSource = courses;
            courseSelect.DataTextField = "CourseCode";
            courseSelect.DataValueField = "OfferingId";
            courseSelect.DataBind();
        }

        private void LoadOffering(UserContext user)
        {
            _offering = SelectedOfferingId > 0
                ? LecturerPortalService.GetAttendanceOffering(user, SelectedOfferingId)
                : null;
        }

        private void LoadRoster()
        {
            DateTime date;
            TimeSpan startTime;
            TimeSpan endTime;
            bool valid = TryGetSessionDetails(out date, out startTime, out endTime);

            if (_offering != null && valid)
            {
                _roster = LecturerPortalService.GetAttendanceRoster(
                    UserContextFactory.FromSession(Session),
                    SelectedOfferingId,
                    date,
                    startTime,
                    endTime);
            }

            rosterRepeater.DataSource = _roster;
            rosterRepeater.DataBind();
            attendanceData.Value = BuildInitialJson(_roster);
            emptyPanel.Visible = _roster.Count == 0;
            rosterPanel.Visible = _offering != null;
            saveBtn.Visible = _offering != null && _roster.Count > 0;
            markAllPresent.Visible = _offering != null && _roster.Count > 0;
        }

        private bool TryGetSessionDetails(out DateTime date, out TimeSpan startTime, out TimeSpan endTime)
        {
            bool validDate = DateTime.TryParseExact(
                dateInput.Text,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date);
            bool validStart = TimeSpan.TryParse(startTimeInput.Text, CultureInfo.InvariantCulture, out startTime);
            bool validEnd = TimeSpan.TryParse(endTimeInput.Text, CultureInfo.InvariantCulture, out endTime);

            if (!validDate || !validStart || !validEnd)
            {
                ShowStatus("Please choose a valid date, start time, and end time.", false);
                return false;
            }
            if (endTime <= startTime)
            {
                ShowStatus("End time must be later than start time.", false);
                return false;
            }
            return true;
        }

        private int SelectedOfferingId
        {
            get
            {
                int offeringId;
                return int.TryParse(courseSelect.SelectedValue, out offeringId) ? offeringId : 0;
            }
        }

        protected string CourseCode { get { return _offering != null ? _offering.CourseCode : "Select a course"; } }
        protected string CourseName { get { return _offering != null ? _offering.CourseName : ""; } }
        protected string EnrolledCountDisplay
        {
            get
            {
                int count = _offering != null ? _offering.EnrolledCount : 0;
                return count + (count == 1 ? " student" : " students");
            }
        }

        protected string AccentColor
        {
            get
            {
                var color = _offering != null ? _offering.Color : null;
                return !string.IsNullOrEmpty(color) &&
                    System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$")
                    ? color
                    : "#64748b";
            }
        }

        protected string BackUrl { get { return ResolveUrl("~/lecturer/lecturer_attendance.aspx"); } }

        protected string SegActive(object rowStatus, string target)
        {
            var status = rowStatus == null ? "" : rowStatus.ToString();
            return string.Equals(status, target, StringComparison.OrdinalIgnoreCase) ? "true" : "false";
        }

        private void ShowStatus(string message, bool success)
        {
            statusBanner.Visible = true;
            statusBanner.CssClass = success
                ? "mt-4 flex items-center gap-2 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-emerald-700"
                : "mt-4 flex items-center gap-2 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-red-700";
            statusIcon.Attributes["data-lucide"] = success ? "check-circle-2" : "circle-alert";
            statusMessage.Text = message;
        }

        private static DateTime ParseDate(string value)
        {
            DateTime parsed;
            return !string.IsNullOrEmpty(value) &&
                DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.Date
                : DateTime.Today;
        }

        private static TimeSpan ParseTime(string value, TimeSpan fallback)
        {
            TimeSpan parsed;
            return !string.IsNullOrEmpty(value) &&
                TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out parsed)
                ? parsed
                : fallback;
        }

        private static string BuildInitialJson(List<RosterEntry> roster)
        {
            var map = new Dictionary<string, string>();
            foreach (var entry in roster)
            {
                if (!string.IsNullOrEmpty(entry.Status))
                    map[entry.EnrolmentId.ToString(CultureInfo.InvariantCulture)] = entry.Status;
            }
            return new JavaScriptSerializer().Serialize(map);
        }

        private static IDictionary<int, string> ParseSubmitted(string json)
        {
            var result = new Dictionary<int, string>();
            if (string.IsNullOrWhiteSpace(json)) return result;

            var raw = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json);
            if (raw == null) return result;

            foreach (var pair in raw)
            {
                int enrolmentId;
                string status = (pair.Value ?? "").Trim().ToUpperInvariant();
                if (int.TryParse(pair.Key, out enrolmentId) &&
                    (status == "PRESENT" || status == "LATE" || status == "ABSENT"))
                {
                    result[enrolmentId] = status;
                }
            }
            return result;
        }
    }
}
