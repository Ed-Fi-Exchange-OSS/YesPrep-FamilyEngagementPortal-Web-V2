﻿using Student1.ParentPortal.Data.Models.EdFi25;
using Student1.ParentPortal.Resources.Providers.Image;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Student1.ParentPortal.Models.Student;
using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Resources.Services.Communications;
using Student1.ParentPortal.Resources.Providers.Configuration;
using Student1.ParentPortal.Resources.Cache;
using Student1.ParentPortal.Resources.Providers.Date;

namespace Student1.ParentPortal.Resources.Services.Students
{
    public interface IStudentSuccessTeamService
    {
        [NoCache]
        Task<List<StudentSuccessTeamMember>> GetStudentSuccessTeamAsync(int studentUSI, string recipientUniqueId, int recipientTypeId);
    }

    public class StudentSuccessTeamService : IStudentSuccessTeamService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IImageProvider _imageUrlProvider;
        private readonly ICommunicationsService _communicationsService;
        private readonly ICustomParametersProvider _customParametersProvider;
        private readonly IDateProvider _dateProvider;

        public StudentSuccessTeamService(IStudentRepository studentRepository, 
                                         IImageProvider imageUrlProvider, 
                                         ICommunicationsService communicationsService,
                                         ICustomParametersProvider customParametersProvider,
                                         IDateProvider dateProvider)
        {
            _studentRepository = studentRepository;
            _imageUrlProvider = imageUrlProvider;
            _communicationsService = communicationsService;
            _customParametersProvider = customParametersProvider;
            _dateProvider = dateProvider;
        }

        public async Task<List<StudentSuccessTeamMember>> GetStudentSuccessTeamAsync(int studentUSI, string recipientUniqueId, int recipientTypeId)
        {
            // In Ed-Fi all people associated with a student are:
            // Teachers through the StaffSectionAssociation table
            // Other Staff like counselors through the StaffCohortAssociation table
            // Parents through StudentParentAssociation table

            var model = new List<StudentSuccessTeamMember>();

            model.AddRange(await GetParents(studentUSI, recipientUniqueId, recipientTypeId));
            model.AddRange(await GetTeachers(studentUSI, recipientUniqueId, recipientTypeId));
            model.AddRange((await GetOtherStaff(studentUSI)));
            model.AddRange((await GetProgramAssociatedStaff(studentUSI)));
            model.AddRange(await GetSiblings(studentUSI));
            model.AddRange(await GetPrincipals(studentUSI, recipientUniqueId, recipientTypeId));

            return model;
        }

        private async Task<List<StudentSuccessTeamMember>> GetTeachers(int studentUsi, string recipientUniqueId, int recipientTypeId)
        {
            // Get the current max date of the students section associations as we only want current teachers.
            var teachers = await _studentRepository.GetTeachers(studentUsi, recipientUniqueId, recipientTypeId,await _dateProvider.BeginningOfDayTodayWithPadding(),_dateProvider.EndOfDayTodayWithPadding());

            //Add Image Urls
            foreach (var t in teachers)
            {
                t.ImageUrl = await _imageUrlProvider.GetStaffImageUrlAsync(t.UniqueId);
            }

            return teachers;
        }

        private async Task<List<StudentSuccessTeamMember>> GetOtherStaff(int studentUsi)
        {
            // Get the current max date of the students cohort associations as we only want current staff/counselors.
            var otherStaff = await _studentRepository.GetOtherStaff(studentUsi, _dateProvider.Today());

            // Add Image Urls
            foreach (var s in otherStaff)
            {
                s.ImageUrl = await _imageUrlProvider.GetStaffImageUrlAsync(s.UniqueId);
            }

            return otherStaff;
        }

        private async Task<List<StudentSuccessTeamMember>> GetParents(int studentUsi, string recipientUniqueId, int recipientTypeId)
        {

            var parents = await _studentRepository.GetParents(studentUsi, recipientUniqueId, recipientTypeId);
            // Add Image Urls
            foreach (var p in parents)
            {
                p.ImageUrl = await _imageUrlProvider.GetParentImageUrlAsync(p.UniqueId);
            }

            return parents;
        }

        private async Task<List<StudentSuccessTeamMember>> GetProgramAssociatedStaff(int studentUsi)
        {
            var otherStaff = await _studentRepository.GetProgramAssociatedStaff(studentUsi);

            // Add Image Urls
            foreach (var s in otherStaff)
            {
                s.ImageUrl = await _imageUrlProvider.GetStaffImageUrlAsync(s.UniqueId);
            }

            return otherStaff;
        }
        private async Task<List<StudentSuccessTeamMember>> GetSiblings(int studentUsi)
        {
            var siblings = await _studentRepository.GetSiblings(studentUsi);

            // Add Image Urls
            foreach (var s in siblings)
            {
                s.ImageUrl = await _imageUrlProvider.GetStudentImageUrlAsync(s.UniqueId);
            }

            return siblings;
        }
        private async Task<List<StudentSuccessTeamMember>> GetPrincipals(int studentUSI, string recipientUniqueId, int recipientTypeId)
        {
            var validLeadersDescriptors = _customParametersProvider.GetParameters().descriptors.validCampusLeaderDescriptorsForStudentSuccessTeam;
            var principals = await _studentRepository.GetPrincipals(studentUSI, validLeadersDescriptors, recipientUniqueId, recipientTypeId);

            foreach (var p in principals)
            {
                p.ImageUrl = await _imageUrlProvider.GetStaffImageUrlAsync(p.UniqueId);
            }

            return principals;
        }

    }
}
