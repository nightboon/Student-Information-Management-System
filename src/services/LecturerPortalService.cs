using System;
using System.Collections.Generic;

namespace src.services
{
    public static class LecturerPortalService
    {
        public static LecturerProfile GetProfile(UserContext user) => LecturerProfileReader.GetProfile(user);

        public static List<LecturerCourseCard> GetCourses(UserContext user) => LecturerCourseReader.GetCourses(user);
        public static List<StudentCourseModule> GetCourseModules(UserContext user, int offeringId) => StudentCourseReader.GetCourseModules(user, offeringId);

        public static CourseDashboardStats GetCourseStats(UserContext user, int offeringId) => LecturerCourseReader.GetCourseStats(user, offeringId);

        public static List<EnrolledStudentRow> GetRoster(UserContext user, int offeringId) => LecturerCourseReader.GetRoster(user, offeringId);

        public static LecturerDashboardData GetDashboard(UserContext user) => LecturerDashboardService.GetDashboard(user);

        public static List<LecturerClassSession> GetClassSessions(UserContext user) => LecturerTimetableReader.GetClassSessions(user);
        public static List<LecturerClassSession> GetManageableClassSessions(UserContext user) => LecturerTimetableReader.GetManageableClassSessions(user);
        public static int SaveClassSession(UserContext user, int timetableId, int offerId, string dayOfWeek, TimeSpan startTime, TimeSpan endTime, string room) => LecturerTimetableReader.SaveClassSession(user, timetableId, offerId, dayOfWeek, startTime, endTime, room);
        public static bool DeleteClassSession(UserContext user, int timetableId) => LecturerTimetableReader.DeleteClassSession(user, timetableId);

        public static AttendanceOffering GetAttendanceOffering(UserContext user, int offeringId) => LecturerAttendanceReader.GetAttendanceOffering(user, offeringId);

        public static List<LecturerAttendanceHistoryRow> GetAttendanceHistory(UserContext user) => LecturerAttendanceReader.GetHistory(user);

        public static List<RosterEntry> GetAttendanceRoster(UserContext user, int offeringId, DateTime date, TimeSpan startTime, TimeSpan endTime) => LecturerAttendanceReader.GetAttendanceRoster(user, offeringId, date, startTime, endTime);

        public static int SaveAttendance(UserContext user, int offeringId, DateTime date, TimeSpan startTime, TimeSpan endTime, IDictionary<int, string> statuses) => LecturerAttendanceReader.SaveAttendance(user, offeringId, date, startTime, endTime, statuses);

        public static List<LecturerAssessmentOption> GetAssessments(UserContext user, int offeringId) => LecturerGradeReader.GetAssessments(user, offeringId);

        public static List<LecturerAssessmentOption> GetAssessments(UserContext user) => LecturerGradeReader.GetAssessments(user, null);

        public static List<LecturerGradeRow> GetGradeRows(UserContext user, int assessmentId) => LecturerGradeReader.GetGradeRows(user, assessmentId);

        public static void SaveGradeMarks(UserContext user, int assessmentId, IDictionary<int, decimal?> marks) => LecturerGradeReader.SaveGradeMarks(user, assessmentId, marks);

        public static GradeNotificationService.GradeEmailSendResult PublishGrades(UserContext user, int assessmentId) => LecturerGradeReader.PublishGrades(user, assessmentId);
        public static bool GrantSubmissionExtension(UserContext user, int submissionId, DateTime deadline) => LecturerGradeReader.GrantExtension(user, submissionId, deadline);
        public static bool ExpireSubmissionExtension(UserContext user, int submissionId) => LecturerGradeReader.ExpireExtension(user, submissionId);

        public static bool SaveSubmissionReview(UserContext user, int submissionId, string feedback, string annotatedFileUrl) => LecturerGradeReader.SaveReview(user, submissionId, feedback, annotatedFileUrl);
        public static string GetAnnotationDraft(UserContext user, int submissionId) => LecturerGradeReader.GetAnnotationDraft(user, submissionId);
        public static bool SaveAnnotationDraft(UserContext user, int submissionId, string annotationsJson) => LecturerGradeReader.SaveAnnotationDraft(user, submissionId, annotationsJson);

        public static List<LecturerAnnouncementRow> GetAnnouncements(UserContext user, int? offeringId) => LecturerAnnouncementReader.GetAnnouncements(user, offeringId);

        public static int AddAnnouncement(UserContext user, LecturerAnnouncementInput input) => LecturerAnnouncementReader.Add(user, input);

        public static bool DeleteAnnouncement(UserContext user, int announcementId) => LecturerAnnouncementReader.Delete(user, announcementId);

        public static bool SetAnnouncementPinned(UserContext user, int announcementId, bool isPinned) => LecturerAnnouncementReader.SetPinned(user, announcementId, isPinned);

        public static List<LecturerMaterialRow> GetMaterials(UserContext user, int? offeringId) => LecturerMaterialReader.GetMaterials(user, offeringId);

        public static LecturerMaterialRow GetMaterial(UserContext user, int materialId) => LecturerMaterialReader.GetMaterial(user, materialId);

        public static int AddMaterial(UserContext user, LecturerMaterialInput input) => LecturerMaterialReader.Add(user, input);

        public static decimal GetMaterialWeightTotal(UserContext user, int offeringId) => LecturerMaterialReader.GetWeightTotal(user, offeringId);
        public static MaterialWeightUpdateResult UpdateMaterialWeight(UserContext user, int materialId, decimal weight) => LecturerMaterialReader.UpdateWeight(user, materialId, weight);

        public static bool DeleteMaterial(UserContext user, int materialId) => LecturerMaterialReader.Delete(user, materialId);
        public static bool UpdateModule(UserContext user, int offeringId, string moduleId, string title, string description) => LecturerMaterialReader.UpdateModule(user, offeringId, moduleId, title, description);

        public static List<AtRiskStudentRow> GetAtRisk(UserContext user) => LecturerAtRiskReader.GetAtRisk(user);

        public static List<LecturerAcademicPerformanceRow> GetAcademicPerformance(UserContext user) => LecturerAtRiskReader.GetAcademicPerformance(user);
    }
}
