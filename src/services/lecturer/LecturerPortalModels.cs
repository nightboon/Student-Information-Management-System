using System;
using System.Collections.Generic;

namespace src.services
{
    public class LecturerProfile
    {
        public string LecturerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string MailingAddress { get; set; }
        public string DepartmentId { get; set; }
        public string IconPath { get; set; }
    }

    public class LecturerCourseCard
    {
        public int OfferingId { get; set; }
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
        public string SemesterName { get; set; }
        public int CreditHours { get; set; }
        public int EnrolledCount { get; set; }
        public string Color { get; set; }
        public string Status { get; set; }      // In progress / Upcoming / Completed
        public string Description
        {
            get { return "Course materials, assignments, announcements, and grades for " + CourseName + "."; }
        }
    }

    public class CourseDashboardStats
    {
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string LevelLabel { get; set; }
        public int CreditHours { get; set; }
        public string SemesterName { get; set; }
        public string Color { get; set; }
        public int EnrolledCount { get; set; }
        public int PendingCount { get; set; }
        public int PendingGrading { get; set; }
        public decimal? AverageGrade { get; set; }
        public decimal? AttendanceRate { get; set; }
        public int AssessmentCount { get; set; }
        public int MaterialCount { get; set; }
    }

    public class LecturerClassSession
    {
        public int TimetableId { get; set; }
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Venue { get; set; }
        public string Color { get; set; }
    }

    public class EnrolledStudentRow
    {
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ProgrammeName { get; set; }
        public string ProgrammeCode { get; set; }
        public string IconPath { get; set; }
        public bool IsPending { get; set; }
        public string StudentNo { get { return StudentId; } }
    }

    public class AttendanceOffering
    {
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int EnrolledCount { get; set; }
        public string Color { get; set; }
    }

    public class RosterEntry
    {
        public int EnrolmentId { get; set; }
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string IconPath { get; set; }
        public string Status { get; set; }
    }

    public class LecturerAttendanceHistoryRow
    {
        public int SessionId { get; set; }
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int PresentCount { get; set; }
        public int LateCount { get; set; }
        public int AbsentCount { get; set; }
        public int RecordedCount { get; set; }
    }

    public class LecturerAssessmentOption
    {
        public int AssessmentId { get; set; }
        public int OfferingId { get; set; }
        public string Label { get; set; }
    }

    public class LecturerGradeRow
    {
        public int SubmissionId { get; set; }
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public string StudentEmail { get; set; }
        public decimal? Marks { get; set; }
        public bool HasMarks { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public string FileUrl { get; set; }
        public string Grade { get; set; }
        public string SubmissionStatus { get; set; }
        public string MarkStatus { get; set; }
        public string SubmissionMode { get; set; }
        public bool IsLinkSubmission { get { return string.Equals(SubmissionMode, "LINK", StringComparison.OrdinalIgnoreCase); } }
        public bool CanReviewFile { get { return HasSubmission && !IsLinkSubmission; } }
        public bool IsMissing { get; set; }
        public bool RequiresSubmission { get; set; }
        public bool CanEnterMarks { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? ExtensionDeadline { get; set; }
        public DateTime? ExtensionGrantedAt { get; set; }
        public bool HasExtension { get { return ExtensionGrantedAt.HasValue; } }
        public string StudentName { get { return FullName; } }
        public string StudentNo { get { return StudentId; } }
        public string LetterGrade { get { return Grade; } }
        public bool HasSubmission { get { return SubmissionId > 0 && RequiresSubmission && !IsMissing && !string.IsNullOrWhiteSpace(FileUrl); } }
        public string SubmissionFileUrl { get { return FileUrl; } }
        public string AnnotatedFileUrl { get; set; }
        public string Feedback { get; set; }
    }

    public class LecturerMaterialRow
    {
        public int MaterialId { get; set; }
        public int OfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
        public string Title { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedAt { get; set; }
        // Inert (no backing column): kept so the .aspx bindings resolve.
        public string Description { get; set; }
        public string FileType { get; set; }
        public string MaterialType { get; set; }
        public int? Week { get; set; }
        public DateTime? DueDate { get; set; }
        public string SubmissionMode { get; set; }
        public decimal? Weight { get; set; }
        public int? FileSizeBytes { get; set; }
    }

    public class LecturerAnnouncementRow
    {
        public int AnnouncementId { get; set; }
        public int OfferingId { get; set; }
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string FileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPinned { get; set; }
        public string TargetCourses { get; set; }
        public bool HasAttachment { get { return !string.IsNullOrWhiteSpace(FileUrl); } }
    }

    public class AtRiskStudentRow
    {
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public decimal? AttendanceRate { get; set; }
        public decimal? AverageGrade { get; set; }
        public bool AttendanceRisk { get; set; }
        public bool AcademicRisk { get; set; }
        public string RiskLevel { get; set; }
        public string StudentName { get { return FullName; } }
        public string StudentNo { get { return StudentId; } }
        public decimal? AttendancePercent { get { return AttendanceRate; } }
        public decimal? CurrentMark { get { return AverageGrade; } }
        public string RiskReason
        {
            get
            {
                if (AttendanceRisk && AcademicRisk) return "Low attendance and low marks";
                if (AttendanceRisk) return "Low attendance";
                if (AcademicRisk) return "Low marks";
                return "";
            }
        }
    }

    public class LecturerAcademicPerformanceRow
    {
        public int OfferingId { get; set; }
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public string ProgrammeCode { get; set; }
        public string AcademicYear { get; set; }
        public string OfferingSemester { get; set; }
        public int Semester { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public decimal? AttendanceRate { get; set; }
        public decimal? AverageMarks { get; set; }
        public decimal GradePoint { get; set; }
        public string LetterGrade { get; set; }
        public bool AttendanceRisk { get; set; }
        public bool AcademicRisk { get; set; }
        public bool IsAtRisk { get { return AttendanceRisk || AcademicRisk; } }
        public string Status { get { return !AverageMarks.HasValue ? "Pending" : AverageMarks.Value >= 50m ? "Pass" : "Fail"; } }
        public string RiskLevel { get { return AttendanceRisk && AcademicRisk ? "High" : "Medium"; } }
        public string RiskReason
        {
            get
            {
                if (AttendanceRisk && AcademicRisk) return "Low attendance and low marks";
                if (AttendanceRisk) return "Attendance below 80%";
                if (AcademicRisk) return "Marks below 50%";
                return "";
            }
        }
    }

    public class LecturerGradingItem
    {
        public int AssessmentId { get; set; }
        public int OfferingId { get; set; }
        public string Title { get; set; }
        public string CourseCode { get; set; }
        public DateTime DueDate { get; set; }
        public int PendingCount { get; set; }
    }

    public class LecturerDashboardData
    {
        public LecturerProfile Profile { get; set; }
        public StudentRegistrationTerm CurrentTerm { get; set; }
        public List<LecturerCourseCard> Courses { get; set; }
        public List<LecturerClassSession> TodayClasses { get; set; }
        public List<LecturerGradingItem> ToGrade { get; set; }
        public List<LecturerAnnouncementRow> Announcements { get; set; }
        public int TotalAnnouncementCount { get; set; }
        public int ActiveCourses { get; set; }
        public int StudentsTaught { get; set; }
        public int SubmissionsToReview { get; set; }
        public decimal? AttendanceRate { get; set; }
    }

    public class LecturerAnnouncementInput
    {
        public int OfferId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string FileUrl { get; set; }
        public bool IsPinned { get; set; }
    }

    public class LecturerMaterialInput
    {
        public int OfferingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MaterialType { get; set; }
        public string SubmissionMode { get; set; }
        public int? Week { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? Weight { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public int? FileSizeBytes { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class MaterialWeightUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal Weight { get; set; }
        public decimal CourseTotal { get; set; }
    }
}
