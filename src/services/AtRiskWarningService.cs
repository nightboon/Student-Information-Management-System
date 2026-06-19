using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using src.services.email;
using static src.services.ServiceMap;

namespace src.services
{
    /// <summary>Outcome of one at-risk warning batch run, surfaced to the admin UI.</summary>
    public class AtRiskRunResult
    {
        public bool Ran { get; set; }
        public string Sem1 { get; set; }
        public string Sem2 { get; set; }
        public int Checked { get; set; }
        public int AtRisk { get; set; }
        public int Emailed { get; set; }
        public int Skipped { get; set; }
        public int Failed { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Manual batch (triggered by an admin button): flags students whose
    /// credit-weighted GPA is below 2.0 for the two most recent ENDED semesters,
    /// emails each one a formal academic warning, and records the warning in
    /// ACADEMIC_WARNINGS so the same period is never emailed twice.
    /// </summary>
    public static class AtRiskWarningService
    {
        private const decimal Threshold = 2.0m;
        private const string WarningType = "Academic Warning";
        private const string WarningTitle = "Academic Warning — Cumulative GPA Below 2.0";

        public static AtRiskRunResult Run()
        {
            var result = new AtRiskRunResult();

            using (var conn = Db.OpenConnection())
            {
                var ended = GetTwoMostRecentEndedSemesters(conn);
                if (ended.Count < 2)
                    return result; // need two ended semesters to judge "two consecutive"

                result.Ran = true;
                result.Sem1 = ended[1]; // older of the two
                result.Sem2 = ended[0]; // most recent ended

                var students = LoadAtRiskCandidates(conn, result.Sem1, result.Sem2);
                result.Checked = students.Count;

                foreach (var s in students)
                {
                    decimal gpa1, gpa2;
                    if (!s.Gpas.TryGetValue(result.Sem1, out gpa1)) continue;
                    if (!s.Gpas.TryGetValue(result.Sem2, out gpa2)) continue;
                    if (gpa1 >= Threshold || gpa2 >= Threshold) continue;

                    result.AtRisk++;

                    try
                    {
                        if (AlreadyWarned(conn, s.StudentId, result.Sem2))
                        {
                            result.Skipped++;
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(s.Email))
                        {
                            result.Failed++;
                            result.Errors.Add(s.StudentId + ": no email on file");
                            continue;
                        }

                        var detail =
                            "Dear " + s.Name + ",\n\n" +
                            "Our records show your credit-weighted GPA has fallen below 2.0 for two " +
                            "consecutive semesters (" + result.Sem1 + " and " + result.Sem2 + "). " +
                            "You are now on academic warning.\n\n" +
                            "Please contact your academic advisor as soon as possible to arrange a " +
                            "recovery plan. If you have already done so, you may disregard this message.\n\n" +
                            "INTI Academic Office";

                        var sent = EmailService.SendNotification(s.Email, WarningTitle, detail);
                        if (!sent.Success)
                        {
                            result.Failed++;
                            result.Errors.Add(s.StudentId + ": " + sent.Error);
                            continue;
                        }

                        InsertWarning(conn, s.StudentId, result.Sem1, result.Sem2);
                        result.Emailed++;
                    }
                    catch (Exception ex)
                    {
                        result.Failed++;
                        result.Errors.Add(s.StudentId + ": " + ex.Message);
                    }
                }
            }

            return result;
        }

        /// <summary>Labels (academic_year + ' ' + semester) of the two most recently
        /// ended sessions, most recent first.</summary>
        private static List<string> GetTwoMostRecentEndedSemesters(SqlConnection conn)
        {
            const string sql =
                "SELECT TOP 2 academic_year + ' ' + semester AS label " +
                "FROM ACADEMIC_SESSIONS " +
                "WHERE end_date IS NOT NULL AND end_date < @today " +
                "ORDER BY end_date DESC";

            var labels = new List<string>();
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@today", DateTime.Today);
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        labels.Add(Text(reader["label"]));
            }
            return labels;
        }

        private class Candidate
        {
            public string StudentId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public Dictionary<string, decimal> Gpas { get; } =
                new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Per active student, the credit-weighted GPA for each of the two
        /// target semesters (published grades only; N/A excluded).</summary>
        private static List<Candidate> LoadAtRiskCandidates(SqlConnection conn, string sem1, string sem2)
        {
            const string sql =
                "SELECT s.student_id, s.student_name, s.student_email, g.semester, " +
                "       SUM(g.grade_point * c.credit_hour) AS wpoints, " +
                "       SUM(c.credit_hour)                 AS wcredits " +
                "FROM STUDENTS s " +
                "JOIN GRADES g            ON g.student_id = s.student_id " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id  = g.offer_id " +
                "JOIN COURSES c           ON c.course_id  = co.course_id " +
                "WHERE s.status = 'ACTIVE' " +
                "  AND g.semester IN (@sem1, @sem2) " +
                "  AND g.letter_grade IS NOT NULL AND UPPER(g.letter_grade) <> 'N/A' " +
                "GROUP BY s.student_id, s.student_name, s.student_email, g.semester";

            var byId = new Dictionary<string, Candidate>(StringComparer.OrdinalIgnoreCase);
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@sem1", sem1);
                cmd.Parameters.AddWithValue("@sem2", sem2);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = Text(reader["student_id"]);
                        Candidate c;
                        if (!byId.TryGetValue(id, out c))
                        {
                            c = new Candidate
                            {
                                StudentId = id,
                                Name = Text(reader["student_name"]),
                                Email = Text(reader["student_email"])
                            };
                            byId[id] = c;
                        }

                        var credits = DecimalValue(reader["wcredits"]) ?? 0m;
                        if (credits <= 0m) continue;
                        var points = DecimalValue(reader["wpoints"]) ?? 0m;
                        c.Gpas[Text(reader["semester"])] = Math.Round(points / credits, 2);
                    }
                }
            }

            // Only students with a GPA in BOTH target semesters can qualify.
            return byId.Values
                .Where(c => c.Gpas.ContainsKey(sem1) && c.Gpas.ContainsKey(sem2))
                .ToList();
        }

        /// <summary>True if this student already has an Academic Warning covering the
        /// most recent ended semester (dedup key = that semester label).</summary>
        private static bool AlreadyWarned(SqlConnection conn, string studentId, string sem2)
        {
            const string sql =
                "SELECT COUNT(*) FROM ACADEMIC_WARNINGS " +
                "WHERE student_id = @sid AND issue_type = @type " +
                "  AND description LIKE @pattern";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@sid", studentId);
                cmd.Parameters.AddWithValue("@type", WarningType);
                cmd.Parameters.Add("@pattern", SqlDbType.VarChar).Value = "%" + sem2 + "%";
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static void InsertWarning(SqlConnection conn, string studentId, string sem1, string sem2)
        {
            const string sql =
                "INSERT INTO ACADEMIC_WARNINGS (student_id, issue_type, description, issued_date, status) " +
                "VALUES (@sid, @type, @desc, @date, @status)";

            var desc = "Cumulative GPA below 2.0 for two consecutive semesters: " + sem1 + " and " + sem2 + ".";
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@sid", studentId);
                cmd.Parameters.AddWithValue("@type", WarningType);
                cmd.Parameters.Add("@desc", SqlDbType.VarChar).Value = desc;
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.Parameters.AddWithValue("@status", "OPEN");
                cmd.ExecuteNonQuery();
            }
        }
    }
}
