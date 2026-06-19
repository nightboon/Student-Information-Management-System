using System;
using System.Data.SqlClient;

namespace src.services
{
    public static class ServiceAccess
    {
        public static void AddUserContextParameters(SqlCommand cmd, UserContext user)
        {
            cmd.Parameters.AddWithValue("@userId", user == null ? 0 : user.UserId);
            cmd.Parameters.AddWithValue("@role", user == null ? "" : user.NormalizedRole);
        }

        /// <summary>
        /// SQL boolean predicate selecting rows whose COURSE_OFFERINGS row (aliased
        /// <paramref name="offeringAlias"/>) is visible to the current user:
        /// ADMIN sees all, LECTURER sees offerings they teach, STUDENT sees offerings
        /// they are enrolled in (ENROLLED or PENDING).
        /// Bind @userId and @role with <see cref="AddUserContextParameters"/>.
        /// </summary>
        public static string VisibleOfferScope(string offeringAlias)
        {
            return
                "(@role = 'ADMIN' " +
                "OR (@role = 'LECTURER' AND EXISTS (SELECT 1 FROM LECTURERS l " +
                "WHERE l.user_id = @userId AND l.lecturer_id = " + offeringAlias + ".lecturer_id)) " +
                "OR (@role = 'STUDENT' AND EXISTS (SELECT 1 FROM STUDENTS s " +
                "JOIN ENROLLMENTS e ON e.student_id = s.student_id " +
                "WHERE s.user_id = @userId AND e.offer_id = " + offeringAlias + ".offer_id " +
                "AND e.status IN ('ENROLLED','PENDING'))))";
        }

        public static object DbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        }

        public static object DbValue(int? value)
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        public static object DbValue(decimal? value)
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        public static object DbValue(DateTime? value)
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        public static object DbValue(TimeSpan? value)
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        public static bool CanManageOffer(SqlConnection conn, UserContext user, int offerId)
        {
            if (user == null) return false;
            if (user.IsAdmin) return true;
            if (!user.IsLecturer) return false;

            const string sql =
                "SELECT 1 " +
                "FROM COURSE_OFFERINGS co " +
                "JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id " +
                "WHERE co.offer_id = @offerId AND l.user_id = @userId";

            return Exists(conn, sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@offerId", offerId);
                cmd.Parameters.AddWithValue("@userId", user.UserId);
            });
        }

        public static bool CanViewOffer(SqlConnection conn, UserContext user, int offerId)
        {
            if (user == null) return false;
            if (CanManageOffer(conn, user, offerId)) return true;
            if (!user.IsStudent) return false;

            const string sql =
                "SELECT 1 " +
                "FROM STUDENTS s " +
                "JOIN ENROLLMENTS e ON e.student_id = s.student_id " +
                "WHERE s.user_id = @userId AND e.offer_id = @offerId AND e.status = 'ENROLLED'";

            return Exists(conn, sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@userId", user.UserId);
                cmd.Parameters.AddWithValue("@offerId", offerId);
            });
        }

        public static bool CanManageCourse(SqlConnection conn, UserContext user, string courseId)
        {
            return user != null && user.IsAdmin;
        }

        public static bool CanViewStudent(SqlConnection conn, UserContext user, string studentId)
        {
            if (user == null || string.IsNullOrWhiteSpace(studentId)) return false;
            if (user.IsAdmin) return true;
            if (IsOwnStudent(conn, user, studentId)) return true;
            return IsLecturerTeachingStudent(conn, user, studentId);
        }

        public static bool CanManageStudentProfile(SqlConnection conn, UserContext user, string studentId)
        {
            if (user == null || string.IsNullOrWhiteSpace(studentId)) return false;
            return user.IsAdmin || IsOwnStudent(conn, user, studentId);
        }

        public static bool CanManageLecturerProfile(SqlConnection conn, UserContext user, string lecturerId)
        {
            if (user == null || string.IsNullOrWhiteSpace(lecturerId)) return false;
            if (user.IsAdmin) return true;
            if (!user.IsLecturer) return false;

            const string sql =
                "SELECT 1 FROM LECTURERS WHERE user_id = @userId AND lecturer_id = @lecturerId";

            return Exists(conn, sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@userId", user.UserId);
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId.Trim());
            });
        }

        public static bool CanManageAssignment(SqlConnection conn, UserContext user, int assignmentId)
        {
            return CanManageOfferByLookup(conn, user,
                "SELECT offer_id FROM ASSIGNMENTS WHERE assignment_id = @id", assignmentId);
        }

        public static bool CanManageAnnouncement(SqlConnection conn, UserContext user, int announcementId)
        {
            return CanManageOfferByLookup(conn, user,
                "SELECT offer_id FROM ANNOUNCEMENTS WHERE announcement_id = @id", announcementId);
        }

        public static bool CanManageGrade(SqlConnection conn, UserContext user, int gradeId)
        {
            return CanManageOfferByLookup(conn, user,
                "SELECT offer_id FROM GRADES WHERE grade_id = @id", gradeId);
        }

        public static bool CanViewAssignment(SqlConnection conn, UserContext user, int assignmentId)
        {
            return CanViewOfferByLookup(conn, user,
                "SELECT offer_id FROM ASSIGNMENTS WHERE assignment_id = @id", assignmentId);
        }

        public static bool CanManageModule(SqlConnection conn, UserContext user, string moduleId)
        {
            return CanManageOfferByLookup(conn, user,
                "SELECT offer_id FROM MODULES WHERE module_id = @id", moduleId);
        }

        public static bool CanViewModule(SqlConnection conn, UserContext user, string moduleId)
        {
            return CanViewOfferByLookup(conn, user,
                "SELECT offer_id FROM MODULES WHERE module_id = @id", moduleId);
        }

        public static bool CanManageMaterial(SqlConnection conn, UserContext user, int materialId)
        {
            return CanManageOfferByLookup(conn, user,
                "SELECT m.offer_id FROM MODULES m JOIN MATERIALS mat ON mat.module_id = m.module_id WHERE mat.material_id = @id", materialId);
        }

        public static bool CanManageAttendanceSession(SqlConnection conn, UserContext user, int sessionId)
        {
            return CanManageOfferByLookup(conn, user,
                "SELECT offer_id FROM ATTENDANCE_SESSIONS WHERE session_id = @id", sessionId);
        }

        public static bool CanViewAttendanceSession(SqlConnection conn, UserContext user, int sessionId)
        {
            return CanViewOfferByLookup(conn, user,
                "SELECT offer_id FROM ATTENDANCE_SESSIONS WHERE session_id = @id", sessionId);
        }

        public static bool CanManageSubmission(SqlConnection conn, UserContext user, int submissionId)
        {
            if (user == null) return false;
            if (user.IsAdmin) return true;

            const string sql =
                "SELECT sub.student_id, a.offer_id " +
                "FROM SUBMISSIONS sub " +
                "JOIN ASSIGNMENTS a ON a.assignment_id = sub.assignment_id " +
                "WHERE sub.submission_id = @submissionId";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@submissionId", submissionId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return false;
                    var studentId = reader["student_id"].ToString();
                    var offerId = (int)reader["offer_id"];
                    reader.Close();

                    if (user.IsLecturer) return CanManageOffer(conn, user, offerId);
                    return user.IsStudent && IsOwnStudent(conn, user, studentId);
                }
            }
        }

        public static bool CanAddSubmission(SqlConnection conn, UserContext user, string studentId, int assignmentId)
        {
            if (user == null || string.IsNullOrWhiteSpace(studentId)) return false;
            if (user.IsAdmin) return true;
            if (!user.IsStudent || !IsOwnStudent(conn, user, studentId)) return false;
            return CanViewAssignment(conn, user, assignmentId);
        }

        public static bool CanManageAttendanceRecord(SqlConnection conn, UserContext user, int attendanceId)
        {
            if (user == null) return false;
            if (user.IsAdmin) return true;

            const string sql =
                "SELECT ar.student_id, ats.offer_id " +
                "FROM ATTENDANCE_RECORDS ar " +
                "JOIN ATTENDANCE_SESSIONS ats ON ats.session_id = ar.session_id " +
                "WHERE ar.attendance_id = @attendanceId";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@attendanceId", attendanceId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return false;
                    var studentId = reader["student_id"].ToString();
                    var offerId = (int)reader["offer_id"];
                    reader.Close();

                    if (user.IsLecturer) return CanManageOffer(conn, user, offerId);
                    return user.IsStudent && IsOwnStudent(conn, user, studentId);
                }
            }
        }

        public static bool CanManageWarning(SqlConnection conn, UserContext user, int warningId)
        {
            return CanManageStudentByLookup(conn, user,
                "SELECT student_id FROM ACADEMIC_WARNINGS WHERE warning_id = @id", warningId);
        }

        public static bool CanManageStudentData(SqlConnection conn, UserContext user, string studentId)
        {
            if (user == null || string.IsNullOrWhiteSpace(studentId)) return false;
            return user.IsAdmin || IsLecturerTeachingStudent(conn, user, studentId);
        }

        public static bool IsStudentEnrolledInOffer(SqlConnection conn, string studentId, int offerId)
        {
            if (string.IsNullOrWhiteSpace(studentId)) return false;

            const string sql =
                "SELECT 1 FROM ENROLLMENTS " +
                "WHERE student_id = @studentId AND offer_id = @offerId AND status = 'ENROLLED'";

            return Exists(conn, sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@studentId", studentId.Trim());
                cmd.Parameters.AddWithValue("@offerId", offerId);
            });
        }

        public static bool IsOwnStudent(SqlConnection conn, UserContext user, string studentId)
        {
            if (user == null || !user.IsStudent || string.IsNullOrWhiteSpace(studentId)) return false;

            const string sql =
                "SELECT 1 FROM STUDENTS WHERE user_id = @userId AND student_id = @studentId";

            return Exists(conn, sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@userId", user.UserId);
                cmd.Parameters.AddWithValue("@studentId", studentId.Trim());
            });
        }

        private static bool IsLecturerTeachingStudent(SqlConnection conn, UserContext user, string studentId)
        {
            if (user == null || !user.IsLecturer || string.IsNullOrWhiteSpace(studentId)) return false;

            const string sql =
                "SELECT 1 " +
                "FROM LECTURERS l " +
                "JOIN COURSE_OFFERINGS co ON co.lecturer_id = l.lecturer_id " +
                "JOIN ENROLLMENTS e ON e.offer_id = co.offer_id " +
                "WHERE l.user_id = @userId AND e.student_id = @studentId";

            return Exists(conn, sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@userId", user.UserId);
                cmd.Parameters.AddWithValue("@studentId", studentId.Trim());
            });
        }

        private static bool CanManageOfferByLookup(SqlConnection conn, UserContext user, string lookupSql, object id)
        {
            var offerId = LookupInt(conn, lookupSql, id);
            return offerId.HasValue && CanManageOffer(conn, user, offerId.Value);
        }

        private static bool CanViewOfferByLookup(SqlConnection conn, UserContext user, string lookupSql, object id)
        {
            var offerId = LookupInt(conn, lookupSql, id);
            return offerId.HasValue && CanViewOffer(conn, user, offerId.Value);
        }

        private static bool CanManageStudentByLookup(SqlConnection conn, UserContext user, string lookupSql, object id)
        {
            var studentId = LookupString(conn, lookupSql, id);
            return !string.IsNullOrWhiteSpace(studentId) && CanManageStudentData(conn, user, studentId);
        }

        private static int? LookupInt(SqlConnection conn, string sql, object id)
        {
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                var value = cmd.ExecuteScalar();
                return value == null || value == DBNull.Value ? (int?)null : Convert.ToInt32(value);
            }
        }

        private static string LookupString(SqlConnection conn, string sql, object id)
        {
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                var value = cmd.ExecuteScalar();
                return value == null || value == DBNull.Value ? "" : value.ToString();
            }
        }

        private static bool Exists(SqlConnection conn, string sql, Action<SqlCommand> addParameters)
        {
            using (var cmd = new SqlCommand(sql, conn))
            {
                addParameters(cmd);
                var value = cmd.ExecuteScalar();
                return value != null && value != DBNull.Value;
            }
        }
    }
}
