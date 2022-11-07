using Student1.ParentPortal.Data.Models.EdFi25;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Student1.ParentPortal.Models.Student;
using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Resources.ExtensionMethods;
using Student1.ParentPortal.Resources.Providers.Configuration;
using Student1.ParentPortal.Models.Shared;
using Student1.ParentPortal.Resources.Providers.Date;

namespace Student1.ParentPortal.Resources.Services.Students
{
    public interface IStudentAssignmentService
    {
        Task<StudentMissingAssignments> GetStudentMissingAssignments(int studentUSI);
    }

    public class StudentAssignmentService : IStudentAssignmentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICustomParametersProvider _customParametersProvider;
        private readonly IDateProvider _dateProvider;

        public StudentAssignmentService(IStudentRepository studentRepository, ICustomParametersProvider customParametersProvider, IDateProvider dateProvider)
        {
            _studentRepository = studentRepository;
            _customParametersProvider = customParametersProvider;
            _dateProvider = dateProvider;
        }

        public async Task<StudentMissingAssignments> GetStudentMissingAssignments(int studentUsi)
        {
            var descriptors = _customParametersProvider.GetParameters().descriptors;
            var missingAssignments = await _studentRepository.GetStudentMissingAssignments(studentUsi, descriptors.gradeBookMissingAssignmentTypeDescriptors, descriptors.missingAssignmentLetterGrade, _dateProvider.Today());
            var externalLink = _customParametersProvider.GetParameters().assignments.linkToSystemWithDetails;
            var missingAssignmentLink = new LinkModel { Title = externalLink.title, LinkText = externalLink.linkText, Url = externalLink.url };

            missingAssignments.Interpretation = Interpret(missingAssignments.MissingAssignmentCount);
            missingAssignments.ExternalLink = missingAssignmentLink;
            return missingAssignments;
        }

        private string Interpret(int count)
        {
            return _customParametersProvider.GetParameters().assignments.thresholdRules.GetRuleThatApplies(count).interpretation;
        }
    }
}
