using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class LecturerAtRiskReader
    {
        private const decimal AttendanceThreshold = 80m;
        private const decimal AcademicThreshold = 50m;

        public static List<AtRiskStudentRow> GetAtRisk(UserContext user)
        {
            var result = new List<AtRiskStudentRow>();
            if (user == null || !user.IsLecturer) return result;

            // (offer_id, student_id) pairs the lecturer teaches, with course code.
            string pairSql =
                "SELECT DISTINCT e.offer_id, s.student_id, s.student_name, c.course_code, c.course_name " +
                "FROM ENROLLMENTS e " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = e.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "JOIN STUDENTS s ON s.student_id = e.student_id " +
                "WHERE e.status = 'ENROLLED' AND " + ServiceAccess.VisibleOfferScope("co");

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(pairSql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                var pairs = new List<Tuple<int, string, string, string, string>>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pairs.Add(Tuple.Create(
                            IntValue(reader["offer_id"]),
                            Text(reader["student_id"]),
                            Text(reader["student_name"]),
                            Text(reader["course_code"]),
                            Text(reader["course_name"])));
                    }
                }

                foreach (var p in pairs)
                {
                    var attendance = AttendanceRate(conn, p.Item1, p.Item2);
                    var academic = AcademicAverage(conn, p.Item1, p.Item2);
                    bool attRisk = attendance.HasValue && attendance.Value < AttendanceThreshold;
                    bool acadRisk = academic.HasValue && academic.Value < AcademicThreshold;
                    if (!attRisk && !acadRisk) continue;

                    result.Add(new AtRiskStudentRow
                    {
                        StudentId = p.Item2,
                        FullName = p.Item3,
                        CourseCode = p.Item4,
                        CourseName = p.Item5,
                        AttendanceRate = attendance,
                        AverageGrade = academic,
                        AttendanceRisk = attRisk,
                        AcademicRisk = acadRisk,
                        RiskLevel = (attRisk && acadRisk) ? "High" : "Medium"
                    });
                }
            }

            return result.OrderByDescending(r => r.RiskLevel == "High").ThenBy(r => r.FullName).ToList();
        }

        public static List<LecturerAcademicPerformanceRow> GetAcademicPerformance(UserContext user)
        {
            var result = new List<LecturerAcademicPerformanceRow>();
            if (user == null || !user.IsLecturer) return result;

            string selectSql =
                "SELECT DISTINCT e.offer_id, co.academic_year, co.semester AS offering_semester, s.student_id, s.student_name, p.programme_code, " +
                "ISNULL(s.semester, 0) AS semester_no, c.course_code, c.course_name, " +
                "ISNULL(g.grade_point, 0) AS grade_point, ISNULL(g.letter_grade, 'N/A') AS letter_grade " +
                "FROM ENROLLMENTS e " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = e.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "JOIN STUDENTS s ON s.student_id = e.student_id " +
                "JOIN PROGRAMMES p ON p.programme_id = s.programme_id " +
                "LEFT JOIN GRADES g ON g.offer_id = e.offer_id AND g.student_id = e.student_id " +
                "WHERE e.status = 'ENROLLED' AND " + ServiceAccess.VisibleOfferScope("co") + " " +
                "ORDER BY c.course_code, s.student_name";

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(selectSql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                var pairs = new List<AcademicPerformancePair>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pairs.Add(new AcademicPerformancePair
                        {
                            OfferId = IntValue(reader["offer_id"]),
                            AcademicYear = Text(reader["academic_year"]),
                            OfferingSemester = Text(reader["offering_semester"]),
                            StudentId = Text(reader["student_id"]),
                            FullName = Text(reader["student_name"]),
                            ProgrammeCode = Text(reader["programme_code"]),
                            Semester = IntValue(reader["semester_no"]),
                            CourseCode = Text(reader["course_code"]),
                            CourseName = Text(reader["course_name"]),
                            GradePoint = DecimalValue(reader["grade_point"]) ?? 0m,
                            LetterGrade = Text(reader["letter_grade"])
                        });
                    }
                }

                foreach (var pair in pairs)
                {
                    var attendance = AttendanceRate(conn, pair.OfferId, pair.StudentId);
                    var marks = AcademicAverage(conn, pair.OfferId, pair.StudentId);
                    result.Add(new LecturerAcademicPerformanceRow
                    {
                        OfferingId = pair.OfferId,
                        StudentId = pair.StudentId,
                        AcademicYear = pair.AcademicYear,
                        OfferingSemester = pair.OfferingSemester,
                        FullName = pair.FullName,
                        ProgrammeCode = pair.ProgrammeCode,
                        Semester = pair.Semester,
                        CourseCode = pair.CourseCode,
                        CourseName = pair.CourseName,
                        GradePoint = pair.GradePoint,
                        LetterGrade = pair.LetterGrade,
                        AttendanceRate = attendance,
                        AverageMarks = marks,
                        AttendanceRisk = attendance.HasValue && attendance.Value < AttendanceThreshold,
                        AcademicRisk = marks.HasValue && marks.Value < AcademicThreshold
                    });
                }
            }

            return result;
        }

        private class AcademicPerformancePair
        {
            public int OfferId { get; set; }
            public string AcademicYear { get; set; }
            public string OfferingSemester { get; set; }
            public string StudentId { get; set; }
            public string FullName { get; set; }
            public string ProgrammeCode { get; set; }
            public int Semester { get; set; }
            public string CourseCode { get; set; }
            public string CourseName { get; set; }
            public decimal GradePoint { get; set; }
            public string LetterGrade { get; set; }
        }

        private static decimal? AttendanceRate(SqlConnection conn, int offerId, string studentId)
        {
            const string sql =
                "SELECT ar.status FROM ATTENDANCE_SESSIONS ats " +
                "JOIN ATTENDANCE_RECORDS ar ON ar.session_id = ats.session_id " +
                "WHERE ats.offer_id = @offerId AND ar.student_id = @studentId";
            int total = 0, present = 0;
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@offerId", offerId);
                cmd.Parameters.AddWithValue("@studentId", studentId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        total++;
                        if (IsAttendedStatus(Text(reader["status"]))) present++;
                    }
                }
            }
            return total == 0 ? (decimal?)null : Math.Round((decimal)present / total * 100m, 1);
        }

        private static decimal? AcademicAverage(SqlConnection conn, int offerId, string studentId)
        {
            const string sql =
                "SELECT a.total_marks, sub.marks_obtained " +
                "FROM ASSIGNMENTS a " +
                "JOIN SUBMISSIONS sub ON sub.assignment_id = a.assignment_id AND sub.student_id = @studentId " +
                "WHERE a.offer_id = @offerId AND sub.marks_obtained IS NOT NULL";
            int n = 0; decimal sum = 0m;
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@offerId", offerId);
                cmd.Parameters.AddWithValue("@studentId", studentId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var m = DecimalValue(reader["marks_obtained"]);
                        if (!m.HasValue) continue;
                        var max = IntValue(reader["total_marks"]);
                        if (max <= 0) max = 100;
                        sum += Math.Round(m.Value / max * 100m, 2);
                        n++;
                    }
                }
            }
            return n == 0 ? (decimal?)null : Math.Round(sum / n, 1);
        }
    }
}
