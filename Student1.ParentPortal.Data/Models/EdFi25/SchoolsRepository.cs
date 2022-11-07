using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Student1.ParentPortal.Models.Schools;
using Student1.ParentPortal.Models.Shared;

namespace Student1.ParentPortal.Data.Models.EdFi25
{
    public class SchoolsRepository : ISchoolsRepository
    {
        private readonly EdFi25Context _edFiDb;
        public SchoolsRepository(EdFi25Context edFiDb)
        {
            _edFiDb = edFiDb;
        }
        public async Task<List<GradeModel>> GetGradeLevelsBySchoolId(int schoolId)
        {
            var data = await (from gld in _edFiDb.GradeLevelDescriptors
                              join d in _edFiDb.Descriptors on gld.GradeLevelDescriptorId equals d.DescriptorId
                              join s in _edFiDb.SchoolGradeLevels on gld.GradeLevelDescriptorId equals s.GradeLevelDescriptorId
                              where s.SchoolId == schoolId
                              select new GradeModel
                              {
                                  Id = gld.GradeLevelDescriptorId,
                                  Name = d.CodeValue
                              }).ToListAsync();
            return data;
        }

        public Task<List<ProgramsModel>> GetDistrictAndSchoolProgramsBySchoolId(int staffUsi)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SchoolBriefDetailModel>> GetSchoolsByPrincipal(int StaffUsi)
        {
            var data = await (from s in _edFiDb.Schools
                                           .Include(x => x.EducationOrganization)
                              join sf in _edFiDb.StaffEducationOrganizationAssignmentAssociations on s.SchoolId equals sf.EducationOrganizationId
                              where sf.StaffUsi == StaffUsi
                              select new SchoolBriefDetailModel
                              {
                                  SchoolId = s.EducationOrganization.EducationOrganizationId,
                                  NameOfInstitution = s.EducationOrganization.NameOfInstitution
                              }).ToListAsync();
            return data;
        }
        public async Task<List<MainSessionsDates>> GetSessionMetadata()
        {
            var sessionsDates = await (from ses in _edFiDb.Sessions
                                       where ses.SessionName.ToLower().Contains("fall") ||
                                             ses.SessionName.ToLower().Contains("spring")
                                       group new { ses } by new { ses.SessionName } into g
                                       select new MainSessionsDates
                                       {
                                           BeginDate = g.Min(x => x.ses.BeginDate),
                                           EndDate = g.Max(x => x.ses.EndDate),
                                           Name = g.Key.SessionName
                                       }
                                       ).ToListAsync();
            return sessionsDates;
        }
    }
}
