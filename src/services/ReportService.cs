using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace src.services
{
    public class ReportService
    {
        private readonly string connStr =
            ConfigurationManager.ConnectionStrings["SimsDb"].ConnectionString;

        public List<SemesterOption> GetSemesters()
        {
            var list = new List<SemesterOption>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT session_id AS semester_id,
                           academic_year + ' ' + semester AS name
                    FROM ACADEMIC_SESSIONS
                    ORDER BY start_date DESC, session_id DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SemesterOption
                        {
                            SemesterId = reader["semester_id"].ToString(),
                            Name = reader["name"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public List<ProgrammeOption> GetProgrammes()
        {
            var list = new List<ProgrammeOption>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT programme_id, programme_code, programme_name
                    FROM PROGRAMMES
                    ORDER BY programme_code";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProgrammeOption
                        {
                            ProgrammeId = reader["programme_id"].ToString(),
                            ProgrammeCode = reader["programme_code"].ToString(),
                            ProgrammeName = reader["programme_name"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public List<StudentAcademicReportRow> GetStudentAcademicReport(
            string semesterId,
            string programmeId,
            string status,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var list = new List<StudentAcademicReportRow>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    WITH report_data AS
                    (
                        SELECT
                            s.student_id,
                            s.student_id AS StudentNo,
                            s.student_name AS StudentName,
                            p.programme_id,
                            p.programme_code AS Programme,
                            sem.session_id,
                            sem.academic_year + ' ' + sem.semester AS SemesterName,

                            CAST(
                                CASE
                                    WHEN SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN c.credit_hour ELSE 0 END) = 0
                                    THEN NULL
                                    ELSE
                                        SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN g.grade_point * c.credit_hour ELSE 0 END)
                                        /
                                        SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN c.credit_hour ELSE 0 END)
                                END
                            AS DECIMAL(4,2)) AS CGPA
                        FROM STUDENTS s
                        INNER JOIN PROGRAMMES p
                            ON s.programme_id = p.programme_id
                        INNER JOIN ENROLLMENTS e
                            ON s.student_id = e.student_id
                        INNER JOIN COURSE_OFFERINGS co
                            ON e.offer_id = co.offer_id
                        INNER JOIN COURSES c
                            ON co.course_id = c.course_id
                        INNER JOIN ACADEMIC_SESSIONS sem
                            ON co.academic_year = sem.academic_year AND co.semester = sem.semester
                        LEFT JOIN GRADES g
                            ON g.student_id = e.student_id AND g.offer_id = e.offer_id
                        WHERE e.status = 'ENROLLED'
                          AND (@SemesterId IS NULL OR sem.session_id = @SemesterId)
                          AND (@ProgrammeId IS NULL OR p.programme_id = @ProgrammeId)
                          AND (@DateFrom IS NULL OR sem.end_date >= @DateFrom)
                          AND (@DateTo IS NULL OR sem.start_date <= @DateTo)
                        GROUP BY
                            s.student_id,
                            s.student_name,
                            p.programme_id,
                            p.programme_code,
                            sem.session_id,
                            sem.academic_year + ' ' + sem.semester
                    ),
                    final_report AS
                    (
                        SELECT
                            StudentNo,
                            StudentName,
                            Programme,
                            SemesterName,
                            CGPA,
                            CASE
                                WHEN CGPA IS NULL THEN 'Pending'
                                WHEN CGPA >= 2.00 THEN 'Good Standing'
                                WHEN CGPA >= 1.50 THEN 'Probation'
                                ELSE 'At Risk'
                            END AS AcademicStatus
                        FROM report_data
                    )
                    SELECT *
                    FROM final_report
                    WHERE (@Status = '' OR AcademicStatus = @Status)
                    ORDER BY StudentName";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SemesterId", (object)semesterId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProgrammeId", (object)programmeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", status ?? "");
                    cmd.Parameters.AddWithValue("@DateFrom", (object)dateFrom ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", (object)dateTo ?? DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new StudentAcademicReportRow
                            {
                                StudentNo = reader["StudentNo"].ToString(),
                                StudentName = reader["StudentName"].ToString(),
                                Programme = reader["Programme"].ToString(),
                                SemesterName = reader["SemesterName"].ToString(),
                                CGPA = reader["CGPA"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["CGPA"]),
                                Status = reader["AcademicStatus"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }

        public List<ProgrammePerformanceReportRow> GetProgrammePerformanceReport(
            string semesterId,
            string programmeId,
            string status,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var list = new List<ProgrammePerformanceReportRow>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    WITH base_data AS
                    (
                        SELECT
                            p.programme_id,
                            p.programme_code,
                            p.programme_name,
                            s.student_id,
                            e.enrollment_id,
                            ISNULL(c.credit_hour, 0) AS credit_hours,
                            CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN 1 ELSE 0 END AS has_grade,
                            CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' AND g.letter_grade <> 'F' THEN 1 ELSE 0 END AS is_pass,
                            CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' AND g.letter_grade = 'F' THEN 1 ELSE 0 END AS is_fail,
                            g.grade_point AS gpa,
                            sem.session_id,
                            sem.start_date,
                            sem.end_date
                        FROM PROGRAMMES p
                        LEFT JOIN STUDENTS s
                            ON s.programme_id = p.programme_id
                        LEFT JOIN ENROLLMENTS e
                            ON e.student_id = s.student_id
                           AND e.status = 'ENROLLED'
                        LEFT JOIN COURSE_OFFERINGS co
                            ON co.offer_id = e.offer_id
                        LEFT JOIN COURSES c
                            ON c.course_id = co.course_id
                        LEFT JOIN ACADEMIC_SESSIONS sem
                            ON sem.academic_year = co.academic_year AND sem.semester = co.semester
                        LEFT JOIN GRADES g
                            ON g.student_id = e.student_id AND g.offer_id = e.offer_id
                        WHERE (@SemesterId IS NULL OR sem.session_id = @SemesterId)
                          AND (@ProgrammeId IS NULL OR p.programme_id = @ProgrammeId)
                          AND (@DateFrom IS NULL OR sem.end_date IS NULL OR sem.end_date >= @DateFrom)
                          AND (@DateTo IS NULL OR sem.start_date IS NULL OR sem.start_date <= @DateTo)
                    ),
                    student_summary AS
                    (
                        SELECT
                            programme_id,
                            student_id,
                            CAST(
                                CASE
                                    WHEN SUM(CASE WHEN has_grade = 1 THEN credit_hours ELSE 0 END) = 0 THEN NULL
                                    ELSE SUM(CASE WHEN has_grade = 1 THEN gpa * credit_hours ELSE 0 END)
                                       / SUM(CASE WHEN has_grade = 1 THEN credit_hours ELSE 0 END)
                                END
                            AS DECIMAL(4,2)) AS student_gpa
                        FROM base_data
                        WHERE student_id IS NOT NULL
                        GROUP BY programme_id, student_id
                    ),
                    programme_summary AS
                    (
                        SELECT
                            programme_id,
                            programme_code,
                            programme_name,
                            COUNT(DISTINCT student_id) AS students,
                            COUNT(enrollment_id) AS enrolments,
                            SUM(has_grade) AS graded_count,
                            SUM(is_pass) AS pass_count,
                            SUM(is_fail) AS fail_count,
                            CAST(
                                CASE
                                    WHEN SUM(CASE WHEN has_grade = 1 THEN credit_hours ELSE 0 END) = 0 THEN NULL
                                    ELSE SUM(CASE WHEN has_grade = 1 THEN gpa * credit_hours ELSE 0 END)
                                       / SUM(CASE WHEN has_grade = 1 THEN credit_hours ELSE 0 END)
                                END
                            AS DECIMAL(4,2)) AS avg_gpa,
                            CAST(
                                CASE
                                    WHEN SUM(has_grade) = 0 THEN NULL
                                    ELSE SUM(is_pass) * 100.0 / SUM(has_grade)
                                END
                            AS DECIMAL(5,1)) AS pass_rate,
                            CAST(
                                CASE
                                    WHEN SUM(has_grade) = 0 THEN NULL
                                    ELSE SUM(is_fail) * 100.0 / SUM(has_grade)
                                END
                            AS DECIMAL(5,1)) AS fail_rate,
                            CAST(
                                CASE
                                    WHEN COUNT(enrollment_id) = 0 THEN NULL
                                    ELSE SUM(has_grade) * 100.0 / COUNT(enrollment_id)
                                END
                            AS DECIMAL(5,1)) AS completion_rate
                        FROM base_data
                        GROUP BY programme_id, programme_code, programme_name
                    ),
                    risk_summary AS
                    (
                        SELECT
                            programme_id,
                            COUNT(CASE WHEN student_gpa < 2.00 THEN 1 END) AS at_risk
                        FROM student_summary
                        GROUP BY programme_id
                    ),
                    cgpa_summary AS
                    (
                        SELECT
                            programme_id,
                            CAST(AVG(student_gpa) AS DECIMAL(4,2)) AS avg_cgpa
                        FROM student_summary
                        WHERE student_gpa IS NOT NULL
                        GROUP BY programme_id
                    ),
                    final_report AS
                    (
                        SELECT
                            ps.programme_code AS ProgrammeCode,
                            ps.programme_name AS ProgrammeName,
                            ps.students AS Students,
                            ps.avg_gpa AS AvgGpa,
                            cs.avg_cgpa AS AvgCgpa,
                            ps.pass_rate AS PassRate,
                            ps.fail_rate AS FailRate,
                            ps.completion_rate AS CompletionRate,
                            CASE
                                WHEN ISNULL(ps.pass_rate, 0) < 80 OR ISNULL(rs.at_risk, 0) >= 10 THEN 'At Risk'
                                WHEN ISNULL(ps.pass_rate, 0) < 90 OR ISNULL(rs.at_risk, 0) > 0 THEN 'Watch'
                                ELSE 'Healthy'
                            END AS ProgrammeStatus
                        FROM programme_summary ps
                        LEFT JOIN risk_summary rs ON rs.programme_id = ps.programme_id
                        LEFT JOIN cgpa_summary cs ON cs.programme_id = ps.programme_id
                    )
                    SELECT *
                    FROM final_report
                    WHERE (@Status = ''
                        OR @Status NOT IN ('Healthy', 'Watch', 'At Risk')
                        OR ProgrammeStatus = @Status)
                    ORDER BY ProgrammeCode";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SemesterId", (object)semesterId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProgrammeId", (object)programmeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", status ?? "");
                    cmd.Parameters.AddWithValue("@DateFrom", (object)dateFrom ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", (object)dateTo ?? DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ProgrammePerformanceReportRow
                            {
                                ProgrammeCode = Text(reader["ProgrammeCode"]),
                                ProgrammeName = Text(reader["ProgrammeName"]),
                                Students = IntValue(reader["Students"]),
                                AvgGpa = DecimalValue(reader["AvgGpa"]),
                                AvgCgpa = DecimalValue(reader["AvgCgpa"]),
                                PassRate = DecimalValue(reader["PassRate"]),
                                FailRate = DecimalValue(reader["FailRate"]),
                                CompletionRate = DecimalValue(reader["CompletionRate"]),
                                Status = Text(reader["ProgrammeStatus"])
                            });
                        }
                    }
                }
            }

            return list;
        }

        private static string Text(object value)
        {
            return value == DBNull.Value || value == null ? "" : value.ToString();
        }

        private static int IntValue(object value)
        {
            return value == DBNull.Value || value == null ? 0 : Convert.ToInt32(value);
        }

        private static decimal? DecimalValue(object value)
        {
            return value == DBNull.Value || value == null ? (decimal?)null : Convert.ToDecimal(value);
        }
    }

    public class StudentAcademicReportRow
    {
        public string StudentNo { get; set; }
        public string StudentName { get; set; }
        public string Programme { get; set; }
        public string SemesterName { get; set; }
        public decimal? CGPA { get; set; }
        public string Status { get; set; }

        public string CGPADisplay
        {
            get { return CGPA.HasValue ? CGPA.Value.ToString("0.00") : "-"; }
        }
    }

    public class SemesterOption
    {
        public string SemesterId { get; set; }
        public string Name { get; set; }
    }

    public class ProgrammeOption
    {
        public string ProgrammeId { get; set; }
        public string ProgrammeCode { get; set; }
        public string ProgrammeName { get; set; }

        public string DisplayName
        {
            get { return ProgrammeCode + " - " + ProgrammeName; }
        }
    }

    public class ProgrammePerformanceReportRow
    {
        public string ProgrammeCode { get; set; }
        public string ProgrammeName { get; set; }
        public int Students { get; set; }
        public decimal? AvgGpa { get; set; }
        public decimal? AvgCgpa { get; set; }
        public decimal? PassRate { get; set; }
        public decimal? FailRate { get; set; }
        public decimal? CompletionRate { get; set; }
        public string Status { get; set; }

        public string AvgGpaDisplay
        {
            get { return AvgGpa.HasValue ? AvgGpa.Value.ToString("0.00") : "-"; }
        }

        public string AvgCgpaDisplay
        {
            get { return AvgCgpa.HasValue ? AvgCgpa.Value.ToString("0.00") : "-"; }
        }

        public string PassRateDisplay
        {
            get { return PassRate.HasValue ? PassRate.Value.ToString("0.0") + "%" : "-"; }
        }

        public string FailRateDisplay
        {
            get { return FailRate.HasValue ? FailRate.Value.ToString("0.0") + "%" : "-"; }
        }

        public string CompletionRateDisplay
        {
            get { return CompletionRate.HasValue ? CompletionRate.Value.ToString("0.0") + "%" : "-"; }
        }

        public string StatusCssClass
        {
            get
            {
                if (string.Equals(Status, "Healthy", StringComparison.OrdinalIgnoreCase))
                {
                    return "inline-flex items-center rounded-full border px-2 py-0.5 bg-emerald-50 text-emerald-700 border-emerald-100";
                }

                if (string.Equals(Status, "Watch", StringComparison.OrdinalIgnoreCase))
                {
                    return "inline-flex items-center rounded-full border px-2 py-0.5 bg-amber-50 text-amber-700 border-amber-100";
                }

                return "inline-flex items-center rounded-full border px-2 py-0.5 bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20";
            }
        }
    }
}
