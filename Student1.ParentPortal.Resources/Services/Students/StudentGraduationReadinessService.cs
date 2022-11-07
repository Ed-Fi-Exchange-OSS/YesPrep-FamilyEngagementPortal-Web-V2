﻿using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Models.Student;
using Student1.ParentPortal.Resources.ExtensionMethods;
using Student1.ParentPortal.Resources.Providers.Configuration;

namespace Student1.ParentPortal.Resources.Services.Students
{
    public interface IStudentGraduationReadinessService
    {
        Task<StudentOnTrackToGraduate> IsStudentOnTrackToGraduateAsync(int studentUsi);
    }

    public class StudentGraduationReadinessService : IStudentGraduationReadinessService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICustomParametersProvider _customParametersProvider;

        public StudentGraduationReadinessService(IStudentRepository studentRepository, ICustomParametersProvider customParametersProvider)
        {
            _studentRepository = studentRepository;
            _customParametersProvider = customParametersProvider;
        }

        public async Task<StudentOnTrackToGraduate> IsStudentOnTrackToGraduateAsync(int studentUsi)
        {
            var isOnTrack = await _studentRepository.IsStudentOnTrackToGraduateAsync(studentUsi);

            var model = new StudentOnTrackToGraduate {
                OnTrackToGraduate = isOnTrack,
                Interpretation = InterpretOnTrackToGraduate(isOnTrack)
            };

            return model;
        }

        private string InterpretOnTrackToGraduate(bool? onTrack)
        {
            if (onTrack == null)
                return null;

            return _customParametersProvider.GetParameters().graduationReadiness.thresholdRules.GetRuleThatApplies(onTrack.Value).interpretation;
        }
    }
}
