using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using src.db;

namespace src.services
{
    public class Submission
    {
        public int SubmissionId { get; set; }
        public int AssignmentId { get; set; }
        public string StudentId { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string FileUrl { get; set; }
        public decimal? MarksObtained { get; set; }
        public string Status { get; set; }
    }

    public class SubmissionSaveRequest : Submission
    {
    }

    public static class SubmissionService
    {
        private const string SelectSubmissions =
            "SELECT DISTINCT sub.submission_id, sub.assignment_id, sub.student_id, sub.submitted_at, sub.file_url, sub.marks_obtained, sub.status " +
            "FROM SUBMISSIONS sub " +
            "JOIN ASSIGNMENTS a ON a.assignment_id = sub.assignment_id " +
            "JOIN COURSE_OFFERINGS co ON co.offer_id = a.offer_id " +
            "WHERE @role = 'ADMIN' " +
            "OR (@role = 'STUDENT' AND EXISTS (SELECT 1 FROM STUDENTS s WHERE s.user_id = @userId AND s.student_id = sub.student_id)) " +
            "OR (@role = 'LECTURER' AND EXISTS (SELECT 1 FROM LECTURERS l WHERE l.user_id = @userId AND l.lecturer_id = co.lecturer_id)) " +
            "ORDER BY sub.submitted_at DESC, sub.submission_id DESC";

        public static List<Submission> GetData(UserContext user)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectSubmissions, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                var list = new List<Submission>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) list.Add(MapSubmission(reader));
                }
                return list;
            }
        }

        public static int Add(UserContext user, SubmissionSaveRequest request)
        {
            if (request == null) return 0;

            const string sql =
                "INSERT INTO SUBMISSIONS (assignment_id, student_id, submitted_at, file_url, marks_obtained, status) " +
                "VALUES (@assignmentId, @studentId, @submittedAt, @fileUrl, @marksObtained, @status); " +
                "SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (!ServiceAccess.CanAddSubmission(conn, user, request.StudentId, request.AssignmentId)) return 0;
                AddSubmissionParameters(cmd, request);
                return (int)cmd.ExecuteScalar();
            }
        }

        public static bool Edit(UserContext user, SubmissionSaveRequest request)
        {
            if (request == null) return false;

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanManageSubmission(conn, user, request.SubmissionId)) return false;

                var sql = user != null && user.IsAdmin
                    ? "UPDATE SUBMISSIONS SET assignment_id = @assignmentId, student_id = @studentId, submitted_at = @submittedAt, file_url = @fileUrl, marks_obtained = @marksObtained, status = @status WHERE submission_id = @submissionId"
                    : user != null && user.IsLecturer
                        ? "UPDATE SUBMISSIONS SET marks_obtained = @marksObtained, status = @status WHERE submission_id = @submissionId"
                        : "UPDATE SUBMISSIONS SET submitted_at = @submittedAt, file_url = @fileUrl, status = @status WHERE submission_id = @submissionId";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    AddSubmissionParameters(cmd, request);
                    cmd.Parameters.AddWithValue("@submissionId", request.SubmissionId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static bool Delete(UserContext user, int submissionId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand("DELETE FROM SUBMISSIONS WHERE submission_id = @submissionId", conn))
            {
                if (!ServiceAccess.CanManageSubmission(conn, user, submissionId)) return false;
                cmd.Parameters.AddWithValue("@submissionId", submissionId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private static Submission MapSubmission(SqlDataReader reader)
        {
            return new Submission
            {
                SubmissionId = (int)reader["submission_id"],
                AssignmentId = (int)reader["assignment_id"],
                StudentId = reader["student_id"].ToString(),
                SubmittedAt = reader["submitted_at"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["submitted_at"],
                FileUrl = reader["file_url"] == DBNull.Value ? "" : reader["file_url"].ToString(),
                MarksObtained = reader["marks_obtained"] == DBNull.Value ? (decimal?)null : (decimal)reader["marks_obtained"],
                Status = reader["status"] == DBNull.Value ? "" : reader["status"].ToString()
            };
        }

        private static void AddSubmissionParameters(SqlCommand cmd, SubmissionSaveRequest request)
        {
            cmd.Parameters.AddWithValue("@assignmentId", request.AssignmentId);
            cmd.Parameters.AddWithValue("@studentId", ServiceAccess.DbValue(request.StudentId));
            cmd.Parameters.AddWithValue("@submittedAt", ServiceAccess.DbValue(request.SubmittedAt));
            cmd.Parameters.AddWithValue("@fileUrl", ServiceAccess.DbValue(request.FileUrl));
            cmd.Parameters.AddWithValue("@marksObtained", ServiceAccess.DbValue(request.MarksObtained));
            cmd.Parameters.AddWithValue("@status", ServiceAccess.DbValue(request.Status));
        }
    }
}
