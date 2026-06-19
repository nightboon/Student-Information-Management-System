using System.Collections.Generic;

namespace src.services
{
    public static class StudentPortalService
    {
        public static StudentAccountProfile GetAccount(UserContext user) => StudentProfileReader.GetAccount(user);

        public static List<StudentCourseCard> GetCourses(UserContext user)
        {
            var account = StudentProfileReader.GetAccount(user);
            return account == null
                ? new List<StudentCourseCard>()
                : StudentCourseReader.GetCourses(user, account.StudentId);
        }

        public static StudentCourseHeader GetCourseHeader(UserContext user, int offeringId) => StudentCourseReader.GetCourseHeader(user, offeringId);

        public static List<StudentCourseModule> GetCourseModules(UserContext user, int offeringId) => StudentCourseReader.GetCourseModules(user, offeringId);

        public static List<StudentLearningOutcome> GetLearningOutcomes(UserContext user, string courseId) => StudentCourseReader.GetLearningOutcomes(user, courseId);

        public static List<StudentCourseAnnouncement> GetAnnouncements(UserContext user, int? offeringId) => StudentAnnouncementReader.GetAnnouncements(user, offeringId);

        public static List<StudentPortalNotification> GetNotifications(UserContext user, ISet<int> readIds) => StudentAnnouncementReader.GetNotifications(user, readIds);

        public static List<StudentCourseAssignment> GetAssignments(UserContext user, int? offeringId)
        {
            var account = StudentProfileReader.GetAccount(user);
            return account == null
                ? new List<StudentCourseAssignment>()
                : StudentAssignmentReader.GetAssignments(user, account.StudentId, offeringId);
        }

        public static StudentGradebook GetGradebook(UserContext user, int offeringId)
        {
            var account = StudentProfileReader.GetAccount(user);
            return account == null
                ? new StudentGradebook { Items = new List<StudentAssessmentRow>(), OverallAverage = null, EarnedWeighted = 0m, CompletedPercent = 0m }
                : StudentAssignmentReader.GetGradebook(user, account.StudentId, offeringId);
        }

        public static bool SaveSubmission(UserContext user, int assignmentId, string fileUrl) => StudentAssignmentReader.SaveSubmission(user, assignmentId, fileUrl);

        public static StudentEnrollmentPage GetEnrollmentPage(UserContext user) => StudentEnrolmentReader.GetEnrollmentPage(user);

        public static int Enrol(UserContext user, int[] offeringIds) => StudentEnrolmentReader.Enrol(user, offeringIds);

        public static StudentAttendancePage GetAttendancePage(UserContext user)
        {
            var account = StudentProfileReader.GetAccount(user);
            return account == null ? null : StudentAttendanceReader.GetAttendancePage(user, account.StudentId);
        }

        public static StudentTimetablePage GetTimetablePage(UserContext user) => StudentTimetableReader.GetTimetablePage(user);

        public static StudentGradePage GetGradePage(UserContext user) => StudentGradeReader.GetGradePage(user);

        public static StudentDashboardData GetDashboard(UserContext user, ISet<int> readIds) => StudentDashboardService.GetDashboard(user, readIds);

    }
}
