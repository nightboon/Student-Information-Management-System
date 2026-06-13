using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using src.db;

namespace src.services
{
    public class LecturerAssessmentOption
    {
        public int AssessmentId { get; set; }
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string AssessmentName { get; set; }
        public decimal Weight { get; set; }
        public int MaxMarks { get; set; }

        public string Label
        {
            get { return CourseCode + " - " + AssessmentName; }
        }
    }

    public class LecturerGradeRow
    {
        public int AssessmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentNo { get; set; }
        public decimal? Marks { get; set; }
        public string LetterGrade { get; set; }
        public bool HasMarks { get; set; }
        public int? SubmissionId { get; set; }
        public int? AssignmentId { get; set; }
        public string AssignmentTitle { get; set; }
        public string SubmissionFileUrl { get; set; }
        public string SubmissionStatus { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string Feedback { get; set; }
        public string AnnotatedFileUrl { get; set; }

        public bool HasSubmission
        {
            get { return SubmissionId.HasValue && !string.IsNullOrWhiteSpace(SubmissionFileUrl); }
        }
    }

    public class LecturerGradeUpdate
    {
        public decimal? Marks { get; set; }
        public int? SubmissionId { get; set; }
        public string Feedback { get; set; }
        public string AnnotatedFileUrl { get; set; }
    }

    public class LecturerMaterialRow
    {
        public int MaterialId { get; set; }
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Week { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public int? FileSizeBytes { get; set; }
        public string MaterialType { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? Weight { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class LecturerAnnouncementRow
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string FileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPinned { get; set; }
        public string TargetCourses { get; set; }

        public bool HasAttachment
        {
            get { return !string.IsNullOrWhiteSpace(FileUrl); }
        }
    }

    public class AtRiskStudentRow
    {
        public string StudentName { get; set; }
        public string StudentNo { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public decimal? AttendancePercent { get; set; }
        public decimal? CurrentMark { get; set; }
        public int MissingSubmissions { get; set; }
        public string RiskReason { get; set; }
        public string RiskLevel { get; set; }
        public bool AttendanceRisk { get; set; }
        public bool AcademicRisk { get; set; }
    }

    public static class LecturerPortalService
    {
        private const string SelectAssessments =
            "SELECT a.assessment_id, a.offering_id, c.course_code, c.course_name, " +
            "a.name, a.weight, a.max_marks " +
            "FROM ASSESSMENTS a " +
            "JOIN COURSE_OFFERINGS o ON o.offering_id = a.offering_id " +
            "JOIN COURSES c ON c.course_id = o.course_id " +
            "JOIN TEACHINGS t ON t.offering_id = o.offering_id " +
            "WHERE t.lecturer_id = @lecturerId " +
            "ORDER BY c.course_code, a.sort_order";

        public static List<LecturerAssessmentOption> GetAssessments(int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectAssessments, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                var list = new List<LecturerAssessmentOption>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LecturerAssessmentOption
                        {
                            AssessmentId = (int)reader["assessment_id"],
                            OfferingId = (int)reader["offering_id"],
                            CourseCode = reader["course_code"].ToString(),
                            CourseName = reader["course_name"].ToString(),
                            AssessmentName = reader["name"].ToString(),
                            Weight = Convert.ToDecimal(reader["weight"]),
                            MaxMarks = Convert.ToInt32(reader["max_marks"])
                        });
                    }
                }
                return list;
            }
        }

        private const string SelectAssessmentRows =
            "SELECT st.student_id, st.full_name, u.email, u.username, sa.marks, " +
            "matched.assignment_id, matched.title AS assignment_title, " +
            "sub.submission_id, sub.file_url, sub.submit_date, sub.status AS submission_status, " +
            "ISNULL(sub.feedback, '') AS feedback, ISNULL(sub.annotated_file_url, '') AS annotated_file_url " +
            "FROM ASSESSMENTS target " +
            "JOIN ENROLMENTS e ON e.offering_id = target.offering_id AND e.status = 'ENROLLED' " +
            "JOIN STUDENTS st ON st.student_id = e.student_id " +
            "JOIN USERS u ON u.user_id = st.user_id " +
            "LEFT JOIN STUDENT_ASSESSMENTS sa ON sa.assessment_id = target.assessment_id AND sa.student_id = st.student_id " +
            "OUTER APPLY ( " +
            "  SELECT TOP 1 asg.assignment_id, asg.title " +
            "  FROM ASSIGNMENTS asg " +
            "  WHERE asg.offering_id = target.offering_id " +
            "  AND (LOWER(asg.title) = LOWER(target.name) " +
            "    OR LOWER(asg.title) LIKE '%' + LOWER(target.name) + '%' " +
            "    OR LOWER(target.name) LIKE '%' + LOWER(asg.title) + '%' " +
            "    OR (target.type IN ('Assignment', 'Project') AND asg.weight = target.weight)) " +
            "  ORDER BY CASE " +
            "    WHEN LOWER(asg.title) = LOWER(target.name) THEN 0 " +
            "    WHEN LOWER(asg.title) LIKE '%' + LOWER(target.name) + '%' THEN 1 " +
            "    WHEN LOWER(target.name) LIKE '%' + LOWER(asg.title) + '%' THEN 2 " +
            "    WHEN asg.weight = target.weight THEN 3 " +
            "    ELSE 4 END, asg.due_date " +
            ") matched " +
            "LEFT JOIN SUBMISSIONS sub ON sub.assignment_id = matched.assignment_id AND sub.student_id = st.student_id " +
            "WHERE target.assessment_id = @assessmentId " +
            "AND EXISTS (SELECT 1 FROM TEACHINGS t WHERE t.offering_id = target.offering_id AND t.lecturer_id = @lecturerId) " +
            "ORDER BY st.full_name";

        public static List<LecturerGradeRow> GetGradeRows(int lecturerId, int assessmentId)
        {
            EnsureSubmissionFeedbackColumn();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectAssessmentRows, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                cmd.Parameters.AddWithValue("@assessmentId", assessmentId);
                var list = new List<LecturerGradeRow>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        decimal? marks = reader["marks"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["marks"]);
                        list.Add(new LecturerGradeRow
                        {
                            AssessmentId = assessmentId,
                            StudentId = (int)reader["student_id"],
                            StudentName = reader["full_name"].ToString(),
                            StudentEmail = reader["email"].ToString(),
                            StudentNo = reader["username"].ToString(),
                            Marks = marks,
                            HasMarks = marks.HasValue,
                            LetterGrade = marks.HasValue ? ToLetterGrade(marks.Value) : "N/A",
                            AssignmentId = reader["assignment_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["assignment_id"]),
                            AssignmentTitle = reader["assignment_title"] == DBNull.Value ? "" : reader["assignment_title"].ToString(),
                            SubmissionId = reader["submission_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["submission_id"]),
                            SubmissionFileUrl = reader["file_url"] == DBNull.Value ? "" : reader["file_url"].ToString(),
                            SubmittedAt = reader["submit_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["submit_date"]),
                            SubmissionStatus = reader["submission_status"] == DBNull.Value ? "NOT SUBMITTED" : reader["submission_status"].ToString(),
                            Feedback = reader["feedback"].ToString(),
                            AnnotatedFileUrl = reader["annotated_file_url"].ToString()
                        });
                    }
                }
                return list;
            }
        }

        private const string UpsertStudentAssessment =
            "IF EXISTS (SELECT 1 FROM STUDENT_ASSESSMENTS WHERE assessment_id = @assessmentId AND student_id = @studentId) " +
            "UPDATE STUDENT_ASSESSMENTS SET marks = @marks WHERE assessment_id = @assessmentId AND student_id = @studentId; " +
            "ELSE INSERT INTO STUDENT_ASSESSMENTS (assessment_id, student_id, marks) VALUES (@assessmentId, @studentId, @marks);";

        public static void SaveAssessmentMarks(int lecturerId, int assessmentId, IDictionary<int, decimal?> marksByStudent)
        {
            if (!LecturerOwnsAssessment(lecturerId, assessmentId)) return;

            using (var conn = Db.OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                foreach (var pair in marksByStudent)
                {
                    if (pair.Value.HasValue && (pair.Value.Value < 0m || pair.Value.Value > 100m)) continue;
                    using (var cmd = new SqlCommand(UpsertStudentAssessment, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@assessmentId", assessmentId);
                        cmd.Parameters.AddWithValue("@studentId", pair.Key);
                        cmd.Parameters.Add("@marks", SqlDbType.Decimal).Value = pair.Value.HasValue ? (object)pair.Value.Value : DBNull.Value;
                        cmd.Parameters["@marks"].Precision = 5;
                        cmd.Parameters["@marks"].Scale = 2;
                        cmd.ExecuteNonQuery();
                    }
                }
                tx.Commit();
            }
        }

        public static void SaveAssessmentRows(int lecturerId, int assessmentId, IDictionary<int, LecturerGradeUpdate> updatesByStudent)
        {
            if (!LecturerOwnsAssessment(lecturerId, assessmentId)) return;
            EnsureSubmissionFeedbackColumn();

            using (var conn = Db.OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                foreach (var pair in updatesByStudent)
                {
                    var update = pair.Value;
                    if (update.Marks.HasValue && (update.Marks.Value < 0m || update.Marks.Value > 100m)) continue;

                    using (var cmd = new SqlCommand(UpsertStudentAssessment, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@assessmentId", assessmentId);
                        cmd.Parameters.AddWithValue("@studentId", pair.Key);
                        cmd.Parameters.Add("@marks", SqlDbType.Decimal).Value = update.Marks.HasValue ? (object)update.Marks.Value : DBNull.Value;
                        cmd.Parameters["@marks"].Precision = 5;
                        cmd.Parameters["@marks"].Scale = 2;
                        cmd.ExecuteNonQuery();
                    }

                    if (update.SubmissionId.HasValue)
                    {
                        using (var cmd = new SqlCommand(
                            "UPDATE sub SET marks = @marks, status = CASE WHEN @marks IS NULL THEN sub.status ELSE 'MARKED' END " +
                            "FROM SUBMISSIONS sub " +
                            "JOIN ASSIGNMENTS asg ON asg.assignment_id = sub.assignment_id " +
                            "JOIN ASSESSMENTS a ON a.offering_id = asg.offering_id " +
                            "WHERE sub.submission_id = @submissionId AND sub.student_id = @studentId AND a.assessment_id = @assessmentId " +
                            "AND EXISTS (SELECT 1 FROM TEACHINGS t WHERE t.offering_id = a.offering_id AND t.lecturer_id = @lecturerId)", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                            cmd.Parameters.AddWithValue("@assessmentId", assessmentId);
                            cmd.Parameters.AddWithValue("@studentId", pair.Key);
                            cmd.Parameters.AddWithValue("@submissionId", update.SubmissionId.Value);
                            cmd.Parameters.Add("@marks", SqlDbType.Decimal).Value = update.Marks.HasValue ? (object)update.Marks.Value : DBNull.Value;
                            cmd.Parameters["@marks"].Precision = 5;
                            cmd.Parameters["@marks"].Scale = 2;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                tx.Commit();
            }
        }

        private static void EnsureSubmissionFeedbackColumn()
        {
            const string sql =
                "IF COL_LENGTH('SUBMISSIONS', 'feedback') IS NULL " +
                "ALTER TABLE SUBMISSIONS ADD feedback varchar(max) NULL; " +
                "IF COL_LENGTH('SUBMISSIONS', 'annotated_file_url') IS NULL " +
                "ALTER TABLE SUBMISSIONS ADD annotated_file_url varchar(255) NULL";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static void SaveSubmissionReview(int lecturerId, int submissionId, string feedback, string annotatedFileUrl)
        {
            EnsureSubmissionFeedbackColumn();

            const string sql =
                "UPDATE sub SET feedback = @feedback, " +
                "annotated_file_url = CASE WHEN @annotatedFileUrl IS NULL THEN annotated_file_url ELSE @annotatedFileUrl END " +
                "FROM SUBMISSIONS sub " +
                "JOIN ASSIGNMENTS asg ON asg.assignment_id = sub.assignment_id " +
                "WHERE sub.submission_id = @submissionId " +
                "AND EXISTS (SELECT 1 FROM TEACHINGS t WHERE t.offering_id = asg.offering_id AND t.lecturer_id = @lecturerId)";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                cmd.Parameters.AddWithValue("@submissionId", submissionId);
                cmd.Parameters.AddWithValue("@feedback", string.IsNullOrWhiteSpace(feedback) ? (object)DBNull.Value : feedback.Trim());
                cmd.Parameters.AddWithValue("@annotatedFileUrl", string.IsNullOrWhiteSpace(annotatedFileUrl) ? (object)DBNull.Value : annotatedFileUrl.Trim());
                cmd.ExecuteNonQuery();
            }
        }

        public static void PublishOfferingGrades(int lecturerId, int assessmentId)
        {
            int offeringId = GetAssessmentOffering(lecturerId, assessmentId);
            if (offeringId == 0) return;

            var rows = GetOfferingGradeTotals(offeringId);
            using (var conn = Db.OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                foreach (var row in rows)
                {
                    UpsertFinalGrade(conn, tx, row.EnrolmentId, row.Mark);
                }
                tx.Commit();
            }
        }

        private class GradeTotal
        {
            public int EnrolmentId { get; set; }
            public decimal Mark { get; set; }
        }

        private static List<GradeTotal> GetOfferingGradeTotals(int offeringId)
        {
            const string sql =
                "SELECT e.enrolment_id, " +
                "SUM(CASE WHEN sa.marks IS NOT NULL AND a.max_marks > 0 THEN sa.marks / a.max_marks * a.weight ELSE 0 END) AS mark " +
                "FROM ENROLMENTS e " +
                "JOIN ASSESSMENTS a ON a.offering_id = e.offering_id " +
                "LEFT JOIN STUDENT_ASSESSMENTS sa ON sa.assessment_id = a.assessment_id AND sa.student_id = e.student_id " +
                "WHERE e.offering_id = @offeringId AND e.status = 'ENROLLED' " +
                "GROUP BY e.enrolment_id";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@offeringId", offeringId);
                var list = new List<GradeTotal>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new GradeTotal
                        {
                            EnrolmentId = (int)reader["enrolment_id"],
                            Mark = reader["mark"] == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(reader["mark"]), 2)
                        });
                    }
                }
                return list;
            }
        }

        private static void UpsertFinalGrade(SqlConnection conn, SqlTransaction tx, int enrolmentId, decimal mark)
        {
            const string sql =
                "IF EXISTS (SELECT 1 FROM GRADES WHERE enrolment_id = @enrolmentId) " +
                "UPDATE GRADES SET marks = @marks, gpa = @gpa, grade = @grade, published = 1 WHERE enrolment_id = @enrolmentId; " +
                "ELSE INSERT INTO GRADES (enrolment_id, marks, gpa, grade, published) VALUES (@enrolmentId, @marks, @gpa, @grade, 1);";
            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.AddWithValue("@enrolmentId", enrolmentId);
                cmd.Parameters.Add("@marks", SqlDbType.Decimal).Value = mark;
                cmd.Parameters["@marks"].Precision = 5;
                cmd.Parameters["@marks"].Scale = 2;
                cmd.Parameters.Add("@gpa", SqlDbType.Decimal).Value = ToGpa(mark);
                cmd.Parameters["@gpa"].Precision = 3;
                cmd.Parameters["@gpa"].Scale = 2;
                cmd.Parameters.AddWithValue("@grade", ToLetterGrade(mark));
                cmd.ExecuteNonQuery();
            }
        }

        public static List<LecturerMaterialRow> GetMaterials(int lecturerId, int? offeringId = null)
        {
            EnsureMaterialColumns();
            const string sql =
                "SELECT cm.material_id, cm.offering_id, c.course_code, c.course_name, cm.title, " +
                "CAST(ISNULL(cm.description, '') AS varchar(max)) AS description, cm.week, cm.file_url, " +
                "cm.file_type, cm.file_size_bytes, cm.uploaded_at, " +
                "ISNULL(cm.material_type, 'Lecture Notes') AS material_type, cm.due_date, cm.weight " +
                "FROM COURSE_MATERIALS cm " +
                "JOIN COURSE_OFFERINGS o ON o.offering_id = cm.offering_id " +
                "JOIN COURSES c ON c.course_id = o.course_id " +
                "WHERE cm.lecturer_id = @lecturerId " +
                "AND cm.file_url IS NOT NULL AND LTRIM(RTRIM(cm.file_url)) <> '' " +
                "AND (@offeringId IS NULL OR cm.offering_id = @offeringId) " +
                "ORDER BY cm.uploaded_at DESC";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                cmd.Parameters.AddWithValue("@offeringId", offeringId.HasValue ? (object)offeringId.Value : DBNull.Value);
                var list = new List<LecturerMaterialRow>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LecturerMaterialRow
                        {
                            MaterialId = (int)reader["material_id"],
                            OfferingId = (int)reader["offering_id"],
                            CourseCode = reader["course_code"].ToString(),
                            CourseName = reader["course_name"].ToString(),
                            Title = reader["title"].ToString(),
                            Description = reader["description"].ToString(),
                            Week = reader["week"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["week"]),
                            FileUrl = reader["file_url"] == DBNull.Value ? "" : reader["file_url"].ToString(),
                            FileType = reader["file_type"] == DBNull.Value ? "" : reader["file_type"].ToString(),
                            FileSizeBytes = reader["file_size_bytes"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["file_size_bytes"]),
                            MaterialType = reader["material_type"].ToString(),
                            DueDate = reader["due_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["due_date"]),
                            Weight = reader["weight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["weight"]),
                            UploadedAt = (DateTime)reader["uploaded_at"]
                        });
                    }
                }
                return list;
            }
        }

        private static void EnsureMaterialColumns()
        {
            const string sql =
                "IF COL_LENGTH('COURSE_MATERIALS', 'material_type') IS NULL " +
                "ALTER TABLE COURSE_MATERIALS ADD material_type varchar(30) NULL; " +
                "IF COL_LENGTH('COURSE_MATERIALS', 'due_date') IS NULL " +
                "ALTER TABLE COURSE_MATERIALS ADD due_date date NULL; " +
                "IF COL_LENGTH('COURSE_MATERIALS', 'weight') IS NULL " +
                "ALTER TABLE COURSE_MATERIALS ADD weight decimal(5,2) NULL";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static bool AddMaterial(int lecturerId, int offeringId, string title, string description, string materialType, DateTime? dueDate, decimal? weight, string fileUrl, string fileType, int? fileSizeBytes)
        {
            if (!LecturerOwnsOffering(lecturerId, offeringId) || string.IsNullOrWhiteSpace(title)) return false;
            if (dueDate.HasValue && dueDate.Value.Date < DateTime.Today) return false;
            if (weight.HasValue && (weight.Value < 0m || weight.Value > 100m)) return false;
            EnsureMaterialColumns();
            materialType = NormalizeMaterialType(materialType);
            const string sql =
                "INSERT INTO COURSE_MATERIALS (offering_id, lecturer_id, title, description, file_url, file_type, file_size_bytes, material_type, due_date, weight, uploaded_at) " +
                "VALUES (@offeringId, @lecturerId, @title, @description, @fileUrl, @fileType, @fileSizeBytes, @materialType, @dueDate, @weight, SYSDATETIME())";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@offeringId", offeringId);
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                cmd.Parameters.AddWithValue("@title", title.Trim());
                cmd.Parameters.AddWithValue("@description", string.IsNullOrWhiteSpace(description) ? (object)DBNull.Value : description.Trim());
                cmd.Parameters.AddWithValue("@fileUrl", string.IsNullOrWhiteSpace(fileUrl) ? (object)DBNull.Value : fileUrl.Trim());
                cmd.Parameters.AddWithValue("@fileType", string.IsNullOrWhiteSpace(fileType) ? (object)DBNull.Value : fileType.Trim().ToLowerInvariant());
                cmd.Parameters.AddWithValue("@fileSizeBytes", fileSizeBytes.HasValue ? (object)fileSizeBytes.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@materialType", materialType);
                cmd.Parameters.AddWithValue("@dueDate", dueDate.HasValue ? (object)dueDate.Value.Date : DBNull.Value);
                cmd.Parameters.Add("@weight", SqlDbType.Decimal).Value = weight.HasValue ? (object)weight.Value : DBNull.Value;
                cmd.Parameters["@weight"].Precision = 5;
                cmd.Parameters["@weight"].Scale = 2;
                cmd.ExecuteNonQuery();
            }
            return true;
        }

        public static bool DeleteMaterial(int lecturerId, int materialId)
        {
            const string sql =
                "DELETE cm FROM COURSE_MATERIALS cm " +
                "WHERE cm.material_id = @materialId " +
                "AND EXISTS (SELECT 1 FROM TEACHINGS t WHERE t.offering_id = cm.offering_id AND t.lecturer_id = @lecturerId)";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                cmd.Parameters.AddWithValue("@materialId", materialId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static LecturerMaterialRow GetMaterialForPreview(int materialId, int userId, string role)
        {
            EnsureMaterialColumns();
            const string sql =
                "SELECT cm.material_id, cm.offering_id, c.course_code, c.course_name, cm.title, " +
                "CAST(ISNULL(cm.description, '') AS varchar(max)) AS description, cm.week, cm.file_url, " +
                "cm.file_type, cm.file_size_bytes, cm.uploaded_at, " +
                "ISNULL(cm.material_type, 'Lecture Notes') AS material_type, cm.due_date, cm.weight " +
                "FROM COURSE_MATERIALS cm " +
                "JOIN COURSE_OFFERINGS o ON o.offering_id = cm.offering_id " +
                "JOIN COURSES c ON c.course_id = o.course_id " +
                "WHERE cm.material_id = @materialId " +
                "AND cm.file_url IS NOT NULL AND LTRIM(RTRIM(cm.file_url)) <> '' " +
                "AND ( " +
                "  (@role = 'Lecturer' AND EXISTS (SELECT 1 FROM LECTURERS l JOIN TEACHINGS t ON t.lecturer_id = l.lecturer_id WHERE l.user_id = @userId AND t.offering_id = cm.offering_id)) " +
                "  OR (@role = 'Student' AND EXISTS (SELECT 1 FROM STUDENTS s JOIN ENROLMENTS e ON e.student_id = s.student_id WHERE s.user_id = @userId AND e.offering_id = cm.offering_id AND e.status = 'ENROLLED')) " +
                ")";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@materialId", materialId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@role", role ?? "");
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new LecturerMaterialRow
                    {
                        MaterialId = (int)reader["material_id"],
                        OfferingId = (int)reader["offering_id"],
                        CourseCode = reader["course_code"].ToString(),
                        CourseName = reader["course_name"].ToString(),
                        Title = reader["title"].ToString(),
                        Description = reader["description"].ToString(),
                        Week = reader["week"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["week"]),
                        FileUrl = reader["file_url"].ToString(),
                        FileType = reader["file_type"] == DBNull.Value ? "" : reader["file_type"].ToString(),
                        FileSizeBytes = reader["file_size_bytes"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["file_size_bytes"]),
                        MaterialType = reader["material_type"].ToString(),
                        DueDate = reader["due_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["due_date"]),
                        Weight = reader["weight"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["weight"]),
                        UploadedAt = (DateTime)reader["uploaded_at"]
                    };
                }
            }
        }

        private static string NormalizeMaterialType(string materialType)
        {
            if (string.Equals(materialType, "Assignment", StringComparison.OrdinalIgnoreCase)) return "Assignment";
            if (string.Equals(materialType, "Quiz", StringComparison.OrdinalIgnoreCase)) return "Quiz";
            if (string.Equals(materialType, "Test", StringComparison.OrdinalIgnoreCase)) return "Test";
            return "Lecture Notes";
        }

        public static List<LecturerAnnouncementRow> GetLecturerAnnouncements(int lecturerId, int? offeringId = null)
        {
            const string sql =
                "SELECT an.announcement_id, an.title, CAST(an.content AS varchar(max)) AS content, an.file_url, an.created_at, an.is_pinned, " +
                "STUFF((SELECT DISTINCT ', ' + c2.course_code FROM ANNOUNCEMENT_TARGETS at2 " +
                "JOIN COURSE_OFFERINGS o2 ON o2.offering_id = at2.offering_id " +
                "JOIN COURSES c2 ON c2.course_id = o2.course_id " +
                "WHERE at2.announcement_id = an.announcement_id FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '') AS targets " +
                "FROM ANNOUNCEMENTS an " +
                "JOIN LECTURERS l ON l.user_id = an.created_by " +
                "WHERE l.lecturer_id = @lecturerId " +
                "AND (@offeringId IS NULL OR EXISTS (SELECT 1 FROM ANNOUNCEMENT_TARGETS atf " +
                "WHERE atf.announcement_id = an.announcement_id AND atf.offering_id = @offeringId)) " +
                "ORDER BY an.is_pinned DESC, an.created_at DESC";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                cmd.Parameters.AddWithValue("@offeringId", offeringId.HasValue ? (object)offeringId.Value : DBNull.Value);
                var list = new List<LecturerAnnouncementRow>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LecturerAnnouncementRow
                        {
                            AnnouncementId = (int)reader["announcement_id"],
                            Title = reader["title"].ToString(),
                            Content = reader["content"].ToString(),
                            FileUrl = reader["file_url"] == DBNull.Value ? "" : reader["file_url"].ToString(),
                            CreatedAt = (DateTime)reader["created_at"],
                            IsPinned = Convert.ToBoolean(reader["is_pinned"]),
                            TargetCourses = reader["targets"] == DBNull.Value ? "No course targets" : reader["targets"].ToString()
                        });
                    }
                }
                return list;
            }
        }

        public static void AddAnnouncement(int lecturerId, int userId, int offeringId, string title, string content, bool pinned)
        {
            AddAnnouncement(lecturerId, userId, offeringId, title, content, pinned, null);
        }

        public static void AddAnnouncement(int lecturerId, int userId, int offeringId, string title, string content, bool pinned, string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content)) return;
            var offeringIds = new List<int>();
            if (offeringId > 0)
            {
                if (!LecturerOwnsOffering(lecturerId, offeringId)) return;
                offeringIds.Add(offeringId);
            }
            else
            {
                foreach (var course in LecturerCourseService.GetCourses(userId))
                {
                    if (LecturerOwnsOffering(lecturerId, course.OfferingId)) offeringIds.Add(course.OfferingId);
                }
            }
            if (offeringIds.Count == 0) return;

            using (var conn = Db.OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                int announcementId;
                using (var cmd = new SqlCommand(
                    "INSERT INTO ANNOUNCEMENTS (created_by, title, content, file_url, created_at, is_pinned) " +
                    "OUTPUT INSERTED.announcement_id VALUES (@userId, @title, @content, @fileUrl, SYSDATETIME(), @pinned)", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@title", title.Trim());
                    cmd.Parameters.AddWithValue("@content", content.Trim());
                    cmd.Parameters.AddWithValue("@fileUrl", string.IsNullOrWhiteSpace(fileUrl) ? (object)DBNull.Value : fileUrl.Trim());
                    cmd.Parameters.AddWithValue("@pinned", pinned);
                    announcementId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                foreach (int id in offeringIds)
                {
                    using (var cmd = new SqlCommand(
                        "INSERT INTO ANNOUNCEMENT_TARGETS (announcement_id, offering_id) VALUES (@announcementId, @offeringId)", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@announcementId", announcementId);
                        cmd.Parameters.AddWithValue("@offeringId", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                tx.Commit();
            }
        }

        public static void SetAnnouncementPinned(int lecturerId, int announcementId, bool pinned)
        {
            const string sql =
                "UPDATE an SET is_pinned = @pinned " +
                "FROM ANNOUNCEMENTS an " +
                "JOIN LECTURERS l ON l.user_id = an.created_by " +
                "WHERE an.announcement_id = @announcementId AND l.lecturer_id = @lecturerId";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                cmd.Parameters.AddWithValue("@announcementId", announcementId);
                cmd.Parameters.AddWithValue("@pinned", pinned);
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteAnnouncement(int lecturerId, int announcementId)
        {
            using (var conn = Db.OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                const string ownsSql =
                    "SELECT COUNT(*) FROM ANNOUNCEMENTS an " +
                    "JOIN LECTURERS l ON l.user_id = an.created_by " +
                    "WHERE an.announcement_id = @announcementId AND l.lecturer_id = @lecturerId";
                using (var cmd = new SqlCommand(ownsSql, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                    cmd.Parameters.AddWithValue("@announcementId", announcementId);
                    if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                    {
                        tx.Rollback();
                        return;
                    }
                }

                using (var cmd = new SqlCommand("DELETE FROM NOTIFICATION_READS WHERE announcement_id = @announcementId", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@announcementId", announcementId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand("DELETE FROM ANNOUNCEMENT_TARGETS WHERE announcement_id = @announcementId", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@announcementId", announcementId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand("DELETE FROM ANNOUNCEMENTS WHERE announcement_id = @announcementId", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@announcementId", announcementId);
                    cmd.ExecuteNonQuery();
                }
                tx.Commit();
            }
        }

        public static List<AtRiskStudentRow> GetAtRiskStudents(int lecturerId)
        {
            const string sql =
                "SELECT st.full_name, u.username, c.course_code, c.course_name, " +
                "attendance.present_count, attendance.total_count, progress.current_mark, missing.missing_count " +
                "FROM TEACHINGS t " +
                "JOIN COURSE_OFFERINGS o ON o.offering_id = t.offering_id " +
                "JOIN COURSES c ON c.course_id = o.course_id " +
                "JOIN ENROLMENTS e ON e.offering_id = o.offering_id AND e.status = 'ENROLLED' " +
                "JOIN STUDENTS st ON st.student_id = e.student_id " +
                "JOIN USERS u ON u.user_id = st.user_id " +
                "OUTER APPLY (SELECT COUNT(*) AS total_count, SUM(CASE WHEN a.status = 'PRESENT' THEN 1 ELSE 0 END) AS present_count FROM ATTENDANCE a WHERE a.enrolment_id = e.enrolment_id) attendance " +
                "OUTER APPLY (SELECT SUM(CASE WHEN sa.marks IS NOT NULL AND a2.max_marks > 0 THEN sa.marks / a2.max_marks * a2.weight ELSE 0 END) AS current_mark FROM ASSESSMENTS a2 LEFT JOIN STUDENT_ASSESSMENTS sa ON sa.assessment_id = a2.assessment_id AND sa.student_id = st.student_id WHERE a2.offering_id = o.offering_id) progress " +
                "OUTER APPLY (SELECT COUNT(*) AS missing_count FROM ASSIGNMENTS asg WHERE asg.offering_id = o.offering_id AND NOT EXISTS (SELECT 1 FROM SUBMISSIONS su WHERE su.assignment_id = asg.assignment_id AND su.student_id = st.student_id)) missing " +
                "WHERE t.lecturer_id = @lecturerId " +
                "ORDER BY c.course_code, st.full_name";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                var list = new List<AtRiskStudentRow>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int total = Convert.ToInt32(reader["total_count"]);
                        int present = reader["present_count"] == DBNull.Value ? 0 : Convert.ToInt32(reader["present_count"]);
                        decimal? attendance = total == 0 ? (decimal?)null : Math.Round(present / (decimal)total * 100m, 1);
                        decimal? mark = reader["current_mark"] == DBNull.Value ? (decimal?)null : Math.Round(Convert.ToDecimal(reader["current_mark"]), 1);
                        int missing = Convert.ToInt32(reader["missing_count"]);
                        bool attendanceRisk = attendance.HasValue && attendance.Value < 80m;
                        bool academicRisk = (mark.HasValue && mark.Value < 50m) || missing > 0;
                        if (!attendanceRisk && !academicRisk) continue;

                        list.Add(new AtRiskStudentRow
                        {
                            StudentName = reader["full_name"].ToString(),
                            StudentNo = reader["username"].ToString(),
                            CourseCode = reader["course_code"].ToString(),
                            CourseName = reader["course_name"].ToString(),
                            AttendancePercent = attendance,
                            CurrentMark = mark,
                            MissingSubmissions = missing,
                            AttendanceRisk = attendanceRisk,
                            AcademicRisk = academicRisk,
                            RiskLevel = academicRisk && attendanceRisk ? "High" : "Medium",
                            RiskReason = BuildRiskReason(attendanceRisk, academicRisk, missing)
                        });
                    }
                }
                return list;
            }
        }

        private static bool LecturerOwnsAssessment(int lecturerId, int assessmentId)
        {
            return GetAssessmentOffering(lecturerId, assessmentId) != 0;
        }

        private static int GetAssessmentOffering(int lecturerId, int assessmentId)
        {
            const string sql =
                "SELECT a.offering_id FROM ASSESSMENTS a " +
                "WHERE a.assessment_id = @assessmentId " +
                "AND EXISTS (SELECT 1 FROM TEACHINGS t WHERE t.offering_id = a.offering_id AND t.lecturer_id = @lecturerId)";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                cmd.Parameters.AddWithValue("@assessmentId", assessmentId);
                object value = cmd.ExecuteScalar();
                return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
            }
        }

        private static bool LecturerOwnsOffering(int lecturerId, int offeringId)
        {
            const string sql = "SELECT COUNT(*) FROM TEACHINGS WHERE lecturer_id = @lecturerId AND offering_id = @offeringId";
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                cmd.Parameters.AddWithValue("@offeringId", offeringId);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public static string ToLetterGrade(decimal mark)
        {
            if (mark >= 90m) return "A";
            if (mark >= 80m) return "A-";
            if (mark >= 75m) return "B+";
            if (mark >= 70m) return "B";
            if (mark >= 65m) return "B-";
            if (mark >= 60m) return "C+";
            if (mark >= 55m) return "C";
            if (mark >= 50m) return "C-";
            if (mark >= 40m) return "D";
            return "F";
        }

        private static decimal ToGpa(decimal mark)
        {
            if (mark >= 90m) return 4.00m;
            if (mark >= 80m) return 3.70m;
            if (mark >= 75m) return 3.30m;
            if (mark >= 70m) return 3.00m;
            if (mark >= 65m) return 2.70m;
            if (mark >= 60m) return 2.30m;
            if (mark >= 55m) return 2.00m;
            if (mark >= 50m) return 1.70m;
            if (mark >= 40m) return 1.00m;
            return 0.00m;
        }

        private static string BuildRiskReason(bool attendanceRisk, bool academicRisk, int missing)
        {
            if (attendanceRisk && academicRisk && missing > 0) return "Low attendance, weak marks, and missing submissions";
            if (attendanceRisk && academicRisk) return "Low attendance and weak marks";
            if (attendanceRisk) return "Attendance below warning threshold";
            if (missing > 0) return missing == 1 ? "Missing 1 submission" : "Missing " + missing + " submissions";
            return "Current mark below passing threshold";
        }
    }
}
