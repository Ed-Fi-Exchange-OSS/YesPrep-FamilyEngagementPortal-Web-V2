using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Student1.ParentPortal.Resources.Providers.Translation;
using Student1.ParentPortal.Resources.Providers.Configuration;

namespace Student1.ParentPortal.Resources.Providers.LoggerAccess
{
    public class DatabaseLoggerAccess : ILogAccess
    {
        private readonly ILogAccessRepository _logRepository; 
        private readonly ITranslationProvider _translationProvider;
        private readonly ICustomParametersProvider _customParametersProvider;

        public DatabaseLoggerAccess(ILogAccessRepository logRepository, ITranslationProvider translationProvider, ICustomParametersProvider customParametersProvider, IApplicationSettingsProvider applicationSettingsProvider)
        {
            _logRepository = logRepository;
            _translationProvider = translationProvider;
            _customParametersProvider = customParametersProvider;
            _logRepository.CampusLeaderDescriptors= _customParametersProvider.GetParameters().descriptors.validCampusLeaderDescriptors;
            _logRepository.StaffDescriptors = _customParametersProvider.GetParameters().descriptors.validStaffDescriptors;
            _logRepository.ParentDescriptors = _customParametersProvider.GetParameters().descriptors.validParentDescriptors;
            _logRepository.LocalEducationAgencyId = int.Parse(applicationSettingsProvider.GetSetting("application.reports.admin.localEducationAgencyId"));
        }


        public async Task LogSaveAsync(LogAccessModel model)
        {
            await _logRepository.AddLogAccessEntryAsync(model);
        }


        public void LogSave(LogAccessModel model)
        {
            _logRepository.AddLogAccessEntry(model);
        }

        public async Task<LogReportTotalModel> GetDistrictStaffLoginSummary()
        {
            return await _logRepository.GetDistrictStaffLoginSummary();
        }

        public async Task<List<LogReportTotalModel>> GetStaffLog()
        {
            return await _logRepository.GetStaffLog();
        }

        public async Task<List<StaffLogReportModel>> GetStaffLogByCampus(int schoolId)
        {
            return await _logRepository.GetStaffLogByCampus(schoolId);
        }

        public async Task<List<StaffLogReportModel>> GetStaffLogByDistrict()
        {
            return await _logRepository.GetStaffLogByDistrict();
        }
        

        public async Task<List<LogReportTotalParentModel>> GetParentLog()
        {
            return await _logRepository.GetParentLog();
        }

        public async Task<List<LogReportParentModel>> GetParentLogByCampus(int schoolId)
        {
            return await _logRepository.GetParentLogByCampus(schoolId);
        }

        public async Task<List<LogReportParentModel>> GetParentLogByDistrict()
        {
            return await _logRepository.GetParentLogByDistrict();
        }        
        

        public async Task<List<CampusLeaderLogReportModel>> GetCampusLeaderLog(int id)
        {
            return await _logRepository.GetCampusLeaderLog(id);

        }

        public async Task<List<CampusLeaderParentReportModel>> GetCampusLeaderParentLog(int id)
        {
            var languageAvailable = await _translationProvider.GetAvailableLanguagesAsync();
            var model= await _logRepository.GetCampusLeaderParentLog(id);
            foreach (CampusLeaderParentReportModel m in model) {
                foreach (LanguageParentReportModel l in m.Languages) {
                    l.ProfileLanguage = languageAvailable.Where(x=> x.Code==l.ProfileLanguage).FirstOrDefault().Name;
                }
            }
            return model;
        }

        public async Task<List<CampusMessageReportModel>> GetStaffMessage()
        {
            return await _logRepository.GetStaffMessage();
        }

        public async Task<List<StaffMessageReportModel>> GetStaffMessageByCampus(int schoolId)
        {
            return await _logRepository.GetStaffMessageByCampus(schoolId);
        }

        public async Task<List<StaffMessageReportModel>> GetStaffMessageByDistrict()
        {
            return await _logRepository.GetStaffMessageByDistrict();
        }

        public async Task<List<CampusMessageReportModel>> GetParentMessage()
        {
            return await _logRepository.GetParentMessage();
        }

        public async Task<List<StaffMessageReportModel>> GetParentMessageByCampus(int schoolId)
        {
            return await _logRepository.GetParentMessageByCampus(schoolId);
        }

        public async Task<List<StaffMessageReportModel>> GetParentMessageByDistrict()
        {
            return await _logRepository.GetParentMessageByDistrict();
        }

        public async Task<List<MessagesReportModel>> GetStaffMessage(int staffUsi)
        {
            return await _logRepository.GetStaffMessage(staffUsi);
        }
        public async Task<List<MessagesReportModel>> GetParentMessageByCampusId(int staffUSI)
        {
            return await _logRepository.GetParentMessageByCampusId(staffUSI);
        }

        public async Task<List<StaffMessageReportModel>> GetTeacherMessages(int staffUsi)
        {
            return await _logRepository.GetTeacherMessages(staffUsi);
        }

        public async Task<List<StaffLogReportModel>> GetTeacherLog(int staffUsi)
        {
            return await _logRepository.GetTeacherLog(staffUsi);
        }
    }
}
