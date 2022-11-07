using Student1.ParentPortal.Models.Shared;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Student1.ParentPortal.Data.Models.EdFi31
{
    public class LogAccessRepository : ILogAccessRepository
    {
        private readonly EdFi31Context _edFiDb;
        //start of the school year
        private readonly DateTime _startOfSchoolYearDate;
        public LogAccessRepository(EdFi31Context edFiDb)
        {
            _edFiDb = edFiDb;
            //get the start of the school year for all YTD queries
            //--_startOfSchoolYearDate was changed to get last BeginDate 
            DateTime today = DateTime.Now.Date;
            _startOfSchoolYearDate = _edFiDb.Sessions.Where(x=>x.BeginDate<today).Select(x => x.BeginDate).Min();
            
        }

        public string[] StaffDescriptors { get; set; }
        public string[] CampusLeaderDescriptors { get; set; }
        public string[] ParentDescriptors { get; set; }
        public int LocalEducationAgencyId { get; set; }
        public void AddLogAccessEntry(LogAccessModel model)
        {
            _edFiDb.LogAccesses.Add(LogAccessModelToLogAccess(model));
            _edFiDb.SaveChanges();
        }


        public async Task AddLogAccessEntryAsync(LogAccessModel model)
        {
            _edFiDb.LogAccesses.Add(LogAccessModelToLogAccess(model));
            try
            {
                await _edFiDb.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<LogReportTotalModel> GetDistrictStaffLoginSummary()
        {
            //TODO:make a model and return these results for the district level login numbers

            var identity = await (from seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                                  join s in _edFiDb.Schools on seoaa.EducationOrganizationId equals s.SchoolId
                                  join d in _edFiDb.Descriptors on seoaa.StaffClassificationDescriptorId equals d.DescriptorId
                                  where seoaa.BeginDate < DateTime.Now
                                            && (seoaa.EndDate == null || seoaa.EndDate > DateTime.Now)
                                            && (CampusLeaderDescriptors.Contains(d.CodeValue) || StaffDescriptors.Contains(d.CodeValue))
                                            && s.LocalEducationAgencyId == LocalEducationAgencyId
                                  group seoaa by new
                                  {
                                      seoaa.EducationOrganization,
                                      seoaa.StaffClassificationDescriptor
                                  } into g
                                  select new LogReportAccessModel
                                  {
                                      EducationOrganizationId = g.Key.EducationOrganization.EducationOrganizationId,
                                      Campus = g.Key.EducationOrganization.NameOfInstitution,
                                      StaffClassificationDescriptorId = g.Key.StaffClassificationDescriptor.StaffClassificationDescriptorId,
                                      PersonType = g.Key.StaffClassificationDescriptor.Descriptor.CodeValue,
                                      TotalPerson = g.Count(),
                                      TotalLogin = 0
                                  }).ToListAsync();

            var access = await (from la in _edFiDb.LogAccesses
                                join seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations on la.USI equals seoaa.StaffUsi
                                join s in _edFiDb.Schools on seoaa.EducationOrganizationId equals s.SchoolId
                                where la.EventDate > _startOfSchoolYearDate
                                group seoaa by new
                                {
                                    seoaa.EducationOrganization,
                                    seoaa.StaffClassificationDescriptor,
                                    seoaa.StaffUsi
                                } into g
                                select new LogReportAccessModel
                                {
                                    EducationOrganizationId = g.Key.EducationOrganization.EducationOrganizationId,
                                    Campus = g.Key.EducationOrganization.NameOfInstitution,
                                    StaffClassificationDescriptorId = g.Key.StaffClassificationDescriptor.StaffClassificationDescriptorId,
                                    PersonType = g.Key.StaffClassificationDescriptor.Descriptor.CodeValue,
                                    TotalPerson = (from la in g select la.StaffUsi).Distinct().Count(),
                                    TotalLogin = 0
                                }).ToListAsync();

            var campus = identity.Select(m => m.EducationOrganizationId).Distinct().ToList();

            
                var teachers = identity.Where(m => StaffDescriptors.Contains(m.PersonType)).Sum(m => m.TotalPerson);
                var campusLeader = identity.Where(m => CampusLeaderDescriptors.Contains(m.PersonType)).Sum(m => m.TotalPerson);
                var teachersLogin = access.Where(m => StaffDescriptors.Contains(m.PersonType)).Sum(m => m.TotalPerson);
                var campusLeaderLogin = access.Where(m => CampusLeaderDescriptors.Contains(m.PersonType)).Sum(m => m.TotalPerson);

                LogReportTotalModel model = new LogReportTotalModel()
                {
                    EducationOrganizationId = LocalEducationAgencyId,
                    Campus = _edFiDb.EducationOrganizations.Where(m=> m.EducationOrganizationId== LocalEducationAgencyId).FirstOrDefault().NameOfInstitution,
                    Teacher = teachers,
                    CampusLeader = campusLeader,
                    EligibleStaff = teachers + campusLeader,
                    TeacherLogin = teachersLogin,
                    CampusLeaderLogin = campusLeaderLogin,
                    EligibleStaffLogin = teachersLogin + campusLeaderLogin
                };

            return model;
        }

        public async Task<Object> GetDistrictParentLoginSummary()
        {
            //TODO: implement and return results for the district wide parent login summary
            throw new NotImplementedException();
            var temp = await (from a in _edFiDb.Staffs select a).ToListAsync();
            return new object();
        }

        public async Task<List<LogReportTotalModel>> GetStaffLog()
        {
            //TODO - rewrite to include dates and local education agency id and change around staff classifications. //Done.
            try
            {
                var identity = await (from seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                                      join s in _edFiDb.Schools on seoaa.EducationOrganizationId equals s.SchoolId
                                      join d in _edFiDb.Descriptors on seoaa.StaffClassificationDescriptorId equals d.DescriptorId
                                      where seoaa.BeginDate < DateTime.Now
                                                && (seoaa.EndDate == null || seoaa.EndDate > DateTime.Now)
                                                && (CampusLeaderDescriptors.Contains(d.CodeValue) || StaffDescriptors.Contains(d.CodeValue))
                                                && s.LocalEducationAgencyId == LocalEducationAgencyId
                                      group seoaa by new
                                      {
                                          seoaa.EducationOrganization,
                                          seoaa.StaffClassificationDescriptor
                                      } into g
                                      select new LogReportAccessModel
                                      {
                                          EducationOrganizationId = g.Key.EducationOrganization.EducationOrganizationId,
                                          Campus = g.Key.EducationOrganization.NameOfInstitution,
                                          StaffClassificationDescriptorId = g.Key.StaffClassificationDescriptor.StaffClassificationDescriptorId,
                                          PersonType = g.Key.StaffClassificationDescriptor.Descriptor.CodeValue,
                                          TotalPerson = g.Count(),
                                          TotalLogin = 0
                                      }).ToListAsync();

                var access = await (from la in _edFiDb.LogAccesses
                                    join seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations on la.USI equals seoaa.StaffUsi
                                    join s in _edFiDb.Schools on seoaa.EducationOrganizationId equals s.SchoolId
                                    where la.EventDate > _startOfSchoolYearDate
                                    group seoaa by new
                                    {
                                        seoaa.EducationOrganization,
                                        seoaa.StaffClassificationDescriptor,
                                        seoaa.StaffUsi
                                    } into g
                                    select new LogReportAccessModel
                                    {
                                        EducationOrganizationId = g.Key.EducationOrganization.EducationOrganizationId,
                                        Campus = g.Key.EducationOrganization.NameOfInstitution,
                                        StaffClassificationDescriptorId = g.Key.StaffClassificationDescriptor.StaffClassificationDescriptorId,
                                        PersonType = g.Key.StaffClassificationDescriptor.Descriptor.CodeValue,
                                        TotalPerson = (from la in g select la.StaffUsi).Distinct().Count(),
                                        TotalLogin = 0
                                    }).ToListAsync();

                var campus = identity.Select(m => m.EducationOrganizationId).Distinct().ToList();

                List<LogReportTotalModel> model = new List<LogReportTotalModel>();

                foreach (int c in campus)
                {
                    var teachers = identity.Where(m => StaffDescriptors.Contains(m.PersonType) && m.EducationOrganizationId == c).Sum(m => m.TotalPerson);
                    var campusLeader = identity.Where(m => CampusLeaderDescriptors.Contains(m.PersonType) && m.EducationOrganizationId == c).Sum(m => m.TotalPerson);
                    var teachersLogin = access.Where(m => StaffDescriptors.Contains(m.PersonType) && m.EducationOrganizationId == c).Sum(m => m.TotalPerson);
                    var campusLeaderLogin = access.Where(m => CampusLeaderDescriptors.Contains(m.PersonType) && m.EducationOrganizationId == c).Sum(m => m.TotalPerson);

                    LogReportTotalModel row = new LogReportTotalModel()
                    {
                        EducationOrganizationId= c,
                        Campus = identity.Where(m => m.EducationOrganizationId == c).FirstOrDefault().Campus,
                        Teacher = teachers,
                        CampusLeader = campusLeader,
                        EligibleStaff = teachers + campusLeader,
                        TeacherLogin = teachersLogin,
                        CampusLeaderLogin = campusLeaderLogin,
                        EligibleStaffLogin = teachersLogin + campusLeaderLogin
                    };

                    model.Add(row);
                }
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<StaffLogReportModel>> GetStaffLogByDistrict()
        {
            //TODO: rewrite to add localeducation agency id and send id not name //Done

            List<StaffLogReportModel> model = new List<StaffLogReportModel>();
            var campus = await (from eo in _edFiDb.EducationOrganizations
                                join s in _edFiDb.Schools on eo.EducationOrganizationId equals s.SchoolId
                                where s.LocalEducationAgencyId == LocalEducationAgencyId
                                select s).ToListAsync();

            foreach (School c in campus)
            {
                var staff = await GetStaffLogByCampus(c.SchoolId);
                model.AddRange(staff);
            }

            return model;
        }

        public async Task<List<StaffLogReportModel>> GetStaffLogByCampus(int schoolId)
        {
            //TODO: use schoolId instead of NameOfInstitution and add date ranges //Date ranges done //schoolId done
            //can remove the next line after the parameter is changed to schoolId //Removed

            var lastWeek = DateTime.Today.AddDays(-7);
            var lastMonth = DateTime.Today.AddMonths(-1);
            var model = await (from seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                               where seoaa.EducationOrganizationId == schoolId
                               && (StaffDescriptors.Contains(seoaa.StaffClassificationDescriptor.Descriptor.CodeValue) || CampusLeaderDescriptors.Contains(seoaa.StaffClassificationDescriptor.Descriptor.CodeValue))
                               && seoaa.BeginDate <= DateTime.Now
                               && (seoaa.EndDate == null || seoaa.EndDate >= DateTime.Now)
                               select new StaffLogReportModel
                               {
                                   StaffName = seoaa.Staff.FirstName + " " + seoaa.Staff.LastSurname,
                                   StaffClassification = seoaa.StaffClassificationDescriptor.Descriptor.CodeValue,
                                   StaffRole = CampusLeaderDescriptors.Contains(seoaa.StaffClassificationDescriptor.Descriptor.CodeValue) ? "Campus leader" : "Teacher",
                                   LastWeekLogs = _edFiDb.LogAccesses.Where(m => m.USI == seoaa.StaffUsi && m.EventDate > lastWeek).Count(),
                                   LastMonthLogs = _edFiDb.LogAccesses.Where(m => m.USI == seoaa.StaffUsi && m.EventDate > lastMonth).Count(),
                                   YTD = _edFiDb.LogAccesses.Where(m => m.USI == seoaa.StaffUsi && m.EventDate > seoaa.BeginDate).Count()
                               }).ToListAsync();

            return model;
        }
        public async Task<List<LogReportTotalParentModel>> GetParentLog()
        {
            //TODO: rewrite to include localEducationAgencyId and date ranges //Done.
            try
            {
                var parentSchoolLogins = await (from ssa in _edFiDb.StudentSchoolAssociations
                                                join s in _edFiDb.Schools on ssa.SchoolId equals s.SchoolId
                                                join spa in _edFiDb.StudentParentAssociations on ssa.StudentUsi equals spa.StudentUsi
                                                join p in _edFiDb.Parents on spa.ParentUsi equals p.ParentUsi
                                                from la in _edFiDb.LogAccesses.Where(l => l.PersonType != 2 && l.USI == spa.ParentUsi).DefaultIfEmpty()
                                                where ssa.EntryDate < DateTime.Now
                                                        && (ssa.ExitWithdrawDate == null || ssa.ExitWithdrawDate > DateTime.Now)
                                                        && (s.LocalEducationAgencyId == LocalEducationAgencyId)
                                                        && (la == null || (la != null && la.EventDate > _startOfSchoolYearDate))
                                                select new
                                                {
                                                    Campus = s.EducationOrganization.NameOfInstitution,
                                                    s.SchoolId,
                                                    spa.ParentUsi,
                                                    p.ParentUniqueId,
                                                    LoginTimeDate = (DateTime?)la.EventDate,
                                                }).ToListAsync();

                var profiles = _edFiDb.ParentProfiles.ToList();
                var identityAlt = (from psl in parentSchoolLogins
                                   group psl by psl.Campus into g
                                   select new LogReportTotalParentModel
                                   {
                                       EducationOrganizationId=g.Select(x=> x.SchoolId).FirstOrDefault(),
                                       Campus = g.Key,
                                       EligibleParent = g.Select(x => x.ParentUsi).Distinct().Count(),
                                       EligibleParentLogin = g.Where(y => y.LoginTimeDate != null).Select(z => z.ParentUsi).Distinct().Count(),
                                       LastMonth = g.Where(y => y.LoginTimeDate != null && y.LoginTimeDate > DateTime.Today.AddMonths(-1)).Select(z => z.ParentUsi).Distinct().Count(),
                                       LastWeek = g.Where(y => y.LoginTimeDate != null && y.LoginTimeDate > DateTime.Today.AddDays(-7)).Select(z => z.ParentUsi).Distinct().Count(),
                                       ProfilesCompleted = profiles.Where(y => g.Select(x => x.ParentUniqueId).Contains(y.ParentUniqueId)).Select(x => x.ParentUniqueId).Distinct().Count(),
                                       NonEnglishLanguage = profiles.Where(y => y.LanguageCode != "en" && g.Select(x => x.ParentUniqueId).Contains(y.ParentUniqueId)).Select(x => x.ParentUniqueId).Distinct().Count()
                                   }).ToList();

                return identityAlt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<LogReportParentModel>> GetParentLogByCampus(int schoolId)
        {
            //TODO: Change parameter in to school id //Done
            //can remove next line after the parameter in is changed to school Id //Removed

            var lastWeek = DateTime.Today.AddDays(-7);
            var lastMonth = DateTime.Today.AddMonths(-1);
            var model = await (from ssa in _edFiDb.StudentSchoolAssociations
                               join spa in _edFiDb.StudentParentAssociations on ssa.StudentUsi equals spa.StudentUsi
                               where ssa.SchoolId == schoolId
                                    && ssa.EntryDate < DateTime.Now
                                    && (ssa.ExitWithdrawDate == null || ssa.ExitWithdrawDate > DateTime.Now)
                               group spa by new
                               {
                                   spa.Parent,
                                   ssa.SchoolId
                               } into g
                               select new LogReportParentModel
                               {
                                   ParentName = g.Key.Parent.FirstName + " " + g.Key.Parent.LastSurname,
                                   LastMonth = _edFiDb.LogAccesses.Where(m => m.USI == g.Key.Parent.ParentUsi && m.EventDate > lastMonth).Count(),
                                   LastWeek = _edFiDb.LogAccesses.Where(m => m.USI == g.Key.Parent.ParentUsi && m.EventDate > lastWeek).Count(),
                                   ProfileCompleted = _edFiDb.ParentProfiles.Where(m => m.ParentUniqueId == g.Key.Parent.ParentUniqueId && m.PreferredMethodOfContactTypeId > 0 && m.LanguageCode != null).Any() ? "Yes" : "No",
                                   EnglishLanguage = _edFiDb.ParentProfiles.Where(m => m.ParentUniqueId == g.Key.Parent.ParentUniqueId && m.LanguageCode == "en").Any() ? "Yes" : "No"
                               }).ToListAsync();

            return model;
        }

        public async Task<List<LogReportParentModel>> GetParentLogByDistrict()
        {
            //TODO: update to limit by educationAgencyid //This is already limited by LocalEducationAgencyId
            List<LogReportParentModel> model = new List<LogReportParentModel>();
            var campus = await (from s in _edFiDb.Schools
                                where s.LocalEducationAgencyId == LocalEducationAgencyId
                                select s.SchoolId).ToListAsync();

            foreach (var schoolId in campus)
            {
                var parents = await GetParentLogByCampus(schoolId);
                model.AddRange(parents);
            }

            return model;
        }

        private ParentPortal.Data.Models.EdFi31.LogAccess LogAccessModelToLogAccess(LogAccessModel model)
        {
            var access = new LogAccess
            {
                Id = model.Id,
                Email = model.Email,
                EventDate = DateTime.Now,
                USI = model.USI,
                UniqueId = model.UniqueId,
                PersonType = model.PersonType,
                Platform = model.Platform,
                PlatformInfo = model.PlatformInfo
            };
            return access;
        }

        public async Task<List<CampusLeaderLogReportModel>> GetCampusLeaderLog(int id)
        {
            //TODO - Does this pull info based on the person logged in? Cy to review with whoever wrote this method
            //TODO: add correct date ranges and limit to localEducationAgencyId

            var identity = await (from seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                                  join s in _edFiDb.Schools on seoaa.EducationOrganizationId equals s.SchoolId
                                  join sub in (from seoaas in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                                               where seoaas.StaffUsi == id
                                               select new
                                               {
                                                   eoId = seoaas.EducationOrganizationId
                                               }
                                    ) on seoaa.EducationOrganizationId equals sub.eoId
                                  group seoaa by new
                                  {
                                      seoaa.EducationOrganization,
                                      seoaa.StaffClassificationDescriptor
                                  } into g
                                  select new LogReportAccessModel
                                  {
                                      EducationOrganizationId = g.Key.EducationOrganization.EducationOrganizationId,
                                      Campus = g.Key.EducationOrganization.NameOfInstitution,
                                      StaffClassificationDescriptorId = g.Key.StaffClassificationDescriptor.StaffClassificationDescriptorId,
                                      PersonType = g.Key.StaffClassificationDescriptor.Descriptor.CodeValue,
                                      TotalPerson = g.Count(),
                                      TotalLogin = 0
                                  }).ToListAsync();

            var access = await (from la in _edFiDb.LogAccesses
                                join seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations on la.USI equals seoaa.StaffUsi
                                join s in _edFiDb.Schools on seoaa.EducationOrganizationId equals s.SchoolId
                                join sub in (from seoaas in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                                             where seoaas.StaffUsi == id
                                             select new
                                             {
                                                 eoId = seoaas.EducationOrganizationId
                                             }
                                    ) on seoaa.EducationOrganizationId equals sub.eoId
                                group seoaa by new
                                {
                                    seoaa.EducationOrganization,
                                    seoaa.StaffClassificationDescriptor,
                                    seoaa.StaffUsi
                                } into g
                                select new LogReportAccessModel
                                {
                                    EducationOrganizationId = g.Key.EducationOrganization.EducationOrganizationId,
                                    Campus = g.Key.EducationOrganization.NameOfInstitution,
                                    StaffClassificationDescriptorId = g.Key.StaffClassificationDescriptor.StaffClassificationDescriptorId,
                                    PersonType = g.Key.StaffClassificationDescriptor.Descriptor.CodeValue,
                                    TotalPerson = (from la in g select la.StaffUsi).Distinct().Count(),
                                    TotalLogin = 0
                                }).ToListAsync();


            var campus = identity.Select(m => m.EducationOrganizationId).Distinct().ToList();

            List<CampusLeaderLogReportModel> result = new List<CampusLeaderLogReportModel>();

            foreach (int c in campus)
            {
                CampusLeaderLogReportModel model = new CampusLeaderLogReportModel();

                var teachers = identity.Where(m => StaffDescriptors.Contains(m.PersonType) && m.EducationOrganizationId==c).Sum(m => m.TotalPerson);
                var campusLeader = identity.Where(m => CampusLeaderDescriptors.Contains(m.PersonType) && m.EducationOrganizationId == c).Sum(m => m.TotalPerson);
                var teachersLogin = access.Where(m => StaffDescriptors.Contains(m.PersonType) && m.EducationOrganizationId == c).Sum(m => m.TotalPerson);
                var campusLeaderLogin = access.Where(m => CampusLeaderDescriptors.Contains(m.PersonType) && m.EducationOrganizationId == c).Sum(m => m.TotalPerson);

                LogReportTotalModel campusLogReport = new LogReportTotalModel()
                {
                    Campus = identity.Where(m => m.EducationOrganizationId == c).FirstOrDefault().Campus,
                    Teacher = teachers,
                    CampusLeader = campusLeader,
                    EligibleStaff = teachers + campusLeader,
                    TeacherLogin = teachersLogin,
                    CampusLeaderLogin = campusLeaderLogin,
                    EligibleStaffLogin = teachersLogin + campusLeaderLogin
                };

                var lastWeek = DateTime.Today.AddDays(-7);
                var lastMonth = DateTime.Today.AddMonths(-1);

                var staff = await (from seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                                   where seoaa.EducationOrganization.EducationOrganizationId == c 
                                   && (StaffDescriptors.Contains(seoaa.StaffClassificationDescriptor.Descriptor.CodeValue) || CampusLeaderDescriptors.Contains(seoaa.StaffClassificationDescriptor.Descriptor.CodeValue))
                                   select new StaffLogReportModel
                                   {
                                       StaffName = seoaa.Staff.FirstName + " " + seoaa.Staff.LastSurname,
                                       StaffClassification = seoaa.StaffClassificationDescriptor.Descriptor.CodeValue,
                                       StaffRole = CampusLeaderDescriptors.Contains(seoaa.StaffClassificationDescriptor.Descriptor.CodeValue) ? "Campus leader" : "Teacher",
                                       LastWeekLogs = _edFiDb.LogAccesses.Where(m => m.USI == seoaa.StaffUsi && m.EventDate > lastWeek).Count(),
                                       LastMonthLogs = _edFiDb.LogAccesses.Where(m => m.USI == seoaa.StaffUsi && m.EventDate > lastMonth).Count(),
                                       YTD = _edFiDb.LogAccesses.Where(m => m.USI == seoaa.StaffUsi && m.EventDate > seoaa.BeginDate).Count()
                                   }).ToListAsync();


                model.CampusLogReports = campusLogReport;
                model.Staff = staff;

                result.Add(model);
            }


            return result;
        }

        public async Task<List<CampusLeaderParentReportModel>> GetCampusLeaderParentLog(int id)
        {
            //TODO: determine what this does- If this is pulling data based on the logged in user's staff id it could cause a problem if there are 2 campuses associated to the campus leader
            //TODO: add correct date ranges and limit to localEducationAgencyId
            var identity = await (from seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                                  join s in _edFiDb.Schools on seoaa.EducationOrganizationId equals s.SchoolId
                                  where seoaa.StaffUsi == id
                                  select new CampusParentReportModel
                                  {
                                      EducationOrganizationId= seoaa.EducationOrganizationId,
                                      Campus = seoaa.EducationOrganization.NameOfInstitution,
                                      EligibleParent = 0,
                                      EligibleParentLogin = 0,
                                      ProfilesCompleted = 0,
                                      NonEnglishLanguages = 0

                                  }).ToListAsync();

            var accessParents = await (from la in _edFiDb.LogAccesses
                                       join spa in _edFiDb.StudentParentAssociations on la.USI equals spa.ParentUsi
                                       join seoa in _edFiDb.StudentSchoolAssociations on spa.StudentUsi equals seoa.StudentUsi
                                       //join s in _edFiDb.Schools on seoa.EducationOrganizationId equals s.SchoolId
                                       where la.EventDate > spa.Student.StudentSchoolAssociations.OrderByDescending(m => m.EntryDate).FirstOrDefault().EntryDate
                                       select new
                                       {
                                           la,
                                           seoa
                                       }).ToListAsync();

            List<CampusLeaderParentReportModel> result = new List<CampusLeaderParentReportModel>();

            foreach (CampusParentReportModel i in identity)
            {
                var eligibleParentLogin = accessParents.Where(m => m.seoa.SchoolId == i.EducationOrganizationId).Select(m => m.la.USI).Distinct().Count();
                var eligibleParent = (from spa in _edFiDb.StudentParentAssociations
                                      join seoa in _edFiDb.StudentSchoolAssociations on spa.StudentUsi equals seoa.StudentUsi
                                      where seoa.SchoolId == i.EducationOrganizationId
                                      select new
                                      {
                                          spa.ParentUsi
                                      }).Distinct().Count();
                var profilesCompleted = (from spa in _edFiDb.StudentParentAssociations
                                         join seoa in _edFiDb.StudentSchoolAssociations on spa.StudentUsi equals seoa.StudentUsi
                                         join pf in _edFiDb.ParentProfiles on spa.Parent.ParentUniqueId equals pf.ParentUniqueId
                                         where seoa.SchoolId == i.EducationOrganizationId && pf.PreferredMethodOfContactTypeId > 0 && pf.LanguageCode != null
                                         select new
                                         {
                                             spa.ParentUsi
                                         }).Distinct().Count();

                var nonEnglishLanguage = (from spa in _edFiDb.StudentParentAssociations
                                          join seoa in _edFiDb.StudentSchoolAssociations on spa.StudentUsi equals seoa.StudentUsi
                                          join pf in _edFiDb.ParentProfiles on spa.Parent.ParentUniqueId equals pf.ParentUniqueId
                                          where seoa.SchoolId == i.EducationOrganizationId && pf.LanguageCode != "en"
                                          select new
                                          {
                                              spa.ParentUsi
                                          }).Distinct().Count();

                i.EligibleParentLogin = eligibleParentLogin;
                i.EligibleParent = eligibleParent;
                i.ProfilesCompleted = profilesCompleted;
                i.NonEnglishLanguages = nonEnglishLanguage;

                CampusLeaderParentReportModel row = new CampusLeaderParentReportModel();
                row.Campus = i;


                var languages = await (from spa in _edFiDb.StudentParentAssociations
                                       join seoa in _edFiDb.StudentSchoolAssociations on spa.StudentUsi equals seoa.StudentUsi
                                       join pf in _edFiDb.ParentProfiles on spa.Parent.ParentUniqueId equals pf.ParentUniqueId
                                       where seoa.SchoolId == i.EducationOrganizationId
                                       group pf by new
                                       {
                                           pf
                                       } into g
                                       select new LanguageParentReportModel
                                       {
                                           ParentsLanguage = (from pf in g select pf.ParentUniqueId).Distinct().Count(),
                                           ProfileLanguage = g.Key.pf.LanguageCode
                                       }).ToListAsync();

                row.Languages = languages;
                result.Add(row);
            }

            return result;
        }

        public async Task<List<StaffLogReportModel>> GetTeacherLog(int usi)
        {
            //TODO: update for date range on the query for start of school year //Done
            var lastWeek = DateTime.Today.AddDays(-7);
            var lastMonth = DateTime.Today.AddMonths(-1);
            var model = await (from seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                               where seoaa.StaffUsi == usi && seoaa.BeginDate>=_startOfSchoolYearDate
                               select new StaffLogReportModel
                               {
                                   StaffName = seoaa.Staff.FirstName + " " + seoaa.Staff.LastSurname,
                                   StaffClassification = seoaa.StaffClassificationDescriptor.Descriptor.CodeValue,
                                   StaffRole = "Teacher",
                                   LastWeekLogs = _edFiDb.LogAccesses.Where(m => m.USI == seoaa.StaffUsi && m.EventDate > lastWeek).Count(),
                                   LastMonthLogs = _edFiDb.LogAccesses.Where(m => m.USI == seoaa.StaffUsi && m.EventDate > lastMonth).Count(),
                                   YTD = _edFiDb.LogAccesses.Where(m => m.USI == seoaa.StaffUsi && m.EventDate > seoaa.BeginDate).Count()
                               }).ToListAsync();

            return model;
        }


        #region Messages

        public async Task<List<CampusMessageReportModel>> GetStaffMessage()
        {
            //TODO: update for localeducationagencyId and date ranges //Done. Date ranges added in GetStaffMessageByCampus
            List<CampusMessageReportModel> identity = new List<CampusMessageReportModel>();

            var campus = await (from eo in _edFiDb.EducationOrganizations
                                join s in _edFiDb.Schools on eo.EducationOrganizationId equals s.SchoolId
                                where s.LocalEducationAgencyId == LocalEducationAgencyId
                                select s).ToListAsync();

            foreach (School c in campus)
            {
                CampusMessageReportModel row = new CampusMessageReportModel();
                var staff = await GetStaffMessageByCampus(c.SchoolId);
                row.EducationOrganizationId = c.SchoolId;
                row.Campus = c.EducationOrganization.NameOfInstitution;
                row.MessagesReceived = staff.Sum(m => m.MessagesReceived);
                row.MessagesSent = staff.Sum(m => m.MessagesSent);
                row.UnreadMessages = staff.Sum(m => m.UnreadMessages);

                identity.Add(row);
            }

            return identity;

        }

        public async Task<List<StaffMessageReportModel>> GetStaffMessageByCampus(int schoolId)
        {
            //TODO: update to limit to localEducationAGencyId and change the parameter to schoolId 
            //Done. localEducationAGencyId unnecessary. schoolId limits the query
            var lastWeek = DateTime.Today.AddDays(-7);
            var lastMonth = DateTime.Today.AddMonths(-1);
            var model = await (from seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                               where seoaa.EducationOrganizationId == schoolId 
                               && (StaffDescriptors.Contains(seoaa.StaffClassificationDescriptor.Descriptor.CodeValue) || CampusLeaderDescriptors.Contains(seoaa.StaffClassificationDescriptor.Descriptor.CodeValue))
                               && seoaa.BeginDate <= DateTime.Now
                               && (seoaa.EndDate == null || seoaa.EndDate >= DateTime.Now)
                               select new StaffMessageReportModel
                               {
                                   EducationOrganizationId= schoolId,
                                   StaffName = seoaa.Staff.FirstName + " " + seoaa.Staff.LastSurname,
                                   StaffRole = CampusLeaderDescriptors.Contains(seoaa.StaffClassificationDescriptor.Descriptor.CodeValue) ? "Campus leader" : "Teacher",
                                   MessagesReceived = _edFiDb.ChatLogs.Where(m => m.RecipientUniqueId == seoaa.Staff.StaffUniqueId && m.DateSent > seoaa.BeginDate).Count(),
                                   MessagesSent = _edFiDb.ChatLogs.Where(m => m.SenderUniqueId == seoaa.Staff.StaffUniqueId && m.DateSent > seoaa.BeginDate).Count(),
                                   UnreadMessages = _edFiDb.ChatLogs.Where(m => m.RecipientUniqueId == seoaa.Staff.StaffUniqueId && m.DateSent > seoaa.BeginDate && !m.RecipientHasRead).Count(),
                               }).ToListAsync();
            model = model.Where(m => m.MessagesReceived > 0 || m.MessagesSent > 0 || m.UnreadMessages > 0).ToList();
            return model;
        }

        public async Task<List<StaffMessageReportModel>> GetStaffMessageByDistrict()
        {
            //TODO: update to limit by localEducationAgencyId and correct date ranges
            //Done. Date ranges added in GetStaffMessageByCampus
            List<StaffMessageReportModel> model = new List<StaffMessageReportModel>();
            var campus = await (from eo in _edFiDb.EducationOrganizations
                                join s in _edFiDb.Schools on eo.EducationOrganizationId equals s.SchoolId
                                where s.LocalEducationAgencyId == LocalEducationAgencyId
                                select s).ToListAsync();

            foreach (School c in campus)
            {
                var staff = await GetStaffMessageByCampus(c.SchoolId);
                model.AddRange(staff);
            }

            return model;

        }

        public async Task<List<MessagesReportModel>> GetStaffMessage(int staffUsi)
        {
            //TODO: update for correct date ranges
           // Date ranges added in GetStaffMessageByCampus
            List<MessagesReportModel> result = new List<MessagesReportModel>();

            var campus = await (from eo in _edFiDb.EducationOrganizations
                                join seo in _edFiDb.StaffEducationOrganizationAssignmentAssociations on eo.EducationOrganizationId equals seo.EducationOrganizationId
                                where seo.StaffUsi == staffUsi
                                select eo).ToListAsync();

            foreach (EducationOrganization c in campus)
            {
                MessagesReportModel model = new MessagesReportModel();
                CampusMessageReportModel row = new CampusMessageReportModel();
                var staff = await GetStaffMessageByCampus(c.EducationOrganizationId);

                row.EducationOrganizationId = c.EducationOrganizationId;
                row.Campus = c.NameOfInstitution;
                row.MessagesReceived = staff.Sum(m => m.MessagesReceived);
                row.MessagesSent = staff.Sum(m => m.MessagesSent);
                row.UnreadMessages = staff.Sum(m => m.UnreadMessages);

                model.CampusReport = row;
                model.StaffMessagesReports = staff;
                result.Add(model);
            }

            return result;
        }

        public async Task<List<CampusMessageReportModel>> GetParentMessage()
        {
            //TODO: update to limit to localEducationAgencyId and correct date ranges //Done
            /*List<CampusMessageReportModel> identity = new List<CampusMessageReportModel>();
            List<StaffMessageReportModel> model = new List<StaffMessageReportModel>();
           
            var parentSchoolLogins = await (from ssa in _edFiDb.StudentSchoolAssociations
                                            join s in _edFiDb.Schools on ssa.SchoolId equals s.SchoolId
                                            join spa in _edFiDb.StudentParentAssociations on ssa.StudentUsi equals spa.StudentUsi
                                            join p in _edFiDb.Parents on spa.ParentUsi equals p.ParentUsi
                                            //from la in _edFiDb.LogAccesses.Where(l => l.PersonType != 2 && l.USI == spa.ParentUsi).DefaultIfEmpty()
                                            where ssa.EntryDate < DateTime.Now
                                                    && (ssa.ExitWithdrawDate == null || ssa.ExitWithdrawDate > DateTime.Now)
                                                    && (s.LocalEducationAgencyId == LocalEducationAgencyId)
                                                   // && (la == null || (la != null && la.EventDate > _startOfSchoolYearDate))
                                            select new
                                            {
                                                Campus = s.EducationOrganization.NameOfInstitution,
                                                s.SchoolId,
                                                spa.ParentUsi,
                                                p.ParentUniqueId,
                                                MessagesReceived = _edFiDb.ChatLogs.Where(m => m.RecipientUniqueId == p.ParentUniqueId && m.DateSent >= _startOfSchoolYearDate).Count(),
                                                MessagesSent = _edFiDb.ChatLogs.Where(m => m.SenderUniqueId == p.ParentUniqueId && m.DateSent >= _startOfSchoolYearDate).Count(),
                                                UnreadMessages = _edFiDb.ChatLogs.Where(m => m.RecipientUniqueId == p.ParentUniqueId && !m.RecipientHasRead && m.DateSent >= _startOfSchoolYearDate).Count(),

                                            }).ToListAsync();

            //var profiles = _edFiDb.ParentProfiles.ToList();
            var identityAlt = (from psl in parentSchoolLogins
                               group psl by psl.Campus into g
                               select new CampusMessageReportModel
                               {
                                   EducationOrganizationId = g.Select(x => x.SchoolId).FirstOrDefault(),
                                   Campus = g.Key,
                                   MessagesReceived=g.Select(x=> x.MessagesReceived).Sum(),
                                   MessagesSent = g.Select(x => x.MessagesSent).Sum(),
                                   UnreadMessages = g.Select(x => x.UnreadMessages).Sum(),
                               }).ToList();

            return identityAlt;*/

            List<CampusMessageReportModel> identity = new List<CampusMessageReportModel>();

            var campus = await (from eo in _edFiDb.EducationOrganizations
                                join s in _edFiDb.Schools on eo.EducationOrganizationId equals s.SchoolId
                                where s.LocalEducationAgencyId == LocalEducationAgencyId
                                select s).ToListAsync();

            foreach (School c in campus)
            {
                CampusMessageReportModel row = new CampusMessageReportModel();
                var staff = await GetParentMessageByCampus(c.SchoolId);
                row.EducationOrganizationId = c.SchoolId;
                row.Campus = c.EducationOrganization.NameOfInstitution;
                row.MessagesReceived = staff.Sum(m => m.MessagesReceived);
                row.MessagesSent = staff.Sum(m => m.MessagesSent);
                row.UnreadMessages = staff.Sum(m => m.UnreadMessages);

                identity.Add(row);
            }

            return identity;

        }

        public async Task<List<StaffMessageReportModel>> GetParentMessageByCampus(int schoolId)
        {
            //TODO: update to limit to correct date ranges and change the parameter into the schoolId //Done

            var model = await (from ssa in _edFiDb.StudentSchoolAssociations
                               join s in _edFiDb.Schools on ssa.SchoolId equals s.SchoolId
                               join spa in _edFiDb.StudentParentAssociations on ssa.StudentUsi equals spa.StudentUsi
                               join p in _edFiDb.Parents on spa.ParentUsi equals p.ParentUsi
                               where ssa.SchoolId==schoolId
                               group spa by new
                               {
                                   spa.Parent,
                                   ssa.SchoolId
                               } into g
                               select new StaffMessageReportModel
                               {
                                   EducationOrganizationId = g.Key.SchoolId,
                                   StaffName = g.Key.Parent.FirstName + " " + g.Key.Parent.LastSurname,
                                   StaffRole = "Parent",
                                   MessagesReceived = _edFiDb.ChatLogs.Where(m => m.RecipientUniqueId == g.Key.Parent.ParentUniqueId && m.DateSent >= _startOfSchoolYearDate).Count(),
                                   MessagesSent = _edFiDb.ChatLogs.Where(m => m.SenderUniqueId == g.Key.Parent.ParentUniqueId && m.DateSent >= _startOfSchoolYearDate).Count(),
                                   UnreadMessages = _edFiDb.ChatLogs.Where(m => m.RecipientUniqueId == g.Key.Parent.ParentUniqueId && !m.RecipientHasRead && m.DateSent >= _startOfSchoolYearDate).Count(),
                               }).ToListAsync();
            //To remove zeros
            model = model.Where(m => m.MessagesReceived > 0 || m.MessagesSent > 0 || m.UnreadMessages > 0).ToList();
            return model;
        }

        public async Task<List<StaffMessageReportModel>> GetParentMessageByDistrict()
        {
            //TODO: update to limit to localEducationAGencyId and correct date ranges
            //Done date ranges added in GetParentMessageByCampus
            List<StaffMessageReportModel> model = new List<StaffMessageReportModel>();
            var campus = await (from eo in _edFiDb.EducationOrganizations
                                join s in _edFiDb.Schools on eo.EducationOrganizationId equals s.SchoolId
                                where s.LocalEducationAgencyId == LocalEducationAgencyId
                                select s).ToListAsync();

            foreach (School c in campus)
            {
                var staff = await GetParentMessageByCampus(c.SchoolId);
                model.AddRange(staff);
            }

            return model;

        }
        public async Task<List<MessagesReportModel>> GetParentMessageByCampusId(int staffUSI)
        {
            //update to schoolId parameter (probably) and correct date ranges //Done
            List<MessagesReportModel> result = new List<MessagesReportModel>();

            var campus = await (from eo in _edFiDb.EducationOrganizations
                                join seo in _edFiDb.StaffEducationOrganizationAssignmentAssociations on eo.EducationOrganizationId equals seo.EducationOrganizationId
                                where seo.StaffUsi == staffUSI
                                select eo).ToListAsync();

            foreach (EducationOrganization c in campus)
            {
                MessagesReportModel model = new MessagesReportModel();
                CampusMessageReportModel row = new CampusMessageReportModel();
                var parent = await GetParentMessageByCampus(c.EducationOrganizationId);

                row.EducationOrganizationId = c.EducationOrganizationId;
                row.Campus = c.NameOfInstitution;
                row.MessagesReceived = parent.Sum(m => m.MessagesReceived);
                row.MessagesSent = parent.Sum(m => m.MessagesSent);
                row.UnreadMessages = parent.Sum(m => m.UnreadMessages);

                model.CampusReport = row;
                model.StaffMessagesReports = parent;
                result.Add(model);
            }

            return result;

        }

        public async Task<List<StaffMessageReportModel>> GetTeacherMessages(int usi)
        {
            //TODO: update for correct date ranges //Done
            var lastWeek = DateTime.Today.AddDays(-7);
            var lastMonth = DateTime.Today.AddMonths(-1);
            var model = await (from seoaa in _edFiDb.StaffEducationOrganizationAssignmentAssociations
                               where seoaa.StaffUsi == usi
                               select new StaffMessageReportModel
                               {
                                   StaffName = seoaa.Staff.FirstName + " " + seoaa.Staff.LastSurname,
                                   StaffRole = "Teacher",
                                   MessagesReceived = _edFiDb.ChatLogs.Where(m => m.RecipientUniqueId == seoaa.Staff.StaffUniqueId && m.DateSent >= _startOfSchoolYearDate).Count(),
                                   MessagesSent = _edFiDb.ChatLogs.Where(m => m.SenderUniqueId == seoaa.Staff.StaffUniqueId && m.DateSent >= _startOfSchoolYearDate).Count(),
                                   UnreadMessages = _edFiDb.ChatLogs.Where(m => m.RecipientUniqueId == seoaa.Staff.StaffUniqueId && m.DateSent >= _startOfSchoolYearDate && !m.RecipientHasRead).Count(),
                               }).ToListAsync();

            return model;
        }
        #endregion

    }

}