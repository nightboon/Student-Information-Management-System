using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using src.db;
using src.services.email;

namespace src.services.admin
{
    public class AdminPortalService
    {
        public AdminLookupData GetLookups()
        {
            AcademicIntakeSchema.Ensure();
            var data = new AdminLookupData();
            using (var conn = Db.OpenConnection())
            {
                data.Programmes = QueryOptions(conn,
                    "SELECT programme_code, programme_code + ' - ' + programme_name FROM PROGRAMMES WHERE status <> 'INACTIVE' OR status IS NULL ORDER BY programme_code");
                data.Departments = QueryOptions(conn,
                    "SELECT department_id, department_id + ' - ' + department_name FROM DEPARTMENTS WHERE status <> 'INACTIVE' OR status IS NULL ORDER BY department_name");
                data.Lecturers = QueryOptions(conn,
                    "SELECT lecturer_id, lecturer_name + ' (' + lecturer_id + ')' FROM LECTURERS WHERE status <> 'INACTIVE' OR status IS NULL ORDER BY lecturer_name");
                data.AcademicSessions = QueryOptions(conn,
                    "SELECT s.session_id, i.intake_name + ' - ' + s.semester " +
                    "FROM ACADEMIC_SESSIONS s LEFT JOIN INTAKES i ON i.intake_id=s.intake_id ORDER BY s.start_date DESC");
                data.AcademicYears = QuerySingleColumnOptions(conn,
                    "SELECT academic_year FROM (SELECT academic_year FROM ACADEMIC_SESSIONS UNION SELECT academic_year FROM COURSE_OFFERINGS) y ORDER BY academic_year DESC");
                data.Semesters = QuerySingleColumnOptions(conn,
                    "SELECT semester FROM (SELECT semester FROM ACADEMIC_SESSIONS UNION SELECT semester FROM COURSE_OFFERINGS) s ORDER BY semester");
                data.StudentSemesters = QuerySingleColumnOptions(conn,
                    "SELECT DISTINCT CONVERT(varchar(10), semester) FROM STUDENTS WHERE semester IS NOT NULL ORDER BY CONVERT(varchar(10), semester)");
                data.EducationLevels = QuerySingleColumnOptions(conn,
                    "SELECT DISTINCT education_level FROM PROGRAMMES WHERE education_level IS NOT NULL AND LTRIM(RTRIM(education_level)) <> '' ORDER BY education_level");
                data.ProgrammeStatuses = QuerySingleColumnOptions(conn,
                    "SELECT DISTINCT status FROM PROGRAMMES WHERE status IS NOT NULL AND LTRIM(RTRIM(status)) <> '' ORDER BY status");
                data.CourseStatuses = QuerySingleColumnOptions(conn,
                    "SELECT DISTINCT status FROM COURSES WHERE status IS NOT NULL AND LTRIM(RTRIM(status)) <> '' ORDER BY status");
                data.UserStatuses = QuerySingleColumnOptions(conn,
                    "SELECT DISTINCT status FROM USERS WHERE status IS NOT NULL AND LTRIM(RTRIM(status)) <> '' ORDER BY status");
                data.UserRoles = QuerySingleColumnOptions(conn,
                    "SELECT DISTINCT role FROM USERS WHERE role IN ('STUDENT','LECTURER') ORDER BY role");
            }

            EnsureOptions(data.UserRoles, "Student", "Lecturer");
            EnsureOptions(data.UserStatuses, "Active", "Pending", "Inactive");
            EnsureOptions(data.ProgrammeStatuses, "Active", "Inactive");
            EnsureOptions(data.CourseStatuses, "Active", "Inactive");
            EnsureOptions(data.EducationLevels, "Undergraduate", "Postgraduate", "Foundation");
            EnsureOptions(data.EventTypes, "Registration opens", "Registration closes",
                "Classes begin", "Semester ends");
            EnsureOptions(data.EventStatuses, "Upcoming", "Scheduled", "Completed");
            EnsureOptions(data.AcademicStatuses, "Good Standing", "Probation", "At Risk", "Pending");
            return data;
        }

        public static string RenderOptions(IEnumerable<AdminOption> options, string placeholder)
        {
            var html = new StringBuilder();
            if (placeholder != null)
            {
                html.Append("<option value=\"\">").Append(HttpUtility.HtmlEncode(placeholder)).Append("</option>");
            }
            foreach (var option in options ?? Enumerable.Empty<AdminOption>())
            {
                html.Append("<option value=\"").Append(HttpUtility.HtmlAttributeEncode(option.Value ?? ""))
                    .Append("\">").Append(HttpUtility.HtmlEncode(option.Text ?? "")).Append("</option>");
            }
            return html.ToString();
        }

        public AdminDashboardData GetDashboard()
        {
            var data = new AdminDashboardData();
            using (var conn = Db.OpenConnection())
            {
                data.TotalStudents = Count(conn, "SELECT COUNT(*) FROM STUDENTS");
                data.TotalLecturers = Count(conn, "SELECT COUNT(*) FROM LECTURERS");
                data.PendingRequests = Count(conn, "SELECT COUNT(*) FROM ENROLLMENTS WHERE status = 'PENDING'");
                data.AtRiskStudents = Count(conn, "SELECT COUNT(*) FROM STUDENTS WHERE current_standing <> 'Good Standing' OR status IN ('AT_RISK','PROBATION')");

                data.Programmes = QueryProgrammeEnrolment(conn);
                var perf = QueryPerformance(conn);
                data.AverageAttendance = perf.AverageAttendance;
                data.AverageCgpa = perf.AverageCgpa;
                data.PassRate = perf.PassRate;
                data.FailRate = perf.FailRate;
                data.CriticalAtRiskStudents = Count(conn,
                    "SELECT COUNT(*) FROM (SELECT g.student_id FROM GRADES g " +
                    "GROUP BY g.student_id HAVING AVG(CAST(g.grade_point AS float)) < 2.0) x");
                data.RecentNotices = QueryRecentNotices(conn);
            }

            return data;
        }

        // Letter-grade distribution across all recorded grades, bucketed by the leading
        // letter (A+/A/A- -> A, etc.) so the Overview chart reflects real data.
        public List<AdminGradeBand> GetGradeDistribution()
        {
            var bands = new List<AdminGradeBand>
            {
                new AdminGradeBand { Key = "A", Label = "A / A+", Color = "bg-emerald-500" },
                new AdminGradeBand { Key = "B", Label = "B / B+", Color = "bg-sky-500" },
                new AdminGradeBand { Key = "C", Label = "C / C+", Color = "bg-amber-500" },
                new AdminGradeBand { Key = "D", Label = "D", Color = "bg-orange-500" },
                new AdminGradeBand { Key = "F", Label = "F", Color = "bg-[#e0162b]" }
            };

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(
                "SELECT UPPER(LEFT(LTRIM(letter_grade), 1)) AS band, COUNT(*) AS cnt " +
                "FROM GRADES WHERE letter_grade IS NOT NULL AND LTRIM(letter_grade) <> '' " +
                "GROUP BY UPPER(LEFT(LTRIM(letter_grade), 1))", conn))
            using (var reader = cmd.ExecuteReader())
            {
                int total = 0;
                var counts = new Dictionary<string, int>();
                while (reader.Read())
                {
                    var band = Text(reader["band"]);
                    var cnt = Int(reader["cnt"]);
                    if (!string.IsNullOrEmpty(band)) { counts[band] = cnt; total += cnt; }
                }
                foreach (var b in bands)
                {
                    b.Count = counts.ContainsKey(b.Key) ? counts[b.Key] : 0;
                    b.Percent = Percentage(b.Count, total);
                }
            }
            return bands;
        }

        // Pass rate per programme (non-F grades), one entry per programme that has grades.
        public List<AdminProgrammePassRate> GetProgrammePassRates()
        {
            const string sql =
                "SELECT p.programme_code, " +
                "SUM(CASE WHEN g.letter_grade <> 'F' THEN 1 ELSE 0 END) AS passed, COUNT(*) AS total " +
                "FROM GRADES g " +
                "JOIN STUDENTS s ON s.student_id = g.student_id " +
                "JOIN PROGRAMMES p ON p.programme_id = s.programme_id " +
                "WHERE g.letter_grade IS NOT NULL AND LTRIM(g.letter_grade) <> '' " +
                "GROUP BY p.programme_code ORDER BY p.programme_code";
            var rows = new List<AdminProgrammePassRate>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var passed = Int(reader["passed"]);
                    var total = Int(reader["total"]);
                    rows.Add(new AdminProgrammePassRate
                    {
                        Code = Text(reader["programme_code"]),
                        PassRate = Percentage(passed, total)
                    });
                }
            }
            return rows;
        }

        public List<AdminUserRow> GetUsers()
        {
            const string sql =
                "SELECT u.user_id, u.email, u.role, u.status, u.created_at, " +
                "COALESCE(s.student_id, l.lecturer_id, CONVERT(varchar(20), u.user_id)) AS profile_id, " +
                "COALESCE(s.student_name, l.lecturer_name, u.username) AS full_name, " +
                "COALESCE(s.phone, l.phone, '') AS phone, " +
                "COALESCE(p.programme_code, l.department_id, '') AS programme " +
                "FROM USERS u " +
                "LEFT JOIN STUDENTS s ON s.user_id = u.user_id " +
                "LEFT JOIN PROGRAMMES p ON p.programme_id = s.programme_id " +
                "LEFT JOIN LECTURERS l ON l.user_id = u.user_id " +
                "WHERE u.role IN ('STUDENT','LECTURER') " +
                "ORDER BY u.created_at DESC, u.user_id DESC";

            var rows = new List<AdminUserRow>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    rows.Add(new AdminUserRow
                    {
                        UserId = Convert.ToInt32(reader["user_id"]),
                        ProfileId = Text(reader["profile_id"]),
                        FullName = Text(reader["full_name"]),
                        Email = Text(reader["email"]),
                        Phone = Text(reader["phone"]),
                        Role = Title(Text(reader["role"])),
                        Status = Title(Text(reader["status"])),
                        Programme = Text(reader["programme"]),
                        CreatedAt = reader["created_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["created_at"])
                    });
                }
            }

            return rows;
        }

        public List<AdminProgrammeRow> GetProgrammes()
        {
            const string sql =
                "SELECT p.programme_id, p.department_id, p.programme_code, p.programme_name, p.education_level, p.duration, p.semester_count, p.status, " +
                "COUNT(DISTINCT c.course_id) AS course_count, COUNT(DISTINCT s.student_id) AS student_count " +
                "FROM PROGRAMMES p " +
                "LEFT JOIN COURSES c ON c.programme_id = p.programme_id " +
                "LEFT JOIN STUDENTS s ON s.programme_id = p.programme_id " +
                "GROUP BY p.programme_id, p.department_id, p.programme_code, p.programme_name, p.education_level, p.duration, p.semester_count, p.status " +
                "ORDER BY p.programme_code";
            var rows = new List<AdminProgrammeRow>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    rows.Add(new AdminProgrammeRow
                    {
                        Id = Text(reader["programme_id"]),
                        DepartmentId = Text(reader["department_id"]),
                        Code = Text(reader["programme_code"]),
                        Name = Text(reader["programme_name"]),
                        Level = Text(reader["education_level"]),
                        Duration = Text(reader["duration"]),
                        Semesters = Int(reader["semester_count"]),
                        Status = Title(Text(reader["status"])),
                        CourseCount = Int(reader["course_count"]),
                        StudentCount = Int(reader["student_count"])
                    });
                }
            }
            return rows;
        }

        public List<AdminDepartmentRow> GetDepartments()
        {
            const string sql =
                "SELECT d.department_id, d.department_name, d.status, COUNT(DISTINCT p.programme_id) AS programme_count, COUNT(DISTINCT l.lecturer_id) AS lecturer_count " +
                "FROM DEPARTMENTS d LEFT JOIN PROGRAMMES p ON p.department_id = d.department_id LEFT JOIN LECTURERS l ON l.department_id = d.department_id " +
                "GROUP BY d.department_id, d.department_name, d.status ORDER BY d.department_name";
            var rows = new List<AdminDepartmentRow>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    rows.Add(new AdminDepartmentRow
                    {
                        Id = Text(reader["department_id"]),
                        Name = Text(reader["department_name"]),
                        Status = Title(Text(reader["status"])),
                        ProgrammeCount = Int(reader["programme_count"]),
                        LecturerCount = Int(reader["lecturer_count"])
                    });
                }
            }
            return rows;
        }

        public List<AdminCourseCatalogRow> GetCourseCatalog()
        {
            const string sql =
                "SELECT c.course_id, c.course_code, c.course_name, c.credit_hour, c.prerequisites, c.status, p.programme_code, l.lecturer_name " +
                "FROM COURSES c " +
                "JOIN PROGRAMMES p ON p.programme_id = c.programme_id " +
                "OUTER APPLY (SELECT TOP 1 l.lecturer_name FROM COURSE_OFFERINGS co JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id WHERE co.course_id = c.course_id ORDER BY co.offer_id DESC) l " +
                "ORDER BY c.course_code";
            var rows = new List<AdminCourseCatalogRow>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    rows.Add(new AdminCourseCatalogRow
                    {
                        Id = Text(reader["course_id"]),
                        Code = Text(reader["course_code"]),
                        Name = Text(reader["course_name"]),
                        Programme = Text(reader["programme_code"]),
                        CreditHours = Int(reader["credit_hour"]),
                        Prerequisites = Text(reader["prerequisites"]),
                        Lecturer = Text(reader["lecturer_name"]),
                        Status = Title(Text(reader["status"]))
                    });
                }
            }
            return rows;
        }

        public int CreateUser(AdminUserSaveRequest request)
        {
            AcademicIntakeSchema.Ensure();
            if (request == null) throw new ArgumentException("Missing user details.");
            var role = (request.Role ?? "").Trim().ToUpperInvariant();
            if (role != "STUDENT" && role != "LECTURER") throw new ArgumentException("Role must be Student or Lecturer.");
            if (string.IsNullOrWhiteSpace(request.FullName)) throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(request.Email)) throw new ArgumentException("Email is required.");

            var tempPassword = GenerateTempPassword();
            string detailLabel;
            string detailId;

            using (var conn = Db.OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                var userId = NextInt(conn, tx, "USERS", "user_id");
                var username = UniqueUsername(conn, tx, request.Email, userId);
                var status = NormalizeStatus(request.Status);

                using (var cmd = new SqlCommand(
                    "INSERT INTO USERS (user_id, username, email, password_hash, role, status, created_at) " +
                    "VALUES (@id, @username, @email, @hash, @role, @status, GETDATE())", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@email", request.Email.Trim());
                    cmd.Parameters.AddWithValue("@hash", Sha256Hex(tempPassword));
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.ExecuteNonQuery();
                }

                if (role == "STUDENT")
                {
                    var studentId = "S" + userId.ToString("00000");
                    var programmeId = ResolveProgramme(conn, tx, request.ProgrammeOrDepartment);
                    using (var cmd = new SqlCommand(
                        "INSERT INTO STUDENTS (student_id, user_id, programme_id, student_name, student_email, phone, semester, current_standing, session, intake_id, status) " +
                        "VALUES (@sid, @uid, @pid, @name, @email, @phone, 0, 'Good Standing', " +
                        "COALESCE(" +
                        "(SELECT TOP 1 academic_year + ' ' + semester FROM ACADEMIC_SESSIONS WHERE end_date >= GETDATE() ORDER BY start_date)," +
                        "(SELECT TOP 1 academic_year + ' ' + semester FROM ACADEMIC_SESSIONS ORDER BY start_date DESC)," +
                        "''" +
                        "), " +
                        "COALESCE(" +
                        "(SELECT TOP 1 intake_id FROM ACADEMIC_SESSIONS WHERE end_date >= GETDATE() ORDER BY start_date)," +
                        "(SELECT TOP 1 intake_id FROM ACADEMIC_SESSIONS ORDER BY start_date DESC)" +
                        "), @status)", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@sid", studentId);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@pid", programmeId);
                        cmd.Parameters.AddWithValue("@name", request.FullName.Trim());
                        cmd.Parameters.AddWithValue("@email", request.Email.Trim());
                        cmd.Parameters.AddWithValue("@phone", (object)(request.Phone ?? "") ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.ExecuteNonQuery();
                    }
                    detailLabel = "Programme";
                    detailId = programmeId;
                }
                else
                {
                    var lecturerId = "LEC" + userId.ToString("000");
                    var departmentId = ResolveDepartment(conn, tx, request.ProgrammeOrDepartment);
                    using (var cmd = new SqlCommand(
                        "INSERT INTO LECTURERS (lecturer_id, user_id, department_id, lecturer_name, lecturer_email, phone, mailing_address, status) " +
                        "VALUES (@lid, @uid, @did, @name, @email, @phone, '', @status)", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@lid", lecturerId);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@did", departmentId);
                        cmd.Parameters.AddWithValue("@name", request.FullName.Trim());
                        cmd.Parameters.AddWithValue("@email", request.Email.Trim());
                        cmd.Parameters.AddWithValue("@phone", (object)(request.Phone ?? "") ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.ExecuteNonQuery();
                    }
                    detailLabel = "Department";
                    detailId = departmentId;
                }

                tx.Commit();

                var personalEmail = string.IsNullOrWhiteSpace(request.PersonalEmail)
                    ? request.Email.Trim() : request.PersonalEmail.Trim();
                var fullName = request.FullName.Trim();
                var email = request.Email.Trim();
                var notes = request.Notes;
                var isStudent = role == "STUDENT";

                // Resolving the display name and sending the email both need extra DB/SMTP
                // round trips that the account creation itself doesn't — push both off the
                // request thread so the admin UI gets its response as soon as the user exists.
                Task.Run(() =>
                {
                    var detailValue = isStudent ? ProgrammeNameStandalone(detailId) : DepartmentNameStandalone(detailId);
                    EmailService.SendNewAccount(
                        personalEmail, fullName, email, tempPassword, detailLabel, detailValue, notes);
                });

                return userId;
            }
        }

        public void SetUserStatus(int userId, string status)
        {
            var normalized = NormalizeStatus(status);
            using (var conn = Db.OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                using (var cmd = new SqlCommand("UPDATE USERS SET status = @status WHERE user_id = @id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@status", normalized);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand("UPDATE STUDENTS SET status = @status WHERE user_id = @id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@status", normalized);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand("UPDATE LECTURERS SET status = @status WHERE user_id = @id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@status", normalized);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
                tx.Commit();
            }
        }

        public void UpdateUser(AdminUserSaveRequest request)
        {
            if (request == null || request.UserId <= 0) throw new ArgumentException("Missing user.");
            if (string.IsNullOrWhiteSpace(request.FullName)) throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(request.Email)) throw new ArgumentException("Email is required.");

            var status = NormalizeStatus(request.Status);

            using (var conn = Db.OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                string role;
                using (var cmd = new SqlCommand("SELECT role FROM USERS WHERE user_id = @id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", request.UserId);
                    role = Convert.ToString(cmd.ExecuteScalar()).ToUpperInvariant();
                }
                if (role != "STUDENT" && role != "LECTURER") throw new ArgumentException("Unsupported user role.");
                if (!string.IsNullOrWhiteSpace(request.Role) &&
                    !string.Equals(role, request.Role.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Changing an account role requires creating the matching profile first.");
                }

                using (var cmd = new SqlCommand("UPDATE USERS SET email = @email, role = @role, status = @status WHERE user_id = @id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@email", request.Email.Trim());
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", request.UserId);
                    cmd.ExecuteNonQuery();
                }

                var programmeId = ResolveProgramme(conn, tx, request.ProgrammeOrDepartment);
                using (var cmd = new SqlCommand(
                    "UPDATE STUDENTS SET programme_id = @programme, student_name = @name, student_email = @email, phone = @phone, status = @status WHERE user_id = @id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@programme", programmeId);
                    cmd.Parameters.AddWithValue("@name", request.FullName.Trim());
                    cmd.Parameters.AddWithValue("@email", request.Email.Trim());
                    cmd.Parameters.AddWithValue("@phone", (object)(request.Phone ?? "") ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", request.UserId);
                    cmd.ExecuteNonQuery();
                }

                var departmentId = ResolveDepartment(conn, tx, request.ProgrammeOrDepartment);
                using (var cmd = new SqlCommand(
                    "UPDATE LECTURERS SET department_id = @department, lecturer_name = @name, lecturer_email = @email, phone = @phone, status = @status WHERE user_id = @id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@department", departmentId);
                    cmd.Parameters.AddWithValue("@name", request.FullName.Trim());
                    cmd.Parameters.AddWithValue("@email", request.Email.Trim());
                    cmd.Parameters.AddWithValue("@phone", (object)(request.Phone ?? "") ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", request.UserId);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
        }

        public void SaveProgramme(AdminProgrammeSaveRequest request)
        {
            if (request == null) throw new ArgumentException("Missing programme details.");
            var code = CleanCode(request.Code);
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Programme code is required.");
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Programme name is required.");
            if (string.IsNullOrWhiteSpace(request.Department)) throw new ArgumentException("Department is required.");

            using (var conn = Db.OpenConnection())
            {
                var status = NormalizeStatus(request.Status);
                var departmentId = ResolveDepartment(conn, null, request.Department);
                if (status == "ACTIVE" && !IsDepartmentActive(conn, null, departmentId))
                    throw new ArgumentException("Programme cannot be active because the selected department is inactive.");
                var exists = Exists(conn, "SELECT 1 FROM PROGRAMMES WHERE programme_id = @id OR programme_code = @id", cmd => cmd.Parameters.AddWithValue("@id", code));
                var sql = exists
                    ? "UPDATE PROGRAMMES SET department_id = @department, programme_code = @code, programme_name = @name, education_level = @level, duration = @duration, semester_count = @semesters, status = @status WHERE programme_id = @id OR programme_code = @id"
                    : "INSERT INTO PROGRAMMES (programme_id, department_id, programme_code, programme_name, education_level, duration, semester_count, status) VALUES (@id, @department, @code, @name, @level, @duration, @semesters, @status)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", code);
                    cmd.Parameters.AddWithValue("@department", departmentId);
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@name", request.Name.Trim());
                    cmd.Parameters.AddWithValue("@level", (object)(request.Level ?? "") ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@duration", (object)(request.Duration ?? "") ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@semesters", request.Semesters <= 0 ? 1 : request.Semesters);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.ExecuteNonQuery();
                }
                if (status == "INACTIVE") SetCoursesInactiveForProgramme(conn, null, code);
            }
        }

        public void SaveDepartment(AdminDepartmentSaveRequest request)
        {
            if (request == null) throw new ArgumentException("Missing department details.");
            var id = CleanCode(request.Id);
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Department code is required.");
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Department name is required.");

            using (var conn = Db.OpenConnection())
            {
                var exists = Exists(conn, "SELECT 1 FROM DEPARTMENTS WHERE department_id = @id", cmd => cmd.Parameters.AddWithValue("@id", id));
                var status = NormalizeStatus(request.Status);
                using (var cmd = new SqlCommand(exists
                    ? "UPDATE DEPARTMENTS SET department_name = @name, status = @status WHERE department_id = @id"
                    : "INSERT INTO DEPARTMENTS (department_id, department_name, status) VALUES (@id, @name, @status)", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", request.Name.Trim());
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.ExecuteNonQuery();
                }
                if (status == "INACTIVE") SetProgrammesAndCoursesInactiveForDepartment(conn, null, id);
            }
        }

        public void SaveCourse(AdminCourseSaveRequest request)
        {
            if (request == null) throw new ArgumentException("Missing course details.");
            var code = CleanCode(request.Code);
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Course code is required.");
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Course title is required.");

            using (var conn = Db.OpenConnection())
            {
                var status = NormalizeStatus(request.Status);
                var programmeId = ResolveProgramme(conn, null, CleanCode(request.Programme));
                if (status == "ACTIVE" && !CanProgrammeHaveActiveCourses(conn, null, programmeId))
                    throw new ArgumentException("Course cannot be active because its programme or department is inactive.");
                var exists = Exists(conn, "SELECT 1 FROM COURSES WHERE course_id = @id OR course_code = @id", cmd => cmd.Parameters.AddWithValue("@id", code));
                var sql = exists
                    ? "UPDATE COURSES SET programme_id = @programme, course_code = @code, course_name = @name, credit_hour = @credits, prerequisites = @prereq, status = @status WHERE course_id = @id OR course_code = @id"
                    : "INSERT INTO COURSES (course_id, programme_id, course_code, course_name, credit_hour, prerequisites, status) VALUES (@id, @programme, @code, @name, @credits, @prereq, @status)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", code);
                    cmd.Parameters.AddWithValue("@programme", programmeId);
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@name", request.Name.Trim());
                    cmd.Parameters.AddWithValue("@credits", request.CreditHours <= 0 ? 1 : request.CreditHours);
                    cmd.Parameters.AddWithValue("@prereq", (object)(request.Prerequisites ?? "") ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProgramme(string programmeCode)
        {
            var code = CleanCode(programmeCode);
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Programme code is required.");

            using (var conn = Db.OpenConnection())
            {
                var hasDependencies = Exists(conn,
                    "SELECT 1 FROM STUDENTS WHERE programme_id = @id UNION SELECT 1 FROM COURSES WHERE programme_id = @id",
                    cmd => cmd.Parameters.AddWithValue("@id", code));
                using (var cmd = new SqlCommand(hasDependencies
                    ? "UPDATE PROGRAMMES SET status = 'INACTIVE' WHERE programme_id = @id OR programme_code = @id"
                    : "DELETE FROM PROGRAMMES WHERE programme_id = @id OR programme_code = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", code);
                    cmd.ExecuteNonQuery();
                }
                if (hasDependencies) SetCoursesInactiveForProgramme(conn, null, code);
            }
        }

        public void DeleteDepartment(string departmentId)
        {
            var id = CleanCode(departmentId);
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Department code is required.");
            using (var conn = Db.OpenConnection())
            {
                var linked = Exists(conn,
                    "SELECT 1 FROM PROGRAMMES WHERE department_id = @id UNION SELECT 1 FROM LECTURERS WHERE department_id = @id",
                    cmd => cmd.Parameters.AddWithValue("@id", id));
                using (var cmd = new SqlCommand(linked
                    ? "UPDATE DEPARTMENTS SET status = 'INACTIVE' WHERE department_id = @id"
                    : "DELETE FROM DEPARTMENTS WHERE department_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                if (linked) SetProgrammesAndCoursesInactiveForDepartment(conn, null, id);
            }
        }

        public void DeleteCourse(string courseCode)
        {
            var code = CleanCode(courseCode);
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Course code is required.");

            using (var conn = Db.OpenConnection())
            {
                var hasDependencies = Exists(conn,
                    "SELECT 1 FROM COURSE_OFFERINGS WHERE course_id = @id UNION SELECT 1 FROM ENROLLMENTS WHERE course_id = @id",
                    cmd => cmd.Parameters.AddWithValue("@id", code));
                using (var cmd = new SqlCommand(hasDependencies
                    ? "UPDATE COURSES SET status = 'INACTIVE' WHERE course_id = @id OR course_code = @id"
                    : "DELETE FROM COURSES WHERE course_id = @id OR course_code = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", code);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveCourseAssignment(AdminCourseAssignmentSaveRequest request)
        {
            AcademicIntakeSchema.Ensure();
            if (request == null) throw new ArgumentException("Missing assignment details.");
            var courseCode = CleanCode(request.CourseCode);
            if (string.IsNullOrWhiteSpace(courseCode)) throw new ArgumentException("Course code is required.");
            if (string.IsNullOrWhiteSpace(request.Lecturer)) throw new ArgumentException("Lecturer is required.");

            using (var conn = Db.OpenConnection())
            {
                var courseId = ResolveCourse(conn, null, courseCode);
                var lecturerId = ResolveLecturer(conn, null, request.Lecturer);
                var sessionId = (request.SessionId ?? "").Trim();
                if (string.IsNullOrWhiteSpace(sessionId)) throw new ArgumentException("Intake semester is required.");
                string semester;
                string academicYear;
                using (var sessionCmd = new SqlCommand("SELECT semester, academic_year FROM ACADEMIC_SESSIONS WHERE session_id=@id", conn))
                {
                    sessionCmd.Parameters.AddWithValue("@id", sessionId);
                    using (var reader = sessionCmd.ExecuteReader())
                    {
                        if (!reader.Read()) throw new ArgumentException("The selected intake semester does not exist.");
                        semester = Text(reader["semester"]);
                        academicYear = Text(reader["academic_year"]);
                    }
                }
                var status = NormalizeStatus(request.Status);
                if (status == "ACTIVE" && !CanCourseBeActive(conn, null, courseId))
                    throw new ArgumentException("Assignment cannot be active because its course, programme, or department is inactive.");

                if (request.OfferId > 0 && Exists(conn, "SELECT 1 FROM COURSE_OFFERINGS WHERE offer_id = @id", cmd => cmd.Parameters.AddWithValue("@id", request.OfferId)))
                {
                    using (var cmd = new SqlCommand("UPDATE COURSE_OFFERINGS SET course_id=@course, lecturer_id=@lecturer, session_id=@session, semester=@semester, academic_year=@year, status=@status WHERE offer_id=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", request.OfferId);
                        cmd.Parameters.AddWithValue("@course", courseId);
                        cmd.Parameters.AddWithValue("@lecturer", lecturerId);
                        cmd.Parameters.AddWithValue("@session", sessionId);
                        cmd.Parameters.AddWithValue("@semester", semester);
                        cmd.Parameters.AddWithValue("@year", academicYear);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.ExecuteNonQuery();
                    }
                    return;
                }

                using (var cmd = new SqlCommand("INSERT INTO COURSE_OFFERINGS (course_id,lecturer_id,session_id,semester,academic_year,section,status) VALUES (@course,@lecturer,@session,@semester,@year,'',@status)", conn))
                {
                    cmd.Parameters.AddWithValue("@course", courseId);
                    cmd.Parameters.AddWithValue("@lecturer", lecturerId);
                    cmd.Parameters.AddWithValue("@session", sessionId);
                    cmd.Parameters.AddWithValue("@semester", semester);
                    cmd.Parameters.AddWithValue("@year", academicYear);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCourseAssignment(int offerId)
        {
            if (offerId <= 0) throw new ArgumentException("Missing course assignment.");
            using (var conn = Db.OpenConnection())
            {
                var hasEnrollments = Exists(conn, "SELECT 1 FROM ENROLLMENTS WHERE offer_id = @id", cmd => cmd.Parameters.AddWithValue("@id", offerId));
                var sql = hasEnrollments
                    ? "UPDATE COURSE_OFFERINGS SET status = 'INACTIVE' WHERE offer_id = @id"
                    : "DELETE FROM COURSE_OFFERINGS WHERE offer_id = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", offerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<AdminAddDropRequestRow> GetAddDropRequests()
        {
            const string sql =
                "SELECT e.enrollment_id, e.student_id, s.student_name, p.programme_code, ISNULL(s.semester, 0) AS semester_no, " +
                "c.course_code, c.course_name, e.status, ISNULL(e.semester, '') AS semester_name, e.request_type " +
                "FROM ENROLLMENTS e " +
                "JOIN STUDENTS s ON s.student_id = e.student_id " +
                "JOIN PROGRAMMES p ON p.programme_id = s.programme_id " +
                "JOIN COURSES c ON c.course_id = e.course_id " +
                "WHERE e.request_type IS NOT NULL " +
                "ORDER BY e.enrollment_id DESC";
            var rows = new List<AdminAddDropRequestRow>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var rawStatus = Text(reader["status"]).ToUpperInvariant();
                    var requestType = Text(reader["request_type"]).ToUpperInvariant();

                    // Approve/reject outcomes land on the same enrollment statuses used
                    // elsewhere (ENROLLED/DROPPED), so the displayed request status has
                    // to be resolved relative to what was requested, not read verbatim.
                    string displayStatus;
                    if (rawStatus == "PENDING") displayStatus = "Pending";
                    else if (requestType == "DROP") displayStatus = rawStatus == "DROPPED" ? "Approved" : "Rejected";
                    else displayStatus = rawStatus == "ENROLLED" ? "Approved" : "Rejected";

                    rows.Add(new AdminAddDropRequestRow
                    {
                        RequestId = Int(reader["enrollment_id"]),
                        StudentId = Text(reader["student_id"]),
                        StudentName = Text(reader["student_name"]),
                        Programme = Text(reader["programme_code"]),
                        Semester = Int(reader["semester_no"]),
                        CourseCode = Text(reader["course_code"]),
                        CourseName = Text(reader["course_name"]),
                        Type = requestType == "DROP" ? "Drop" : "Add",
                        Status = displayStatus,
                        Reason = Text(reader["semester_name"])
                    });
                }
            }
            return rows;
        }

        public static bool SetEnrollmentStatus(int enrollmentId, string action)
        {
            // 1. Read current request_type from DB
            string requestType;
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand("SELECT request_type FROM ENROLLMENTS WHERE enrollment_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", enrollmentId);
                var result = cmd.ExecuteScalar();
                requestType = result == null || result == DBNull.Value ? null : result.ToString();
            }
            if (string.IsNullOrEmpty(requestType)) return false;

            // 2. Resolve final status
            string newStatus;
            if (action == "approve")
                newStatus = requestType == "DROP" ? "DROPPED" : "ENROLLED";   // approve ADD → ENROLLED, approve DROP → DROPPED
            else
                newStatus = requestType == "DROP" ? "ENROLLED" : "REJECTED";  // reject DROP → revert to ENROLLED, reject ADD → REJECTED

            // 3. Update status. request_type is kept (not cleared) so the request
            // still shows up in GetAddDropRequests() with its resolved outcome.
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(
                "UPDATE ENROLLMENTS SET status = @status WHERE enrollment_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@status", newStatus);
                cmd.Parameters.AddWithValue("@id", enrollmentId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<AdminCalendarEventRow> GetCalendarEvents()
        {
            AcademicIntakeSchema.Ensure();
            const string sql =
                "SELECT s.session_id, s.academic_year, s.semester, s.start_date, s.end_date, s.status, " +
                "s.intake_id, i.intake_name, i.intake_month, s.min_credits, s.max_credits " +
                "FROM ACADEMIC_SESSIONS s LEFT JOIN INTAKES i ON i.intake_id=s.intake_id ORDER BY s.start_date";
            var rows = new List<AdminCalendarEventRow>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var sessionId = Text(reader["session_id"]);
                    var start = Convert.ToDateTime(reader["start_date"]);
                    var end = Convert.ToDateTime(reader["end_date"]);
                    var academicYear = Text(reader["academic_year"]);
                    var semester = Text(reader["semester"]);
                    var intakeId = Text(reader["intake_id"]);
                    var intakeName = Text(reader["intake_name"]);
                    var intakeMonth = Convert.ToDateTime(reader["intake_month"]);
                    var minCredits = reader["min_credits"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["min_credits"]);
                    var maxCredits = reader["max_credits"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["max_credits"]);
                    var sem = intakeName + " - " + semester;
                    var registrationStart = start.AddDays(-7);
                    var registrationEnd = start.AddDays(-1);
                    var addDropEnd = start.AddDays(7);
                    AddMilestone(rows, sessionId, intakeId, intakeName, academicYear, semester, sem,
                        "Registration opens", registrationStart, intakeMonth, registrationStart, registrationEnd, addDropEnd, start, end, minCredits, maxCredits);
                    AddMilestone(rows, sessionId, intakeId, intakeName, academicYear, semester, sem,
                        "Registration closes", addDropEnd, intakeMonth, registrationStart, registrationEnd, addDropEnd, start, end, minCredits, maxCredits);
                    AddMilestone(rows, sessionId, intakeId, intakeName, academicYear, semester, sem,
                        "Classes begin", start, intakeMonth, registrationStart, registrationEnd, addDropEnd, start, end, minCredits, maxCredits);
                    AddMilestone(rows, sessionId, intakeId, intakeName, academicYear, semester, sem,
                        "Semester ends", end, intakeMonth, registrationStart, registrationEnd, addDropEnd, start, end, minCredits, maxCredits);
                }
            }
            return rows;
        }

        public List<AdminIntakeRow> GetIntakes()
        {
            AcademicIntakeSchema.Ensure();
            var rows = new List<AdminIntakeRow>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(
                "SELECT i.intake_id, i.intake_name, i.intake_month, i.status, COUNT(s.session_id) semester_count " +
                "FROM INTAKES i LEFT JOIN ACADEMIC_SESSIONS s ON s.intake_id=i.intake_id " +
                "GROUP BY i.intake_id,i.intake_name,i.intake_month,i.status ORDER BY i.intake_month DESC", conn))
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    rows.Add(new AdminIntakeRow
                    {
                        IntakeId = Text(reader["intake_id"]),
                        IntakeName = Text(reader["intake_name"]),
                        IntakeMonth = Convert.ToDateTime(reader["intake_month"]),
                        Status = Text(reader["status"]),
                        SemesterCount = Int(reader["semester_count"])
                    });
            return rows;
        }

        public void SaveAcademicSession(AdminAcademicSessionSaveRequest request)
        {
            AcademicIntakeSchema.Ensure();
            if (request == null) throw new ArgumentException("Missing calendar event details.");
            if (!DateTime.TryParse(request.StartDate, out var startDate)) throw new ArgumentException("Start date is required.");
            if (!DateTime.TryParse(request.EndDate, out var endDate)) throw new ArgumentException("End date is required.");
            if (endDate <= startDate) throw new ArgumentException("Semester end date must be after the classes begin date.");

            var intakeId = (request.IntakeId ?? "").Trim().ToUpperInvariant();
            var intakeName = (request.IntakeName ?? "").Trim().ToUpperInvariant();
            DateTime intakeMonth;
            if (!DateTime.TryParse(request.IntakeMonth, out intakeMonth)) throw new ArgumentException("Intake month is required.");
            if (string.IsNullOrWhiteSpace(intakeId)) intakeId = intakeMonth.ToString("MMMyyyy").ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(intakeName)) intakeName = intakeMonth.ToString("MMMM yyyy").ToUpperInvariant();
            var academicYear = startDate.Year.ToString();
            var semester = (request.Semester ?? "").Trim();
            if (string.IsNullOrWhiteSpace(semester)) semester = "Semester";

            var sessionId = CleanSessionId(request.SessionId);
            var status = NormalizeAcademicSessionStatus(request.Status);

            if (!int.TryParse(request.MinCredits, out var minCredits)) throw new ArgumentException("Minimum credits is required.");
            if (!int.TryParse(request.MaxCredits, out var maxCredits)) throw new ArgumentException("Maximum credits is required.");
            if (minCredits < 0 || maxCredits < 0) throw new ArgumentException("Credits cannot be negative.");
            if (maxCredits < minCredits) throw new ArgumentException("Maximum credits cannot be less than minimum credits.");

            using (var conn = Db.OpenConnection())
            {
                // New sessions always get the next sequential id (1, 2, 3, ...), even if the
                // intake/semester combination matches an existing session.
                if (string.IsNullOrWhiteSpace(sessionId)) sessionId = NextSessionId(conn);

                using (var intakeCmd = new SqlCommand(
                    "IF EXISTS(SELECT 1 FROM INTAKES WHERE intake_id=@id) " +
                    "UPDATE INTAKES SET intake_name=@name,intake_month=@month,status='ACTIVE' WHERE intake_id=@id " +
                    "ELSE INSERT INTO INTAKES(intake_id,intake_name,intake_month,status) VALUES(@id,@name,@month,'ACTIVE')", conn))
                {
                    intakeCmd.Parameters.AddWithValue("@id", intakeId);
                    intakeCmd.Parameters.AddWithValue("@name", intakeName);
                    intakeCmd.Parameters.AddWithValue("@month", new DateTime(intakeMonth.Year, intakeMonth.Month, 1));
                    intakeCmd.ExecuteNonQuery();
                }
                var exists = Exists(conn, "SELECT 1 FROM ACADEMIC_SESSIONS WHERE session_id = @id", cmd => cmd.Parameters.AddWithValue("@id", sessionId));
                var sql = exists
                    ? "UPDATE ACADEMIC_SESSIONS SET intake_id=@intake, academic_year=@year, semester=@semester, start_date=@start, end_date=@end, status=@status, min_credits=@min, max_credits=@max WHERE session_id=@id"
                    : "INSERT INTO ACADEMIC_SESSIONS (session_id,intake_id,academic_year,semester,start_date,end_date,status,min_credits,max_credits) VALUES (@id,@intake,@year,@semester,@start,@end,@status,@min,@max)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", sessionId);
                    cmd.Parameters.AddWithValue("@intake", intakeId);
                    cmd.Parameters.AddWithValue("@year", academicYear);
                    cmd.Parameters.AddWithValue("@semester", semester);
                    cmd.Parameters.AddWithValue("@start", startDate.Date);
                    cmd.Parameters.AddWithValue("@end", endDate.Date);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@min", minCredits);
                    cmd.Parameters.AddWithValue("@max", maxCredits);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void AddMilestone(List<AdminCalendarEventRow> rows, string sessionId,
            string intakeId, string intakeName, string academicYear, string semester,
            string semesterLabel, string type, DateTime date, DateTime intakeMonth, DateTime registrationStart,
            DateTime registrationEnd, DateTime addDropEnd, DateTime start, DateTime end, int? minCredits, int? maxCredits)
        {
            rows.Add(new AdminCalendarEventRow
            {
                Id = sessionId + "-" + type.Replace(" ", "-"),
                SessionId = sessionId,
                Title = type,
                Type = type,
                StartDate = date,
                EndDate = date,
                SessionStartDate = start,
                SessionEndDate = end,
                Semester = semesterLabel,
                AcademicYear = academicYear,
                SemesterName = semester,
                IntakeId = intakeId,
                IntakeName = intakeName,
                IntakeMonth = intakeMonth,
                RegistrationStart = registrationStart,
                RegistrationEnd = registrationEnd,
                AddDropEnd = addDropEnd,
                MinCredits = minCredits,
                MaxCredits = maxCredits,
                Status = date < DateTime.Today ? "Completed" : date == DateTime.Today ? "Active" : "Scheduled"
            });
        }

        public void DeleteAcademicSession(string sessionId)
        {
            var id = CleanSessionId(sessionId);
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Missing academic session.");
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand("DELETE FROM ACADEMIC_SESSIONS WHERE session_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public AdminStudentDetailSummary GetStudentDetail(string studentId)
        {
            const string sql =
                "SELECT TOP 1 s.student_id, s.student_name, s.student_email, ISNULL(s.semester, 0) AS semester_no, " +
                "p.programme_name, p.programme_code, s.current_standing, s.session " +
                "FROM STUDENTS s JOIN PROGRAMMES p ON p.programme_id = s.programme_id " +
                "WHERE s.student_id = @id OR @id = '' " +
                "ORDER BY CASE WHEN s.student_id = @id THEN 0 ELSE 1 END, s.student_id";
            var summary = new AdminStudentDetailSummary();
            using (var conn = Db.OpenConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", studentId ?? "");
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            summary.StudentId = Text(reader["student_id"]);
                            summary.Name = Text(reader["student_name"]);
                            summary.Email = Text(reader["student_email"]);
                            summary.Semester = Int(reader["semester_no"]);
                            summary.Programme = Text(reader["programme_name"]);
                            summary.ProgrammeCode = Text(reader["programme_code"]);
                            summary.Standing = Text(reader["current_standing"]);
                            summary.Session = Text(reader["session"]);
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(summary.StudentId)) return summary;

                using (var cmd = new SqlCommand("SELECT AVG(CAST(grade_point AS float)) FROM GRADES WHERE student_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", summary.StudentId);
                    summary.Cgpa = Decimal(cmd.ExecuteScalar());
                }
                using (var cmd = new SqlCommand(
                    "SELECT AVG(CAST(g.grade_point AS float)) FROM GRADES g " +
                    "WHERE g.student_id = @id AND g.semester = (SELECT TOP 1 semester FROM GRADES WHERE student_id = @id ORDER BY grade_id DESC)", conn))
                {
                    cmd.Parameters.AddWithValue("@id", summary.StudentId);
                    summary.CurrentGpa = Decimal(cmd.ExecuteScalar());
                }
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(DISTINCT e.course_id), ISNULL(SUM(DISTINCT c.credit_hour), 0) " +
                    "FROM ENROLLMENTS e JOIN COURSES c ON c.course_id = e.course_id " +
                    "WHERE e.student_id = @id AND e.status = 'ENROLLED'", conn))
                {
                    cmd.Parameters.AddWithValue("@id", summary.StudentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            summary.CompletedCourses = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            summary.CreditHours = reader.IsDBNull(1) ? 0 : Convert.ToInt32(reader.GetValue(1));
                        }
                    }
                }
                using (var cmd = new SqlCommand(
                    "SELECT SUM(CASE WHEN ar.status IN ('PRESENT','LATE') THEN 1 ELSE 0 END), COUNT(ar.attendance_id) " +
                    "FROM ATTENDANCE_RECORDS ar WHERE ar.student_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", summary.StudentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var present = reader.IsDBNull(0) ? 0 : Convert.ToDecimal(reader.GetValue(0));
                            var total = reader.IsDBNull(1) ? 0 : Convert.ToDecimal(reader.GetValue(1));
                            summary.Attendance = Percentage(present, total);
                        }
                    }
                }

                // Per-semester GPA for the CGPA trend chart. Cumulative GPA is built up in
                // code so it stays consistent with the overall CGPA (simple grade-point average).
                using (var cmd = new SqlCommand(
                    "SELECT g.semester, AVG(CAST(g.grade_point AS float)) AS sem_gpa, " +
                    "COUNT(*) AS grade_count, MIN(g.grade_id) AS first_id " +
                    "FROM GRADES g WHERE g.student_id = @id AND g.semester IS NOT NULL " +
                    "GROUP BY g.semester ORDER BY MIN(g.grade_id)", conn))
                {
                    cmd.Parameters.AddWithValue("@id", summary.StudentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        double runningSum = 0;
                        int runningCount = 0;
                        while (reader.Read())
                        {
                            var semGpa = reader.IsDBNull(1) ? 0d : Convert.ToDouble(reader.GetValue(1));
                            var count = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2));
                            runningSum += semGpa * count;
                            runningCount += count;
                            summary.CgpaTrend.Add(new AdminStudentSemesterGpa
                            {
                                SemesterLabel = Text(reader["semester"]),
                                SemesterGpa = Math.Round((decimal)semGpa, 2),
                                CumulativeGpa = runningCount == 0 ? 0m : Math.Round((decimal)(runningSum / runningCount), 2)
                            });
                        }
                    }
                }

                // Per-course attendance, grouped into the semesters the student was enrolled in.
                using (var cmd = new SqlCommand(
                    "SELECT co.semester AS semester_name, ISNULL(co.academic_year, '') AS academic_year, " +
                    "c.course_code, c.course_name, " +
                    "SUM(CASE WHEN ar.status IN ('PRESENT','LATE') THEN 1 ELSE 0 END) AS present_count, " +
                    "COUNT(ar.attendance_id) AS total_count " +
                    "FROM ENROLLMENTS e " +
                    "JOIN COURSE_OFFERINGS co ON co.offer_id = e.offer_id " +
                    "JOIN COURSES c ON c.course_id = co.course_id " +
                    "LEFT JOIN ATTENDANCE_SESSIONS ats ON ats.offer_id = co.offer_id " +
                    "LEFT JOIN ATTENDANCE_RECORDS ar ON ar.session_id = ats.session_id AND ar.student_id = e.student_id " +
                    "WHERE e.student_id = @id AND e.status IN ('ENROLLED','PENDING') " +
                    "GROUP BY co.semester, co.academic_year, c.course_code, c.course_name " +
                    "ORDER BY co.academic_year, co.semester, c.course_code", conn))
                {
                    cmd.Parameters.AddWithValue("@id", summary.StudentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        AdminStudentSemesterAttendance current = null;
                        while (reader.Read())
                        {
                            var semesterLabel = Text(reader["semester_name"]);
                            var academicYear = Text(reader["academic_year"]);
                            var present = Int(reader["present_count"]);
                            var total = Int(reader["total_count"]);

                            if (current == null || current.SemesterLabel != semesterLabel || current.AcademicYear != academicYear)
                            {
                                current = new AdminStudentSemesterAttendance
                                {
                                    SemesterLabel = semesterLabel,
                                    AcademicYear = academicYear
                                };
                                summary.AttendanceBySemester.Add(current);
                            }

                            current.Courses.Add(new AdminStudentCourseAttendance
                            {
                                Code = Text(reader["course_code"]),
                                Name = Text(reader["course_name"]),
                                Present = present,
                                Total = total,
                                Rate = Percentage(present, total)
                            });
                        }

                        // Overall semester rate is records-weighted, not an average of per-course rates.
                        foreach (var sem in summary.AttendanceBySemester)
                        {
                            var totalRecords = sem.Courses.Sum(x => x.Total);
                            var presentRecords = sem.Courses.Sum(x => x.Present);
                            sem.Rate = Percentage(presentRecords, totalRecords);
                        }
                    }
                }

                // Course grades grouped by semester for the "Semester Results" accordion.
                using (var cmd = new SqlCommand(
                    "SELECT g.semester AS semester_name, ISNULL(co.academic_year, '') AS academic_year, " +
                    "c.course_code, c.course_name, c.credit_hour, " +
                    "ISNULL(g.letter_grade, '') AS letter_grade, g.grade_point, g.grade_id " +
                    "FROM GRADES g " +
                    "JOIN COURSE_OFFERINGS co ON co.offer_id = g.offer_id " +
                    "JOIN COURSES c ON c.course_id = co.course_id " +
                    "WHERE g.student_id = @id " +
                    "ORDER BY g.grade_id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", summary.StudentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        AdminStudentSemesterResult current = null;
                        while (reader.Read())
                        {
                            var semesterLabel = Text(reader["semester_name"]);
                            var academicYear = Text(reader["academic_year"]);

                            if (current == null || current.SemesterLabel != semesterLabel || current.AcademicYear != academicYear)
                            {
                                current = new AdminStudentSemesterResult
                                {
                                    SemesterLabel = semesterLabel,
                                    AcademicYear = academicYear
                                };
                                summary.SemesterResults.Add(current);
                            }

                            current.Courses.Add(new AdminStudentCourseGrade
                            {
                                Code = Text(reader["course_code"]),
                                Name = Text(reader["course_name"]),
                                Credits = Int(reader["credit_hour"]),
                                LetterGrade = Text(reader["letter_grade"]),
                                GradePoint = Decimal(reader["grade_point"])
                            });
                        }

                        // Sem GPA / cumulative CGPA use the same simple grade-point average as
                        // the overall CGPA and the trend chart, so all three stay consistent.
                        decimal runningSum = 0m;
                        int runningCount = 0;
                        foreach (var sem in summary.SemesterResults)
                        {
                            var semSum = sem.Courses.Sum(x => x.GradePoint);
                            var semCount = sem.Courses.Count;
                            sem.SemGpa = semCount == 0 ? 0m : Math.Round(semSum / semCount, 2);
                            runningSum += semSum;
                            runningCount += semCount;
                            sem.Cgpa = runningCount == 0 ? 0m : Math.Round(runningSum / runningCount, 2);
                        }
                    }
                }
            }
            return summary;
        }

        public List<AdminStudentPerformanceRow> GetStudentPerformanceRows()
        {
            const string sql =
                "SELECT s.student_id, s.student_name, p.programme_code, ISNULL(s.semester, 0) AS semester_no, " +
                "ISNULL(latest.course_code, '') AS course_code, ISNULL(latest.course_name, '') AS course_name, " +
                "ISNULL(latest.letter_grade, '-') AS letter_grade, ISNULL(latest.grade_point, 0) AS current_gpa, " +
                "ISNULL(grades.cgpa, 0) AS cgpa, ISNULL(grades.failed_count, 0) AS failed_count, ISNULL(att.rate, 0) AS attendance, " +
                "ISNULL(s.current_standing, '') AS standing " +
                "FROM STUDENTS s JOIN PROGRAMMES p ON p.programme_id = s.programme_id " +
                "OUTER APPLY (SELECT TOP 1 c.course_code, c.course_name, g.letter_grade, g.grade_point FROM GRADES g JOIN COURSE_OFFERINGS co ON co.offer_id = g.offer_id JOIN COURSES c ON c.course_id = co.course_id WHERE g.student_id = s.student_id ORDER BY g.grade_id DESC) latest " +
                "OUTER APPLY (SELECT AVG(CAST(g.grade_point AS decimal(5,2))) AS cgpa, SUM(CASE WHEN g.letter_grade = 'F' THEN 1 ELSE 0 END) AS failed_count FROM GRADES g WHERE g.student_id = s.student_id) grades " +
                "OUTER APPLY (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 100.0 * SUM(CASE WHEN ar.status IN ('PRESENT','LATE') THEN 1 ELSE 0 END) / COUNT(*) END AS rate FROM ATTENDANCE_RECORDS ar WHERE ar.student_id = s.student_id) att " +
                "ORDER BY s.student_name";
            var rows = new List<AdminStudentPerformanceRow>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    rows.Add(new AdminStudentPerformanceRow
                    {
                        StudentId = Text(reader["student_id"]),
                        Name = Text(reader["student_name"]),
                        Programme = Text(reader["programme_code"]),
                        Semester = Int(reader["semester_no"]),
                        CourseCode = Text(reader["course_code"]),
                        CourseName = Text(reader["course_name"]),
                        LetterGrade = Text(reader["letter_grade"]),
                        CurrentGpa = Decimal(reader["current_gpa"]),
                        Cgpa = Decimal(reader["cgpa"]),
                        FailedCourses = Int(reader["failed_count"]),
                        Attendance = Decimal(reader["attendance"]),
                        Standing = Text(reader["standing"])
                    });
                }
            }
            return rows;
        }

        public List<AdminCourseMetric> GetCourseMetrics()
        {
            AcademicIntakeSchema.Ensure();
            const string sql =
                "SELECT co.offer_id, co.session_id, COALESCE(i.intake_name + ' - ' + sem.semester, co.semester) AS offering_semester, c.course_code, c.course_name, c.credit_hour, p.programme_code, " +
                "MAX(COALESCE(st.semester, 0)) AS semester_no, l.lecturer_name, " +
                "COUNT(DISTINCT e.student_id) AS enrolled, " +
                "COUNT(DISTINCT ats.session_id) AS sessions_held, " +
                "SUM(CASE WHEN ar.status = 'PRESENT' THEN 1 ELSE 0 END) AS present_count, " +
                "SUM(CASE WHEN ar.status = 'LATE' THEN 1 ELSE 0 END) AS late_count, " +
                "SUM(CASE WHEN ar.status = 'ABSENT' THEN 1 ELSE 0 END) AS absent_count, " +
                "AVG(CAST(sub.marks_obtained AS float)) AS avg_marks " +
                "FROM COURSE_OFFERINGS co " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "JOIN PROGRAMMES p ON p.programme_id = c.programme_id " +
                "LEFT JOIN ACADEMIC_SESSIONS sem ON sem.session_id=co.session_id " +
                "LEFT JOIN INTAKES i ON i.intake_id=sem.intake_id " +
                "LEFT JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id " +
                "LEFT JOIN ENROLLMENTS e ON e.offer_id = co.offer_id AND e.status IN ('ENROLLED','PENDING') " +
                "LEFT JOIN STUDENTS st ON st.student_id = e.student_id " +
                "LEFT JOIN ATTENDANCE_SESSIONS ats ON ats.offer_id = co.offer_id " +
                "LEFT JOIN ATTENDANCE_RECORDS ar ON ar.session_id = ats.session_id AND ar.student_id = e.student_id " +
                "LEFT JOIN ASSIGNMENTS a ON a.offer_id = co.offer_id " +
                "LEFT JOIN SUBMISSIONS sub ON sub.assignment_id = a.assignment_id AND sub.student_id = e.student_id " +
                "GROUP BY co.offer_id, co.session_id, i.intake_name, sem.semester, co.semester, c.course_code, c.course_name, c.credit_hour, p.programme_code, l.lecturer_name " +
                "ORDER BY c.course_code";

            var list = new List<AdminCourseMetric>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var present = Int(reader["present_count"]);
                    var late = Int(reader["late_count"]);
                    var absent = Int(reader["absent_count"]);
                    var avgMarks = Decimal(reader["avg_marks"]);
                    var passRate = EstimatePassRate(avgMarks);

                    list.Add(new AdminCourseMetric
                    {
                        OfferId = Int(reader["offer_id"]),
                        SessionId = Text(reader["session_id"]),
                        OfferingSemester = Text(reader["offering_semester"]),
                        Code = Text(reader["course_code"]),
                        Title = Text(reader["course_name"]),
                        CreditHours = Int(reader["credit_hour"]),
                        Programme = Text(reader["programme_code"]),
                        Semester = Int(reader["semester_no"]),
                        Lecturer = Text(reader["lecturer_name"]),
                        Enrolled = Int(reader["enrolled"]),
                        SessionsHeld = Int(reader["sessions_held"]),
                        Present = present,
                        Absent = absent,
                        AverageAttendance = Percentage(present + late, present + late + absent),
                        AverageMarks = avgMarks,
                        PassRate = passRate,
                        Passed = 0,
                        Failed = 0
                    });
                }
            }

            FillPassFail(list);
            return list;
        }

        public List<AdminCourseStudentMetric> GetCourseStudents(string courseCode)
        {
            const string sql =
                "SELECT s.student_id, s.student_name, p.programme_code, ISNULL(s.semester, 0) AS semester_no, " +
                "SUM(CASE WHEN ar.status = 'PRESENT' THEN 1 ELSE 0 END) AS present_count, " +
                "SUM(CASE WHEN ar.status = 'LATE' THEN 1 ELSE 0 END) AS late_count, " +
                "SUM(CASE WHEN ar.status = 'ABSENT' THEN 1 ELSE 0 END) AS absent_count, " +
                "AVG(CAST(sub.marks_obtained AS float)) AS avg_marks " +
                "FROM COURSE_OFFERINGS co " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "JOIN ENROLLMENTS e ON e.offer_id = co.offer_id AND e.status IN ('ENROLLED','PENDING') " +
                "JOIN STUDENTS s ON s.student_id = e.student_id " +
                "JOIN PROGRAMMES p ON p.programme_id = s.programme_id " +
                "LEFT JOIN ATTENDANCE_SESSIONS ats ON ats.offer_id = co.offer_id " +
                "LEFT JOIN ATTENDANCE_RECORDS ar ON ar.session_id = ats.session_id AND ar.student_id = s.student_id " +
                "LEFT JOIN ASSIGNMENTS a ON a.offer_id = co.offer_id " +
                "LEFT JOIN SUBMISSIONS sub ON sub.assignment_id = a.assignment_id AND sub.student_id = s.student_id " +
                "WHERE c.course_code = @code " +
                "GROUP BY s.student_id, s.student_name, p.programme_code, ISNULL(s.semester, 0) " +
                "ORDER BY s.student_name";

            var rows = new List<AdminCourseStudentMetric>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@code", courseCode ?? "");
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var present = Int(reader["present_count"]);
                        var late = Int(reader["late_count"]);
                        var absent = Int(reader["absent_count"]);
                        var marks = Decimal(reader["avg_marks"]);
                        rows.Add(new AdminCourseStudentMetric
                        {
                            Id = Text(reader["student_id"]),
                            Name = Text(reader["student_name"]),
                            Programme = Text(reader["programme_code"]),
                            Semester = Int(reader["semester_no"]),
                            Present = present,
                            Absent = absent,
                            AttendancePercentage = Percentage(present + late, present + late + absent),
                            Marks = marks,
                            Grade = LetterGrade(marks)
                        });
                    }
                }
            }

            return rows;
        }

        private void FillPassFail(List<AdminCourseMetric> courses)
        {
            foreach (var course in courses)
            {
                var students = GetCourseStudents(course.Code);
                course.Passed = students.Count(s => s.Grade != "F");
                course.Failed = students.Count(s => s.Grade == "F");
                course.PassRate = Percentage(course.Passed, course.Passed + course.Failed);
                if (students.Count > 0) course.AverageMarks = students.Average(s => s.Marks);
            }
        }

        private static List<AdminDashboardNotice> QueryRecentNotices(SqlConnection conn)
        {
            const string sql =
                "SELECT TOP 4 academic_year, semester, start_date, end_date " +
                "FROM ACADEMIC_SESSIONS " +
                "WHERE end_date >= DATEADD(day, -7, GETDATE()) " +
                "ORDER BY start_date ASC";
            var notices = new List<AdminDashboardNotice>();
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var academicYear = Text(reader["academic_year"]);
                    var semester = Text(reader["semester"]);
                    var start = Convert.ToDateTime(reader["start_date"]);
                    var end = Convert.ToDateTime(reader["end_date"]);
                    var today = DateTime.Today;
                    string title, description;
                    DateTime date;
                    if (today < start.Date)
                    {
                        title = semester + " " + academicYear + " Starting Soon";
                        description = "Semester begins " + start.ToString("d MMM yyyy") + ". Ensure all course offerings and enrolments are finalized.";
                        date = start;
                    }
                    else if (today <= end.Date)
                    {
                        var daysLeft = (end.Date - today).Days;
                        title = semester + " " + academicYear + " In Progress";
                        description = "Currently in session, ending " + end.ToString("d MMM yyyy") + ". " + daysLeft + " day" + (daysLeft != 1 ? "s" : "") + " remaining.";
                        date = end;
                    }
                    else
                    {
                        title = semester + " " + academicYear + " Recently Ended";
                        description = "Semester ended " + end.ToString("d MMM yyyy") + ". Final grades and results may still require review.";
                        date = end;
                    }
                    notices.Add(new AdminDashboardNotice { Title = title, Description = description, Date = date });
                }
            }
            return notices;
        }

        private static List<ProgrammeEnrolment> QueryProgrammeEnrolment(SqlConnection conn)
        {
            const string sql =
                "SELECT p.programme_code, p.programme_name, COUNT(s.student_id) AS students " +
                "FROM PROGRAMMES p LEFT JOIN STUDENTS s ON s.programme_id = p.programme_id " +
                "GROUP BY p.programme_code, p.programme_name ORDER BY students DESC, p.programme_code";
            var list = new List<ProgrammeEnrolment>();
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new ProgrammeEnrolment { Code = Text(reader["programme_code"]), Name = Text(reader["programme_name"]), Students = Int(reader["students"]) });
                }
            }
            return list;
        }

        private static List<AdminOption> QueryOptions(SqlConnection conn, string sql)
        {
            var list = new List<AdminOption>();
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new AdminOption { Value = Text(reader.GetValue(0)), Text = Text(reader.GetValue(1)) });
                }
            }
            return list;
        }

        private static List<AdminOption> QuerySingleColumnOptions(SqlConnection conn, string sql)
        {
            var list = new List<AdminOption>();
            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var value = Text(reader.GetValue(0));
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        list.Add(new AdminOption { Value = TitleLookup(value), Text = TitleLookup(value) });
                    }
                }
            }
            return list;
        }

        private static void EnsureOptions(List<AdminOption> options, params string[] values)
        {
            foreach (var value in values)
            {
                if (!options.Any(o => string.Equals(o.Value, value, StringComparison.OrdinalIgnoreCase)))
                {
                    options.Add(new AdminOption { Value = value, Text = value });
                }
            }
        }

        private static string TitleLookup(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            if (value.All(ch => !char.IsLetter(ch) || char.IsUpper(ch))) return Title(value);
            return value.Trim();
        }

        private static AdminPerformanceSummary QueryPerformance(SqlConnection conn)
        {
            var summary = new AdminPerformanceSummary();
            summary.AverageCgpa = DecimalScalar(conn, "SELECT AVG(CAST(grade_point AS float)) FROM GRADES");
            summary.AverageAttendance = DecimalScalar(conn,
                "SELECT AVG(CASE WHEN x.total_count = 0 THEN NULL ELSE CAST(x.present_count AS float) * 100.0 / x.total_count END) " +
                "FROM (SELECT ats.offer_id, SUM(CASE WHEN ar.status IN ('PRESENT','LATE') THEN 1 ELSE 0 END) present_count, COUNT(ar.attendance_id) total_count " +
                "FROM ATTENDANCE_SESSIONS ats LEFT JOIN ATTENDANCE_RECORDS ar ON ar.session_id = ats.session_id GROUP BY ats.offer_id) x");

            var passed = Count(conn, "SELECT COUNT(*) FROM GRADES WHERE letter_grade <> 'F'");
            var failed = Count(conn, "SELECT COUNT(*) FROM GRADES WHERE letter_grade = 'F'");
            summary.PassRate = Percentage(passed, passed + failed);
            summary.FailRate = Percentage(failed, passed + failed);
            return summary;
        }

        private static int Count(SqlConnection conn, string sql)
        {
            using (var cmd = new SqlCommand(sql, conn))
            {
                var value = cmd.ExecuteScalar();
                return value == DBNull.Value || value == null ? 0 : Convert.ToInt32(value);
            }
        }

        private static bool Exists(SqlConnection conn, string sql, Action<SqlCommand> addParameters)
        {
            using (var cmd = new SqlCommand(sql, conn))
            {
                addParameters(cmd);
                return cmd.ExecuteScalar() != null;
            }
        }

        private static decimal DecimalScalar(SqlConnection conn, string sql)
        {
            using (var cmd = new SqlCommand(sql, conn))
            {
                return Decimal(cmd.ExecuteScalar());
            }
        }

        private static string Text(object value) { return value == DBNull.Value || value == null ? "" : Convert.ToString(value); }
        private static int Int(object value) { return value == DBNull.Value || value == null ? 0 : Convert.ToInt32(value); }
        private static decimal Decimal(object value) { return value == DBNull.Value || value == null ? 0m : Convert.ToDecimal(value); }
        private static decimal Percentage(decimal value, decimal total) { return total <= 0 ? 0 : Math.Round(value * 100m / total, 1); }
        private static decimal EstimatePassRate(decimal avgMarks) { return avgMarks <= 0 ? 0 : avgMarks >= 50 ? 100 : 0; }

        private static string LetterGrade(decimal marks)
        {
            if (marks >= 90) return "A+";
            if (marks >= 80) return "A";
            if (marks >= 75) return "A-";
            if (marks >= 70) return "B+";
            if (marks >= 65) return "B";
            if (marks >= 60) return "B-";
            if (marks >= 55) return "C+";
            if (marks >= 50) return "C";
            return "F";
        }

        private static string Title(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Substring(0, 1).ToUpperInvariant() + value.Substring(1).ToLowerInvariant();
        }

        private static int NextInt(SqlConnection conn, SqlTransaction tx, string table, string column)
        {
            using (var cmd = new SqlCommand("SELECT ISNULL(MAX(" + column + "), 0) + 1 FROM " + table, conn, tx))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private static string UniqueUsername(SqlConnection conn, SqlTransaction tx, string email, int userId)
        {
            var baseName = (email ?? "").Split('@')[0];
            if (string.IsNullOrWhiteSpace(baseName)) baseName = "user";
            var username = baseName;
            using (var cmd = new SqlCommand("SELECT 1 FROM USERS WHERE username = @username", conn, tx))
            {
                cmd.Parameters.AddWithValue("@username", username);
                if (cmd.ExecuteScalar() == null) return username;
            }
            return username + userId;
        }

        private static string ResolveProgramme(SqlConnection conn, SqlTransaction tx, string value)
        {
            using (var cmd = new SqlCommand("SELECT TOP 1 programme_id FROM PROGRAMMES WHERE programme_id = @v OR programme_code = @v ORDER BY programme_id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@v", (value ?? "").Trim());
                var found = cmd.ExecuteScalar();
                if (found != null) return Convert.ToString(found);
            }
            using (var cmd = new SqlCommand("SELECT TOP 1 programme_id FROM PROGRAMMES ORDER BY programme_id", conn, tx))
            {
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        private static string ResolveDepartment(SqlConnection conn, SqlTransaction tx, string value)
        {
            using (var cmd = new SqlCommand(
                "SELECT TOP 1 COALESCE(p.department_id, d.department_id) " +
                "FROM DEPARTMENTS d FULL OUTER JOIN PROGRAMMES p ON p.department_id = d.department_id " +
                "WHERE d.department_id = @v OR p.programme_id = @v OR p.programme_code = @v", conn, tx))
            {
                cmd.Parameters.AddWithValue("@v", (value ?? "").Trim());
                var found = cmd.ExecuteScalar();
                if (found != null && found != DBNull.Value) return Convert.ToString(found);
            }
            using (var cmd = new SqlCommand("SELECT TOP 1 department_id FROM DEPARTMENTS ORDER BY department_id", conn, tx))
            {
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        private static bool IsDepartmentActive(SqlConnection conn, SqlTransaction tx, string departmentId)
        {
            using (var cmd = new SqlCommand("SELECT 1 FROM DEPARTMENTS WHERE department_id=@id AND (status IS NULL OR UPPER(status) <> 'INACTIVE')", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", departmentId ?? "");
                return cmd.ExecuteScalar() != null;
            }
        }

        private static bool CanProgrammeHaveActiveCourses(SqlConnection conn, SqlTransaction tx, string programmeId)
        {
            using (var cmd = new SqlCommand(
                "SELECT 1 FROM PROGRAMMES p JOIN DEPARTMENTS d ON d.department_id=p.department_id " +
                "WHERE p.programme_id=@id AND (p.status IS NULL OR UPPER(p.status) <> 'INACTIVE') " +
                "AND (d.status IS NULL OR UPPER(d.status) <> 'INACTIVE')", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", programmeId ?? "");
                return cmd.ExecuteScalar() != null;
            }
        }

        private static bool CanCourseBeActive(SqlConnection conn, SqlTransaction tx, string courseId)
        {
            using (var cmd = new SqlCommand(
                "SELECT 1 FROM COURSES c JOIN PROGRAMMES p ON p.programme_id=c.programme_id JOIN DEPARTMENTS d ON d.department_id=p.department_id " +
                "WHERE c.course_id=@id AND (c.status IS NULL OR UPPER(c.status) <> 'INACTIVE') " +
                "AND (p.status IS NULL OR UPPER(p.status) <> 'INACTIVE') " +
                "AND (d.status IS NULL OR UPPER(d.status) <> 'INACTIVE')", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", courseId ?? "");
                return cmd.ExecuteScalar() != null;
            }
        }

        private static void SetCoursesInactiveForProgramme(SqlConnection conn, SqlTransaction tx, string programmeIdOrCode)
        {
            using (var cmd = new SqlCommand(
                "UPDATE COURSES SET status='INACTIVE' WHERE programme_id IN " +
                "(SELECT programme_id FROM PROGRAMMES WHERE programme_id=@id OR programme_code=@id)", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", programmeIdOrCode ?? "");
                cmd.ExecuteNonQuery();
            }
        }

        private static void SetProgrammesAndCoursesInactiveForDepartment(SqlConnection conn, SqlTransaction tx, string departmentId)
        {
            using (var cmd = new SqlCommand(
                "UPDATE PROGRAMMES SET status='INACTIVE' WHERE department_id=@id; " +
                "UPDATE c SET c.status='INACTIVE' FROM COURSES c JOIN PROGRAMMES p ON p.programme_id=c.programme_id WHERE p.department_id=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", departmentId ?? "");
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Opens its own connection; only called from the background email task
        /// after CreateUser's transaction has already committed and closed.</summary>
        private static string ProgrammeNameStandalone(string programmeId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand("SELECT programme_name FROM PROGRAMMES WHERE programme_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", programmeId);
                var found = cmd.ExecuteScalar();
                return found == null || found == DBNull.Value ? programmeId : Convert.ToString(found);
            }
        }

        /// <summary>Opens its own connection; only called from the background email task
        /// after CreateUser's transaction has already committed and closed.</summary>
        private static string DepartmentNameStandalone(string departmentId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand("SELECT department_name FROM DEPARTMENTS WHERE department_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", departmentId);
                var found = cmd.ExecuteScalar();
                return found == null || found == DBNull.Value ? departmentId : Convert.ToString(found);
            }
        }

        /// <summary>12-char random password meeting AccountPasswordService strength rules
        /// (upper, lower, digit, symbol) for new-account emails.</summary>
        private static string GenerateTempPassword()
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lower = "abcdefghijkmnopqrstuvwxyz";
            const string digits = "23456789";
            const string symbols = "!@#$%&*";
            const string all = upper + lower + digits + symbols;

            var bytes = new byte[12];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);

            var chars = new char[12];
            chars[0] = upper[bytes[0] % upper.Length];
            chars[1] = lower[bytes[1] % lower.Length];
            chars[2] = digits[bytes[2] % digits.Length];
            chars[3] = symbols[bytes[3] % symbols.Length];
            for (int i = 4; i < chars.Length; i++)
                chars[i] = all[bytes[i] % all.Length];

            return new string(chars);
        }

        private static string ResolveCourse(SqlConnection conn, SqlTransaction tx, string value)
        {
            using (var cmd = new SqlCommand("SELECT TOP 1 course_id FROM COURSES WHERE course_id = @v OR course_code = @v ORDER BY course_id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@v", CleanCode(value));
                var found = cmd.ExecuteScalar();
                if (found != null) return Convert.ToString(found);
            }
            throw new ArgumentException("Course not found.");
        }

        private static string ResolveLecturer(SqlConnection conn, SqlTransaction tx, string value)
        {
            using (var cmd = new SqlCommand("SELECT TOP 1 lecturer_id FROM LECTURERS WHERE lecturer_id = @v OR lecturer_name = @v OR lecturer_email = @v ORDER BY lecturer_id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@v", (value ?? "").Trim());
                var found = cmd.ExecuteScalar();
                if (found != null) return Convert.ToString(found);
            }
            throw new ArgumentException("Lecturer not found.");
        }

        private static string NormalizeStatus(string status)
        {
            var value = (status ?? "ACTIVE").Trim().ToUpperInvariant();
            if (value != "ACTIVE" && value != "PENDING" && value != "INACTIVE") return "ACTIVE";
            return value;
        }

        private static string NormalizeAcademicSessionStatus(string status)
        {
            var value = Title((status ?? "Scheduled").Trim());
            if (value != "Upcoming" && value != "Scheduled" && value != "Completed" && value != "Active") return "Scheduled";
            return value;
        }

        private static string Sha256Hex(string value)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value ?? ""));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private static string EventStatus(DateTime start, DateTime end)
        {
            var today = DateTime.Today;
            if (today < start.Date) return "Upcoming";
            if (today > end.Date) return "Completed";
            return "Active";
        }

        private static string CleanCode(string value)
        {
            var text = (value ?? "").Trim();
            var cut = text.IndexOf(' ');
            return cut > 0 ? text.Substring(0, cut).Trim() : text;
        }

        private static string CleanSessionId(string value)
        {
            return (value ?? "").Replace("-START", "").Replace("-END", "").Trim();
        }

        // Always increases, regardless of whether the intake/semester combination
        // already has a session — ignores any non-numeric legacy session ids.
        private static string NextSessionId(SqlConnection conn)
        {
            using (var cmd = new SqlCommand(
                "SELECT ISNULL(MAX(TRY_CAST(session_id AS INT)), 0) + 1 FROM ACADEMIC_SESSIONS", conn))
            {
                return Convert.ToInt32(cmd.ExecuteScalar()).ToString();
            }
        }

        private static void SplitSemesterLabel(string label, out string academicYear, out string semester)
        {
            var text = (label ?? "").Trim();
            academicYear = "";
            semester = text;
            var parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime intakeMonth;
            if (parts.Length >= 4 && DateTime.TryParseExact(
                parts[0], "MMMM", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AllowWhiteSpaces, out intakeMonth))
            {
                academicYear = parts[1];
                semester = string.Join(" ", parts.Skip(2));
            }
            else if (parts.Length >= 3)
            {
                academicYear = parts[0];
                semester = string.Join(" ", parts.Skip(1));
            }
        }
    }

    public class AdminUserSaveRequest
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PersonalEmail { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string ProgrammeOrDepartment { get; set; }
        public string Status { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
    }

    public class AdminOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class AdminLookupData
    {
        public List<AdminOption> Programmes { get; set; } = new List<AdminOption>();
        public List<AdminOption> Departments { get; set; } = new List<AdminOption>();
        public List<AdminOption> Lecturers { get; set; } = new List<AdminOption>();
        public List<AdminOption> AcademicSessions { get; set; } = new List<AdminOption>();
        public List<AdminOption> AcademicYears { get; set; } = new List<AdminOption>();
        public List<AdminOption> Semesters { get; set; } = new List<AdminOption>();
        public List<AdminOption> StudentSemesters { get; set; } = new List<AdminOption>();
        public List<AdminOption> EducationLevels { get; set; } = new List<AdminOption>();
        public List<AdminOption> ProgrammeStatuses { get; set; } = new List<AdminOption>();
        public List<AdminOption> CourseStatuses { get; set; } = new List<AdminOption>();
        public List<AdminOption> UserStatuses { get; set; } = new List<AdminOption>();
        public List<AdminOption> UserRoles { get; set; } = new List<AdminOption>();
        public List<AdminOption> EventTypes { get; set; } = new List<AdminOption>();
        public List<AdminOption> EventStatuses { get; set; } = new List<AdminOption>();
        public List<AdminOption> AcademicStatuses { get; set; } = new List<AdminOption>();
    }

    public class AdminProgrammeSaveRequest
    {
        public string Code { get; set; }
        public string Department { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Duration { get; set; }
        public int Semesters { get; set; }
        public string Status { get; set; }
    }

    public class AdminDepartmentSaveRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }

    public class AdminCourseSaveRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Programme { get; set; }
        public int CreditHours { get; set; }
        public string Prerequisites { get; set; }
        public string Status { get; set; }
    }

    public class AdminCourseAssignmentSaveRequest
    {
        public int OfferId { get; set; }
        public string CourseCode { get; set; }
        public string Lecturer { get; set; }
        public string Semester { get; set; }
        public string AcademicYear { get; set; }
        public string SessionId { get; set; }
        public string Status { get; set; }
    }

    public class AdminAcademicSessionSaveRequest
    {
        public string SessionId { get; set; }
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
        public string IntakeId { get; set; }
        public string IntakeName { get; set; }
        public string IntakeMonth { get; set; }
        public string SemesterLabel { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
        public string MinCredits { get; set; }
        public string MaxCredits { get; set; }
    }

    public class AdminAddDropRequestRow
    {
        public int RequestId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string Programme { get; set; }
        public int Semester { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
    }

    public class AdminCalendarEventRow
    {
        public string Id { get; set; }
        public string SessionId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SessionStartDate { get; set; }
        public DateTime SessionEndDate { get; set; }
        public string Semester { get; set; }
        public string AcademicYear { get; set; }
        public string SemesterName { get; set; }
        public string IntakeId { get; set; }
        public string IntakeName { get; set; }
        public DateTime IntakeMonth { get; set; }
        public DateTime RegistrationStart { get; set; }
        public DateTime RegistrationEnd { get; set; }
        public DateTime AddDropEnd { get; set; }
        public int? MinCredits { get; set; }
        public int? MaxCredits { get; set; }
        public string Status { get; set; }
    }

    public class AdminIntakeRow
    {
        public string IntakeId { get; set; }
        public string IntakeName { get; set; }
        public DateTime IntakeMonth { get; set; }
        public string Status { get; set; }
        public int SemesterCount { get; set; }
    }

    public class AdminStudentDetailSummary
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Programme { get; set; }
        public string ProgrammeCode { get; set; }
        public string Standing { get; set; }
        public string Session { get; set; }
        public int Semester { get; set; }
        public decimal Cgpa { get; set; }
        public decimal CurrentGpa { get; set; }
        public int CompletedCourses { get; set; }
        public int CreditHours { get; set; }
        public decimal Attendance { get; set; }
        public List<AdminStudentSemesterGpa> CgpaTrend { get; set; } = new List<AdminStudentSemesterGpa>();
        public List<AdminStudentSemesterAttendance> AttendanceBySemester { get; set; } = new List<AdminStudentSemesterAttendance>();
        public List<AdminStudentSemesterResult> SemesterResults { get; set; } = new List<AdminStudentSemesterResult>();
    }

    public class AdminStudentSemesterGpa
    {
        public string SemesterLabel { get; set; }
        public decimal SemesterGpa { get; set; }
        public decimal CumulativeGpa { get; set; }
    }

    public class AdminStudentSemesterAttendance
    {
        public string SemesterLabel { get; set; }
        public string AcademicYear { get; set; }
        public decimal Rate { get; set; }
        public List<AdminStudentCourseAttendance> Courses { get; set; } = new List<AdminStudentCourseAttendance>();
    }

    public class AdminStudentCourseAttendance
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Present { get; set; }
        public int Total { get; set; }
        public decimal Rate { get; set; }
    }

    public class AdminStudentSemesterResult
    {
        public string SemesterLabel { get; set; }
        public string AcademicYear { get; set; }
        public decimal SemGpa { get; set; }
        public decimal Cgpa { get; set; }
        public List<AdminStudentCourseGrade> Courses { get; set; } = new List<AdminStudentCourseGrade>();
    }

    public class AdminStudentCourseGrade
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public string LetterGrade { get; set; }
        public decimal GradePoint { get; set; }
    }

    public class AdminGradeBand
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string Color { get; set; }
        public int Count { get; set; }
        public decimal Percent { get; set; }
    }

    public class AdminProgrammePassRate
    {
        public string Code { get; set; }
        public decimal PassRate { get; set; }
    }

    public class AdminDashboardData
    {
        public int TotalStudents { get; set; }
        public int TotalLecturers { get; set; }
        public int PendingRequests { get; set; }
        public int AtRiskStudents { get; set; }
        public int CriticalAtRiskStudents { get; set; }
        public decimal AverageAttendance { get; set; }
        public decimal AverageCgpa { get; set; }
        public decimal PassRate { get; set; }
        public decimal FailRate { get; set; }
        public List<ProgrammeEnrolment> Programmes { get; set; }
        public List<AdminDashboardNotice> RecentNotices { get; set; } = new List<AdminDashboardNotice>();
    }

    public class AdminDashboardNotice
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }

    public class ProgrammeEnrolment
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Students { get; set; }
    }

    public class AdminPerformanceSummary
    {
        public decimal AverageAttendance { get; set; }
        public decimal AverageCgpa { get; set; }
        public decimal PassRate { get; set; }
        public decimal FailRate { get; set; }
    }

    public class AdminUserRow
    {
        public int UserId { get; set; }
        public string ProfileId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string Programme { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class AdminProgrammeRow
    {
        public string Id { get; set; }
        public string DepartmentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Duration { get; set; }
        public int Semesters { get; set; }
        public string Status { get; set; }
        public int CourseCount { get; set; }
        public int StudentCount { get; set; }
    }

    public class AdminDepartmentRow
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int ProgrammeCount { get; set; }
        public int LecturerCount { get; set; }
    }

    public class AdminStudentPerformanceRow
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string Programme { get; set; }
        public int Semester { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string LetterGrade { get; set; }
        public decimal CurrentGpa { get; set; }
        public decimal Cgpa { get; set; }
        public int FailedCourses { get; set; }
        public decimal Attendance { get; set; }
        public string Standing { get; set; }
    }

    public class AdminCourseCatalogRow
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Programme { get; set; }
        public int CreditHours { get; set; }
        public string Prerequisites { get; set; }
        public string Lecturer { get; set; }
        public string Status { get; set; }
    }

    public class AdminCourseMetric
    {
        public int OfferId { get; set; }
        public string SessionId { get; set; }
        public string OfferingSemester { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public int CreditHours { get; set; }
        public string Programme { get; set; }
        public int Semester { get; set; }
        public string Lecturer { get; set; }
        public int Enrolled { get; set; }
        public int SessionsHeld { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public decimal AverageAttendance { get; set; }
        public decimal AverageMarks { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public decimal PassRate { get; set; }
    }

    public class AdminCourseStudentMetric
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Programme { get; set; }
        public int Semester { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public decimal AttendancePercentage { get; set; }
        public decimal Marks { get; set; }
        public string Grade { get; set; }
    }
}
