using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using src.db;
using static src.services.ServiceMap;

namespace src.services
{
    public static class LecturerGradeReader
    {
        public static List<LecturerAssessmentOption> GetAssessments(UserContext user, int offeringId)
        {
            return GetAssessments(user, (int?)offeringId);
        }

        public static List<LecturerAssessmentOption> GetAssessments(UserContext user, int? offeringId)
        {
            var options = new List<LecturerAssessmentOption>();
            if (user == null || !user.IsLecturer) return options;
            LecturerMaterialReader.EnsureAssessmentAssignments();

            string sql =
                "SELECT a.assignment_id, a.offer_id, a.title, c.course_code " +
                "FROM ASSIGNMENTS a " +
                "JOIN MATERIALS mat ON mat.assignment_id = a.assignment_id " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = a.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "WHERE " + ServiceAccess.VisibleOfferScope("co") + " " +
                "AND (@offerId = 0 OR a.offer_id = @offerId) " +
                "AND mat.weight IS NOT NULL AND mat.weight > 0 " +
                "ORDER BY a.due_date, a.assignment_id";

            using (var conn = Db.OpenConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    ServiceAccess.AddUserContextParameters(cmd, user);
                    cmd.Parameters.AddWithValue("@offerId", offeringId.GetValueOrDefault());
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rowOfferingId = IntValue(reader["offer_id"]);
                            string title = Text(reader["title"]);
                            string courseCode = Text(reader["course_code"]);
                            options.Add(new LecturerAssessmentOption
                            {
                                AssessmentId = IntValue(reader["assignment_id"]),
                                OfferingId = rowOfferingId,
                                Label = offeringId.HasValue ? title : courseCode + " - " + title
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
            EnsureReviewColumns();

            // Every enrolled student in the assignment's offering, with their submission (if any)
            // and their current published course letter grade (if any).
            const string sql =
                "SELECT s.student_id, s.student_name, s.student_email, " +
                "a.due_date, ISNULL(a.submission_mode, 'FILE') AS submission_mode, " +
                "sub.submission_id, sub.marks_obtained, sub.submitted_at, sub.file_url, " +
                "ISNULL(sub.feedback, '') AS feedback, ISNULL(sub.annotated_file_url, '') AS annotated_file_url, " +
                "ISNULL(sub.status, '') AS sub_status, sub.extension_deadline, sub.extension_granted_at, " +
                "sub.published_at, sub.published_marks_obtained, ISNULL(sub.published_feedback, '') AS published_feedback, " +
                "ISNULL(sub.published_annotated_file_url, '') AS published_annotated_file_url, " +
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
                EnsureAssessmentRows(conn, assessmentId);
                EnsureMissingSubmissions(conn, assessmentId, null);
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
                            var dueDate = DateValue(reader["due_date"]);
                            var mode = Text(reader["submission_mode"]).ToUpperInvariant();
                            bool requiresSubmission = mode != "MANUAL";
                            bool isMissing = string.Equals(subStatus, "MISSING", StringComparison.OrdinalIgnoreCase);
                            bool isPublished = reader["published_at"] != DBNull.Value &&
                                Nullable.Equals(marks, DecimalValue(reader["published_marks_obtained"])) &&
                                string.Equals(Text(reader["feedback"]), Text(reader["published_feedback"]), StringComparison.Ordinal) &&
                                string.Equals(Text(reader["annotated_file_url"]), Text(reader["published_annotated_file_url"]), StringComparison.Ordinal);
                            var extensionDeadline = DateValue(reader["extension_deadline"]);
                            var extensionGrantedAt = DateValue(reader["extension_granted_at"]);
                            string markStatus = isMissing
                                ? "Missing · Auto 0"
                                : extensionDeadline.HasValue && string.IsNullOrWhiteSpace(Text(reader["file_url"]))
                                    ? "Awaiting late submission"
                                : !marks.HasValue
                                    ? (requiresSubmission ? "Draft" : "Awaiting marks")
                                    : isPublished ? "Published" : "Ready to publish";
                            rows.Add(new LecturerGradeRow
                            {
                                SubmissionId = submissionId,
                                StudentId = Text(reader["student_id"]),
                                FullName = Text(reader["student_name"]),
                                StudentEmail = Text(reader["student_email"]),
                                Marks = isMissing ? 0m : marks,
                                HasMarks = isMissing || marks.HasValue,
                                SubmittedAt = DateValue(reader["submitted_at"]),
                                DueDate = dueDate,
                                FileUrl = Text(reader["file_url"]),
                                Feedback = Text(reader["feedback"]),
                                AnnotatedFileUrl = Text(reader["annotated_file_url"]),
                                Grade = isMissing ? "F" : (string.IsNullOrWhiteSpace(letter) ? "N/A" : letter),
                                IsMissing = isMissing,
                                SubmissionStatus = isMissing
                                    ? "MISSING"
                                    : extensionDeadline.HasValue && string.IsNullOrWhiteSpace(Text(reader["file_url"]))
                                    ? "EXTENDED UNTIL " + extensionDeadline.Value.ToString("d MMM yyyy, HH:mm")
                                    : !requiresSubmission && string.IsNullOrWhiteSpace(Text(reader["file_url"]))
                                    ? "NO UPLOAD REQUIRED"
                                    : !string.IsNullOrWhiteSpace(subStatus)
                                    ? subStatus
                                    : (marks.HasValue ? "GRADED" : (submissionId > 0 ? "SUBMITTED" : "PENDING"))
                                ,
                                MarkStatus = markStatus,
                                SubmissionMode = mode,
                                RequiresSubmission = requiresSubmission,
                                CanEnterMarks = !isMissing && (!requiresSubmission || !string.IsNullOrWhiteSpace(Text(reader["file_url"]))),
                                IsPublished = isPublished,
                                ExtensionDeadline = extensionDeadline,
                                ExtensionGrantedAt = extensionGrantedAt
                            });
                        }
                    }
                }
            }
            return rows;
        }

        public static bool SaveReview(UserContext user, int submissionId, string feedback, string annotatedFileUrl)
        {
            if (user == null || submissionId <= 0) return false;
            EnsureReviewColumns();

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageSubmission(conn, user, submissionId)) return false;
                const string sql =
                    "UPDATE SUBMISSIONS SET feedback = @feedback, " +
                    "annotated_file_url = CASE WHEN @annotatedFileUrl IS NULL THEN annotated_file_url ELSE @annotatedFileUrl END " +
                    "WHERE submission_id = @submissionId";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@feedback", ServiceAccess.DbValue(feedback));
                    cmd.Parameters.AddWithValue("@annotatedFileUrl",
                        string.IsNullOrWhiteSpace(annotatedFileUrl) ? (object)DBNull.Value : annotatedFileUrl);
                    cmd.Parameters.AddWithValue("@submissionId", submissionId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static string GetAnnotationDraft(UserContext user, int submissionId)
        {
            if (user == null || submissionId <= 0) return "";
            EnsureReviewColumns();

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageSubmission(conn, user, submissionId)) return "";
                using (var cmd = new SqlCommand(
                    "SELECT ISNULL(annotation_draft_json, '') FROM SUBMISSIONS WHERE submission_id = @submissionId",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@submissionId", submissionId);
                    return Convert.ToString(cmd.ExecuteScalar()) ?? "";
                }
            }
        }

        public static bool SaveAnnotationDraft(UserContext user, int submissionId, string annotationsJson)
        {
            if (user == null || submissionId <= 0) return false;
            EnsureReviewColumns();

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageSubmission(conn, user, submissionId)) return false;
                using (var cmd = new SqlCommand(
                    "UPDATE SUBMISSIONS SET annotation_draft_json = @annotationsJson WHERE submission_id = @submissionId",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@annotationsJson",
                        string.IsNullOrWhiteSpace(annotationsJson) ? (object)DBNull.Value : annotationsJson);
                    cmd.Parameters.AddWithValue("@submissionId", submissionId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        internal static void EnsureReviewColumns()
        {
            const string sql =
                "IF COL_LENGTH('SUBMISSIONS', 'feedback') IS NULL " +
                "ALTER TABLE SUBMISSIONS ADD feedback varchar(2000) NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'annotated_file_url') IS NULL " +
                "ALTER TABLE SUBMISSIONS ADD annotated_file_url varchar(255) NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'annotation_draft_json') IS NULL " +
                "ALTER TABLE SUBMISSIONS ADD annotation_draft_json nvarchar(max) NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'published_marks_obtained') IS NULL " +
                "BEGIN " +
                "ALTER TABLE SUBMISSIONS ADD published_marks_obtained decimal(5,2) NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'published_feedback') IS NULL " +
                "ALTER TABLE SUBMISSIONS ADD published_feedback varchar(2000) NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'published_annotated_file_url') IS NULL " +
                "ALTER TABLE SUBMISSIONS ADD published_annotated_file_url varchar(255) NULL; " +
                "EXEC(N'UPDATE sub SET published_marks_obtained = sub.marks_obtained, " +
                "published_feedback = sub.feedback, published_annotated_file_url = sub.annotated_file_url " +
                "FROM SUBMISSIONS sub JOIN ASSIGNMENTS a ON a.assignment_id = sub.assignment_id " +
                "WHERE EXISTS (SELECT 1 FROM GRADES g WHERE g.offer_id = a.offer_id AND g.student_id = sub.student_id)'); " +
                "END; " +
                "IF COL_LENGTH('SUBMISSIONS', 'published_feedback') IS NULL " +
                "ALTER TABLE SUBMISSIONS ADD published_feedback varchar(2000) NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'published_annotated_file_url') IS NULL " +
                "ALTER TABLE SUBMISSIONS ADD published_annotated_file_url varchar(255) NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'published_at') IS NULL ALTER TABLE SUBMISSIONS ADD published_at datetime NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'extension_deadline') IS NULL ALTER TABLE SUBMISSIONS ADD extension_deadline datetime NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'extension_granted_at') IS NULL ALTER TABLE SUBMISSIONS ADD extension_granted_at datetime NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'file_url') < 1000 ALTER TABLE SUBMISSIONS ALTER COLUMN file_url varchar(1000) NULL; " +
                "IF COL_LENGTH('ASSIGNMENTS', 'submission_mode') IS NULL ALTER TABLE ASSIGNMENTS ADD submission_mode varchar(20) NULL;";
            using (var conn = Db.OpenConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                    cmd.ExecuteNonQuery();
                using (var cmd = new SqlCommand(
                    "UPDATE sub SET published_at = GETDATE() FROM SUBMISSIONS sub " +
                    "JOIN ASSIGNMENTS a ON a.assignment_id = sub.assignment_id " +
                    "WHERE sub.published_at IS NULL AND " +
                    "(sub.published_marks_obtained IS NOT NULL OR sub.published_feedback IS NOT NULL OR sub.published_annotated_file_url IS NOT NULL) " +
                    "AND EXISTS (SELECT 1 FROM GRADES g WHERE g.offer_id = a.offer_id AND g.student_id = sub.student_id)", conn))
                    cmd.ExecuteNonQuery();
            }
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
        public static GradeNotificationService.GradeEmailSendResult PublishGrades(UserContext user, int assessmentId)
        {
            var emailResult = new GradeNotificationService.GradeEmailSendResult();
            if (user == null) return emailResult;
            EnsureReviewColumns();
            GradeNotificationService.EnsureTables();

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageAssignment(conn, user, assessmentId)) return emailResult;

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
                        if (!reader.Read()) return emailResult;
                        offerId = IntValue(reader["offer_id"]);
                        semester = Text(reader["sem"]);
                    }
                }

                EnsureMissingSubmissions(conn, null, offerId);

                // Detect changed marks before replacing the previous student-visible
                // snapshot, then commit the alert and new snapshot atomically.
                var gradeEmailNotifications = new List<GradeNotificationService.GradeEmailNotification>();
                using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    gradeEmailNotifications = GradeNotificationService.InsertChangedMarks(conn, transaction, assessmentId);
                    using (var cmd = new SqlCommand(
                        "UPDATE SUBMISSIONS SET " +
                        "published_marks_obtained = marks_obtained, " +
                        "published_feedback = feedback, " +
                        "published_annotated_file_url = annotated_file_url, published_at = GETDATE(), " +
                        "auto_zero_notified = CASE WHEN status = 'MISSING' AND marks_obtained = 0 " +
                        "THEN 1 ELSE auto_zero_notified END " +
                        "WHERE assignment_id = @assignmentId", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@assignmentId", assessmentId);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                emailResult = GradeNotificationService.SendPublishedGradeEmails(gradeEmailNotifications);

                var percentByStudent = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
                var countByStudent = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = new SqlCommand(
                    "SELECT s.student_id, a.total_marks, " +
                    "CASE WHEN sub.status = 'MISSING' THEN CAST(0 AS decimal(5,2)) " +
                    "ELSE sub.published_marks_obtained END AS marks_obtained " +
                    "FROM ENROLLMENTS e " +
                    "JOIN STUDENTS s ON s.student_id = e.student_id " +
                    "JOIN ASSIGNMENTS a ON a.offer_id = e.offer_id " +
                    "JOIN MATERIALS mat ON mat.assignment_id = a.assignment_id " +
                    "LEFT JOIN SUBMISSIONS sub ON sub.assignment_id = a.assignment_id AND sub.student_id = s.student_id " +
                    "WHERE e.offer_id = @offerId AND e.status = 'ENROLLED' " +
                    "AND mat.weight IS NOT NULL AND mat.weight > 0", conn))
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

                return emailResult;
            }
        }

        private static void EnsureMissingSubmissions(SqlConnection conn, int? assessmentId, int? offerId)
        {
            const string expireExtensions =
                "UPDATE sub SET status = 'MISSING', marks_obtained = 0 " +
                "FROM SUBMISSIONS sub JOIN ASSIGNMENTS a ON a.assignment_id = sub.assignment_id " +
                "WHERE ISNULL(a.submission_mode, 'FILE') IN ('FILE','LINK') AND sub.extension_granted_at IS NOT NULL " +
                "AND sub.extension_deadline < GETDATE() AND sub.file_url IS NULL AND sub.status = 'EXTENDED' " +
                "AND (@assignmentId IS NULL OR a.assignment_id = @assignmentId) " +
                "AND (@offerId IS NULL OR a.offer_id = @offerId);";
            const string sql =
                "INSERT INTO SUBMISSIONS (assignment_id, student_id, submitted_at, file_url, marks_obtained, status) " +
                "SELECT a.assignment_id, e.student_id, NULL, NULL, 0, 'MISSING' " +
                "FROM ASSIGNMENTS a " +
                "JOIN ENROLLMENTS e ON e.offer_id = a.offer_id AND e.status = 'ENROLLED' " +
                "WHERE ISNULL(a.submission_mode, 'FILE') IN ('FILE','LINK') " +
                "AND a.due_date IS NOT NULL AND a.due_date < GETDATE() " +
                "AND (@assignmentId IS NULL OR a.assignment_id = @assignmentId) " +
                "AND (@offerId IS NULL OR a.offer_id = @offerId) " +
                "AND (@offerId IS NULL OR EXISTS (" +
                "SELECT 1 FROM MATERIALS mat WHERE mat.assignment_id = a.assignment_id " +
                "AND mat.weight IS NOT NULL AND mat.weight > 0)) " +
                "AND NOT EXISTS (" +
                "SELECT 1 FROM SUBMISSIONS existing WITH (UPDLOCK, HOLDLOCK) " +
                "WHERE existing.assignment_id = a.assignment_id AND existing.student_id = e.student_id);";

            using (var expire = new SqlCommand(expireExtensions, conn))
            {
                expire.Parameters.AddWithValue("@assignmentId",
                    assessmentId.HasValue ? (object)assessmentId.Value : DBNull.Value);
                expire.Parameters.AddWithValue("@offerId",
                    offerId.HasValue ? (object)offerId.Value : DBNull.Value);
                expire.ExecuteNonQuery();
            }
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@assignmentId",
                    assessmentId.HasValue ? (object)assessmentId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@offerId",
                    offerId.HasValue ? (object)offerId.Value : DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        private static void EnsureAssessmentRows(SqlConnection conn, int assessmentId)
        {
            const string sql =
                "INSERT INTO SUBMISSIONS (assignment_id, student_id, submitted_at, file_url, marks_obtained, status) " +
                "SELECT a.assignment_id, e.student_id, NULL, NULL, NULL, 'AWAITING_MARKS' " +
                "FROM ASSIGNMENTS a JOIN ENROLLMENTS e ON e.offer_id = a.offer_id AND e.status = 'ENROLLED' " +
                "WHERE a.assignment_id = @assignmentId AND ISNULL(a.submission_mode, 'FILE') = 'MANUAL' " +
                "AND NOT EXISTS (SELECT 1 FROM SUBMISSIONS sub WHERE sub.assignment_id = a.assignment_id AND sub.student_id = e.student_id);";
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@assignmentId", assessmentId);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool GrantExtension(UserContext user, int submissionId, DateTime deadline)
        {
            if (user == null || submissionId <= 0 || deadline <= DateTime.Now) return false;
            EnsureReviewColumns();
            ExtensionNotificationService.EnsureTables();
            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageSubmission(conn, user, submissionId)) return false;
                using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    const string sql =
                        "UPDATE sub SET extension_deadline = @deadline, extension_granted_at = GETDATE(), " +
                        "status = 'EXTENDED', marks_obtained = NULL, published_marks_obtained = NULL, " +
                        "published_feedback = NULL, published_annotated_file_url = NULL, published_at = NULL " +
                        "FROM SUBMISSIONS sub JOIN ASSIGNMENTS a ON a.assignment_id = sub.assignment_id " +
                        "WHERE sub.submission_id = @submissionId AND sub.status = 'MISSING' " +
                        "AND sub.extension_granted_at IS NULL AND ISNULL(a.submission_mode, 'FILE') IN ('FILE','LINK');";
                    using (var cmd = new SqlCommand(sql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@deadline", deadline);
                        cmd.Parameters.AddWithValue("@submissionId", submissionId);
                        if (cmd.ExecuteNonQuery() == 0) return false;
                    }

                    ExtensionNotificationService.InsertGrantedExtension(
                        conn, transaction, submissionId, deadline);
                    transaction.Commit();
                    return true;
                }
            }
        }

        public static bool ExpireExtension(UserContext user, int submissionId)
        {
            if (user == null || submissionId <= 0) return false;
            EnsureReviewColumns();
            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageSubmission(conn, user, submissionId)) return false;
                const string sql =
                    "UPDATE sub SET status = 'MISSING', marks_obtained = 0 " +
                    "FROM SUBMISSIONS sub JOIN ASSIGNMENTS a ON a.assignment_id = sub.assignment_id " +
                    "WHERE sub.submission_id = @submissionId AND sub.status = 'EXTENDED' " +
                    "AND sub.extension_deadline <= GETDATE() AND sub.file_url IS NULL " +
                    "AND ISNULL(a.submission_mode, 'FILE') IN ('FILE','LINK'); " +
                    "SELECT CASE WHEN EXISTS (" +
                    "SELECT 1 FROM SUBMISSIONS sub JOIN ASSIGNMENTS a ON a.assignment_id = sub.assignment_id " +
                    "WHERE sub.submission_id = @submissionId AND sub.status = 'MISSING' " +
                    "AND sub.extension_deadline <= GETDATE() AND sub.file_url IS NULL " +
                    "AND ISNULL(a.submission_mode, 'FILE') IN ('FILE','LINK')) THEN 1 ELSE 0 END;";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@submissionId", submissionId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
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
