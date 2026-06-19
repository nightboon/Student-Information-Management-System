using System;
using System.Collections.Generic;

namespace src.services
{
    public static class LecturerPortalService
    {
        public static LecturerProfile GetProfile(UserContext user) => LecturerProfileReader.GetProfile(user);

        public static List<LecturerCourseCard> GetCourses(UserContext user) => LecturerCourseReader.GetCourses(user);

        public static CourseDashboardStats GetCourseStats(UserContext user, int offeringId) => LecturerCourseReader.GetCourseStats(user, offeringId);

        public static List<EnrolledStudentRow> GetRoster(UserContext user, int offeringId) => LecturerCourseReader.GetRoster(user, offeringId);

        public static LecturerDashboardData GetDashboard(UserContext user) => LecturerDashboardService.GetDashboard(user);

        public static List<LecturerClassSession> GetClassSessions(UserContext user) => LecturerTimetableReader.GetClassSessions(user);

        public static AttendanceOffering GetAttendanceOffering(UserContext user, int offeringId) => LecturerAttendanceReader.GetAttendanceOffering(user, offeringId);

        public static List<RosterEntry> GetAttendanceRoster(UserContext user, int offeringId, DateTime date) => LecturerAttendanceReader.GetAttendanceRoster(user, offeringId, date);

        public static int SaveAttendance(UserContext user, int offeringId, DateTime date, IDictionary<int, string> statuses) => LecturerAttendanceReader.SaveAttendance(user, offeringId, date, statuses);

        public static List<LecturerAssessmentOption> GetAssessments(UserContext user, int offeringId) => LecturerGradeReader.GetAssessments(user, offeringId);

        public static List<LecturerGradeRow> GetGradeRows(UserContext user, int assessmentId) => LecturerGradeReader.GetGradeRows(user, assessmentId);

        public static void SaveGradeMarks(UserContext user, int assessmentId, IDictionary<int, decimal?> marks) => LecturerGradeReader.SaveGradeMarks(user, assessmentId, marks);

        public static void PublishGrades(UserContext user, int assessmentId) => LecturerGradeReader.PublishGrades(user, assessmentId);

        public static List<LecturerAnnouncementRow> GetAnnouncements(UserContext user, int? offeringId) => LecturerAnnouncementReader.GetAnnouncements(user, offeringId);

        public static int AddAnnouncement(UserContext user, LecturerAnnouncementInput input) => LecturerAnnouncementReader.Add(user, input);

        public static bool DeleteAnnouncement(UserContext user, int announcementId) => LecturerAnnouncementReader.Delete(user, announcementId);

        public static List<LecturerMaterialRow> GetMaterials(UserContext user, int? offeringId) => LecturerMaterialReader.GetMaterials(user, offeringId);

        public static int AddMaterial(UserContext user, LecturerMaterialInput input) => LecturerMaterialReader.Add(user, input);

        public static bool DeleteMaterial(UserContext user, int materialId) => LecturerMaterialReader.Delete(user, materialId);

        public static List<AtRiskStudentRow> GetAtRisk(UserContext user) => LecturerAtRiskReader.GetAtRisk(user);
    }
}
