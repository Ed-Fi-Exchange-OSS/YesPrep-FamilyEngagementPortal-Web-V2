﻿using Student1.ParentPortal.Resources.Providers.Image;
using Student1.ParentPortal.Resources.Services.Students;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Student1.ParentPortal.Models.Shared;
using Student1.ParentPortal.Models.Staff;
using Student1.ParentPortal.Models.Student;
using Student1.ParentPortal.Models.User;
using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Resources.Providers.Configuration;
using Student1.ParentPortal.Resources.ExtensionMethods;
using System;
using Student1.ParentPortal.Resources.Services.Communications;
using System.Net.Http;
using Student1.ParentPortal.Resources.Services.Alerts;
using Student1.ParentPortal.Resources.Providers.ClassPeriodName;
using Student1.ParentPortal.Resources.Cache;
using Student1.ParentPortal.Resources.Providers.Date;

namespace Student1.ParentPortal.Resources.Services
{
    public interface ITeachersService {
        Task<List<StaffSectionModel>> GetStaffSectionsAsync(int staffUsi);
        Task<List<StudentBriefModel>> GetStudentsInSection(int staffUsi, TeacherStudentsRequestModel model, string recipientUniqueId, int recipientTypeId);
        [NoCache]
        Task<UserProfileModel> GetStaffProfileAsync(int staffUsi);
        [NoCache]
        Task<BriefProfileModel> GetBriefStaffProfileAsync(int staffUsi);
        Task<UserProfileModel> SaveStaffProfileAsync(int staffUsi, UserProfileModel model);
        Task UploadStaffImageAsync(string staffUniqueId, byte[] image, string contentType);
        Task UpdateStaffLanguage(string staffUniqueId, string languageCode);
    }
    public class TeachersService : ITeachersService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentAttendanceService _studentAttendanceService;
        private readonly IStudentBehaviorService _studentBehaviorService;
        private readonly IStudentCourseGradesService _studentCourseGradesService;
        private readonly IStudentAssignmentService _studentAssignmentService;
        private readonly IImageProvider _imageProvider;
        private readonly ICustomParametersProvider _customParametersProvider;
        private readonly ICommunicationsService _communicationsService;
        private readonly ISpotlightIntegrationsService _spotlightIntegrationsService;
        private readonly IAlertService _alertService;
        private readonly IClassPeriodNameProvider _classPeriodNameProvider;
        private readonly IStudentsService _studentsService;
        private readonly IStudentRepository _studentRepository;
        private readonly IDateProvider _dateProvider;
        private readonly ISchoolsRepository _schoolsRepository;

        public TeachersService(ITeacherRepository teacherRepository, IStudentAttendanceService studentAttendanceService, IStudentBehaviorService studentBehaviorService, IStudentCourseGradesService studentCourseGradesService, IImageProvider imageProvider, IStudentAssignmentService studentAssignmentService, ICustomParametersProvider customParametersProvider, ICommunicationsService communicationsService, ISpotlightIntegrationsService spotlightIntegrationsService, IAlertService alertService, IClassPeriodNameProvider classPeriodNameProvider, IStudentsService studentsService, IStudentRepository studentRepository, IDateProvider dateProvider)
        {
            _teacherRepository = teacherRepository;
            _studentAttendanceService = studentAttendanceService;
            _studentBehaviorService = studentBehaviorService;
            _studentCourseGradesService = studentCourseGradesService;
            _imageProvider = imageProvider;
            _studentAssignmentService = studentAssignmentService;
            _customParametersProvider = customParametersProvider;
            _communicationsService = communicationsService;
            _spotlightIntegrationsService = spotlightIntegrationsService;
            _alertService = alertService;
            _classPeriodNameProvider = classPeriodNameProvider;
            _studentsService = studentsService;
            _studentRepository = studentRepository;
            _dateProvider = dateProvider;
        }

        public async Task<List<StaffSectionModel>> GetStaffSectionsAsync(int staffUsi)
        {
            var beginningOfDayTodayWithPadding = await  _dateProvider.BeginningOfDayTodayWithPadding();
            var endOfDayTodayWithPadding = _dateProvider.EndOfDayTodayWithPadding();
            var beginningOfDayToday = _dateProvider.BeginningOfDayToday();
            var endOfDayToday = _dateProvider.EndOfDayToday();
            var data = await _teacherRepository.GetStaffSectionsAsync(staffUsi, beginningOfDayTodayWithPadding, endOfDayTodayWithPadding, beginningOfDayToday,endOfDayToday);

            // Get the current sections by grouping to remove duplicates that are from a previous term.
            var model = (from d in data
                         //group d by new{ d.LocalCourseCode,d.SessionName } into g
                         select new StaffSectionModel
                         {
                             LocalCourseCode = d.LocalCourseCode,
                             SchoolId = d.SchoolId,
                             SchoolYear = d.SchoolYear,
                             UniqueSectionCode = d.UniqueSectionCode,
                             SessionName = d.SessionName,
                             ClassPeriodName = d.ClassPeriodName,
                             ClassroomIdentificationCode = d.ClassroomIdentificationCode,
                             TermDescriptorId = d.TermDescriptorId,
                             SequenceOfCourse = d.SequenceOfCourse,
                             BeginDate = d.BeginDate,
                             EndDate = d.EndDate,
                             CourseTitle = d.CourseTitle
                         }).ToList();

            return model;
        }

        public async Task<List<StudentBriefModel>> GetStudentsInSection(int staffUsi, TeacherStudentsRequestModel model, string recipientUniqueId, int recipientTypeId)
        {
            var beginningOfDayTodayWithPadding = await _dateProvider.BeginningOfDayTodayWithPadding();

            var endOfDayTodayWithPadding = _dateProvider.EndOfDayTodayWithPadding();

            var studentsAssociatedWithParent = await _teacherRepository.GetTeacherStudents(staffUsi, model, recipientUniqueId, recipientTypeId, beginningOfDayTodayWithPadding, endOfDayTodayWithPadding);
            var studentSummaries = await _studentsService.GetStudentsSummary(studentsAssociatedWithParent.Select(x => x.StudentUsi).ToList());
            // Get other calculated fields.
            foreach (var student in studentsAssociatedWithParent)
            {
                var summary = studentSummaries.Find(x => x.StudentUsi == student.StudentUsi);
                student.StudentUniqueId = summary.StudentUniqueId;
                student.FirstName = summary.FirstName;
                student.MiddleName = summary.MiddleName;
                student.LastSurname = summary.LastSurname;
                student.GradeLevel = summary.GradeLevel;
                student.SexType = summary.SexType;
                student.YTDGPA = summary.Gpa;
                student.GPAInterpretation = summary.GpaInterpretation;
                student.AbsenceThresholdDays = _customParametersProvider.GetParameters().attendance.ADA.maxAbsencesCountThreshold;
                student.AdaAbsences = summary.AbsenceCount;
                student.AdaAbsentInterpretation = summary.AbsenceInterpretation;
                student.YTDDisciplineIncidentCount = summary.DisciplineIncidentCount;
                student.YTDDisciplineInterpretation = summary.DisciplineIncidentInterpretation;
                //student.UnexcusedInterpretation = attendance.UnexcusedInterpretation;
                //student.ExcusedInterpretation = attendance.ExcusedInterpretation;
                //student.TardyInterpretation = attendance.TardyInterpretation;
                student.MissingAssignments = summary.MissingassignmentCount;
                student.MissingAssignmentsInterpretation = summary.MissingAssigmentInterpretation;

                student.CourseAverage = new StudentCurrentGradeAverage { Evaluation = summary.CourseAverageInterpretation, GradeAverage = summary.CourseAverage };
                student.ImageUrl = await _imageProvider.GetStudentImageUrlAsync(student.StudentUniqueId);
            }

            return studentsAssociatedWithParent;
        }

        public async Task<UserProfileModel> GetStaffProfileAsync(int staffUsi)
        {
            var model = await _teacherRepository.GetStaffProfileAsync(staffUsi);
            model.ImageUrl = await _imageProvider.GetStaffImageUrlAsync(model.UniqueId);

            return model;
        }

        public async Task<BriefProfileModel> GetBriefStaffProfileAsync(int staffUsi)
        {
            var model = await _teacherRepository.GetBriefStaffProfileAsync(staffUsi);
            model.ImageUrl = await _imageProvider.GetStaffImageUrlAsync(model.UniqueId);
            model.FeedbackExternalUrl = _customParametersProvider.GetParameters().feedbackExternalUrl;
            return model;
        }

        public async Task<UserProfileModel> SaveStaffProfileAsync(int staffUsi, UserProfileModel model)
        {
            return await _teacherRepository.SaveStaffProfileAsync(staffUsi, model);

        }

        public async Task UploadStaffImageAsync(string staffUniqueId, byte[] image, string contentType)
        {
            await _imageProvider.UploadStaffImageAsync(staffUniqueId, image, contentType);
        }

        public async Task UpdateStaffLanguage(string staffUniqueId, string languageCode)
        {
            await _teacherRepository.SaveStaffLanguage(staffUniqueId, languageCode);
        }
    }
}
