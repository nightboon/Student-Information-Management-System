using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class AcademicTermReader
    {
        public static StudentRegistrationTerm GetCurrentTerm()
        {
            const string sql =
                "SELECT TOP 1 session_id, academic_year, semester, start_date, end_date " +
                "FROM ACADEMIC_SESSIONS " +
                "WHERE start_date <= @today AND end_date >= @today " +
                "ORDER BY start_date DESC";
            return GetTerm(sql);
        }

        public static StudentRegistrationTerm GetRegistrationTerm()
        {
            var current = GetCurrentTerm();
            if (current != null) return current;

            const string sql =
                "SELECT TOP 1 session_id, academic_year, semester, start_date, end_date " +
                "FROM ACADEMIC_SESSIONS " +
                "WHERE start_date >= @today " +
                "ORDER BY start_date";
            return GetTerm(sql);
        }

        private static StudentRegistrationTerm GetTerm(string sql)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@today", DateTime.Today);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new StudentRegistrationTerm
                    {
                        SessionId = Text(reader["session_id"]),
                        AcademicYear = Text(reader["academic_year"]),
                        Name = Text(reader["semester"]),
                        StartDate = DateValue(reader["start_date"]) ?? DateTime.Today,
                        EndDate = DateValue(reader["end_date"]) ?? DateTime.Today.AddMonths(4)
                    };
                }
            }
        }

        public static StudentRegistrationWindow BuildRegistrationWindow(StudentRegistrationTerm term)
        {
            if (term == null) return new StudentRegistrationWindow();
            var registrationStart = term.StartDate.AddDays(-14);
            var registrationEnd = term.StartDate.AddDays(21);
            var addDropStart = term.StartDate;
            var addDropEnd = term.StartDate.AddDays(14);
            var today = DateTime.Today;
            return new StudentRegistrationWindow
            {
                RegistrationStart = registrationStart,
                RegistrationEnd = registrationEnd,
                AddDropStart = addDropStart,
                AddDropEnd = addDropEnd,
                IsOpen = today >= registrationStart && today <= addDropEnd,
                ActivePhase = today <= registrationEnd ? 1 : today <= addDropEnd ? 2 : 3
            };
        }

        public static Dictionary<string, DateTime> GetSessionLookup()
        {
            var lookup = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
            const string sql = "SELECT academic_year, semester, start_date FROM ACADEMIC_SESSIONS";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lookup[Text(reader["academic_year"]) + " " + Text(reader["semester"])] = DateValue(reader["start_date"]) ?? DateTime.Today;
                }
            }
            return lookup;
        }
    }
}
