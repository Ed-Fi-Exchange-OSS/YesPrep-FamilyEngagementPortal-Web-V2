using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Student1.ParentPortal.Models.Shared;
using Student1.ParentPortal.Models.Staff;
using Student1.ParentPortal.Models.Student;

namespace Student1.ParentPortal.Data.Models
{
    public interface IStudentRepository
    {
        Task<PersonBriefModel> GetStudentBriefModelAsync(int studentUsi);
        Task<StudentDetailModel> GetStudentDetailAsync(int studentUsi);
        Task<List<StudentAttendanceEvent>> GetStudentAttendanceEventsAsync(int studentUsi);
        Task<List<StudentAbsencesCount>> GetStudentsWithAbsenceCountGreaterOrEqualThanThresholdAsync(int thresholdCount);
        Task<List<DisciplineIncident>> GetStudentDisciplineIncidentsAsync(int studentUsi, string disciplineIncidentDescriptor, DateTime date);
        Task<List<StudentCourse>> GetStudentTranscriptCoursesAsync(int studentUsi);
        Task<decimal?> GetStudentGPAAsync(int studentUsi);
        Task<List<StudentCurrentCourse>> GetStudentGradesByGradingPeriodAsync(int studentUsi, string gradeTypeGradingPeriodDescriptor, string gradeTypeSemesterDescriptor, string gradeTypeExamDescriptor, string gradeTypeFinalDescriptor);
        Task<List<AssessmentRecord>> GetStudentAssessmentAsync(int studentUsi, string assessmentReportingMethodTypeDescriptor, string assessmentTitle, List<string> assessmentIdentifiers);
        Task<List<AssessmentRecord>> GetStudentObjectiveAssessmentAsync(int studentUsi, string assessmentReportingMethodTypeDescriptor, string assessmentTitle, List<string> assessmentIdentifiers, List<string> objectiveAssesmentIdentificationCodes);

        Task<StudentMissingAssignments> GetStudentMissingAssignments(int studentUsi, string[] gradeBookMissingAssignmentTypeDescriptors, string missingAssignmentLetterGrade, DateTime today);
        Task<bool?> IsStudentOnTrackToGraduateAsync(int studentUsi);
        Task<List<StudentIndicator>> GetStudentIndicatorsAsync(int studentUsi);
        Task<List<StudentProgram>> GetStudentProgramsAsync(int studentUsi);
        Task<List<ScheduleItem>> GetStudentScheduleAsync(int studentUsi, DateTime mondayDate, DateTime fridayDate);
        Task<List<StudentSuccessTeamMember>> GetTeachers(int studentUsi, string recipientUniqueId, int recipientTypeId, DateTime beginningOfDayTodayWithPadding, DateTime endOfDayTodayWithPadding);
        Task<List<StudentSuccessTeamMember>> GetParents(int studentUsi, string recipientUniqueId, int recipientTypeId);
        Task<List<StudentSuccessTeamMember>> GetOtherStaff(int studentUsi, DateTime today);
        Task<List<StudentSuccessTeamMember>> GetProgramAssociatedStaff(int studentUsi);
        Task<List<StudentSuccessTeamMember>> GetSiblings(int studentUsi);
        Task<List<StudentSuccessTeamMember>> GetPrincipals(int studentUsi, string[] validLeadersDescriptors, string recipientUniqueId, int recipientTypeId);
        Task<List<StudentSummary>> GetStudentsSummary(List<int> StudentUsis);
        Task<PersonBriefModel> GetStudentBriefModelAsyncByUniqueId(string studentUniqueId);
        Task<List<StudentParentAssociationModel>> GetParentAssociation(int studentUsi);
        Task<List<ParentStudentsModel>> GetParentsBySection(Guid queueId, int staffUsi, StaffSectionModel model, string[] validEmailTypeDescriptors, DateTime beginningOfDayTodayWithPadding, DateTime endOfDayTodayWithPadding);
        Task<List<ParentStudentsModel>> GetParentsByStaff(Guid queueId, int staffUsi, StaffSectionModel model, string[] validEmailTypeDescriptors, DateTime beginningOfDayTodayWithPadding, DateTime endOfDayTodayWithPadding);
        Task<ParentStudentCountModel> GetFamilyMembersBySection(int staffUsi, StaffSectionModel model, string[] validParentDescriptors, DateTime today, DateTime beginningOfDayTodayWithPadding, DateTime endOfDayTodayWithPadding);
    }
}
