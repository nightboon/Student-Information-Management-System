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
                                WHEN ps.students = 0 THEN 'N/A'
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

        public List<CoursePerformanceReportRow> GetCoursePerformanceReport(
            string semesterId,
            string programmeId,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var list = new List<CoursePerformanceReportRow>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT
                        c.course_code AS CourseCode,
                        c.course_name AS CourseName,
                        p.programme_code AS Programme,
                        COUNT(e.enrollment_id) AS Enrolled,
                        SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN 1 ELSE 0 END) AS Graded,
                        CAST(AVG(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN g.grade_point END) AS DECIMAL(4,2)) AS AvgGpa,
                        CAST(
                            CASE
                                WHEN SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN 1 ELSE 0 END) = 0 THEN NULL
                                ELSE SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' AND UPPER(g.letter_grade) <> 'F' THEN 1 ELSE 0 END) * 100.0
                                   / SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN 1 ELSE 0 END)
                            END
                        AS DECIMAL(5,1)) AS PassRate,
                        SUM(CASE WHEN UPPER(ISNULL(g.letter_grade, '')) LIKE 'A%' THEN 1 ELSE 0 END) AS GradeA,
                        SUM(CASE WHEN UPPER(ISNULL(g.letter_grade, '')) LIKE 'B%' THEN 1 ELSE 0 END) AS GradeB,
                        SUM(CASE WHEN UPPER(ISNULL(g.letter_grade, '')) LIKE 'C%' THEN 1 ELSE 0 END) AS GradeC,
                        SUM(CASE WHEN UPPER(ISNULL(g.letter_grade, '')) = 'D' THEN 1 ELSE 0 END) AS GradeD,
                        SUM(CASE WHEN UPPER(ISNULL(g.letter_grade, '')) = 'F' THEN 1 ELSE 0 END) AS GradeF
                    FROM COURSES c
                    INNER JOIN PROGRAMMES p
                        ON p.programme_id = c.programme_id
                    LEFT JOIN COURSE_OFFERINGS co
                        ON co.course_id = c.course_id
                    LEFT JOIN ACADEMIC_SESSIONS sem
                        ON sem.academic_year = co.academic_year AND sem.semester = co.semester
                    LEFT JOIN ENROLLMENTS e
                        ON e.offer_id = co.offer_id AND e.status = 'ENROLLED'
                    LEFT JOIN GRADES g
                        ON g.student_id = e.student_id AND g.offer_id = e.offer_id
                    WHERE (@SemesterId IS NULL OR sem.session_id = @SemesterId)
                      AND (@ProgrammeId IS NULL OR p.programme_id = @ProgrammeId)
                      AND (@DateFrom IS NULL OR sem.end_date >= @DateFrom)
                      AND (@DateTo IS NULL OR sem.start_date <= @DateTo)
                    GROUP BY c.course_id, c.course_code, c.course_name, p.programme_code
                    ORDER BY c.course_code";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SemesterId", (object)semesterId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProgrammeId", (object)programmeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateFrom", (object)dateFrom ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", (object)dateTo ?? DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CoursePerformanceReportRow
                            {
                                CourseCode = Text(reader["CourseCode"]),
                                CourseName = Text(reader["CourseName"]),
                                Programme = Text(reader["Programme"]),
                                Enrolled = IntValue(reader["Enrolled"]),
                                Graded = IntValue(reader["Graded"]),
                                AvgGpa = DecimalValue(reader["AvgGpa"]),
                                PassRate = DecimalValue(reader["PassRate"]),
                                GradeA = IntValue(reader["GradeA"]),
                                GradeB = IntValue(reader["GradeB"]),
                                GradeC = IntValue(reader["GradeC"]),
                                GradeD = IntValue(reader["GradeD"]),
                                GradeF = IntValue(reader["GradeF"])
                            });
                        }
                    }
                }
            }

            return list;
        }

        public List<AttendanceSummaryReportRow> GetAttendanceSummaryReport(
            string semesterId,
            string programmeId,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var list = new List<AttendanceSummaryReportRow>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT
                        p.programme_code AS Programme,
                        c.course_code AS CourseCode,
                        c.course_name AS CourseName,
                        sem.academic_year + ' ' + sem.semester AS SemesterName,
                        COUNT(DISTINCT e.student_id) AS EnrolledStudents,
                        COUNT(DISTINCT ats.session_id) AS ClassSessions,
                        SUM(CASE WHEN UPPER(ISNULL(ar.status, '')) = 'PRESENT' THEN 1 ELSE 0 END) AS PresentCount,
                        SUM(CASE WHEN UPPER(ISNULL(ar.status, '')) = 'LATE' THEN 1 ELSE 0 END) AS LateCount,
                        SUM(CASE WHEN UPPER(ISNULL(ar.status, '')) = 'ABSENT' THEN 1 ELSE 0 END) AS AbsentCount,
                        SUM(CASE WHEN NULLIF(LTRIM(RTRIM(ar.status)), '') IS NOT NULL THEN 1 ELSE 0 END) AS RecordedSessions,
                        CAST(
                            CASE
                                WHEN SUM(CASE WHEN NULLIF(LTRIM(RTRIM(ar.status)), '') IS NOT NULL THEN 1 ELSE 0 END) = 0 THEN NULL
                                ELSE SUM(CASE WHEN UPPER(ISNULL(ar.status, '')) = 'PRESENT' THEN 1 ELSE 0 END) * 100.0
                                   / SUM(CASE WHEN NULLIF(LTRIM(RTRIM(ar.status)), '') IS NOT NULL THEN 1 ELSE 0 END)
                            END
                        AS DECIMAL(5,1)) AS AttendancePercentage
                    FROM COURSE_OFFERINGS co
                    INNER JOIN COURSES c
                        ON c.course_id = co.course_id
                    INNER JOIN PROGRAMMES p
                        ON p.programme_id = c.programme_id
                    INNER JOIN ACADEMIC_SESSIONS sem
                        ON sem.academic_year = co.academic_year AND sem.semester = co.semester
                    LEFT JOIN ENROLLMENTS e
                        ON e.offer_id = co.offer_id AND e.status = 'ENROLLED'
                    LEFT JOIN ATTENDANCE_SESSIONS ats
                        ON ats.offer_id = co.offer_id
                       AND (@DateFrom IS NULL OR ats.session_date >= @DateFrom)
                       AND (@DateTo IS NULL OR ats.session_date <= @DateTo)
                    LEFT JOIN ATTENDANCE_RECORDS ar
                        ON ar.session_id = ats.session_id AND ar.student_id = e.student_id
                    WHERE (@SemesterId IS NULL OR sem.session_id = @SemesterId)
                      AND (@ProgrammeId IS NULL OR p.programme_id = @ProgrammeId)
                    GROUP BY
                        p.programme_code,
                        co.offer_id,
                        c.course_code,
                        c.course_name,
                        sem.academic_year,
                        sem.semester
                    ORDER BY c.course_code";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SemesterId", (object)semesterId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProgrammeId", (object)programmeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateFrom", (object)dateFrom ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", (object)dateTo ?? DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new AttendanceSummaryReportRow
                            {
                                Programme = Text(reader["Programme"]),
                                CourseCode = Text(reader["CourseCode"]),
                                CourseName = Text(reader["CourseName"]),
                                SemesterName = Text(reader["SemesterName"]),
                                EnrolledStudents = IntValue(reader["EnrolledStudents"]),
                                ClassSessions = IntValue(reader["ClassSessions"]),
                                PresentCount = IntValue(reader["PresentCount"]),
                                LateCount = IntValue(reader["LateCount"]),
                                AbsentCount = IntValue(reader["AbsentCount"]),
                                RecordedSessions = IntValue(reader["RecordedSessions"]),
                                AttendancePercentage = DecimalValue(reader["AttendancePercentage"])
                            });
                        }
                    }
                }
            }

            return list;
        }

        public List<AtRiskStudentReportRow> GetAtRiskStudentReport(
            string semesterId,
            string programmeId,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var list = new List<AtRiskStudentReportRow>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    WITH scoped_enrolments AS
                    (
                        SELECT
                            s.student_id,
                            s.student_name,
                            s.semester AS semester_no,
                            ISNULL(s.current_standing, '') AS current_standing,
                            p.programme_code,
                            e.offer_id,
                            c.credit_hour
                        FROM STUDENTS s
                        INNER JOIN PROGRAMMES p
                            ON p.programme_id = s.programme_id
                        INNER JOIN ENROLLMENTS e
                            ON e.student_id = s.student_id AND e.status = 'ENROLLED'
                        INNER JOIN COURSE_OFFERINGS co
                            ON co.offer_id = e.offer_id
                        INNER JOIN COURSES c
                            ON c.course_id = co.course_id
                        INNER JOIN ACADEMIC_SESSIONS sem
                            ON sem.academic_year = co.academic_year AND sem.semester = co.semester
                        WHERE s.status = 'ACTIVE'
                          AND (@SemesterId IS NULL OR sem.session_id = @SemesterId)
                          AND (@ProgrammeId IS NULL OR p.programme_id = @ProgrammeId)
                          AND (@DateFrom IS NULL OR sem.end_date >= @DateFrom)
                          AND (@DateTo IS NULL OR sem.start_date <= @DateTo)
                    ),
                    student_scope AS
                    (
                        SELECT DISTINCT student_id, student_name, semester_no, current_standing, programme_code
                        FROM scoped_enrolments
                    ),
                    academic_summary AS
                    (
                        SELECT
                            se.student_id,
                            CAST(
                                CASE
                                    WHEN SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN se.credit_hour ELSE 0 END) = 0 THEN NULL
                                    ELSE SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN g.grade_point * se.credit_hour ELSE 0 END)
                                       / SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN se.credit_hour ELSE 0 END)
                                END
                            AS DECIMAL(4,2)) AS Cgpa,
                            SUM(CASE WHEN UPPER(ISNULL(g.letter_grade, '')) = 'F' THEN 1 ELSE 0 END) AS FailedCourses
                        FROM scoped_enrolments se
                        LEFT JOIN GRADES g
                            ON g.student_id = se.student_id AND g.offer_id = se.offer_id
                        GROUP BY se.student_id
                    ),
                    attendance_summary AS
                    (
                        SELECT
                            se.student_id,
                            SUM(CASE WHEN UPPER(ISNULL(ar.status, '')) = 'PRESENT' THEN 1 ELSE 0 END) AS PresentCount,
                            SUM(CASE WHEN NULLIF(LTRIM(RTRIM(ar.status)), '') IS NOT NULL THEN 1 ELSE 0 END) AS RecordedCount
                        FROM scoped_enrolments se
                        LEFT JOIN ATTENDANCE_SESSIONS ats
                            ON ats.offer_id = se.offer_id
                           AND (@DateFrom IS NULL OR ats.session_date >= @DateFrom)
                           AND (@DateTo IS NULL OR ats.session_date <= @DateTo)
                        LEFT JOIN ATTENDANCE_RECORDS ar
                            ON ar.session_id = ats.session_id AND ar.student_id = se.student_id
                        GROUP BY se.student_id
                    ),
                    metrics AS
                    (
                        SELECT
                            ss.student_id AS StudentNo,
                            ss.student_name AS StudentName,
                            ss.programme_code AS Programme,
                            ISNULL(ss.semester_no, 0) AS SemesterNo,
                            ss.current_standing AS CurrentStanding,
                            ac.Cgpa,
                            ISNULL(ac.FailedCourses, 0) AS FailedCourses,
                            CAST(
                                CASE
                                    WHEN ISNULL(att.RecordedCount, 0) = 0 THEN NULL
                                    ELSE att.PresentCount * 100.0 / att.RecordedCount
                                END
                            AS DECIMAL(5,1)) AS AttendancePercentage
                        FROM student_scope ss
                        LEFT JOIN academic_summary ac ON ac.student_id = ss.student_id
                        LEFT JOIN attendance_summary att ON att.student_id = ss.student_id
                    ),
                    risk_report AS
                    (
                        SELECT *,
                            CASE
                                WHEN (Cgpa IS NOT NULL AND Cgpa < 1.50)
                                  OR (AttendancePercentage IS NOT NULL AND AttendancePercentage < 60) THEN 'Critical'
                                WHEN (Cgpa IS NOT NULL AND Cgpa < 2.00)
                                  OR (AttendancePercentage IS NOT NULL AND AttendancePercentage < 75) THEN 'High'
                                ELSE 'Medium'
                            END AS RiskLevel,
                            CASE
                                WHEN Cgpa IS NOT NULL AND Cgpa < 2.00 THEN 'CGPA below 2.00'
                                WHEN AttendancePercentage IS NOT NULL AND AttendancePercentage < 75 THEN 'Attendance below 75%'
                                WHEN FailedCourses > 0 THEN 'Failed course requires follow-up'
                                ELSE 'Academic standing requires follow-up'
                            END AS RiskReason
                        FROM metrics
                        WHERE (Cgpa IS NOT NULL AND Cgpa < 2.00)
                           OR (AttendancePercentage IS NOT NULL AND AttendancePercentage < 75)
                           OR FailedCourses > 0
                           OR CurrentStanding LIKE '%risk%'
                           OR CurrentStanding LIKE '%probation%'
                    )
                    SELECT * FROM risk_report
                    ORDER BY
                        CASE RiskLevel WHEN 'Critical' THEN 1 WHEN 'High' THEN 2 ELSE 3 END,
                        StudentName";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SemesterId", (object)semesterId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProgrammeId", (object)programmeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateFrom", (object)dateFrom ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", (object)dateTo ?? DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new AtRiskStudentReportRow
                            {
                                StudentNo = Text(reader["StudentNo"]),
                                StudentName = Text(reader["StudentName"]),
                                Programme = Text(reader["Programme"]),
                                SemesterNo = IntValue(reader["SemesterNo"]),
                                Cgpa = DecimalValue(reader["Cgpa"]),
                                AttendancePercentage = DecimalValue(reader["AttendancePercentage"]),
                                FailedCourses = IntValue(reader["FailedCourses"]),
                                RiskLevel = Text(reader["RiskLevel"]),
                                RiskReason = Text(reader["RiskReason"])
                            });
                        }
                    }
                }
            }

            return list;
        }

        public List<TopPerformingStudentReportRow> GetTopPerformingStudentReport(
            string semesterId,
            string programmeId,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var list = new List<TopPerformingStudentReportRow>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    WITH student_results AS
                    (
                        SELECT
                            s.student_id AS StudentNo,
                            s.student_name AS StudentName,
                            p.programme_code AS Programme,
                            ISNULL(s.semester, 0) AS SemesterNo,
                            COUNT(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN 1 END) AS CoursesGraded,
                            CAST(
                                CASE
                                    WHEN SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN c.credit_hour ELSE 0 END) = 0 THEN NULL
                                    ELSE SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN g.grade_point * c.credit_hour ELSE 0 END)
                                       / SUM(CASE WHEN g.grade_id IS NOT NULL AND UPPER(ISNULL(g.letter_grade, '')) <> 'N/A' THEN c.credit_hour ELSE 0 END)
                                END
                            AS DECIMAL(4,2)) AS Cgpa
                        FROM STUDENTS s
                        INNER JOIN PROGRAMMES p
                            ON p.programme_id = s.programme_id
                        INNER JOIN ENROLLMENTS e
                            ON e.student_id = s.student_id AND e.status = 'ENROLLED'
                        INNER JOIN COURSE_OFFERINGS co
                            ON co.offer_id = e.offer_id
                        INNER JOIN COURSES c
                            ON c.course_id = co.course_id
                        INNER JOIN ACADEMIC_SESSIONS sem
                            ON sem.academic_year = co.academic_year AND sem.semester = co.semester
                        LEFT JOIN GRADES g
                            ON g.student_id = e.student_id AND g.offer_id = e.offer_id
                        WHERE s.status = 'ACTIVE'
                          AND (@SemesterId IS NULL OR sem.session_id = @SemesterId)
                          AND (@ProgrammeId IS NULL OR p.programme_id = @ProgrammeId)
                          AND (@DateFrom IS NULL OR sem.end_date >= @DateFrom)
                          AND (@DateTo IS NULL OR sem.start_date <= @DateTo)
                        GROUP BY s.student_id, s.student_name, p.programme_code, s.semester
                    )
                    SELECT
                        StudentNo,
                        StudentName,
                        Programme,
                        SemesterNo,
                        CoursesGraded,
                        Cgpa,
                        CASE
                            WHEN Cgpa >= 3.70 THEN 'Dean''s List'
                            ELSE 'Scholarship Candidate'
                        END AS Eligibility
                    FROM student_results
                    WHERE Cgpa >= 3.50
                    ORDER BY Cgpa DESC, StudentName";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SemesterId", (object)semesterId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProgrammeId", (object)programmeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateFrom", (object)dateFrom ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", (object)dateTo ?? DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TopPerformingStudentReportRow
                            {
                                StudentNo = Text(reader["StudentNo"]),
                                StudentName = Text(reader["StudentName"]),
                                Programme = Text(reader["Programme"]),
                                SemesterNo = IntValue(reader["SemesterNo"]),
                                CoursesGraded = IntValue(reader["CoursesGraded"]),
                                Cgpa = DecimalValue(reader["Cgpa"]),
                                Eligibility = Text(reader["Eligibility"])
                            });
                        }
                    }
                }
            }

            return list;
        }

        public List<EnrolmentSummaryReportRow> GetEnrolmentSummaryReport(
            string semesterId,
            string programmeId,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var list = new List<EnrolmentSummaryReportRow>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT
                        sem.academic_year AS AcademicYear,
                        sem.semester AS SemesterName,
                        p.programme_code AS ProgrammeCode,
                        p.programme_name AS ProgrammeName,
                        COUNT(DISTINCT CASE WHEN e.status = 'ENROLLED' THEN e.student_id END) AS ActiveStudents,
                        SUM(CASE WHEN e.status = 'ENROLLED' THEN 1 ELSE 0 END) AS ActiveEnrolments,
                        SUM(CASE WHEN e.status = 'PENDING' THEN 1 ELSE 0 END) AS PendingEnrolments,
                        SUM(CASE WHEN e.status = 'DROPPED' THEN 1 ELSE 0 END) AS DroppedEnrolments,
                        SUM(CASE WHEN e.status IN ('ENROLLED', 'PENDING', 'DROPPED') THEN 1 ELSE 0 END) AS TotalEnrolments
                    FROM ACADEMIC_SESSIONS sem
                    INNER JOIN COURSE_OFFERINGS co
                        ON co.academic_year = sem.academic_year AND co.semester = sem.semester
                    INNER JOIN COURSES c
                        ON c.course_id = co.course_id
                    INNER JOIN PROGRAMMES p
                        ON p.programme_id = c.programme_id
                    LEFT JOIN ENROLLMENTS e
                        ON e.offer_id = co.offer_id
                    WHERE (@SemesterId IS NULL OR sem.session_id = @SemesterId)
                      AND (@ProgrammeId IS NULL OR p.programme_id = @ProgrammeId)
                      AND (@DateFrom IS NULL OR sem.end_date >= @DateFrom)
                      AND (@DateTo IS NULL OR sem.start_date <= @DateTo)
                    GROUP BY
                        sem.session_id,
                        sem.academic_year,
                        sem.semester,
                        sem.start_date,
                        p.programme_id,
                        p.programme_code,
                        p.programme_name
                    ORDER BY sem.start_date DESC, p.programme_code";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SemesterId", (object)semesterId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProgrammeId", (object)programmeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateFrom", (object)dateFrom ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", (object)dateTo ?? DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new EnrolmentSummaryReportRow
                            {
                                AcademicYear = Text(reader["AcademicYear"]),
                                SemesterName = Text(reader["SemesterName"]),
                                ProgrammeCode = Text(reader["ProgrammeCode"]),
                                ProgrammeName = Text(reader["ProgrammeName"]),
                                ActiveStudents = IntValue(reader["ActiveStudents"]),
                                ActiveEnrolments = IntValue(reader["ActiveEnrolments"]),
                                PendingEnrolments = IntValue(reader["PendingEnrolments"]),
                                DroppedEnrolments = IntValue(reader["DroppedEnrolments"]),
                                TotalEnrolments = IntValue(reader["TotalEnrolments"])
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

                if (string.Equals(Status, "N/A", StringComparison.OrdinalIgnoreCase))
                {
                    return "inline-flex items-center rounded-full border px-2 py-0.5 bg-slate-100 text-slate-600 border-slate-200";
                }

                return "inline-flex items-center rounded-full border px-2 py-0.5 bg-[#e0162b]/10 text-[#a01020] border-[#e0162b]/20";
            }
        }
    }

    public class CoursePerformanceReportRow
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Programme { get; set; }
        public int Enrolled { get; set; }
        public int Graded { get; set; }
        public decimal? AvgGpa { get; set; }
        public decimal? PassRate { get; set; }
        public int GradeA { get; set; }
        public int GradeB { get; set; }
        public int GradeC { get; set; }
        public int GradeD { get; set; }
        public int GradeF { get; set; }

        public string AvgGpaDisplay
        {
            get { return AvgGpa.HasValue ? AvgGpa.Value.ToString("0.00") : "-"; }
        }

        public string PassRateDisplay
        {
            get { return PassRate.HasValue ? PassRate.Value.ToString("0.0") + "%" : "-"; }
        }
    }

    public class AttendanceSummaryReportRow
    {
        public string Programme { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string SemesterName { get; set; }
        public int EnrolledStudents { get; set; }
        public int ClassSessions { get; set; }
        public int PresentCount { get; set; }
        public int LateCount { get; set; }
        public int AbsentCount { get; set; }
        public int RecordedSessions { get; set; }
        public decimal? AttendancePercentage { get; set; }

        public string AttendancePercentageDisplay
        {
            get { return AttendancePercentage.HasValue ? AttendancePercentage.Value.ToString("0.0") + "%" : "-"; }
        }
    }

    public class AtRiskStudentReportRow
    {
        public string StudentNo { get; set; }
        public string StudentName { get; set; }
        public string Programme { get; set; }
        public int SemesterNo { get; set; }
        public decimal? Cgpa { get; set; }
        public decimal? AttendancePercentage { get; set; }
        public int FailedCourses { get; set; }
        public string RiskLevel { get; set; }
        public string RiskReason { get; set; }

        public string CgpaDisplay
        {
            get { return Cgpa.HasValue ? Cgpa.Value.ToString("0.00") : "-"; }
        }

        public string AttendancePercentageDisplay
        {
            get { return AttendancePercentage.HasValue ? AttendancePercentage.Value.ToString("0.0") + "%" : "-"; }
        }
    }

    public class TopPerformingStudentReportRow
    {
        public string StudentNo { get; set; }
        public string StudentName { get; set; }
        public string Programme { get; set; }
        public int SemesterNo { get; set; }
        public int CoursesGraded { get; set; }
        public decimal? Cgpa { get; set; }
        public string Eligibility { get; set; }

        public string CgpaDisplay
        {
            get { return Cgpa.HasValue ? Cgpa.Value.ToString("0.00") : "-"; }
        }
    }

    public class EnrolmentSummaryReportRow
    {
        public string AcademicYear { get; set; }
        public string SemesterName { get; set; }
        public string ProgrammeCode { get; set; }
        public string ProgrammeName { get; set; }
        public int ActiveStudents { get; set; }
        public int ActiveEnrolments { get; set; }
        public int PendingEnrolments { get; set; }
        public int DroppedEnrolments { get; set; }
        public int TotalEnrolments { get; set; }
    }
}
