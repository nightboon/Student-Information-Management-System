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

            var sql =
                "SELECT a.assignment_id, a.offer_id, a.title, CAST(a.description AS varchar(max)) AS description, a.total_marks, a.due_date, " +
                "c.course_code, sub.submission_id, sub.file_url, sub.marks_obtained, sub.status AS submission_status " +
                "FROM ASSIGNMENTS a " +
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
                            var hasSubmission = reader["submission_id"] != DBNull.Value;
                            var rawStatus = Text(reader["submission_status"]).ToUpperInvariant();
                            var dueDate = DateValue(reader["due_date"]) ?? DateTime.Today;
                            var marks = DecimalValue(reader["marks_obtained"]);
                            assignments.Add(new StudentCourseAssignment
                            {
                                AssignmentId = IntValue(reader["assignment_id"]),
                                OfferId = IntValue(reader["offer_id"]),
                                Title = Text(reader["title"]),
                                Description = Text(reader["description"]),
                                DueDate = dueDate,
                                Weight = null,
                                AssignmentType = "Assignment",
                                GroupSize = "",
                                SubmissionStatus = marks.HasValue || rawStatus == "GRADED" ? "MARKED" : hasSubmission ? "SUBMITTED" : "PENDING",
                                HasSubmission = hasSubmission,
                                SubmissionFileUrl = Text(reader["file_url"]),
                                Feedback = "",
                                Marks = marks,
                                CourseCode = Text(reader["course_code"])
                            });
                        }
                    }
                }
            }

            if (assignments.Count > 0)
            {
                var weight = Math.Round(100m / assignments.Count, 2);
                foreach (var assignment in assignments) assignment.Weight = weight;
            }
            return assignments;
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

            var weight = Math.Round(100m / assignments.Count, 2);
            foreach (var assignment in assignments)
            {
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

            var assignments = AssignmentService.GetData(user);
            var assignment = assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);
            if (assignment == null) return false;

            var status = assignment.DueDate.HasValue && DateTime.Now > assignment.DueDate.Value ? "LATE" : "SUBMITTED";
            var existing = SubmissionService.GetData(user).FirstOrDefault(s => s.AssignmentId == assignmentId && string.Equals(s.StudentId, account.StudentId, StringComparison.OrdinalIgnoreCase));
            var request = new SubmissionSaveRequest
            {
                SubmissionId = existing == null ? 0 : existing.SubmissionId,
                AssignmentId = assignmentId,
                StudentId = account.StudentId,
                SubmittedAt = DateTime.Now,
                FileUrl = fileUrl,
                MarksObtained = existing == null ? null : existing.MarksObtained,
                Status = status
            };

            if (existing == null)
            {
                return SubmissionService.Add(user, request) > 0;
            }
            return SubmissionService.Edit(user, request);
        }
    }
}
