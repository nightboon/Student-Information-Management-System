using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class LecturerTimetableReader
    {
        // Current-semester weekly class sessions for the lecturer's offerings.
        public static List<LecturerClassSession> GetClassSessions(UserContext user)
        {
            var list = new List<LecturerClassSession>();
            if (user == null || !user.IsLecturer) return list;

            var currentLabel = TermLabel(AcademicTermReader.GetCurrentTerm());

            string sql =
                "SELECT t.timetable_id, t.offer_id, t.day_of_week, t.start_time, t.end_time, t.room, " +
                "c.course_code, c.course_name, c.colour, " +
                "co.academic_year + ' ' + co.semester AS semester_name " +
                "FROM TIMETABLES t " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = t.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "WHERE " + ServiceAccess.VisibleOfferScope("co");

            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                ServiceAccess.AddUserContextParameters(cmd, user);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var semesterName = Text(reader["semester_name"]);
                        if (!string.IsNullOrEmpty(currentLabel) && !IsSameTerm(semesterName, currentLabel)) continue;
                        var code = Text(reader["course_code"]);
                        list.Add(new LecturerClassSession
                        {
                            TimetableId = IntValue(reader["timetable_id"]),
                            OfferingId = IntValue(reader["offer_id"]),
                            CourseCode = code,
                            CourseName = Text(reader["course_name"]),
                            DayOfWeek = Text(reader["day_of_week"]),
                            StartTime = TimeValue(reader["start_time"]) ?? new TimeSpan(8, 0, 0),
                            EndTime = TimeValue(reader["end_time"]) ?? new TimeSpan(9, 0, 0),
                            Venue = string.IsNullOrWhiteSpace(Text(reader["room"])) ? "TBA" : Text(reader["room"]),
                            Color = ColorOrFallback(Text(reader["colour"]), code)
                        });
                    }
                }
            }
            return list;
        }
    }
}
