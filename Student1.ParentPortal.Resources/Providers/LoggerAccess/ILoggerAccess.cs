using Student1.ParentPortal.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student1.ParentPortal.Resources.Providers.LoggerAccess
{
    public interface ILogAccess
    {
        Task LogSaveAsync(LogAccessModel model);
        void LogSave(LogAccessModel model);
        Task<LogReportTotalModel> GetDistrictStaffLoginSummary();
        Task<List<LogReportTotalModel>> GetStaffLog();
        Task<List<StaffLogReportModel>> GetStaffLogByCampus(int schoolId);
        Task<List<LogReportTotalParentModel>> GetParentLog();
        Task<List<CampusLeaderLogReportModel>> GetCampusLeaderLog(int id);
        Task<List<CampusLeaderParentReportModel>> GetCampusLeaderParentLog(int id);
        Task<List<StaffLogReportModel>> GetStaffLogByDistrict();
        Task<List<CampusMessageReportModel>> GetStaffMessage();
        Task<List<MessagesReportModel>> GetStaffMessage(int staffUsi);
        Task<List<StaffMessageReportModel>> GetStaffMessageByCampus(int schoolId);
        Task<List<StaffMessageReportModel>> GetStaffMessageByDistrict();
        Task<List<MessagesReportModel>> GetParentMessageByCampusId(int staffUSI);
        Task<List<CampusMessageReportModel>> GetParentMessage();
        Task<List<StaffMessageReportModel>> GetParentMessageByCampus(int schoolId);
        Task<List<StaffMessageReportModel>> GetParentMessageByDistrict();
        Task<List<LogReportParentModel>> GetParentLogByCampus(int schoolId);
        Task<List<LogReportParentModel>> GetParentLogByDistrict();
        Task<List<StaffMessageReportModel>> GetTeacherMessages(int usi);
        Task<List<StaffLogReportModel>> GetTeacherLog(int usi);
    }
}
