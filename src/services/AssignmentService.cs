using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using src.db;

namespace src.services
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public int OfferId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public int? TotalMarks { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class AssignmentSaveRequest
    {
        public int AssignmentId { get; set; }
        public int OfferId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public int? TotalMarks { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public static class AssignmentService
    {
        private const string SelectAssignments =
            "SELECT DISTINCT a.assignment_id, a.offer_id, a.title, CAST(a.description AS varchar(max)) AS description, a.file_path, a.total_marks, a.due_date " +
            "FROM ASSIGNMENTS a " +
            "JOIN COURSE_OFFERINGS co ON co.offer_id = a.offer_id " +
            "WHERE @role = 'ADMIN' " +
            "OR (@role = 'LECTURER' AND EXISTS (SELECT 1 FROM LECTURERS l WHERE l.user_id = @userId AND l.lecturer_id = co.lecturer_id)) " +
            "OR (@role = 'STUDENT' AND EXISTS (SELECT 1 FROM STUDENTS s JOIN ENROLLMENTS e ON e.student_id = s.student_id WHERE s.user_id = @userId AND e.offer_id = a.offer_id AND e.status = 'ENROLLED')) " +
            "ORDER BY a.due_date DESC, a.assignment_id DESC";

        public static List<Assignment> GetData(UserContext user)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectAssignments, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                var list = new List<Assignment>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) list.Add(MapAssignment(reader));
                }
                return list;
            }
        }

        public static int Add(UserContext user, AssignmentSaveRequest request)
        {
            if (request == null) return 0;

            const string sql =
                "INSERT INTO ASSIGNMENTS (offer_id, title, description, file_path, total_marks, due_date) " +
                "VALUES (@offerId, @title, @description, @filePath, @totalMarks, @dueDate); " +
                "SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (!ServiceAccess.CanManageOffer(conn, user, request.OfferId)) return 0;
                AddAssignmentParameters(cmd, request);
                return (int)cmd.ExecuteScalar();
            }
        }

        public static bool Edit(UserContext user, AssignmentSaveRequest request)
        {
            if (request == null) return false;

            const string sql =
                "UPDATE ASSIGNMENTS SET offer_id = @offerId, title = @title, description = @description, " +
                "file_path = @filePath, total_marks = @totalMarks, due_date = @dueDate WHERE assignment_id = @assignmentId";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (!ServiceAccess.CanManageAssignment(conn, user, request.AssignmentId)) return false;
                if (!ServiceAccess.CanManageOffer(conn, user, request.OfferId)) return false;
                cmd.Parameters.AddWithValue("@assignmentId", request.AssignmentId);
                AddAssignmentParameters(cmd, request);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool Delete(UserContext user, int assignmentId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand("DELETE FROM ASSIGNMENTS WHERE assignment_id = @assignmentId", conn))
            {
                if (!ServiceAccess.CanManageAssignment(conn, user, assignmentId)) return false;
                cmd.Parameters.AddWithValue("@assignmentId", assignmentId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private static Assignment MapAssignment(SqlDataReader reader)
        {
            return new Assignment
            {
                AssignmentId = (int)reader["assignment_id"],
                OfferId = (int)reader["offer_id"],
                Title = reader["title"] == DBNull.Value ? "" : reader["title"].ToString(),
                Description = reader["description"] == DBNull.Value ? "" : reader["description"].ToString(),
                FilePath = reader["file_path"] == DBNull.Value ? "" : reader["file_path"].ToString(),
                TotalMarks = reader["total_marks"] == DBNull.Value ? (int?)null : (int)reader["total_marks"],
                DueDate = reader["due_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["due_date"]
            };
        }

        private static void AddAssignmentParameters(SqlCommand cmd, AssignmentSaveRequest request)
        {
            cmd.Parameters.AddWithValue("@offerId", request.OfferId);
            cmd.Parameters.AddWithValue("@title", ServiceAccess.DbValue(request.Title));
            cmd.Parameters.AddWithValue("@description", ServiceAccess.DbValue(request.Description));
            cmd.Parameters.AddWithValue("@filePath", ServiceAccess.DbValue(request.FilePath));
            cmd.Parameters.AddWithValue("@totalMarks", ServiceAccess.DbValue(request.TotalMarks));
            cmd.Parameters.AddWithValue("@dueDate", ServiceAccess.DbValue(request.DueDate));
        }
    }
}
