using System;

namespace src.services
{
    /// <summary>Display/formatting helpers for the student portal view models.</summary>
    public static class StudentPortalFormat
    {
        public static bool IsStudent(UserContext user)
        {
            return user != null && user.IsStudent;
        }

        public static bool IsPublishedGrade(string grade)
        {
            return !string.IsNullOrWhiteSpace(grade) && !string.Equals(grade, "N/A", StringComparison.OrdinalIgnoreCase);
        }

        public static string TermLabel(StudentRegistrationTerm term)
        {
            return term == null ? "" : term.AcademicYear + " " + term.Name;
        }

        public static bool IsSameTerm(string left, string right)
        {
            return !string.IsNullOrWhiteSpace(left)
                && !string.IsNullOrWhiteSpace(right)
                && string.Equals(left.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public static string LecturerOrFallback(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "Lecturer not assigned" : value;
        }

        public static string FileTypeFromUrl(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl)) return "link";
            var dot = fileUrl.LastIndexOf('.');
            if (dot < 0 || dot == fileUrl.Length - 1) return "link";
            return fileUrl.Substring(dot + 1).ToLowerInvariant();
        }

        public static string ColorFor(string code)
        {
            string[] palette = { "#e0162b", "#0ea5e9", "#10b981", "#f59e0b", "#8b5cf6", "#64748b" };
            if (string.IsNullOrWhiteSpace(code)) return palette[0];
            var sum = 0;
            foreach (var ch in code) sum += ch;
            return palette[Math.Abs(sum) % palette.Length];
        }

        public static string ColorOrFallback(string colour, string code)
        {
            return string.IsNullOrWhiteSpace(colour) ? ColorFor(code) : colour.Trim();
        }

        public static string ShortDay(string day)
        {
            if (string.IsNullOrWhiteSpace(day)) return "";
            return day.Length <= 3 ? day : day.Substring(0, 3);
        }

        public static string FormatTime(TimeSpan? time)
        {
            return time.HasValue ? DateTime.Today.Add(time.Value).ToString("HH:mm") : "";
        }

        public static int GradeOrder(string grade)
        {
            switch ((grade ?? "").ToUpperInvariant())
            {
                case "A+": return 1;
                case "A": return 2;
                case "A-": return 3;
                case "B+": return 4;
                case "B": return 5;
                case "B-": return 6;
                case "C+": return 7;
                case "C": return 8;
                case "C-": return 9;
                case "D": return 10;
                case "F": return 11;
                default: return 99;
            }
        }
    }
}
