using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using src.db;

namespace src.services
{
    /// <summary>
    /// A lecturer's profile row from the LECTURERS table.
    /// </summary>
    public class Lecturer
    {
        public int LecturerId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string MailingAddress { get; set; }
        public string IconPath { get; set; }
    }

    /// <summary>
    /// One assignment awaiting grading: it has at least one ungraded submission.
    /// <see cref="PendingCount"/> is how many submissions are still unmarked.
    /// </summary>
    public class GradingItem
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string CourseCode { get; set; }
        public DateTime DueDate { get; set; }
        public int PendingCount { get; set; }
    }

    /// <summary>
    /// Read-only access to lecturer profiles. Returns null when the user id has
    /// no LECTURERS row. SQL exceptions are not caught here; they propagate to
    /// the caller.
    /// </summary>
    public static class LecturerService
    {
        private const string SelectByUser =
            "SELECT l.lecturer_id, l.user_id, l.full_name, l.department, " +
            "u.email, u.username, " +
            "ISNULL(u.phone, '') AS phone, " +
            "ISNULL(u.mailing_address, '') AS mailing_address, " +
            "ISNULL(u.icon_path, '') AS icon_path " +
            "FROM LECTURERS l " +
            "JOIN USERS u ON u.user_id = l.user_id " +
            "WHERE l.user_id = @userId";

        public static Lecturer GetByUserId(int userId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectByUser, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new Lecturer
                    {
                        LecturerId = (int)reader["lecturer_id"],
                        UserId = (int)reader["user_id"],
                        FullName = reader["full_name"].ToString(),
                        Department = reader["department"].ToString(),
                        Email = reader["email"].ToString(),
                        Username = reader["username"].ToString(),
                        Phone = reader["phone"].ToString(),
                        MailingAddress = reader["mailing_address"].ToString(),
                        IconPath = reader["icon_path"].ToString()
                    };
                }
            }
        }

        // Scheduled classes for the lecturer on the given weekday (e.g. "Monday").
        private const string CountClassesOnDay =
            "SELECT COUNT(*) FROM TEACHINGS t " +
            "JOIN TIMETABLES tt ON tt.offering_id = t.offering_id " +
            "WHERE t.lecturer_id = @lecturerId AND tt.day_of_week = @day";

        /// <summary>Number of classes the lecturer teaches on the given weekday.</summary>
        public static int CountClassesOn(int lecturerId, DayOfWeek day)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(CountClassesOnDay, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                // DayOfWeek.ToString() yields invariant English ("Monday"), matching
                // the day_of_week values stored in TIMETABLES.
                cmd.Parameters.AddWithValue("@day", day.ToString());
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Submissions to the lecturer's assignments that have not been marked yet
        // (marks is NULL once a submission is graded it carries a mark).
        private const string CountUnmarkedSubmissions =
            "SELECT COUNT(*) FROM SUBMISSIONS s " +
            "JOIN ASSIGNMENTS a ON s.assignment_id = a.assignment_id " +
            "WHERE a.lecturer_id = @lecturerId AND s.marks IS NULL";

        /// <summary>Number of ungraded submissions awaiting the lecturer's review.</summary>
        public static int CountSubmissionsToReview(int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(CountUnmarkedSubmissions, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Offerings the lecturer teaches in the current semester.
        private const string CountActiveCoursesSql =
            "SELECT COUNT(*) FROM TEACHINGS t " +
            "JOIN COURSE_OFFERINGS o ON t.offering_id = o.offering_id " +
            "JOIN SEMESTERS sem ON o.semester_id = sem.semester_id " +
            "WHERE t.lecturer_id = @lecturerId AND sem.is_current = 1";

        /// <summary>Number of course offerings the lecturer teaches this semester.</summary>
        public static int CountActiveCourses(int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(CountActiveCoursesSql, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Distinct students enrolled in the lecturer's current-semester offerings.
        private const string CountStudentsTaughtSql =
            "SELECT COUNT(DISTINCT e.student_id) " +
            "FROM TEACHINGS t " +
            "JOIN COURSE_OFFERINGS o ON t.offering_id = o.offering_id " +
            "JOIN SEMESTERS sem ON o.semester_id = sem.semester_id " +
            "JOIN ENROLMENTS e ON e.offering_id = o.offering_id " +
            "WHERE t.lecturer_id = @lecturerId AND sem.is_current = 1 AND e.status = 'ENROLLED'";

        /// <summary>Distinct students taught across the lecturer's current-semester offerings.</summary>
        public static int CountStudentsTaught(int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(CountStudentsTaughtSql, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Present-vs-total attendance over the lecturer's current-semester offerings.
        // Only PRESENT is credited; LATE and ABSENT count against the rate.
        private const string SelectAttendanceCounts =
            "SELECT SUM(CASE WHEN a.status = 'PRESENT' THEN 1 ELSE 0 END) AS present_count, " +
            "COUNT(*) AS total_count " +
            "FROM TEACHINGS t " +
            "JOIN COURSE_OFFERINGS o ON t.offering_id = o.offering_id " +
            "JOIN SEMESTERS sem ON o.semester_id = sem.semester_id " +
            "JOIN ENROLMENTS e ON e.offering_id = o.offering_id " +
            "JOIN ATTENDANCE a ON a.enrolment_id = e.enrolment_id " +
            "WHERE t.lecturer_id = @lecturerId AND sem.is_current = 1";

        /// <summary>
        /// Fraction (0..1) of current-semester attendance records marked PRESENT
        /// across the lecturer's offerings, or null when there are no records.
        /// </summary>
        public static decimal? GetCurrentSemesterAttendanceRate(int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectAttendanceCounts, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    int total = (int)reader["total_count"];
                    if (total == 0) return null;

                    int present = reader["present_count"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(reader["present_count"]);
                    return Math.Round((decimal)present / total, 4);
                }
            }
        }

        // Today's class meetings for the lecturer, current semester only, by weekday.
        // Shares the ClassSession shape with the student timetable.
        private const string SelectTodayClasses =
            "SELECT tt.timetable_id, o.offering_id, c.course_code, c.course_name, c.credit_hours, " +
            "ISNULL(l.full_name, '') AS lecturer_name, " +
            "tt.venue, tt.day_of_week, tt.start_time, tt.end_time, ISNULL(c.color, '') AS color " +
            "FROM TEACHINGS tg " +
            "JOIN LECTURERS l ON tg.lecturer_id = l.lecturer_id " +
            "JOIN COURSE_OFFERINGS o ON tg.offering_id = o.offering_id " +
            "JOIN COURSES c ON o.course_id = c.course_id " +
            "JOIN SEMESTERS sem ON o.semester_id = sem.semester_id " +
            "JOIN TIMETABLES tt ON tt.offering_id = o.offering_id " +
            "WHERE tg.lecturer_id = @lecturerId AND sem.is_current = 1 AND tt.day_of_week = @today " +
            "ORDER BY tt.start_time";

        /// <summary>The lecturer's classes today (current semester), earliest first.</summary>
        public static List<ClassSession> GetTodayClasses(int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectTodayClasses, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                cmd.Parameters.AddWithValue("@today", DateTime.Now.DayOfWeek.ToString());
                var sessions = new List<ClassSession>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sessions.Add(new ClassSession
                        {
                            TimetableId = (int)reader["timetable_id"],
                            OfferingId = (int)reader["offering_id"],
                            CourseCode = reader["course_code"].ToString(),
                            CourseName = reader["course_name"].ToString(),
                            CreditHours = (int)reader["credit_hours"],
                            LecturerName = reader["lecturer_name"].ToString(),
                            Venue = reader["venue"].ToString(),
                            DayOfWeek = reader["day_of_week"].ToString(),
                            StartTime = (TimeSpan)reader["start_time"],
                            EndTime = (TimeSpan)reader["end_time"],
                            Color = reader["color"].ToString()
                        });
                    }
                }
                return sessions;
            }
        }

        // Assignments by the lecturer that still have unmarked submissions, with
        // how many remain, soonest due first. The INNER JOIN on unmarked
        // submissions excludes anything already fully graded.
        private const string SelectAssignmentsToGrade =
            "SELECT a.assignment_id, a.title, c.course_code, a.due_date, " +
            "COUNT(s.submission_id) AS pending_count " +
            "FROM ASSIGNMENTS a " +
            "JOIN COURSE_OFFERINGS o ON a.offering_id = o.offering_id " +
            "JOIN COURSES c ON o.course_id = c.course_id " +
            "JOIN SUBMISSIONS s ON s.assignment_id = a.assignment_id AND s.marks IS NULL " +
            "WHERE a.lecturer_id = @lecturerId " +
            "GROUP BY a.assignment_id, a.title, c.course_code, a.due_date " +
            "ORDER BY a.due_date";

        /// <summary>Assignments awaiting grading (those with unmarked submissions), soonest due first.</summary>
        public static List<GradingItem> GetAssignmentsToGrade(int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectAssignmentsToGrade, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                var items = new List<GradingItem>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new GradingItem
                        {
                            AssignmentId = (int)reader["assignment_id"],
                            Title = reader["title"].ToString(),
                            CourseCode = reader["course_code"].ToString(),
                            DueDate = (DateTime)reader["due_date"],
                            PendingCount = Convert.ToInt32(reader["pending_count"])
                        });
                    }
                }
                return items;
            }
        }

        // Announcements targeting the lecturer's current-semester offerings, newest
        // first. DISTINCT guards against one announcement targeting several of the
        // lecturer's offerings; TOP 5 keeps the dashboard widget concise.
        private const string SelectAnnouncements =
            "SELECT DISTINCT TOP 5 an.announcement_id, an.title, " +
            "CAST(an.content AS varchar(max)) AS content, an.created_at " +
            "FROM ANNOUNCEMENTS an " +
            "JOIN ANNOUNCEMENT_TARGETS at ON at.announcement_id = an.announcement_id " +
            "JOIN COURSE_OFFERINGS o ON at.offering_id = o.offering_id " +
            "JOIN SEMESTERS sem ON o.semester_id = sem.semester_id " +
            "JOIN TEACHINGS tg ON tg.offering_id = o.offering_id " +
            "WHERE tg.lecturer_id = @lecturerId AND sem.is_current = 1 " +
            "ORDER BY an.created_at DESC";

        /// <summary>Recent announcements targeting the lecturer's current-semester offerings.</summary>
        public static List<Announcement> GetAnnouncements(int lecturerId)
        {
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(SelectAnnouncements, conn))
            {
                cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                var list = new List<Announcement>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Announcement
                        {
                            AnnouncementId = (int)reader["announcement_id"],
                            Title = reader["title"].ToString(),
                            Content = reader["content"].ToString(),
                            CreatedAt = (DateTime)reader["created_at"]
                        });
                    }
                }
                return list;
            }
        }
    }
}
