using Student1.ParentPortal.Resources.Providers.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Models.Student;
using Assessment = Student1.ParentPortal.Models.Student.Assessment;
using StudentAssessment = Student1.ParentPortal.Models.Student.StudentAssessment;

namespace Student1.ParentPortal.Resources.Services.Students
{
    public interface IStudentAssessmentService
    {
        Task<StudentAssessment> GetStudentAssessmentsAsync(int studentUsi);
        Task<List<AssessmentSubSection>> GetStudentStaarAssessmentHistoryAsync(int studentUsi);
    }

    public class StudentAssessmentService : IStudentAssessmentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICustomParametersProvider _customParametersProvider;

        public StudentAssessmentService(IStudentRepository studentRepository, ICustomParametersProvider customParametersProvider)
        {
            _studentRepository = studentRepository;
            _customParametersProvider = customParametersProvider;
        }

        public async Task<StudentAssessment> GetStudentAssessmentsAsync(int studentUsi)
        {
            var model = new StudentAssessment();
            
            foreach(var assessmentParam in _customParametersProvider.GetParameters().assessments)
            {
                //objectiveAssesmentIdentificationCodes
               

                var assessment = await GetStudentAssessmentAsync(studentUsi, assessmentParam.assessmentReportingMethodTypeDescriptor, assessmentParam.title, assessmentParam.assessmentIdentifiers, assessmentParam.shortDescription, assessmentParam.objectiveAssesmentIdentificationCode);

                if (assessment != null)
                {
                    if (assessmentParam.objectiveAssesmentIdentificationCode == null)
                    {
                        assessment.SubSections = FillNotTakenSubsections(assessmentParam.assessmentIdentifiers, assessment.SubSections);
                    }
                    else {
                        assessment.SubSections = FillNotTakenSubsections(assessmentParam.objectiveAssesmentIdentificationCode, assessment.SubSections);
                    }
                    

                }
                else // Add default empty ones
                {
                    // Student hasnt taken assessment

                    assessment = FillNotTakenassessment(assessmentParam.title, assessmentParam.assessmentIdentifiers, assessmentParam.shortDescription, assessmentParam.objectiveAssesmentIdentificationCode);
                    
                }

                foreach (var subsection in assessment.SubSections)
                {
                    try
                    {
                        var title = assessmentParam.assesmentidentifierdisplaytitles.Where(m => m.assesmentidentifier == subsection.Title).FirstOrDefault().Display;

                        subsection.Title = title;
                    }
                    catch (Exception) { }
                }

                assessment.ExternalLink = assessmentParam.externalLink;
                assessment.ExternalLinkLegend = assessmentParam.externalLinkLegend;
                model.Assessments.Add(assessment);
            }

            return model;
        }

        public async Task<List<AssessmentSubSection>> GetStudentStaarAssessmentHistoryAsync(int studentUsi)
        {
            List<AssessmentSubSection> assessments = new List<AssessmentSubSection>();
            var studentDetailFeaturesToggles =_customParametersProvider.GetParameters().featureToggle.Where(f => f.page == "StudentDetail").FirstOrDefault();
            if (!studentDetailFeaturesToggles.features.Where(f => f.name == "STAAR").FirstOrDefault().enabled)
                return assessments;

            var assessmentParam =_customParametersProvider.GetParameters().staarAssessmentHistory;

            assessments = await GetStudentStaarAssessmentAsync(studentUsi, assessmentParam.title, assessmentParam.assessmentReportingMethodTypeDescriptor, assessmentParam.title, assessmentParam.assessmentIdentifiers, assessmentParam.shortDescription);
            foreach (var assessment in assessments) {
                assessment.Title = assessmentParam.assesmentidentifierdisplaytitles.Where(m => m.assesmentidentifier == assessment.Title).FirstOrDefault().Display;
            }

            return assessments;
        }

        private async Task<List<AssessmentSubSection>> GetStudentStaarAssessmentAsync(int studentUsi, string title, string assessmentReportingMethodTypeDescriptor,  string title1, List<string> assessmentIdentifiers, string shortDescription)
        {
            var data = await _studentRepository.GetStudentAssessmentAsync(studentUsi, assessmentReportingMethodTypeDescriptor, title, assessmentIdentifiers);

            // We are grouping here because we only want the latest administration.
            var model = (from d in data
                                where assessmentIdentifiers.Contains(d.Identifier)
                                orderby d.GradeLevel ascending
                                group d by new { d.Title, d.Identifier, d.GradeLevel } into g
                                select new AssessmentSubSection
                                {
                                Title = g.Key.Identifier,
                                Score = Convert.ToInt32(g.First(x => x.AdministrationDate == g.Max(d => d.AdministrationDate)).Result),
                                PerformanceLevelMet = g.First(x => x.AdministrationDate == g.Max(d => d.AdministrationDate)).PerformanceLevelMet,
                                GradeLevel = g.Key.GradeLevel,
                                Index= GetGradeLevelIndex(g.Key.GradeLevel)
                                }).ToList();

            return model;
        }


    

        private Assessment FillNotTakenassessment(string title, List<string> assessmentIdentifiers, string shortDescription, List<string> objectiveAssesmentIdentificationCodes)
        {
            var assessment = new Assessment {
                Title = title,
                ShortDescription = shortDescription,
                MaxRawScore = 0,
                SubSections = FillNotTakenSubsections(objectiveAssesmentIdentificationCodes==null ? assessmentIdentifiers:objectiveAssesmentIdentificationCodes, new List<AssessmentSubSection>())
            };

            return assessment;

        }

        private List<AssessmentSubSection> FillNotTakenSubsections(List<string> assessmentIdentifiers, List<AssessmentSubSection> subsections)
        {
            List<AssessmentSubSection> result = new List<AssessmentSubSection>();
            assessmentIdentifiers.ForEach(assesmnentIdentifier => {
                var assesment = subsections.FirstOrDefault(x => x.Title == assesmnentIdentifier);
                if (assesment == null)
                {
                    var subsection = new AssessmentSubSection
                    {
                        Title = assesmnentIdentifier,
                        Score = 0,
                        PerformanceLevelMet = "Not yet Taken"
                    };
                    result.Add(subsection);
                }
                else
                    result.Add(assesment);
            });

            return result;
        }

        private async Task<Assessment> GetStudentAssessmentAsync(int studentUsi, string assessmentReportingMethodTypeDescriptor, string assessmentTitle, List<string> assessmentIdentifiers, string shortDescription, List<string> objectiveAssesmentIdentificationCodes)
        {
            List<AssessmentRecord> data;

            if (objectiveAssesmentIdentificationCodes == null)
                data = await _studentRepository.GetStudentAssessmentAsync(studentUsi, assessmentReportingMethodTypeDescriptor, assessmentTitle, assessmentIdentifiers);
            else
                data = await _studentRepository.GetStudentObjectiveAssessmentAsync(studentUsi, assessmentReportingMethodTypeDescriptor, assessmentTitle, assessmentIdentifiers, objectiveAssesmentIdentificationCodes);

            // We are grouping here because we only want the latest administration.
            var groupingForLatestDate = (from d in data
                group d by new { d.Title, d.Identifier, d.MaxRawScore } into g
                select new
                {
                    g.Key.Title,
                    g.Key.Identifier,
                    g.Key.MaxRawScore,
                    LatestAdminitrationDate = g.Max(x => x.AdministrationDate),
                    g.First(x => x.AdministrationDate == g.Max(d => d.AdministrationDate)).Result,
                    g.First(x => x.AdministrationDate == g.Max(d => d.AdministrationDate)).PerformanceLevelMet
                });

            var model = groupingForLatestDate
                .GroupBy(a => a.Title)
                .Select(g => new Assessment
                {
                    Title = g.Key,
                    ShortDescription = shortDescription,
                    MaxRawScore = g.Sum(s => Convert.ToInt32(s.MaxRawScore)),
                    SubSections = g.Select(x => new AssessmentSubSection
                    {
                        Title = x.Identifier,
                        Score = Convert.ToInt32(x.Result),
                        PerformanceLevelMet = x.PerformanceLevelMet,
                        AdministrationDate = x.LatestAdminitrationDate
                    }).ToList()
                }).SingleOrDefault();

            return model;
        }


        private int GetGradeLevelIndex(string gl)
        {
            var edfiGradeLevels = new[] { "First grade", "Second grade", "Third grade", "Fourth grade", "Fith grade", "Sixth grade", "Seventh grade", "Eighth grade", "Ninth grade", "Tenth grade", "Eleventh grade", "Twelfth grade" };

            for (var i = 0; i < edfiGradeLevels.Length; i++)
                if (edfiGradeLevels[i] == gl)
                    return i;

            return -1;
        }

    }
}
