using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace src.services
{
    public static class AttendancePdfService
    {
        private const float PageWidth = 595f;
        private const float PageHeight = 842f;
        private const float Left = 42f;
        private const float Right = 553f;
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        public static byte[] Create(StudentAccountProfile account, StudentAttendancePage attendance, DateTime generatedAt)
        {
            return Create(account, attendance, generatedAt, null);
        }

        public static byte[] Create(
            StudentAccountProfile account,
            StudentAttendancePage attendance,
            DateTime generatedAt,
            AttendancePdfFilters filters)
        {
            var courses = attendance.Courses ?? new List<StudentAttendanceCourse>();
            var pages = new List<PdfPage>();
            var page = NewPage(pages, false);
            DrawStudentDetails(page, account, generatedAt);
            DrawAppliedFilters(page, filters);

            var present = courses.Sum(c => c.PresentCount);
            var late = courses.Sum(c => c.LateCount);
            var absent = courses.Sum(c => c.AbsentCount);
            var total = courses.Sum(c => c.TotalCount);
            DrawSummary(page, 586f, courses.Count, present, late, absent, total);

            var y = 526f;
            if (courses.Count == 0)
            {
                Text(page, Left, y, "F2", 11f, "No attendance records match the selected filters.");
            }
            else
            {
                foreach (var semester in courses.GroupBy(c => new { c.SemesterId, c.SemesterName }))
                {
                    EnsureSpace(ref page, pages, ref y, 66f);
                    DrawSemesterHeader(page, y, semester.Key.SemesterName);
                    y -= 30f;

                    foreach (var course in semester)
                    {
                        EnsureSpace(ref page, pages, ref y, 72f);
                        DrawCourseHeader(page, y, course, false);
                        y -= 42f;

                        if (course.Sessions == null || course.Sessions.Count == 0)
                        {
                            Text(page, Left + 8f, y, "F1", 8.5f, "No sessions recorded for this course.");
                            y -= 24f;
                            continue;
                        }

                        DrawSessionHeader(page, y);
                        y -= 18f;
                        foreach (var session in course.Sessions.OrderByDescending(s => s.AttendanceDate).ThenByDescending(s => s.StartTime))
                        {
                            if (y < 82f)
                            {
                                page = NewPage(pages, true);
                                y = 750f;
                                DrawCourseHeader(page, y, course, true);
                                y -= 42f;
                                DrawSessionHeader(page, y);
                                y -= 18f;
                            }

                            DrawSessionRow(page, y, session);
                            y -= 19f;
                        }
                        y -= 14f;
                    }
                }
            }

            for (var i = 0; i < pages.Count; i++)
            {
                DrawFooter(pages[i], i + 1, pages.Count, generatedAt);
            }
            return BuildPdf(pages);
        }

        private static PdfPage NewPage(List<PdfPage> pages, bool continued)
        {
            var page = new PdfPage();
            pages.Add(page);
            if (continued)
            {
                Text(page, Left, 802f, "F2", 15f, "INTI INTERNATIONAL UNIVERSITY & COLLEGES");
                Text(page, Left, 780f, "F2", 12f, "STUDENT ATTENDANCE REPORT - CONTINUED");
                Line(page, Left, 768f, Right, 768f, 0.88f, 0.09f, 0.17f, 1.4f);
            }
            return page;
        }

        private static void DrawStudentDetails(PdfPage page, StudentAccountProfile account, DateTime generatedAt)
        {
            Text(page, Left, 804f, "F2", 16f, "INTI INTERNATIONAL UNIVERSITY & COLLEGES");
            Text(page, Left, 778f, "F2", 20f, "STUDENT ATTENDANCE REPORT");
            TextRight(page, Right, 784f, "F1", 8f, "Generated " + generatedAt.ToString("dd MMM yyyy HH:mm", Invariant));
            Line(page, Left, 762f, Right, 762f, 0.88f, 0.09f, 0.17f, 1.5f);

            LabelValue(page, Left, 735f, "STUDENT NAME", account.FullName);
            LabelValue(page, Left, 696f, "STUDENT ID", account.StudentId);
            LabelValue(page, 310f, 735f, "PROGRAMME", account.ProgrammeName);
            LabelValue(page, 310f, 696f, "CURRENT SESSION", account.CurrentSession);
        }

        private static void LabelValue(PdfPage page, float x, float y, string label, string value)
        {
            Text(page, x, y, "F1", 7.5f, label);
            Text(page, x, y - 16f, "F2", 10f, Shorten(value, x < 100f ? 39 : 40));
        }

        private static void DrawAppliedFilters(PdfPage page, AttendancePdfFilters filters)
        {
            filters = filters ?? new AttendancePdfFilters();
            Text(page, Left, 653f, "F2", 8f, "APPLIED FILTERS");
            Text(page, Left, 638f, "F1", 8f,
                "Semester: " + Default(filters.Semester, "All semesters") +
                "   |   Course: " + Default(filters.Course, "All courses") +
                "   |   Status: " + Default(filters.Status, "All statuses"));
            Text(page, Left, 623f, "F1", 8f, "Date duration: " + DateDuration(filters.DateFrom, filters.DateTo));
        }

        private static void DrawSummary(PdfPage page, float y, int courseCount, int present, int late, int absent, int total)
        {
            var rate = total == 0 ? "N/A" : ((decimal)(present + late) / total * 100m).ToString("0.#", Invariant) + "%";
            FillRect(page, Left, y - 7f, Right - Left, 42f, 0.97f, 0.97f, 0.98f);
            page.Content.Append("0 0 0 rg\n");
            SummaryValue(page, Left + 10f, y + 14f, "ATTENDANCE", rate);
            SummaryValue(page, 145f, y + 14f, "COURSES", courseCount.ToString(Invariant));
            SummaryValue(page, 240f, y + 14f, "PRESENT", present.ToString(Invariant));
            SummaryValue(page, 335f, y + 14f, "LATE", late.ToString(Invariant));
            SummaryValue(page, 425f, y + 14f, "ABSENT", absent.ToString(Invariant));
            SummaryValue(page, 505f, y + 14f, "TOTAL", total.ToString(Invariant));
        }

        private static void SummaryValue(PdfPage page, float x, float y, string label, string value)
        {
            Text(page, x, y, "F1", 7f, label);
            Text(page, x, y - 17f, "F2", 11f, value);
        }

        private static void DrawSemesterHeader(PdfPage page, float y, string semesterName)
        {
            FillRect(page, Left, y - 5f, Right - Left, 23f, 0.88f, 0.09f, 0.17f);
            page.Content.Append("1 1 1 rg\n");
            Text(page, Left + 8f, y + 2f, "F2", 10f, string.IsNullOrWhiteSpace(semesterName) ? "SEMESTER" : semesterName.ToUpperInvariant());
            page.Content.Append("0 0 0 rg\n");
        }

        private static void DrawCourseHeader(PdfPage page, float y, StudentAttendanceCourse course, bool continued)
        {
            FillRect(page, Left, y - 20f, Right - Left, 35f, 0.95f, 0.96f, 0.97f);
            page.Content.Append("0 0 0 rg\n");
            var title = course.CourseCode + "  " + course.CourseName + (continued ? " (continued)" : "");
            Text(page, Left + 8f, y, "F2", 9.5f, Shorten(title, 70));
            Text(page, Left + 8f, y - 14f, "F1", 7.5f, "Lecturer: " + Default(course.LecturerName, "Not assigned"));
            var summary = "Present " + course.PresentCount + "   Late " + course.LateCount + "   Absent " + course.AbsentCount +
                "   Rate " + Rate(course.PresentCount + course.LateCount, course.TotalCount);
            TextRight(page, Right - 8f, y - 14f, "F1", 7.5f, summary);
        }

        private static void DrawSessionHeader(PdfPage page, float y)
        {
            FillRect(page, Left, y - 4f, Right - Left, 17f, 0.22f, 0.25f, 0.30f);
            page.Content.Append("1 1 1 rg\n");
            Text(page, 50f, y + 1f, "F2", 7.5f, "DATE");
            Text(page, 145f, y + 1f, "F2", 7.5f, "TIME");
            Text(page, 255f, y + 1f, "F2", 7.5f, "TYPE");
            Text(page, 345f, y + 1f, "F2", 7.5f, "ROOM");
            Text(page, 470f, y + 1f, "F2", 7.5f, "STATUS");
            page.Content.Append("0 0 0 rg\n");
        }

        private static void DrawSessionRow(PdfPage page, float y, StudentAttendanceSession session)
        {
            Text(page, 50f, y, "F1", 8f, session.AttendanceDate.ToString("dd MMM yyyy (ddd)", Invariant));
            Text(page, 145f, y, "F1", 8f, TimeRange(session.StartTime, session.EndTime));
            Text(page, 255f, y, "F1", 8f, Default(session.SessionType, "Class"));
            Text(page, 345f, y, "F1", 8f, Shorten(Default(session.Venue, "TBA"), 20));
            Text(page, 470f, y, "F2", 8f, Default(session.Status, "N/A").ToUpperInvariant());
            Line(page, Left, y - 5f, Right, y - 5f, 0.86f, 0.87f, 0.89f, 0.35f);
        }

        private static void EnsureSpace(ref PdfPage page, List<PdfPage> pages, ref float y, float required)
        {
            if (y - required >= 82f) return;
            page = NewPage(pages, true);
            y = 750f;
        }

        private static void DrawFooter(PdfPage page, int pageNumber, int pageCount, DateTime generatedAt)
        {
            Line(page, Left, 48f, Right, 48f, 0.75f, 0.75f, 0.75f, 0.5f);
            Text(page, Left, 32f, "F1", 7.5f, "Student-generated attendance report for reference purposes.");
            TextRight(page, Right, 32f, "F1", 7.5f, "Page " + pageNumber + " of " + pageCount);
        }

        private static string Rate(int present, int total)
        {
            return total == 0 ? "N/A" : ((decimal)present / total * 100m).ToString("0.#", Invariant) + "%";
        }

        private static string DateDuration(DateTime? dateFrom, DateTime? dateTo)
        {
            if (dateFrom.HasValue && dateTo.HasValue)
            {
                return dateFrom.Value.ToString("dd MMM yyyy", Invariant) + " to " +
                    dateTo.Value.ToString("dd MMM yyyy", Invariant);
            }
            if (dateFrom.HasValue) return "From " + dateFrom.Value.ToString("dd MMM yyyy", Invariant) + " onward";
            if (dateTo.HasValue) return "Up to " + dateTo.Value.ToString("dd MMM yyyy", Invariant);
            return "All dates";
        }

        private static string TimeRange(TimeSpan? start, TimeSpan? end)
        {
            if (!start.HasValue || !end.HasValue) return "TBA";
            return DateTime.Today.Add(start.Value).ToString("HH:mm", Invariant) + " - " +
                DateTime.Today.Add(end.Value).ToString("HH:mm", Invariant);
        }

        private static string Default(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private static string Shorten(string value, int max)
        {
            value = value ?? "";
            return value.Length <= max ? value : value.Substring(0, Math.Max(0, max - 3)) + "...";
        }

        private static void Text(PdfPage page, float x, float y, string font, float size, string value)
        {
            page.Content.Append("BT /").Append(font).Append(' ').Append(F(size)).Append(" Tf 1 0 0 1 ")
                .Append(F(x)).Append(' ').Append(F(y)).Append(" Tm (").Append(Escape(value)).Append(") Tj ET\n");
        }

        private static void TextRight(PdfPage page, float right, float y, string font, float size, string value)
        {
            var width = Clean(value).Length * size * 0.48f;
            Text(page, Math.Max(Left, right - width), y, font, size, value);
        }

        private static void Line(PdfPage page, float x1, float y1, float x2, float y2, float r, float g, float b, float width)
        {
            page.Content.Append(F(r)).Append(' ').Append(F(g)).Append(' ').Append(F(b)).Append(" RG ")
                .Append(F(width)).Append(" w ").Append(F(x1)).Append(' ').Append(F(y1)).Append(" m ")
                .Append(F(x2)).Append(' ').Append(F(y2)).Append(" l S\n");
        }

        private static void FillRect(PdfPage page, float x, float y, float width, float height, float r, float g, float b)
        {
            page.Content.Append(F(r)).Append(' ').Append(F(g)).Append(' ').Append(F(b)).Append(" rg ")
                .Append(F(x)).Append(' ').Append(F(y)).Append(' ').Append(F(width)).Append(' ').Append(F(height)).Append(" re f\n");
        }

        private static string Escape(string value)
        {
            return Clean(value).Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }

        private static string Clean(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            var chars = value.Select(c => c >= 32 && c <= 126 ? c : '?').ToArray();
            return new string(chars);
        }

        private static string F(float value)
        {
            return value.ToString("0.###", Invariant);
        }

        private static byte[] BuildPdf(List<PdfPage> pages)
        {
            var objects = new List<byte[]>();
            objects.Add(Ascii("<< /Type /Catalog /Pages 2 0 R >>"));

            var kids = new StringBuilder();
            for (var i = 0; i < pages.Count; i++) kids.Append(5 + i * 2).Append(" 0 R ");
            objects.Add(Ascii("<< /Type /Pages /Kids [" + kids + "] /Count " + pages.Count + " >>"));
            objects.Add(Ascii("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"));
            objects.Add(Ascii("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>"));

            foreach (var page in pages)
            {
                var contentId = objects.Count + 2;
                objects.Add(Ascii("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 " + F(PageWidth) + " " + F(PageHeight) +
                    "] /Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents " + contentId + " 0 R >>"));
                var content = Ascii(page.Content.ToString());
                objects.Add(Combine(Ascii("<< /Length " + content.Length + " >>\nstream\n"), content, Ascii("\nendstream")));
            }

            using (var output = new MemoryStream())
            {
                Write(output, "%PDF-1.4\n");
                var offsets = new List<long> { 0 };
                for (var i = 0; i < objects.Count; i++)
                {
                    offsets.Add(output.Position);
                    Write(output, (i + 1) + " 0 obj\n");
                    output.Write(objects[i], 0, objects[i].Length);
                    Write(output, "\nendobj\n");
                }

                var xref = output.Position;
                Write(output, "xref\n0 " + (objects.Count + 1) + "\n0000000000 65535 f \n");
                for (var i = 1; i < offsets.Count; i++) Write(output, offsets[i].ToString("0000000000", Invariant) + " 00000 n \n");
                Write(output, "trailer\n<< /Size " + (objects.Count + 1) + " /Root 1 0 R >>\nstartxref\n" + xref + "\n%%EOF");
                return output.ToArray();
            }
        }

        private static byte[] Ascii(string value) { return Encoding.ASCII.GetBytes(value); }

        private static byte[] Combine(params byte[][] parts)
        {
            using (var output = new MemoryStream())
            {
                foreach (var part in parts) output.Write(part, 0, part.Length);
                return output.ToArray();
            }
        }

        private static void Write(Stream stream, string value)
        {
            var bytes = Ascii(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        private sealed class PdfPage
        {
            public readonly StringBuilder Content = new StringBuilder();
        }
    }

    public class AttendancePdfFilters
    {
        public string Semester { get; set; }
        public string Course { get; set; }
        public string Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
