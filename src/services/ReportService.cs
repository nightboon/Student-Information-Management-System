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
}