using System;
using System.Data.SqlClient;
using src.db;

namespace src.services
{
    /// <summary>
    /// Header fields and overview metrics for one course offering, as shown on the
    /// lecturer course dashboard. Counts and averages are scoped to the offering.
    /// </summary>
    public class CourseDashboardStats
    {
        public int OfferingId { get; set; }
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string LevelLabel { get; set; }
        public int CreditHours { get; set; }
        public string Color { get; set; }
        public string SemesterName { get; set; }

        public int EnrolledCount { get; set; }
        public int AssessmentCount { get; set; }
        public int MaterialCount { get; set; }
        public int PendingGrading { get; set; }

        /// <summary>Class average over published grades (0-100); null when none are published.</summary>
        public decimal? AverageGrade { get; set; }

        public int AttendancePresent { get; set; }
        public int AttendanceTotal { get; set; }

        /// <summary>Fraction (0..1) of attendance records marked PRESENT; null when none recorded.</summary>
        public decimal? AttendanceRate
        {
            get
            {
                return AttendanceTotal > 0
                    ? (decimal?)Math.Round((decimal)AttendancePresent / AttendanceTotal, 4)
                    : null;
            }
        }
    }

    /// <summary>
    /// Read-only overview metrics for one of a lecturer's course offerings, for the
    /// course dashboard. The single query is guarded by an EXISTS check on TEACHINGS,
    /// so a lecturer can only read stats for a class they actually teach. SQL
    /// exceptions are not caught here; they propagate to the caller.
    /// </summary>
    public static class LecturerCourseDashboardService
    {
        // Course header plus offering-scoped overview metrics in one round trip.
        // Every metric is a correlated subquery on the offering; the EXISTS guard at
        // the end is the authorisation check (returns no row when the lecturer does
        // not teach the offering).
        private const string SelectStats =
            "SELECT c.course_id, c.course_code, c.course_name, " +
            "ISNULL(c.description, '') AS description, " +
            "ISNULL(c.level_label, '') AS level_label, " +
            "c.credit_hours, ISNULL(c.color, '') AS color, " +
            "sem.name AS semester_name, " +
            "(SELECT COUNT(*) FROM ENROLMENTS e WHERE e.offering_id = o.offering_id " +
            "AND e.status = 'ENROLLED') AS enrolled_count, " +
            "(SELECT COUNT(*) FROM ASSESSMENTS a WHERE a.offering_id = o.offering_id) AS assessment_count, " +
            "(SELECT COUNT(*) FROM COURSE_MATERIALS cm " +
            "JOIN MODULES m ON m.module_id = cm.module_id " +
            "WHERE m.offering_id = o.offering_id) AS material_count, " +
            "(SELECT COUNT(*) FROM SUBMISSIONS su " +
            "JOIN ASSIGNMENTS asg ON su.assignment_id = asg.assignment_id " +
            "WHERE asg.offering_id = o.offering_id AND su.marks IS NULL) AS pending_grading, " +
            "(SELECT AVG(CAST(g.marks AS decimal(9,2))) FROM GRADES g " +
            "JOIN ENROLMENTS e2 ON e2.enrolment_id = g.enrolment_id " +
            "WHERE e2.offering_id = o.offering_id AND g.published = 1) AS avg_grade, " +
            "(SELECT COUNT(*) FROM ATTENDANCE a " +
            "JOIN ENROLMENTS e3 ON e3.enrolment_id = a.enrolment_id " +
            "WHERE e3.offering_id = o.offering_id) AS attendance_total, " +
            "(SELECT SUM(CASE WHEN a.status = 'PRESENT' THEN 1 ELSE 0 END) FROM ATTENDANCE a " +
            "JOIN ENROLMENTS e3 ON e3.enrolment_id = a.enrolment_id " +
            "WHERE e3.offering_id = o.offering_id) AS present_count " +
            "FROM COURSE_OFFERINGS o " +
            "JOIN COURSES c ON o.course_id = c.course_id " +
            "JOIN SEMESTERS sem ON o.semester_id = sem.semester_id " +
            "WHERE o.offering_id = @offeringId " +
            "AND EXISTS (SELECT 1 FROM TEACHINGS t " +
            "WHERE t.offering_id = o.offering_id AND t.lecturer_id = @lecturerId)";

        /// <summary>
        /// Dashboard stats for the offering, or null when the offering does not exist
        /// or the lecturer does not teach it (callers redirect).
        /// </summary>
        public static CourseDashboardStats GetStats(int offeringId, int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectStats, conn))
            {
                cmd.Parameters.AddWithValue("@offeringId", offeringId);
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new CourseDashboardStats
                    {
                        OfferingId = offeringId,
                        CourseId = (int)reader["course_id"],
                        CourseCode = reader["course_code"].ToString(),
                        CourseName = reader["course_name"].ToString(),
                        Description = reader["description"].ToString(),
                        LevelLabel = reader["level_label"].ToString(),
                        CreditHours = (int)reader["credit_hours"],
                        Color = reader["color"].ToString(),
                        SemesterName = reader["semester_name"].ToString(),
                        EnrolledCount = Convert.ToInt32(reader["enrolled_count"]),
                        AssessmentCount = Convert.ToInt32(reader["assessment_count"]),
                        MaterialCount = Convert.ToInt32(reader["material_count"]),
                        PendingGrading = Convert.ToInt32(reader["pending_grading"]),
                        AverageGrade = reader["avg_grade"] == DBNull.Value
                            ? (decimal?)null
                            : Math.Round(Convert.ToDecimal(reader["avg_grade"]), 1),
                        AttendanceTotal = Convert.ToInt32(reader["attendance_total"]),
                        AttendancePresent = reader["present_count"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(reader["present_count"])
                    };
                }
            }
        }
    }
}
