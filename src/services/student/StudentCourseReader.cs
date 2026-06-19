using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class StudentCourseReader
    {
        public static List<StudentCourseCard> GetCourses(UserContext user, string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId)) return new List<StudentCourseCard>();

            var current = AcademicTermReader.GetCurrentTerm();
            var currentLabel = TermLabel(current);
            const string sql =
                "SELECT e.offer_id, co.course_id, c.course_code, c.course_name, c.credit_hour, c.colour, " +
                "ISNULL(l.lecturer_name, '') AS lecturer_name, ISNULL(e.semester, co.academic_year + ' ' + co.semester) AS semester_name " +
                "FROM ENROLLMENTS e " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = e.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "LEFT JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id " +
                "WHERE e.student_id = @studentId AND e.status IN ('ENROLLED', 'PENDING') " +
                "ORDER BY co.academic_year DESC, co.semester, c.course_code";

            var courses = new List<StudentCourseCard>();
            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanViewStudent(conn, user, studentId)) return courses;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var code = Text(reader["course_code"]);
                            var semesterName = Text(reader["semester_name"]);
                            courses.Add(new StudentCourseCard
                            {
                                OfferingId = IntValue(reader["offer_id"]),
                                CourseId = Text(reader["course_id"]),
                                CourseCode = code,
                                CourseName = Text(reader["course_name"]),
                                LecturerName = LecturerOrFallback(Text(reader["lecturer_name"])),
                                CreditHours = IntValue(reader["credit_hour"]),
                                SemesterName = semesterName,
                                IsCurrent = IsSameTerm(semesterName, currentLabel),
                                Color = ColorOrFallback(Text(reader["colour"]), code)
                            });
                        }
                    }
                }
            }
            return courses;
        }

        public static StudentCourseHeader GetCourseHeader(UserContext user, int offeringId)
        {
            if (user == null) return null;

            var sql =
                "SELECT co.offer_id, c.course_id, c.course_code, c.course_name, c.credit_hour, c.prerequisites, c.colour, " +
                "ISNULL(p.education_level, '') AS education_level, ISNULL(l.lecturer_name, '') AS lecturer_name, " +
                "(SELECT COUNT(*) FROM MODULES m WHERE m.offer_id = co.offer_id) AS module_count " +
                "FROM COURSE_OFFERINGS co " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "LEFT JOIN PROGRAMMES p ON p.programme_id = c.programme_id " +
                "LEFT JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id " +
                "WHERE co.offer_id = @offerId AND " + ServiceAccess.VisibleOfferScope("co");

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                cmd.Parameters.AddWithValue("@offerId", offeringId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    var code = Text(reader["course_code"]);
                    var name = Text(reader["course_name"]);
                    var credits = IntValue(reader["credit_hour"]);
                    return new StudentCourseHeader
                    {
                        OfferingId = IntValue(reader["offer_id"]),
                        CourseId = Text(reader["course_id"]),
                        CourseCode = code,
                        CourseName = name,
                        Description = "Course materials, assignments, announcements, and grades for " + name + ".",
                        LecturerName = LecturerOrFallback(Text(reader["lecturer_name"])),
                        CreditHours = credits,
                        ModuleCount = IntValue(reader["module_count"]),
                        Color = ColorOrFallback(Text(reader["colour"]), code),
                        LevelLabel = string.IsNullOrWhiteSpace(Text(reader["education_level"])) ? "Course" : Text(reader["education_level"]),
                        Mode = "On campus",
                        ContactHours = Math.Max(credits * 2, credits).ToString() + " hours / week",
                        Prerequisites = string.IsNullOrWhiteSpace(Text(reader["prerequisites"])) ? "None" : Text(reader["prerequisites"]),
                        Textbook = "Refer to lecturer materials",
                        OfficeHours = "By appointment"
                    };
                }
            }
        }

        public static List<StudentCourseModule> GetCourseModules(UserContext user, int offeringId)
        {
            if (user == null) return new List<StudentCourseModule>();

            var sql =
                "SELECT m.module_id, m.module_title, CAST(m.module_description AS varchar(max)) AS module_description, m.week_number, " +
                "mat.material_id, mat.title AS material_title, mat.file_url " +
                "FROM MODULES m " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = m.offer_id " +
                "LEFT JOIN MATERIALS mat ON mat.module_id = m.module_id " +
                "WHERE m.offer_id = @offerId AND " + ServiceAccess.VisibleOfferScope("co") + " " +
                "ORDER BY m.week_number, m.module_id, mat.uploaded_at DESC, mat.material_id DESC";

            var modules = new List<StudentCourseModule>();
            var byId = new Dictionary<string, StudentCourseModule>(StringComparer.OrdinalIgnoreCase);
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                cmd.Parameters.AddWithValue("@offerId", offeringId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var moduleId = Text(reader["module_id"]);
                        StudentCourseModule module;
                        if (!byId.TryGetValue(moduleId, out module))
                        {
                            module = new StudentCourseModule
                            {
                                ModuleId = moduleId,
                                Week = Math.Max(1, IntValue(reader["week_number"])),
                                Title = Text(reader["module_title"]),
                                Description = Text(reader["module_description"]),
                                Items = new List<StudentModuleMaterial>()
                            };
                            byId[moduleId] = module;
                            modules.Add(module);
                        }

                        if (reader["material_id"] != DBNull.Value)
                        {
                            var fileUrl = Text(reader["file_url"]);
                            module.Items.Add(new StudentModuleMaterial
                            {
                                MaterialId = IntValue(reader["material_id"]),
                                Title = Text(reader["material_title"]),
                                FileUrl = fileUrl,
                                FileType = FileTypeFromUrl(fileUrl),
                                FileSizeBytes = null
                            });
                        }
                    }
                }
            }
            return modules;
        }

        public static List<StudentLearningOutcome> GetLearningOutcomes(UserContext user, string courseId)
        {
            if (!IsStudent(user)) return new List<StudentLearningOutcome>();
            var account = StudentProfileReader.GetAccount(user);
            var myCourses = account == null ? new List<StudentCourseCard>() : GetCourses(user, account.StudentId);
            var course = myCourses.FirstOrDefault(c => string.Equals(c.CourseId, courseId, StringComparison.OrdinalIgnoreCase));
            var name = course == null ? "the course" : course.CourseName;
            return new List<StudentLearningOutcome>
            {
                new StudentLearningOutcome { Text = "Explain the core concepts covered in " + name + "." },
                new StudentLearningOutcome { Text = "Apply course knowledge to practical academic and professional tasks." },
                new StudentLearningOutcome { Text = "Communicate solutions clearly using appropriate terminology and tools." }
            };
        }
    }
}
