using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Student1.ParentPortal.Models.Schools;
using Student1.ParentPortal.Models.Shared;

namespace Student1.ParentPortal.Data.Models.EdFi31
{
    public class SchoolsRepository : ISchoolsRepository
    {
        private readonly EdFi31Context _edFiDb;

        public SchoolsRepository(EdFi31Context edFiDb) 
        {
            _edFiDb = edFiDb;
        }
        public async Task<List<GradeModel>> GetGradeLevelsBySchoolId(int schoolId)
        {
            var data = await (from ssa in _edFiDb.StudentSchoolAssociations
                              join d in _edFiDb.Descriptors on ssa.EntryGradeLevelDescriptorId equals d.DescriptorId
                              where ssa.SchoolId == schoolId
                              group new { ssa, d } by new { ssa.EntryGradeLevelDescriptorId, d.CodeValue } into g
                              select new GradeModel
                              {
                                  Id = g.FirstOrDefault().ssa.EntryGradeLevelDescriptorId,
                                  Name = g.FirstOrDefault().d.CodeValue
                              }).ToListAsync();
            return data;
        }

        public async Task<List<ProgramsModel>> GetDistrictAndSchoolProgramsBySchoolId(int schoolId)
        {
            var EdOrgId = (from s in _edFiDb.School1
                           where s.SchoolId == schoolId
                           select s.LocalEducationAgencyId).FirstOrDefault();

            var data = await(from p in _edFiDb.GeneralStudentProgramAssociations
                             join d in _edFiDb.Descriptors on p.ProgramTypeDescriptorId equals d.DescriptorId
                             where p.EducationOrganizationId == EdOrgId || p.EducationOrganizationId == schoolId
                             select new ProgramsModel
                             {
                                 Id = p.ProgramTypeDescriptorId,
                                 Name = d.CodeValue
                             }).Distinct().ToListAsync();
            return data;
        }

        public async Task<List<SchoolBriefDetailModel>> GetSchoolsByPrincipal(int staffUsi)
        {
            var data = await (from s in _edFiDb.Schools
                                           .Include(x => x.EducationOrganization)
                              join sf in _edFiDb.StaffEducationOrganizationAssignmentAssociations on s.SchoolId equals sf.EducationOrganizationId
                              where sf.StaffUsi == staffUsi
                              select new SchoolBriefDetailModel
                              {
                                  SchoolId = s.EducationOrganization.EducationOrganizationId,
                                  NameOfInstitution = s.EducationOrganization.NameOfInstitution
                              }).OrderBy(x => x.NameOfInstitution).ToListAsync();
            return data;
        }

        public async Task<List<MainSessionsDates>> GetSessionMetadata()
        {
            var sessionsDates = await(from ses in _edFiDb.Sessions
                                      join des in _edFiDb.Descriptors on ses.TermDescriptorId equals des.DescriptorId
                                      where ses.SessionName.ToLower().Contains("fall") ||
                                            ses.SessionName.ToLower().Contains("spring")
                                      group new { ses,des } by new { ses.SessionName } into g
                                      select new MainSessionsDates
                                      {
                                          BeginDate = g.Min(x => x.ses.BeginDate),
                                          EndDate = g.Max(x => x.ses.EndDate),
                                          Name = g.FirstOrDefault().des.CodeValue
                                      }
                                       ).ToListAsync();
            return sessionsDates;
        }
    }
}
