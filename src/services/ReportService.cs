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
                    SELECT semester_id, name
                    FROM SEMESTERS
                    ORDER BY start_date DESC, semester_id DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SemesterOption
                        {
                            SemesterId = Convert.ToInt32(reader["semester_id"]),
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
                            ProgrammeId = Convert.ToInt32(reader["programme_id"]),
                            ProgrammeCode = reader["programme_code"].ToString(),
                            ProgrammeName = reader["programme_name"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public List<StudentAcademicReportRow> GetStudentAcademicReport(
            int? semesterId,
            int? programmeId,
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
                            'S' + RIGHT('00000' + CAST(s.student_id AS VARCHAR(10)), 5) AS StudentNo,
                            s.full_name AS StudentName,
                            p.programme_id,
                            p.programme_code AS Programme,
                            sem.semester_id,
                            sem.name AS SemesterName,

                            CAST(
                                CASE 
                                    WHEN SUM(CASE WHEN g.published = 1 AND g.gpa IS NOT NULL THEN c.credit_hours ELSE 0 END) = 0 
                                    THEN NULL
                                    ELSE
                                        SUM(CASE WHEN g.published = 1 AND g.gpa IS NOT NULL THEN g.gpa * c.credit_hours ELSE 0 END)
                                        /
                                        SUM(CASE WHEN g.published = 1 AND g.gpa IS NOT NULL THEN c.credit_hours ELSE 0 END)
                                END
                            AS DECIMAL(4,2)) AS CGPA
                        FROM STUDENTS s
                        INNER JOIN PROGRAMMES p 
                            ON s.programme_id = p.programme_id
                        INNER JOIN ENROLMENTS e 
                            ON s.student_id = e.student_id
                        INNER JOIN COURSE_OFFERINGS co 
                            ON e.offering_id = co.offering_id
                        INNER JOIN COURSES c 
                            ON co.course_id = c.course_id
                        INNER JOIN SEMESTERS sem 
                            ON co.semester_id = sem.semester_id
                        LEFT JOIN GRADES g 
                            ON e.enrolment_id = g.enrolment_id
                        WHERE e.status = 'ENROLLED'
                          AND (@SemesterId IS NULL OR sem.semester_id = @SemesterId)
                          AND (@ProgrammeId IS NULL OR p.programme_id = @ProgrammeId)
                          AND (@DateFrom IS NULL OR sem.end_date >= @DateFrom)
                          AND (@DateTo IS NULL OR sem.start_date <= @DateTo)
                        GROUP BY
                            s.student_id,
                            s.full_name,
                            p.programme_id,
                            p.programme_code,
                            sem.semester_id,
                            sem.name
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
            int? semesterId,
            int? programmeId,
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
                            e.enrolment_id,
                            ISNULL(c.credit_hours, 0) AS credit_hours,
                            CASE WHEN g.published = 1 AND g.gpa IS NOT NULL THEN 1 ELSE 0 END AS has_grade,
                            CASE WHEN g.published = 1 AND g.gpa IS NOT NULL AND g.gpa >= 2.00 THEN 1 ELSE 0 END AS is_pass,
                            CASE WHEN g.published = 1 AND g.gpa IS NOT NULL AND g.gpa < 2.00 THEN 1 ELSE 0 END AS is_fail,
                            g.gpa,
                            sem.semester_id,
                            sem.start_date,
                            sem.end_date
                        FROM PROGRAMMES p
                        LEFT JOIN STUDENTS s
                            ON s.programme_id = p.programme_id
                        LEFT JOIN ENROLMENTS e
                            ON e.student_id = s.student_id
                           AND e.status = 'ENROLLED'
                        LEFT JOIN COURSE_OFFERINGS co
                            ON co.offering_id = e.offering_id
                        LEFT JOIN COURSES c
                            ON c.course_id = co.course_id
                        LEFT JOIN SEMESTERS sem
                            ON sem.semester_id = co.semester_id
                        LEFT JOIN GRADES g
                            ON g.enrolment_id = e.enrolment_id
                        WHERE (@SemesterId IS NULL OR sem.semester_id = @SemesterId)
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
                            COUNT(enrolment_id) AS enrolments,
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
                                    WHEN COUNT(enrolment_id) = 0 THEN NULL
                                    ELSE SUM(has_grade) * 100.0 / COUNT(enrolment_id)
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
        public int SemesterId { get; set; }
        public string Name { get; set; }
    }

    public class ProgrammeOption
    {
        public int ProgrammeId { get; set; }
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
