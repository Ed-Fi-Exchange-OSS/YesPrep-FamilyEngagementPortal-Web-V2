using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student1.ParentPortal.Models.Shared
{
    public class StaffMessageReportModel
    {
        public int EducationOrganizationId { get; set; }
        public string StaffName { get; set; }
        public string StaffRole { get; set; }
        public int MessagesSent { get; set; }
        public int MessagesReceived { get; set; }
        public int UnreadMessages { get; set; }
    }

    public class CampusMessageReportModel
    {
        public int EducationOrganizationId { get; set; }
        public string Campus { get; set; }
        public int MessagesSent { get; set; }
        public int MessagesReceived { get; set; }
        public int UnreadMessages { get; set; }
    }

   public class MessagesReportModel
    {
        public CampusMessageReportModel CampusReport { get; set; }
        public List<StaffMessageReportModel> StaffMessagesReports { get; set; }
    }

}
