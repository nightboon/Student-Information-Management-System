using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using src.services;

namespace student_information_management_system
{
    public partial class lecturer_at_risk : src.security.LecturerPage
    {
        private Lecturer _lecturer;
        private List<AtRiskStudentRow> _rows = new List<AtRiskStudentRow>();

        protected void Page_Load(object sender, EventArgs e)
        {
            _lecturer = Session["user_id"] != null ? LecturerService.GetByUserId((int)Session["user_id"]) : null;
            if (_lecturer == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }
            _rows = LecturerPortalService.GetAtRiskStudents(_lecturer.LecturerId);
            riskRepeater.DataSource = _rows;
            riskRepeater.DataBind();
            emptyPanel.Visible = _rows.Count == 0;
        }

        protected int FlaggedCount { get { return _rows.Count; } }
        protected int AttendanceRiskCount { get { return _rows.FindAll(r => r.AttendanceRisk).Count; } }
        protected int AcademicRiskCount { get { return _rows.FindAll(r => r.AcademicRisk).Count; } }
        protected int HighRiskCount { get { return _rows.FindAll(r => r.RiskLevel == "High").Count; } }

        protected string Html(object value)
        {
            return HttpUtility.HtmlEncode(value == null ? "" : value.ToString());
        }

        protected string Percent(object value)
        {
            if (value == null || value == DBNull.Value) return "-";
            return Convert.ToDecimal(value).ToString("0.#", CultureInfo.InvariantCulture) + "%";
        }

        protected string RiskBadgeClass(object value)
        {
            decimal number = value == null || value == DBNull.Value ? 100m : Convert.ToDecimal(value);
            if (number < 50m) return "bg-[#e0162b]/10 text-[#a01020]";
            if (number < 80m) return "bg-amber-50 text-amber-700";
            return "bg-emerald-50 text-emerald-700";
        }

        protected string LevelClass(object level)
        {
            return level != null && level.ToString() == "High"
                ? "bg-[#e0162b] text-white"
                : "border border-slate-200 bg-white text-slate-700";
        }
    }
}
