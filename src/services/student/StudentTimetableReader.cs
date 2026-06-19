using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class StudentTimetableReader
    {
        public static List<StudentClassSession> GetClassSessions(UserContext user)
        {
            if (user == null) return new List<StudentClassSession>();

            var sql =
                "SELECT t.timetable_id, t.offer_id, t.day_of_week, t.start_time, t.end_time, t.room, " +
                "c.course_code, c.course_name, c.colour, ISNULL(l.lecturer_name, '') AS lecturer_name " +
                "FROM TIMETABLES t " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = t.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "LEFT JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id " +
                "WHERE " + ServiceAccess.VisibleOfferScope("co") + " " +
                "ORDER BY t.day_of_week, t.start_time";

            var sessions = new List<StudentClassSession>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var code = Text(reader["course_code"]);
                        sessions.Add(new StudentClassSession
                        {
                            TimetableId = IntValue(reader["timetable_id"]),
                            OfferingId = IntValue(reader["offer_id"]),
                            CourseCode = code,
                            CourseName = Text(reader["course_name"]),
                            LecturerName = LecturerOrFallback(Text(reader["lecturer_name"])),
                            DayOfWeek = Text(reader["day_of_week"]),
                            StartTime = TimeValue(reader["start_time"]) ?? new TimeSpan(8, 0, 0),
                            EndTime = TimeValue(reader["end_time"]) ?? new TimeSpan(9, 0, 0),
                            Venue = Text(reader["room"]),
                            Color = ColorOrFallback(Text(reader["colour"]), code)
                        });
                    }
                }
            }
            return sessions;
        }

        public static StudentTimetablePage GetTimetablePage(UserContext user)
        {
            if (!IsStudent(user)) return null;

            var account = StudentProfileReader.GetAccount(user);
            if (account == null) return null;

            var current = AcademicTermReader.GetCurrentTerm();
            var sessions = GetClassSessions(user);
            var courseCards = StudentCourseReader.GetCourses(user, account.StudentId);
            var courses = sessions
                .GroupBy(s => s.OfferingId)
                .Select(g => new StudentTimetableCourse
                {
                    OfferingId = g.Key,
                    CourseCode = g.First().CourseCode,
                    CourseName = g.First().CourseName,
                    LecturerName = g.First().LecturerName,
                    CreditHours = 0,
                    Color = g.First().Color
                })
                .OrderBy(c => c.CourseCode)
                .ToList();

            foreach (var course in courses)
            {
                var card = courseCards.FirstOrDefault(c => c.OfferingId == course.OfferingId);
                if (card != null) course.CreditHours = card.CreditHours;
            }

            return new StudentTimetablePage
            {
                SemesterName = string.IsNullOrWhiteSpace(account.CurrentSession) ? TermLabel(current) : account.CurrentSession,
                SemesterStartDate = current == null ? DateTime.Today : current.StartDate,
                SemesterEndDate = current == null ? DateTime.Today.AddMonths(4) : current.EndDate,
                CurrentSemesterNo = account.CurrentSemesterNo,
                ProgrammeName = account.ProgrammeName,
                CourseCount = courseCards.Count,
                TotalCreditHours = courseCards.Sum(c => c.CreditHours),
                WeeklyContactHours = Math.Round((decimal)sessions.Sum(s => (s.EndTime - s.StartTime).TotalHours), 1),
                Courses = courses,
                Sessions = sessions
            };
        }
    }
}
