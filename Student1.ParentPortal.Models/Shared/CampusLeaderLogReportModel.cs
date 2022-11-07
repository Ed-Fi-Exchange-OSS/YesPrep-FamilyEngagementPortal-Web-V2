using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student1.ParentPortal.Models.Shared
{
    public class CampusLeaderLogReportModel 
    {
        public LogReportTotalModel CampusLogReports { get; set; }
        public List<StaffLogReportModel> Staff { get; set; }
    }

    public class CampusLeaderParentReportModel
    {
        public CampusParentReportModel Campus { get; set; }
        public List<LanguageParentReportModel> Languages { get; set; }
    }
    public class StaffLogReportModel
    {
        public string StaffName { get; set; }
        public string StaffRole { get; set; }
        public string StaffClassification { get; set; }
        public string StaffPositionTitle { get; set; }
        public int LastWeekLogs { get; set; }
        public int LastMonthLogs { get; set; }
        public int YTD { get; set; }
    }

    public class CampusParentReportModel
    {
        public int EducationOrganizationId { get; set; }
        public string Campus { get; set; }
        public int EligibleParent { get; set; }
        public int EligibleParentLogin { get; set; }
        public int ProfilesCompleted { get; set; }
        public int NonEnglishLanguages { get; set; }
    }

    public class LanguageParentReportModel
    {
        public string ProfileLanguage { get; set; }
        public int ParentsLanguage { get; set; }
    }

}
