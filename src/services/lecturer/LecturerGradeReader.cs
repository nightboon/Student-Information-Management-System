using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using src.db;
using static src.services.ServiceMap;

namespace src.services
{
    public static class LecturerGradeReader
    {
        public static List<LecturerAssessmentOption> GetAssessments(UserContext user, int offeringId)
        {
            var options = new List<LecturerAssessmentOption>();
            if (user == null) return options;

            const string sql =
                "SELECT assignment_id, title FROM ASSIGNMENTS WHERE offer_id = @offerId ORDER BY due_date, assignment_id";

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageOffer(conn, user, offeringId)) return options;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@offerId", offeringId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            options.Add(new LecturerAssessmentOption
                            {
                                AssessmentId = IntValue(reader["assignment_id"]),
                                OfferingId = offeringId,
                                Label = Text(reader["title"])
                            });
                        }
                    }
                }
            }
            return options;
        }

        public static List<LecturerGradeRow> GetGradeRows(UserContext user, int assessmentId)
        {
            var rows = new List<LecturerGradeRow>();
            if (user == null) return rows;

            // Every enrolled student in the assignment's offering, with their submission (if any)
            // and their current published course letter grade (if any).
            const string sql =
                "SELECT s.student_id, s.student_name, s.student_email, " +
                "sub.submission_id, sub.marks_obtained, sub.submitted_at, sub.file_url, ISNULL(sub.status, '') AS sub_status, " +
                "ISNULL(g.letter_grade, '') AS letter_grade " +
                "FROM ASSIGNMENTS a " +
                "JOIN ENROLLMENTS e ON e.offer_id = a.offer_id AND e.status = 'ENROLLED' " +
                "JOIN STUDENTS s ON s.student_id = e.student_id " +
                "LEFT JOIN SUBMISSIONS sub ON sub.assignment_id = a.assignment_id AND sub.student_id = s.student_id " +
                "LEFT JOIN GRADES g ON g.offer_id = a.offer_id AND g.student_id = s.student_id " +
                "WHERE a.assignment_id = @assignmentId " +
                "ORDER BY s.student_name";

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageAssignment(conn, user, assessmentId)) return rows;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@assignmentId", assessmentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var marks = DecimalValue(reader["marks_obtained"]);
                            var letter = Text(reader["letter_grade"]);
                            var submissionId = IntValue(reader["submission_id"]);
                            var subStatus = Text(reader["sub_status"]);
                            rows.Add(new LecturerGradeRow
                            {
                                SubmissionId = submissionId,
                                StudentId = Text(reader["student_id"]),
                                FullName = Text(reader["student_name"]),
                                StudentEmail = Text(reader["student_email"]),
                                Marks = marks,
                                HasMarks = marks.HasValue,
                                SubmittedAt = DateValue(reader["submitted_at"]),
                                FileUrl = Text(reader["file_url"]),
                                Grade = string.IsNullOrWhiteSpace(letter) ? "N/A" : letter,
                                SubmissionStatus = !string.IsNullOrWhiteSpace(subStatus)
                                    ? subStatus
                                    : (marks.HasValue ? "GRADED" : (submissionId > 0 ? "SUBMITTED" : "PENDING"))
                            });
                        }
                    }
                }
            }
            return rows;
        }

        public static void SaveGradeMarks(UserContext user, int assessmentId, IDictionary<int, decimal?> marks)
        {
            if (user == null || marks == null || marks.Count == 0) return;

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageAssignment(conn, user, assessmentId)) return;
                foreach (var pair in marks)
                {
                    if (pair.Key <= 0) continue;
                    const string sql =
                        "UPDATE SUBMISSIONS SET marks_obtained = @marks, " +
                        "status = CASE WHEN @marks IS NULL THEN status ELSE 'GRADED' END " +
                        "WHERE submission_id = @submissionId";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@marks", ServiceAccess.DbValue(pair.Value));
                        cmd.Parameters.AddWithValue("@submissionId", pair.Key);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // Computes each enrolled student's average submission percentage across the
        // offering, maps to a letter + grade point, and UPSERTs into GRADES.
        public static void PublishGrades(UserContext user, int assessmentId)
        {
            if (user == null) return;

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageAssignment(conn, user, assessmentId)) return;

                int offerId;
                string semester;
                using (var cmd = new SqlCommand(
                    "SELECT co.offer_id, co.academic_year + ' ' + co.semester AS sem " +
                    "FROM ASSIGNMENTS a JOIN COURSE_OFFERINGS co ON co.offer_id = a.offer_id " +
                    "WHERE a.assignment_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", assessmentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return;
                        offerId = IntValue(reader["offer_id"]);
                        semester = Text(reader["sem"]);
                    }
                }

                var percentByStudent = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
                var countByStudent = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = new SqlCommand(
                    "SELECT s.student_id, a.total_marks, sub.marks_obtained " +
                    "FROM ENROLLMENTS e " +
                    "JOIN STUDENTS s ON s.student_id = e.student_id " +
                    "JOIN ASSIGNMENTS a ON a.offer_id = e.offer_id " +
                    "LEFT JOIN SUBMISSIONS sub ON sub.assignment_id = a.assignment_id AND sub.student_id = s.student_id " +
                    "WHERE e.offer_id = @offerId AND e.status = 'ENROLLED'", conn))
                {
                    cmd.Parameters.AddWithValue("@offerId", offerId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sid = Text(reader["student_id"]);
                            var m = DecimalValue(reader["marks_obtained"]);
                            if (!m.HasValue) continue;
                            var max = IntValue(reader["total_marks"]);
                            if (max <= 0) max = 100;
                            if (!percentByStudent.ContainsKey(sid)) { percentByStudent[sid] = 0m; countByStudent[sid] = 0; }
                            percentByStudent[sid] += Math.Round(m.Value / max * 100m, 2);
                            countByStudent[sid] += 1;
                        }
                    }
                }

                foreach (var sid in percentByStudent.Keys)
                {
                    if (countByStudent[sid] == 0) continue;
                    var avg = percentByStudent[sid] / countByStudent[sid];
                    var letter = LetterFor(avg);
                    var point = PointFor(letter);
                    UpsertGrade(conn, offerId, sid, point, letter, semester);
                }
            }
        }

        private static void UpsertGrade(SqlConnection conn, int offerId, string studentId, decimal point, string letter, string semester)
        {
            const string sql =
                "UPDATE GRADES SET grade_point = @point, letter_grade = @letter, semester = @sem " +
                "WHERE offer_id = @offerId AND student_id = @studentId; " +
                "IF @@ROWCOUNT = 0 " +
                "INSERT INTO GRADES (student_id, offer_id, grade_point, letter_grade, semester) " +
                "VALUES (@studentId, @offerId, @point, @letter, @sem);";
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@point", point);
                cmd.Parameters.AddWithValue("@letter", letter);
                cmd.Parameters.AddWithValue("@sem", ServiceAccess.DbValue(semester));
                cmd.Parameters.AddWithValue("@offerId", offerId);
                cmd.Parameters.AddWithValue("@studentId", studentId);
                cmd.ExecuteNonQuery();
            }
        }

        private static string LetterFor(decimal pct)
        {
            if (pct >= 90m) return "A+";
            if (pct >= 80m) return "A";
            if (pct >= 75m) return "A-";
            if (pct >= 70m) return "B+";
            if (pct >= 65m) return "B";
            if (pct >= 60m) return "B-";
            if (pct >= 55m) return "C+";
            if (pct >= 50m) return "C";
            if (pct >= 45m) return "C-";
            if (pct >= 40m) return "D";
            return "F";
        }

        private static decimal PointFor(string letter)
        {
            switch (letter)
            {
                case "A+": return 4.00m;
                case "A": return 4.00m;
                case "A-": return 3.70m;
                case "B+": return 3.30m;
                case "B": return 3.00m;
                case "B-": return 2.70m;
                case "C+": return 2.30m;
                case "C": return 2.00m;
                case "C-": return 1.70m;
                case "D": return 1.00m;
                default: return 0.00m;
            }
        }
    }
}
