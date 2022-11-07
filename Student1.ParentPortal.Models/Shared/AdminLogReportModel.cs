using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student1.ParentPortal.Models.Shared
{
    public class LogReportAccessModel
    {
        public int EducationOrganizationId { get; set; }
        public string Campus { get; set; }
        public int StaffClassificationDescriptorId { get; set; }
        public string PersonType { get; set; }
        public int TotalPerson { get; set; }
        public int TotalLogin { get; set; }
    }

    public class LogReportStaffModel
    {

        public string StaffName { get; set; }
        public string StaffRole { get; set; }
        public string StaffClasification { get; set; }
        public string LoggedIn { get; set; }
    }
    public class LogReportTotalModel
    {
        public int EducationOrganizationId { get; set; }
        public string Campus { get; set; }
        public int EligibleStaff { get; set; }
        public int EligibleStaffLogin { get; set; }
        public int Teacher { get; set; }
        public int TeacherLogin { get; set; }
        public int CampusLeader { get; set; }
        public int CampusLeaderLogin { get; set; }
    }

    public class LogReportTotalParentModel
    {
        public int EducationOrganizationId { get; set; }
        public string Campus { get; set; }
        public int EligibleParent { get; set; }
        public int EligibleParentLogin { get; set; }
        public int LastMonth { get; set; }
        public int LastWeek { get; set; }
        public int ProfilesCompleted { get; set; }
        public int NonEnglishLanguage { get; set; }
    }

    public class LogReportParentModel
    {

        public string ParentName { get; set; }
        public int LastMonth { get; set; }
        public int LastWeek { get; set; }
        public string ProfileCompleted { get; set; }
        public string EnglishLanguage { get; set; }
    }



}
