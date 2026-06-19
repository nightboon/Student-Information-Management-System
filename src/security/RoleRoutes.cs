using System;

namespace src.security
{
    /// <summary>
    /// Single source of truth mapping a user role to its landing page.
    /// Returns app-relative ("~/...") URLs. Comparison is case-insensitive.
    /// </summary>
    public static class RoleRoutes
    {
        public const string Student = "STUDENT";
        public const string Lecturer = "LECTURER";
        public const string Admin = "ADMIN";

        public static string HomePageFor(string role)
        {
            if (string.Equals(role, Admin, StringComparison.OrdinalIgnoreCase))
                return "~/admin/admin_dashboard.aspx";
            if (string.Equals(role, Lecturer, StringComparison.OrdinalIgnoreCase))
                return "~/lecturer/lecturer_dashboard.aspx";
            if (string.Equals(role, Student, StringComparison.OrdinalIgnoreCase))
                return "~/student/student_dashboard.aspx";

            // Unknown/empty role -> fail safe to login.
            return "~/login/login.aspx";
        }
    }
}
