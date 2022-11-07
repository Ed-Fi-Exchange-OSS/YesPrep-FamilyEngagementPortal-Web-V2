
using Student1.ParentPortal.Models.Staff;
using Student1.ParentPortal.Models.Student;
using Student1.ParentPortal.Models.Types;
using Student1.ParentPortal.Models.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using Student1.ParentPortal.Models.Alert;

namespace Student1.ParentPortal.Data.Models
{
    public interface ILogAccessRepository
    {
        Task AddLogAccessEntryAsync(LogAccessModel model);
        void AddLogAccessEntry(LogAccessModel model);
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
        Task<List<CampusMessageReportModel>> GetParentMessage();
        Task<List<StaffMessageReportModel>> GetParentMessageByCampus(int schoolId);
        Task<List<StaffMessageReportModel>> GetParentMessageByDistrict();
        Task<List<MessagesReportModel>> GetParentMessageByCampusId(int staffUSI);
        Task<List<LogReportParentModel>> GetParentLogByCampus(int schoolId);
        Task<List<LogReportParentModel>> GetParentLogByDistrict();
        Task<List<StaffMessageReportModel>> GetTeacherMessages(int usi);
        Task<List<StaffLogReportModel>> GetTeacherLog(int usi);

        string[] StaffDescriptors { get; set; }
        string[] CampusLeaderDescriptors { get; set; }
        string[] ParentDescriptors { get; set; }
        int LocalEducationAgencyId { get; set; }
    }
}
