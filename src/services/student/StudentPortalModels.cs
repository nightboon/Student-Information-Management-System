using System;
using System.Collections.Generic;

namespace src.services
{
    public class AcademicSessionOption
    {
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class StudentAccountProfile
    {
        public string StudentId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProgrammeName { get; set; }
        public DateTime? IntakeDate { get; set; }
        public int CurrentSemesterNo { get; set; }
        public int ProgrammeSemesterCount { get; set; }
        public int SemestersPerYear { get; set; }
        public string Phone { get; set; }
        public string MailingAddress { get; set; }
        public string IconPath { get; set; }
        public string Status { get; set; }
        public string CurrentSession { get; set; }
    }

    public class StudentCourseCard
    {
        public int OfferingId { get; set; }
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string LecturerName { get; set; }
        public int CreditHours { get; set; }
        public string SemesterName { get; set; }
        public bool IsCurrent { get; set; }
        public string Color { get; set; }
    }

    public class StudentCourseHeader
    {
        public int OfferingId { get; set; }
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string LecturerName { get; set; }
        public int CreditHours { get; set; }
        public int ModuleCount { get; set; }
        public string Color { get; set; }
        public string LevelLabel { get; set; }
        public string Mode { get; set; }
        public string ContactHours { get; set; }
        public string Prerequisites { get; set; }
        public string Textbook { get; set; }
        public string OfficeHours { get; set; }
    }

    public class StudentLearningOutcome
    {
        public string Text { get; set; }
    }

    public class StudentCourseModule
    {
        public string ModuleId { get; set; }
        public int Week { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<StudentModuleMaterial> Items { get; set; }
    }

    public class StudentModuleMaterial
    {
        public int MaterialId { get; set; }
        public string Title { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public int? FileSizeBytes { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class StudentCourseAnnouncement
    {
        public int NotificationId { get; set; }
        public string NotificationType { get; set; }
        public int AnnouncementId { get; set; }
        public int OfferId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string AuthorRole { get; set; }
        public bool IsPinned { get; set; }
        public bool HasAttachment { get; set; }
        public string FileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class StudentPortalNotification : StudentCourseAnnouncement
    {
        public bool IsRead { get; set; }
    }

    public class StudentCourseAssignment
    {
        public int MaterialId { get; set; }
        public int AssignmentId { get; set; }
        public int OfferId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? Weight { get; set; }
        public string AssignmentType { get; set; }
        public string SubmissionMode { get; set; }
        public bool IsLinkSubmission { get { return string.Equals(SubmissionMode, "LINK", StringComparison.OrdinalIgnoreCase); } }
        public bool IsFileSubmission { get { return string.Equals(SubmissionMode, "FILE", StringComparison.OrdinalIgnoreCase); } }
        public bool RequiresSubmission { get; set; }
        public bool CanSubmit { get; set; }
        public DateTime? ExtensionDeadline { get; set; }
        public string GroupSize { get; set; }
        public string SubmissionStatus { get; set; }
        public bool HasSubmission { get; set; }
        public string SubmissionFileUrl { get; set; }
        public string MaterialFileUrl { get; set; }
        public string Feedback { get; set; }
        public string AnnotatedFileUrl { get; set; }
        public decimal? Marks { get; set; }
        public string CourseCode { get; set; }
    }

    public class StudentAssessmentRow
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Weight { get; set; }
        public decimal? Marks { get; set; }
        public int MaxMarks { get; set; }
        public bool IsGraded { get; set; }
        public decimal? Contribution { get; set; }
    }

    public class StudentGradebook
    {
        public List<StudentAssessmentRow> Items { get; set; }
        public decimal? OverallAverage { get; set; }
        public decimal EarnedWeighted { get; set; }
        public decimal CompletedPercent { get; set; }
    }

    public class StudentRegistrationTerm
    {
        public string SessionId { get; set; }
        public string Name { get; set; }
        public string AcademicYear { get; set; }
        public string IntakeId { get; set; }
        public string IntakeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationStart { get; set; }
        public DateTime RegistrationEnd { get; set; }
        public DateTime AddDropEnd { get; set; }
        public int MinCredits { get; set; }
        public int MaxCredits { get; set; }
    }

    public class StudentRegistrationWindow
    {
        public bool IsOpen { get; set; }
        public int ActivePhase { get; set; }
        public DateTime? RegistrationStart { get; set; }
        public DateTime? RegistrationEnd { get; set; }
        public DateTime? AddDropStart { get; set; }
        public DateTime? AddDropEnd { get; set; }
    }

    public class StudentOfferingOption
    {
        public int OfferingId { get; set; }
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string LecturerName { get; set; }
        public string Schedule { get; set; }
        public int CreditHours { get; set; }
        public string Prerequisites { get; set; }
        public bool PrerequisiteMet { get; set; }
        public int EnrolledCount { get; set; }
        public int Capacity { get; set; }
        public string MyStatus { get; set; }
        public decimal FeePerCredit { get; set; }
        public string Color { get; set; }
    }

    public class StudentEnrollmentPage
    {
        public StudentRegistrationTerm Term { get; set; }
        public StudentRegistrationWindow Window { get; set; }
        public List<StudentOfferingOption> Offerings { get; set; }
        public int SemesterNo { get; set; }
        public int SemesterCount { get; set; }
        public int AlreadyRegisteredCount { get; set; }
    }

    public class StudentAttendanceSession
    {
        public int SessionId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string SessionType { get; set; }
        public string Venue { get; set; }
        public string Status { get; set; }
    }

    public class StudentAttendanceCourse
    {
        public int OfferingId { get; set; }
        public int SemesterId { get; set; }
        public string SemesterName { get; set; }
        public bool IsCurrent { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string LecturerName { get; set; }
        public string Color { get; set; }
        public int PresentCount { get; set; }
        public int LateCount { get; set; }
        public int AbsentCount { get; set; }
        public int TotalCount { get; set; }
        public decimal? AttendanceRate { get; set; }
        public List<StudentAttendanceSession> Sessions { get; set; }
    }

    public class StudentAttendancePage
    {
        public List<StudentAttendanceCourse> Courses { get; set; }
    }

    public class StudentTimetableCourse
    {
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string LecturerName { get; set; }
        public int CreditHours { get; set; }
        public string Color { get; set; }
    }

    public class StudentClassSession
    {
        public int TimetableId { get; set; }
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string LecturerName { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Venue { get; set; }
        public string Color { get; set; }
    }

    public class StudentTimetablePage
    {
        public string SemesterName { get; set; }
        public DateTime SemesterStartDate { get; set; }
        public DateTime SemesterEndDate { get; set; }
        public int CurrentSemesterNo { get; set; }
        public string ProgrammeName { get; set; }
        public int CourseCount { get; set; }
        public int TotalCreditHours { get; set; }
        public decimal WeeklyContactHours { get; set; }
        public List<StudentTimetableCourse> Courses { get; set; }
        public List<StudentClassSession> Sessions { get; set; }
    }

    public class StudentGradeCourse
    {
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string LecturerName { get; set; }
        public int CreditHours { get; set; }
        public string Color { get; set; }
        public decimal? CurrentAverage { get; set; }
        public decimal CompletedPercent { get; set; }
        public bool HasFinalAssessment { get; set; }
        public decimal? FinalExamMarks { get; set; }
        public bool GradePublished { get; set; }
        public string LetterGrade { get; set; }
        public decimal? Gpa { get; set; }
    }

    public class StudentGradeSemester
    {
        public string SemesterName { get; set; }
        public DateTime StartDate { get; set; }
        public int SemesterNo { get; set; }
        public bool IsCurrent { get; set; }
        public int CourseCount { get; set; }
        public int Credits { get; set; }
        public int EarnedCredits { get; set; }
        public decimal? Gpa { get; set; }
        public List<StudentGradeCourse> Courses { get; set; }
    }

    public class StudentGradeDistributionItem
    {
        public string Grade { get; set; }
        public int Count { get; set; }
        public decimal BarWidth { get; set; }
    }

    public class StudentDashboardAssignment
    {
        public string Title { get; set; }
        public string CourseCode { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class StudentDashboardData
    {
        public StudentAccountProfile Account { get; set; }
        public StudentRegistrationTerm CurrentTerm { get; set; }
        public List<StudentCourseCard> Courses { get; set; }
        public List<StudentClassSession> TodayClasses { get; set; }
        public List<StudentDashboardAssignment> AssignmentsDueThisWeek { get; set; }
        public List<StudentPortalNotification> Announcements { get; set; }
        public decimal? Cgpa { get; set; }
        public decimal? AttendanceRate { get; set; }
        public int CreditsEarned { get; set; }
        public int PendingTaskCount { get; set; }
    }

    public class StudentGradePage
    {
        public List<StudentGradeSemester> Semesters { get; set; }
        public List<StudentGradeDistributionItem> GradeDistribution { get; set; }
        public decimal? Cgpa { get; set; }
        public decimal? CurrentGpa { get; set; }
        public StudentGradeSemester CurrentSemester { get; set; }
        public StudentGradeSemester BestSemester { get; set; }
        public int CreditsEarned { get; set; }
        public int CreditsAttempted { get; set; }
        public int CoursesGraded { get; set; }
    }

    public class StudentPaymentRow
    {
        public string InvoiceNo { get; set; }
        public string Description { get; set; }
        public string TermLabel { get; set; }
        public DateTime PaidDate { get; set; }
        public string Method { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }

    public class StudentPaymentHistoryPage
    {
        public List<StudentPaymentRow> Rows { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal PaidThisYear { get; set; }
        public decimal Refunded { get; set; }
        public int ReceiptCount { get; set; }
    }
}
