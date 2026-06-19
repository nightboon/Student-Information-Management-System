using System;

namespace src.services
{
    public class UserContext
    {
        public int UserId { get; set; }
        public string Role { get; set; }

        public bool IsAdmin
        {
            get { return IsRole("ADMIN"); }
        }

        public bool IsLecturer
        {
            get { return IsRole("LECTURER"); }
        }

        public bool IsStudent
        {
            get { return IsRole("STUDENT"); }
        }

        public bool CanWrite
        {
            get { return IsAdmin; }
        }

        public string NormalizedRole
        {
            get { return (Role ?? "").Trim().ToUpperInvariant(); }
        }

        private bool IsRole(string role)
        {
            return string.Equals(NormalizedRole, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}
