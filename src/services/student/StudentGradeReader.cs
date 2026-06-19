using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using src.db;
using static src.services.ServiceMap;
using static src.services.StudentPortalFormat;

namespace src.services
{
    public static class StudentGradeReader
    {
        public static StudentGradePage GetGradePage(UserContext user)
        {
            if (!IsStudent(user)) return null;

            var account = StudentProfileReader.GetAccount(user);
            if (account == null) return null;

            var courses = GetGradeCourses(user, account.StudentId);
            var semesterGroups = courses.GroupBy(c => c.SemesterName).ToList();
            var sessions = AcademicTermReader.GetSessionLookup();
            var semesters = new List<StudentGradeSemester>();

            for (var i = 0; i < semesterGroups.Count; i++)
            {
                var group = semesterGroups[i];
                DateTime startDate;
                if (!sessions.TryGetValue(group.Key, out startDate)) startDate = DateTime.Today.AddMonths(i);
                var list = group.Cast<StudentGradeCourseWithSemester>().Select(g => (StudentGradeCourse)g).ToList();
                var published = list.Where(c => c.GradePublished && c.Gpa.HasValue).ToList();
                var credits = list.Sum(c => c.CreditHours);
                var earned = published.Where(c => c.Gpa.GetValueOrDefault() > 0m).Sum(c => c.CreditHours);
                var weightedPoints = published.Sum(c => c.Gpa.Value * c.CreditHours);
                var weightedCredits = published.Sum(c => c.CreditHours);
                semesters.Add(new StudentGradeSemester
                {
                    SemesterName = group.Key,
                    StartDate = startDate,
                    SemesterNo = account.CurrentSemesterNo > 0 && IsSameTerm(group.Key, account.CurrentSession) ? account.CurrentSemesterNo : i + 1,
                    IsCurrent = IsSameTerm(group.Key, account.CurrentSession),
                    CourseCount = list.Count,
                    Credits = credits,
                    EarnedCredits = earned,
                    Gpa = weightedCredits > 0 ? Math.Round(weightedPoints / weightedCredits, 2) : (decimal?)null,
                    Courses = list
                });
            }

            semesters = semesters.OrderBy(s => s.StartDate).ToList();
            var allPublished = semesters.SelectMany(s => s.Courses).Where(c => c.GradePublished && c.Gpa.HasValue).ToList();
            var allWeightedCredits = allPublished.Sum(c => c.CreditHours);
            var cgpa = allWeightedCredits > 0 ? Math.Round(allPublished.Sum(c => c.Gpa.Value * c.CreditHours) / allWeightedCredits, 2) : (decimal?)null;
            var maxDistribution = allPublished.GroupBy(c => c.LetterGrade).Select(g => g.Count()).DefaultIfEmpty(0).Max();

            return new StudentGradePage
            {
                Semesters = semesters,
                CurrentSemester = semesters.FirstOrDefault(s => s.IsCurrent) ?? semesters.LastOrDefault(),
                BestSemester = semesters.Where(s => s.Gpa.HasValue).OrderByDescending(s => s.Gpa.Value).FirstOrDefault(),
                Cgpa = cgpa,
                CurrentGpa = (semesters.FirstOrDefault(s => s.IsCurrent) ?? semesters.LastOrDefault()) == null ? null : (semesters.FirstOrDefault(s => s.IsCurrent) ?? semesters.LastOrDefault()).Gpa,
                CreditsEarned = semesters.Sum(s => s.EarnedCredits),
                CreditsAttempted = semesters.Sum(s => s.Credits),
                CoursesGraded = allPublished.Count,
                GradeDistribution = allPublished
                    .GroupBy(c => c.LetterGrade)
                    .OrderBy(g => GradeOrder(g.Key))
                    .Select(g => new StudentGradeDistributionItem
                    {
                        Grade = g.Key,
                        Count = g.Count(),
                        BarWidth = maxDistribution == 0 ? 0m : Math.Round(g.Count() * 100m / maxDistribution, 2)
                    })
                    .ToList()
            };
        }

        private static List<StudentGradeCourseWithSemester> GetGradeCourses(UserContext user, string studentId)
        {
            const string sql =
                "SELECT e.offer_id, ISNULL(e.semester, co.academic_year + ' ' + co.semester) AS semester_name, " +
                "c.course_code, c.course_name, c.credit_hour, c.colour, ISNULL(l.lecturer_name, '') AS lecturer_name, " +
                "g.grade_point, g.letter_grade " +
                "FROM ENROLLMENTS e " +
                "JOIN COURSE_OFFERINGS co ON co.offer_id = e.offer_id " +
                "JOIN COURSES c ON c.course_id = co.course_id " +
                "LEFT JOIN LECTURERS l ON l.lecturer_id = co.lecturer_id " +
                "LEFT JOIN GRADES g ON g.offer_id = e.offer_id AND g.student_id = e.student_id " +
                "WHERE e.student_id = @studentId AND e.status IN ('ENROLLED', 'PENDING') " +
                "ORDER BY semester_name, c.course_code";

            var courses = new List<StudentGradeCourseWithSemester>();
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
                            var summary = GetAssessmentSummary(user, studentId, IntValue(reader["offer_id"]));
                            var letter = Text(reader["letter_grade"]);
                            var gpa = DecimalValue(reader["grade_point"]);
                            courses.Add(new StudentGradeCourseWithSemester
                            {
                                OfferingId = IntValue(reader["offer_id"]),
                                SemesterName = Text(reader["semester_name"]),
                                CourseCode = code,
                                CourseName = Text(reader["course_name"]),
                                LecturerName = LecturerOrFallback(Text(reader["lecturer_name"])),
                                CreditHours = IntValue(reader["credit_hour"]),
                                Color = ColorOrFallback(Text(reader["colour"]), code),
                                CurrentAverage = summary.CurrentAverage,
                                CompletedPercent = summary.CompletedPercent,
                                HasFinalAssessment = false,
                                FinalExamMarks = null,
                                GradePublished = IsPublishedGrade(letter),
                                LetterGrade = IsPublishedGrade(letter) ? letter : "",
                                Gpa = IsPublishedGrade(letter) ? gpa : null
                            });
                        }
                    }
                }
            }
            return courses;
        }

        private static AssessmentSummary GetAssessmentSummary(UserContext user, string studentId, int offerId)
        {
            const string sql =
                "SELECT a.total_marks, sub.marks_obtained " +
                "FROM ASSIGNMENTS a " +
                "LEFT JOIN SUBMISSIONS sub ON sub.assignment_id = a.assignment_id AND sub.student_id = @studentId " +
                "WHERE a.offer_id = @offerId";

            var total = 0;
            var graded = 0;
            decimal sumPercent = 0m;
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@studentId", studentId);
                cmd.Parameters.AddWithValue("@offerId", offerId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        total++;
                        var marks = DecimalValue(reader["marks_obtained"]);
                        if (!marks.HasValue) continue;
                        var max = IntValue(reader["total_marks"]);
                        if (max <= 0) max = 100;
                        graded++;
                        sumPercent += Math.Round(marks.Value / max * 100m, 2);
                    }
                }
            }

            return new AssessmentSummary
            {
                CurrentAverage = graded == 0 ? null : (decimal?)Math.Round(sumPercent / graded, 1),
                CompletedPercent = total == 0 ? 0m : Math.Round(graded * 100m / total, 1)
            };
        }

        private class AssessmentSummary
        {
            public decimal? CurrentAverage { get; set; }
            public decimal CompletedPercent { get; set; }
        }

        private class StudentGradeCourseWithSemester : StudentGradeCourse
        {
            public string SemesterName { get; set; }
        }
    }
}
