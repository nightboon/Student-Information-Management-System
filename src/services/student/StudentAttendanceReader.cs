using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class StudentAttendanceReader
    {
        public static StudentAttendancePage GetAttendancePage(UserContext user, string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId)) return null;

            var courses = StudentCourseReader.GetCourses(user, studentId).Select(c => new StudentAttendanceCourse
            {
                OfferingId = c.OfferingId,
                SemesterName = c.SemesterName,
                IsCurrent = c.IsCurrent,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                LecturerName = c.LecturerName,
                Color = c.Color,
                Sessions = new List<StudentAttendanceSession>()
            }).ToList();

            AssignSemesterIds(courses);
            var byOffer = courses.ToDictionary(c => c.OfferingId);

            const string sql =
                "SELECT ats.session_id, ats.offer_id, ats.session_date, ats.start_time, ats.end_time, ISNULL(t.room, '') AS room, ar.status " +
                "FROM ENROLLMENTS e " +
                "JOIN ATTENDANCE_SESSIONS ats ON ats.offer_id = e.offer_id " +
                "LEFT JOIN ATTENDANCE_RECORDS ar ON ar.session_id = ats.session_id AND ar.student_id = e.student_id " +
                "OUTER APPLY (SELECT TOP 1 room FROM TIMETABLES t WHERE t.offer_id = ats.offer_id ORDER BY t.timetable_id) t " +
                "WHERE e.student_id = @studentId AND e.status IN ('ENROLLED', 'PENDING') " +
                "ORDER BY ats.session_date DESC, ats.start_time DESC";

            using (var conn = Db.OpenConnection())
            {
                if (!ServiceAccess.CanViewStudent(conn, user, studentId)) return new StudentAttendancePage { Courses = courses };
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudentAttendanceCourse course;
                            if (!byOffer.TryGetValue(IntValue(reader["offer_id"]), out course)) continue;
                            var status = Text(reader["status"]).ToUpperInvariant();
                            var session = new StudentAttendanceSession
                            {
                                SessionId = IntValue(reader["session_id"]),
                                AttendanceDate = DateValue(reader["session_date"]) ?? DateTime.Today,
                                StartTime = TimeValue(reader["start_time"]),
                                EndTime = TimeValue(reader["end_time"]),
                                SessionType = "Class",
                                Venue = string.IsNullOrWhiteSpace(Text(reader["room"])) ? "TBA" : Text(reader["room"]),
                                Status = string.IsNullOrWhiteSpace(status) ? "N/A" : status
                            };
                            course.Sessions.Add(session);
                            if (status == "PRESENT") course.PresentCount++;
                            else if (status == "LATE") course.LateCount++;
                            else if (status == "ABSENT") course.AbsentCount++;
                            if (!string.IsNullOrWhiteSpace(status)) course.TotalCount++;
                        }
                    }
                }
            }

            foreach (var course in courses)
            {
                // Late counts as present for the attendance rate (still tracked/shown separately).
                course.AttendanceRate = course.TotalCount == 0 ? (decimal?)null : Math.Round((decimal)(course.PresentCount + course.LateCount) / course.TotalCount, 4);
            }

            return new StudentAttendancePage { Courses = courses };
        }

        private static void AssignSemesterIds(List<StudentAttendanceCourse> courses)
        {
            var ids = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var next = 1;
            foreach (var course in courses)
            {
                if (!ids.ContainsKey(course.SemesterName))
                {
                    ids[course.SemesterName] = next++;
                }
                course.SemesterId = ids[course.SemesterName];
            }
        }
    }
}
