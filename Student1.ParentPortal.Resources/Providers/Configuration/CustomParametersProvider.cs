using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace Student1.ParentPortal.Resources.Providers.Configuration
{
    public interface ICustomParametersProvider
    {
        CustomParameters GetParameters();
    }
    public class CustomParametersProvider : ICustomParametersProvider
    {
        public CustomParameters GetParameters()
        {
            string target = ConfigurationManager.AppSettings["application.ed-fi.target"];
            var version = ConfigurationManager.AppSettings["application.ed-fi.version"]+target;
            var pathToJson = HttpContext.Current.Server.MapPath($"~/customizableParameters{version}.json");
            return JsonConvert.DeserializeObject<CustomParameters>(File.ReadAllText(pathToJson));
        }
    }
    /* Classes below are generated with http://json2csharp.com/ */
    // To regenerate: 
    //  - Copy and paste JSON from the root ~.Web/customizableParameters.json file.
    //  - Rename "RootObject" to "CustomParameters"

    public class CustomParameters
    {
        public Attendance attendance { get; set; }
        public Behavior behavior { get; set; }
        public CourseGrades courseGrades { get; set; }
        public Assignments assignments { get; set; }
        public List<Assessment> assessments { get; set; }
        public Assessment staarAssessmentHistory { get; set; }
        public GraduationReadiness graduationReadiness { get; set; }
        public List<ExternalLink> studentProfileExternalLinks { get; set; }
        public Descriptors descriptors { get; set; }
        public Tooltips tooltips { get; set; }
        public string feedbackExternalUrl { get; set; }
        public bool showReports { get; set; }
        public bool showReportsAdmin { get; set; }
        public bool showReportsCL { get; set; }
        public bool showReportsTeacher { get; set; }
        public string[] mostCommonLanguageCodes { get; set; }
        public List<Page> featureToggle { get; set; }
        public SessionPadding sessionPadding { get; set; }
    }
    public class ThresholdRule<T>
    {
        public string interpretation { get; set; }
        public string @operator { get; set; }
        public T value { get; set; }
    }

    public class Attendance
    {
        public AttendanceType ADA { get; set; }
        public AttendanceType periodLevelAttendance { get; set; }
    }

    public class AttendanceType
    {
       public int maxAbsencesCountThreshold { get; set; }
       public AttendanceCategory excused { get; set; }
       public AttendanceCategory unexcused { get; set; }
       public AttendanceCategory tardy { get; set; }
    }

    public class AttendanceCategory
    {
        public int maxAbsencesCountThreshold { get; set; }
        public int alertThreshold { get; set; }
        public List<ThresholdRule<int>> thresholdRules { get; set; }
    }

    public class Behavior
    {
        public int maxDisciplineIncidentsCountThreshold { get; set; }
        public int alertThreshold { get; set; }
        public List<ThresholdRule<int>> thresholdRules { get; set; }
        public ExternalLink linkToSystemWithDetails { get; set; }
    }

    public class Gpa
    {
        public double nationalAverage { get; set; }
        public List<ThresholdRule<decimal>> thresholdRules { get; set; }
    }

    public class CourseAverage
    {
        public int alertThreshold { get; set; }
        public List<ThresholdRule<decimal>> thresholdRules { get; set; }
    }

    public class CourseGrades
    {
        public Gpa gpa { get; set; }
        public CourseAverage courseAverage { get; set; }
        public ExternalLink linkToSystemWithDetails { get; set; }
    }

    public class Assignments
    {
        public int alertThreshold { get; set; }
        public int maxAssigmentsCountThreshold { get; set; }
        public List<ThresholdRule<int>> thresholdRules { get; set; }
        public ExternalLink linkToSystemWithDetails { get; set; }
    }

    public class Assessment
    {
        public string title { get; set; }
        public List<string> assessmentIdentifiers { get; set; }
        public string assessmentReportingMethodTypeDescriptor { get; set; }
        public string shortDescription { get; set; }
        public string externalLink { get; set; }
        public string externalLinkLegend { get; set; }
        public List<string>  objectiveAssesmentIdentificationCode { get; set; }
        public List<assesmentidentifierdisplays> assesmentidentifierdisplaytitles { get; set; }

    }

    public class assesmentidentifierdisplays    {
      public string   assesmentidentifier { get; set; }
      public string Display { get; set; }
    }

    public class GraduationReadiness
    {
        public List<ThresholdRule<bool>> thresholdRules { get; set; }
    }

    public class ExternalLink
    {
        public string title { get; set; }
        public string url { get; set; }
        public string linkText { get; set; }
        public string linkTextTeacher { get; set; }
    }

    public class Descriptors
    {
        public string excusedAbsenceDescriptorCodeValue { get; set; }
        public string unexcusedAbsenceDescriptorCodeValue { get; set; }
        public string tardyDescriptorCodeValue { get; set; }
        public string absentDescriptorCodeValue { get; set; }
        public string[] gradeBookMissingAssignmentTypeDescriptors { get; set; }
        public string gradeTypeGradingPeriodDescriptor { get; set; }
        public string gradeTypeSemesterDescriptor { get; set; }
        public string gradeTypeExamDescriptor { get; set; }
        public string gradeTypeFinalDescriptor { get; set; }
        public string[] validParentDescriptors { get; set; }
        public string[] validStaffDescriptors { get; set; }
        public string [] validCampusLeaderDescriptors { get; set; }
        public string [] validCampusLeaderDescriptorsForStudentSuccessTeam { get; set; }
        public string [] schoolGradingPeriodDescriptors { get; set; }
        public string [] examGradingPeriods { get; set; }
        public string disciplineIncidentDescriptor { get; set; }
        public string missingAssignmentLetterGrade { get; set; }
        public string[] validEmailTypeDescriptors { get; set; }
        public string[] validMobileTelephoneNumberTypeDescriptors { get; set; }
        
    }

    public class Tooltips
    {
        public string eligibleStaff { get; set; }
        public string campusLeaders { get; set; }
        public string teachers { get; set; }
        public string eligibleParents { get; set; }
        public string messagesSent { get; set; }
        public string messagesReceived { get; set; }
        public string unreadMessages { get; set; }
        public string ytd { get; set; }
    }

    public class Page
    {
        public string page { get; set; }
        public string route { get; set; }
        public List<Feature> features { get; set; }
    }

    public class Feature
    {
        public string name { get; set; }
        public bool enabled { get; set; }
        public StudentAbc studentAbc { get; set; }
    }

    public class StudentAbc 
    {
        public bool missingAssignments { get; set; }
        public bool courseAverage { get; set; }
        public bool behavior { get; set; }
        public bool absence { get; set; }
    }

    public class SessionPadding
    {
        public int DaysBeforeSessionStart { get; set; }
        public int DaysAfterSessionEnd { get; set; }
    }
}
