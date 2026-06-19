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
                    SELECT DISTINCT
                        academic_year + '|' + semester AS semester_id,
                        academic_year + ' ' + semester AS name
                    FROM COURSE_OFFERINGS
                    ORDER BY name DESC";

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
                            co.academic_year + '|' + co.semester AS semester_id,
                            co.academic_year + ' ' + co.semester AS SemesterName,
                            CAST(AVG(CAST(g.grade_point AS DECIMAL(5,2))) AS DECIMAL(4,2)) AS CGPA
                        FROM STUDENTS s
                        INNER JOIN PROGRAMMES p 
                            ON s.programme_id = p.programme_id
                        INNER JOIN ENROLLMENTS e 
                            ON s.student_id = e.student_id
                        INNER JOIN COURSE_OFFERINGS co 
                            ON e.offer_id = co.offer_id
                        INNER JOIN COURSES c 
                            ON co.course_id = c.course_id
                        LEFT JOIN GRADES g 
                            ON g.student_id = e.student_id
                           AND g.offer_id = e.offer_id
                        LEFT JOIN ACADEMIC_SESSIONS sem
                            ON sem.academic_year = co.academic_year
                           AND sem.semester = co.semester
                        WHERE e.status = 'ENROLLED'
                          AND (@SemesterId = '' OR co.academic_year + '|' + co.semester = @SemesterId)
                          AND (@ProgrammeId = '' OR p.programme_id = @ProgrammeId)
                          AND (@DateFrom IS NULL OR sem.end_date >= @DateFrom)
                          AND (@DateTo IS NULL OR sem.start_date <= @DateTo)
                        GROUP BY
                            s.student_id,
                            s.student_name,
                            p.programme_id,
                            p.programme_code,
                            co.academic_year,
                            co.semester
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
                    cmd.Parameters.AddWithValue("@SemesterId", semesterId ?? "");
                    cmd.Parameters.AddWithValue("@ProgrammeId", programmeId ?? "");
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
}
