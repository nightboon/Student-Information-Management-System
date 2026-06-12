using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using src.db;

namespace src.services
{
    /// <summary>
    /// One course offering the lecturer teaches, in any semester, with the
    /// course metadata and live enrolled-student count the My Courses card needs.
    /// </summary>
    public class LecturerCourse
    {
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int CreditHours { get; set; }
        public string Color { get; set; }
        public string SemesterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EnrolledCount { get; set; }

        /// <summary>
        /// Teaching status derived from where today sits relative to the
        /// semester window: "Completed" once it has ended, "Upcoming" before it
        /// starts, otherwise "In progress".
        /// </summary>
        public string Status
        {
            get
            {
                var today = DateTime.Today;
                if (today > EndDate) return "Completed";
                if (today < StartDate) return "Upcoming";
                return "In progress";
            }
        }
    }

    public class EnrolledStudentRow
    {
        public string FullName { get; set; }
        public string StudentNo { get; set; }
        public string Email { get; set; }
        public string ProgrammeCode { get; set; }
        public string ProgrammeName { get; set; }
        public string Phone { get; set; }
    }

    /// <summary>
    /// Read-only access to the courses a lecturer teaches. Returns an empty list
    /// when the lecturer teaches nothing. SQL exceptions are not caught here;
    /// they propagate to the caller.
    /// </summary>
    public static class LecturerCourseService
    {
        // Every offering taught by the lecturer (resolved from the signed-in
        // user id), across all semesters, one row each. enrolled_count is the
        // live count of ENROLLED students for the offering.
        private const string SelectCourses =
            "SELECT o.offering_id, c.course_code, c.course_name, " +
            "ISNULL(c.description, '') AS description, c.credit_hours, " +
            "ISNULL(c.color, '') AS color, " +
            "sem.name AS semester_name, sem.start_date, sem.end_date, " +
            "(SELECT COUNT(*) FROM ENROLMENTS en WHERE en.offering_id = o.offering_id " +
            "AND en.status = 'ENROLLED') AS enrolled_count " +
            "FROM LECTURERS l " +
            "JOIN TEACHINGS t ON t.lecturer_id = l.lecturer_id " +
            "JOIN COURSE_OFFERINGS o ON t.offering_id = o.offering_id " +
            "JOIN COURSES c ON o.course_id = c.course_id " +
            "JOIN SEMESTERS sem ON o.semester_id = sem.semester_id " +
            "WHERE l.user_id = @userId " +
            "ORDER BY sem.start_date DESC, c.course_code";

        public static List<LecturerCourse> GetCourses(int userId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectCourses, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                var courses = new List<LecturerCourse>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new LecturerCourse
                        {
                            OfferingId = (int)reader["offering_id"],
                            CourseCode = reader["course_code"].ToString(),
                            CourseName = reader["course_name"].ToString(),
                            Description = reader["description"].ToString(),
                            CreditHours = (int)reader["credit_hours"],
                            Color = reader["color"].ToString(),
                            SemesterName = reader["semester_name"].ToString(),
                            StartDate = (DateTime)reader["start_date"],
                            EndDate = (DateTime)reader["end_date"],
                            EnrolledCount = Convert.ToInt32(reader["enrolled_count"])
                        });
                    }
                }
                return courses;
            }
        }

        private const string SelectEnrolledStudents =
            "SELECT st.full_name, u.username, u.email, " +
            "p.programme_code, p.programme_name, ISNULL(u.phone, '') AS phone " +
            "FROM ENROLMENTS e " +
            "JOIN STUDENTS st ON st.student_id = e.student_id " +
            "JOIN USERS u ON u.user_id = st.user_id " +
            "JOIN PROGRAMMES p ON p.programme_id = st.programme_id " +
            "WHERE e.offering_id = @offeringId AND e.status = 'ENROLLED' " +
            "AND EXISTS (SELECT 1 FROM TEACHINGS t " +
            "WHERE t.offering_id = e.offering_id AND t.lecturer_id = @lecturerId) " +
            "ORDER BY st.full_name";

        public static List<EnrolledStudentRow> GetEnrolledStudents(int offeringId, int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectEnrolledStudents, conn))
            {
                cmd.Parameters.AddWithValue("@offeringId", offeringId);
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                var students = new List<EnrolledStudentRow>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new EnrolledStudentRow
                        {
                            FullName = reader["full_name"].ToString(),
                            StudentNo = reader["username"].ToString(),
                            Email = reader["email"].ToString(),
                            ProgrammeCode = reader["programme_code"].ToString(),
                            ProgrammeName = reader["programme_name"].ToString(),
                            Phone = reader["phone"].ToString()
                        });
                    }
                }
                return students;
            }
        }
    }
}
