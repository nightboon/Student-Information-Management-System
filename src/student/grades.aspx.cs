using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using src.services;

namespace src.student
{
    public partial class grade : src.security.StudentPage
    {
        protected string FilterLabel { get; private set; }
        protected string CgpaDisplay { get; private set; }
        protected string CgpaSubtext { get; private set; }
        protected string StandingDisplay { get; private set; }
        protected string CreditsEarnedDisplay { get; private set; }
        protected string CreditsAttemptedDisplay { get; private set; }
        protected string CurrentGpaDisplay { get; private set; }
        protected string CurrentGpaContext { get; private set; }
        protected string BestSemesterDisplay { get; private set; }
        protected string BestSemesterSubtext { get; private set; }
        protected string CoursesGradedDisplay { get; private set; }

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

            BindGradePage((int)Session["user_id"]);
        }

        protected void semesterRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var semester = e.Item.DataItem as StudentGradeSemester;
            var courses = e.Item.FindControl("coursesRepeater") as Repeater;
            if (semester != null && courses != null)
            {
                courses.DataSource = semester.Courses;
                courses.DataBind();
            }
        }

        protected string FormatGpa(object value)
        {
            decimal? gpa = ToNullableDecimal(value);
            return gpa.HasValue ? gpa.Value.ToString("0.00") : "Pending";
        }

        protected string SemesterShortLabel(object dataItem)
        {
            var semester = dataItem as StudentGradeSemester;
            if (semester == null) return "";
            return "Y" + YearNo(semester.SemesterNo) + " T" + TrimesterNo(semester.SemesterNo);
        }

        protected string SemesterTitle(object dataItem)
        {
            var semester = dataItem as StudentGradeSemester;
            if (semester == null) return "";

            return "Year " + YearNo(semester.SemesterNo)
                + " - Trimester " + TrimesterNo(semester.SemesterNo)
                + " (" + semester.StartDate.ToString("MMM yyyy") + ")";
        }

        protected string SemesterSubtitle(object dataItem)
        {
            var semester = dataItem as StudentGradeSemester;
            if (semester == null) return "";

            return semester.CourseCount + " " + Pluralize("course", semester.CourseCount)
                + " - " + semester.Credits + " " + Pluralize("credit", semester.Credits);
        }

        protected string GpaBarStyle(object dataItem)
        {
            var semester = dataItem as StudentGradeSemester;
            decimal value = semester != null && semester.Gpa.HasValue ? semester.Gpa.Value : 0m;
            decimal width = Math.Max(0m, Math.Min(100m, value / 4m * 100m));
            return "width:" + width.ToString("0.##") + "%";
        }

        protected string DistributionBarStyle(object dataItem)
        {
            var item = dataItem as StudentGradeDistributionItem;
            decimal width = item == null ? 0m : item.BarWidth;
            return "width:" + width.ToString("0.##") + "%;background-color:" + GradeColor(item == null ? "" : item.Grade);
        }

        protected string GradeBadgeStyle(object gradeValue)
        {
            string grade = gradeValue == null ? "" : gradeValue.ToString();
            string color = GradeColor(grade);
            return "font-size:12px;font-weight:700;background-color:" + color + "15;border-color:" + color + "40;color:" + color;
        }

        protected string CourseColorStyle(object color)
        {
            return "background-color:" + SafeColor(color);
        }

        protected string LecturerDisplay(object lecturer)
        {
            string name = lecturer == null ? "" : lecturer.ToString();
            return string.IsNullOrWhiteSpace(name) ? "Lecturer not assigned" : name;
        }

        protected string CurrentScoreDisplay(object dataItem)
        {
            var course = dataItem as StudentGradeCourse;
            if (course == null || !course.CurrentAverage.HasValue) return "Pending";
            return course.CurrentAverage.Value.ToString("0.#") + "%";
        }

        protected string CurrentScoreSubtext(object dataItem)
        {
            var course = dataItem as StudentGradeCourse;
            if (course == null) return "";
            if (course.CompletedPercent > 0)
            {
                return course.CompletedPercent.ToString("0.#") + "% graded";
            }
            if (course.GradePublished)
            {
                return "Final grade published";
            }
            return "No marks yet";
        }

        protected string FinalExamDisplay(object dataItem)
        {
            var course = dataItem as StudentGradeCourse;
            if (course == null || !course.HasFinalAssessment) return "N/A";
            if (!course.FinalExamMarks.HasValue) return "Pending";
            return course.FinalExamMarks.Value.ToString("0.#") + " /100";
        }

        protected string FinalExamCss(object dataItem)
        {
            var course = dataItem as StudentGradeCourse;
            if (course != null && course.HasFinalAssessment && course.FinalExamMarks.HasValue)
            {
                return "text-slate-900";
            }

            return "rounded bg-slate-50 border border-slate-200 text-slate-500 px-1.5 py-0.5";
        }

        protected string LetterGradeDisplay(object dataItem)
        {
            var course = dataItem as StudentGradeCourse;
            if (course == null || !course.GradePublished || string.IsNullOrWhiteSpace(course.LetterGrade))
            {
                return "Pending";
            }
            return course.LetterGrade;
        }

        protected string GradePointDisplay(object dataItem)
        {
            var course = dataItem as StudentGradeCourse;
            if (course == null || !course.GradePublished || !course.Gpa.HasValue)
            {
                return "-";
            }
            return course.Gpa.Value.ToString("0.00");
        }

        private void BindGradePage(int userId)
        {
            var user = UserContextFactory.FromSession(Session);
            var grades = StudentPortalService.GetGradePage(user);
            if (grades == null)
            {
                Response.Redirect("~/login/login.aspx");
                return;
            }

            var semestersWithGpa = grades.Semesters
                .Where(s => s.Gpa.HasValue)
                .OrderBy(s => s.StartDate)
                .ToList();

            FilterLabel = grades.Semesters.Count == 0 ? "All semesters" : grades.Semesters.Count + " " + Pluralize("semester", grades.Semesters.Count);
            CgpaDisplay = grades.Cgpa.HasValue ? grades.Cgpa.Value.ToString("0.00") : "N/A";
            CgpaSubtext = grades.Cgpa.HasValue
                ? "out of 4.00 - across " + grades.Semesters.Count + " " + Pluralize("semester", grades.Semesters.Count)
                : "No published grades yet";
            StandingDisplay = StandingFromCgpa(grades.Cgpa);
            CreditsEarnedDisplay = grades.CreditsEarned.ToString();
            CreditsAttemptedDisplay = grades.CreditsAttempted.ToString();
            CurrentGpaDisplay = grades.CurrentGpa.HasValue ? grades.CurrentGpa.Value.ToString("0.00") : "Pending";
            CurrentGpaContext = grades.CurrentSemester == null ? "Current term" : SemesterShortLabel(grades.CurrentSemester);
            BestSemesterDisplay = grades.BestSemester == null ? "No data" : SemesterShortLabel(grades.BestSemester);
            BestSemesterSubtext = grades.BestSemester == null ? "No published GPA" : "GPA " + FormatGpa(grades.BestSemester.Gpa);
            CoursesGradedDisplay = grades.CoursesGraded.ToString();

            gpaRepeater.DataSource = semestersWithGpa;
            gpaRepeater.DataBind();
            emptyGpaPanel.Visible = semestersWithGpa.Count == 0;

            distributionRepeater.DataSource = grades.GradeDistribution;
            distributionRepeater.DataBind();
            emptyDistributionPanel.Visible = grades.GradeDistribution.Count == 0;

            semesterRepeater.DataSource = grades.Semesters;
            semesterRepeater.DataBind();
            emptyGradesPanel.Visible = grades.Semesters.Count == 0;
        }

        private static string StandingFromCgpa(decimal? cgpa)
        {
            if (!cgpa.HasValue) return "Not graded";
            if (cgpa.Value >= 3.67m) return "First Class";
            if (cgpa.Value >= 3.00m) return "Second Upper";
            if (cgpa.Value >= 2.00m) return "Good Standing";
            return "At Risk";
        }

        private static string GradeColor(string grade)
        {
            switch ((grade ?? "").ToUpperInvariant())
            {
                case "A+":
                case "A":
                case "A-":
                    return "#10b981";
                case "B+":
                case "B":
                case "B-":
                    return "#0ea5e9";
                case "C+":
                case "C":
                case "C-":
                    return "#f59e0b";
                case "D":
                    return "#f97316";
                case "F":
                    return "#ef4444";
                default:
                    return "#64748b";
            }
        }

        private static string SafeColor(object color)
        {
            string value = color == null ? "" : color.ToString();
            if (value.Length == 7 && value[0] == '#' && value.Skip(1).All(Uri.IsHexDigit))
            {
                return value;
            }
            return "#e0162b";
        }

        private static decimal? ToNullableDecimal(object value)
        {
            if (value == null || value == DBNull.Value) return null;
            if (value is decimal) return (decimal)value;
            return Convert.ToDecimal(value);
        }

        private static int YearNo(int semesterNo)
        {
            return Math.Max(1, ((semesterNo - 1) / 3) + 1);
        }

        private static int TrimesterNo(int semesterNo)
        {
            return Math.Max(1, ((semesterNo - 1) % 3) + 1);
        }

        private static string Pluralize(string word, int count)
        {
            return count == 1 ? word : word + "s";
        }
    }
}
