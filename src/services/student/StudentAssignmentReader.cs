using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class StudentAssignmentReader
    {
        public static List<StudentCourseAssignment> GetAssignments(UserContext user, string studentId, int? offeringId)
        {
            if (string.IsNullOrWhiteSpace(studentId)) return new List<StudentCourseAssignment>();
            EnsureReviewColumns();

            var sql =
                "SELECT mat.material_id, a.assignment_id, a.offer_id, a.title, CAST(a.description AS varchar(max)) AS description, a.total_marks, a.due_date, " +
                "ISNULL(a.submission_mode, 'FILE') AS submission_mode, sub.extension_deadline, sub.extension_granted_at, " +
                "mat.material_type, mat.weight, mat.file_url AS material_file_url, " +
                "c.course_code, sub.submission_id, sub.file_url, " +
                "CASE WHEN ISNULL(a.submission_mode, 'FILE') IN ('FILE','LINK') AND (sub.status = 'MISSING' OR " +
                "(sub.status = 'EXTENDED' AND sub.extension_deadline < GETDATE()) OR " +
                "(sub.submission_id IS NULL AND a.due_date < GETDATE())) " +
                "THEN CAST(0 AS decimal(5,2)) ELSE sub.published_marks_obtained END AS student_marks, " +
                "sub.status AS submission_status, " +
                "ISNULL(sub.published_feedback, '') AS feedback, " +
                "ISNULL(sub.published_annotated_file_url, '') AS annotated_file_url " +
                "FROM ASSIGNMENTS a " +
                "JOIN MATERIALS mat ON mat.assignment_id = a.assignment_id " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = a.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "JOIN ENROLLMENTS e ON e.offer_id = co.offer_id AND e.student_id = @studentId AND e.status IN ('ENROLLED', 'PENDING') " +
                "LEFT JOIN SUBMISSIONS sub ON sub.assignment_id = a.assignment_id AND sub.student_id = @studentId " +
                "WHERE 1 = 1 ";

            if (offeringId.HasValue) sql += "AND a.offer_id = @offerId ";
            sql += "ORDER BY a.due_date, a.assignment_id";

            var assignments = new List<StudentCourseAssignment>();
            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanViewStudent(conn, user, studentId)) return assignments;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    if (offeringId.HasValue) cmd.Parameters.AddWithValue("@offerId", offeringId.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rawStatus = Text(reader["submission_status"]).ToUpperInvariant();
                            var hasSubmission = reader["submission_id"] != DBNull.Value &&
                                rawStatus != "MISSING" && !string.IsNullOrWhiteSpace(Text(reader["file_url"]));
                            var dueDate = DateValue(reader["due_date"]) ?? DateTime.Today;
                            var mode = Text(reader["submission_mode"]).ToUpperInvariant();
                            bool requiresSubmission = mode != "MANUAL";
                            var extensionDeadline = DateValue(reader["extension_deadline"]);
                            bool extensionOpen = extensionDeadline.HasValue && extensionDeadline.Value >= DateTime.Now;
                            var marks = DecimalValue(reader["student_marks"]);
                            assignments.Add(new StudentCourseAssignment
                            {
                                MaterialId = IntValue(reader["material_id"]),
                                AssignmentId = IntValue(reader["assignment_id"]),
                                OfferId = IntValue(reader["offer_id"]),
                                Title = Text(reader["title"]),
                                Description = Text(reader["description"]),
                                DueDate = extensionOpen ? extensionDeadline.Value : dueDate,
                                Weight = DecimalValue(reader["weight"]),
                                AssignmentType = Text(reader["material_type"]),
                                SubmissionMode = mode,
                                RequiresSubmission = requiresSubmission,
                                CanSubmit = requiresSubmission && (DateTime.Now <= dueDate || extensionOpen),
                                ExtensionDeadline = extensionDeadline,
                                GroupSize = "",
                                SubmissionStatus = marks.HasValue ? "MARKED" :
                                    extensionOpen && !hasSubmission ? "EXTENDED" :
                                    hasSubmission ? (rawStatus == "LATE" ? "LATE" : "SUBMITTED") :
                                    requiresSubmission ? "PENDING" : "NO UPLOAD REQUIRED",
                                HasSubmission = hasSubmission && requiresSubmission,
                                SubmissionFileUrl = Text(reader["file_url"]),
                                MaterialFileUrl = Text(reader["material_file_url"]),
                                Feedback = Text(reader["feedback"]),
                                AnnotatedFileUrl = Text(reader["annotated_file_url"]),
                                Marks = marks,
                                CourseCode = Text(reader["course_code"])
                            });
                        }
                    }
                }
            }

            if (assignments.Count > 0)
            {
                var missingWeight = assignments.Where(a => !a.Weight.HasValue).ToList();
                if (missingWeight.Count > 0)
                {
                    var assignedWeight = assignments.Where(a => a.Weight.HasValue).Sum(a => a.Weight.Value);
                    var fallbackWeight = Math.Round(Math.Max(0m, 100m - assignedWeight) / missingWeight.Count, 2);
                    foreach (var assignment in missingWeight) assignment.Weight = fallbackWeight;
                }
            }
            return assignments;
        }

        private static void EnsureReviewColumns()
        {
            const string sql =
                "IF COL_LENGTH('SUBMISSIONS', 'feedback') IS NULL " +
                "ALTER TABLE SUBMISSIONS ADD feedback varchar(2000) NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'annotated_file_url') IS NULL " +
                "ALTER TABLE SUBMISSIONS ADD annotated_file_url varchar(255) NULL; " +
                "IF COL_LENGTH('MATERIALS', 'assignment_id') IS NULL " +
                "ALTER TABLE MATERIALS ADD assignment_id int NULL;";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
                cmd.ExecuteNonQuery();
            LecturerGradeReader.EnsureReviewColumns();
            LecturerMaterialReader.EnsureAssessmentAssignments();
        }

        public static StudentGradebook GetGradebook(UserContext user, string studentId, int offeringId)
        {
            var assignments = GetAssignments(user, studentId, offeringId);
            var rows = new List<StudentAssessmentRow>();
            if (assignments.Count == 0)
            {
                return new StudentGradebook
                {
                    Items = rows,
                    OverallAverage = null,
                    EarnedWeighted = 0m,
                    CompletedPercent = 0m
                };
            }

            foreach (var assignment in assignments)
            {
                var weight = assignment.Weight.GetValueOrDefault();
                var maxMarks = 100;
                var isGraded = assignment.Marks.HasValue;
                var contribution = isGraded ? Math.Round((assignment.Marks.Value / maxMarks) * weight, 2) : (decimal?)null;
                rows.Add(new StudentAssessmentRow
                {
                    Name = assignment.Title,
                    Type = assignment.AssignmentType,
                    Weight = weight,
                    Marks = assignment.Marks,
                    MaxMarks = maxMarks,
                    IsGraded = isGraded,
                    Contribution = contribution
                });
            }

            var completed = rows.Where(r => r.IsGraded).Sum(r => r.Weight);
            var earned = rows.Where(r => r.Contribution.HasValue).Sum(r => r.Contribution.Value);
            return new StudentGradebook
            {
                Items = rows,
                CompletedPercent = Math.Round(completed, 1),
                EarnedWeighted = Math.Round(earned, 1),
                OverallAverage = completed > 0 ? Math.Round(earned / completed * 100m, 1) : (decimal?)null
            };
        }

        public static bool SaveSubmission(UserContext user, int assignmentId, string fileUrl)
        {
            if (!IsStudent(user) || string.IsNullOrWhiteSpace(fileUrl)) return false;

            var account = StudentProfileReader.GetAccount(user);
            if (account == null) return false;

            EnsureReviewColumns();
            SubmissionNotificationService.EnsureTables();
            using (var conn = Db.OpenConnection())
            {
                const string lookup =
                    "SELECT a.due_date, ISNULL(a.submission_mode, 'FILE') AS submission_mode, " +
                    "sub.submission_id, sub.extension_deadline " +
                    "FROM ASSIGNMENTS a JOIN ENROLLMENTS e ON e.offer_id = a.offer_id " +
                    "AND e.student_id = @studentId AND e.status = 'ENROLLED' " +
                    "LEFT JOIN SUBMISSIONS sub ON sub.assignment_id = a.assignment_id AND sub.student_id = @studentId " +
                    "WHERE a.assignment_id = @assignmentId";
                DateTime? dueDate;
                DateTime? extensionDeadline;
                int submissionId;
                string mode;
                using (var cmd = new SqlCommand(lookup, conn))
                {
                    cmd.Parameters.AddWithValue("@studentId", account.StudentId);
                    cmd.Parameters.AddWithValue("@assignmentId", assignmentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return false;
                        dueDate = DateValue(reader["due_date"]);
                        extensionDeadline = DateValue(reader["extension_deadline"]);
                        submissionId = IntValue(reader["submission_id"]);
                        mode = Text(reader["submission_mode"]).ToUpperInvariant();
                    }
                }

                if (mode == "MANUAL") return false;
                Uri submittedUrl = null;
                if (mode == "LINK" && !IsGoogleDriveUrl(fileUrl, out submittedUrl)) return false;
                if (mode == "LINK") fileUrl = submittedUrl.AbsoluteUri;
                if (mode == "FILE" &&
                    !fileUrl.StartsWith("~/uploads/submissions/", StringComparison.OrdinalIgnoreCase))
                    return false;
                var now = DateTime.Now;
                bool onTime = !dueDate.HasValue || now <= dueDate.Value;
                bool extensionOpen = extensionDeadline.HasValue && now <= extensionDeadline.Value;
                if (!onTime && !extensionOpen) return false;
                string status = onTime ? "SUBMITTED" : "LATE";

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int savedSubmissionId = submissionId;
                        string sql = submissionId > 0
                            ? "UPDATE SUBMISSIONS SET submitted_at=@now,file_url=@file,status=@status,marks_obtained=NULL," +
                              "published_marks_obtained=NULL,published_feedback=NULL,published_annotated_file_url=NULL,published_at=NULL " +
                              "WHERE submission_id=@submissionId"
                            : "INSERT INTO SUBMISSIONS (assignment_id,student_id,submitted_at,file_url,marks_obtained,status) " +
                              "OUTPUT INSERTED.submission_id VALUES (@assignmentId,@studentId,@now,@file,NULL,@status)";
                        using (var cmd = new SqlCommand(sql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@assignmentId", assignmentId);
                            cmd.Parameters.AddWithValue("@studentId", account.StudentId);
                            cmd.Parameters.AddWithValue("@submissionId", submissionId);
                            cmd.Parameters.AddWithValue("@now", now);
                            cmd.Parameters.AddWithValue("@file", fileUrl);
                            cmd.Parameters.AddWithValue("@status", status);
                            if (submissionId > 0)
                            {
                                if (cmd.ExecuteNonQuery() != 1) return false;
                            }
                            else
                            {
                                savedSubmissionId = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }

                        SubmissionNotificationService.InsertSubmitted(
                            conn, transaction, savedSubmissionId, now, status);
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static bool IsGoogleDriveUrl(string value, out Uri uri)
        {
            if (!Uri.TryCreate((value ?? "").Trim(), UriKind.Absolute, out uri) ||
                uri.Scheme != Uri.UriSchemeHttps)
                return false;

            return string.Equals(uri.Host.TrimEnd('.'), "drive.google.com", StringComparison.OrdinalIgnoreCase) &&
                uri.AbsolutePath.StartsWith("/file/d/", StringComparison.OrdinalIgnoreCase) &&
                uri.AbsolutePath.Length > "/file/d/".Length;
        }
    }
}
