using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student1.ParentPortal.Models.Shared
{
    public class GroupMessagesFilterParamsModel
    {
        public int[] gradeLevels { get; set; }
        public int[] programs { get; set; }
    }
    public class GroupMessagesTeachersFilterParamsModel
    {
        public int StaffUsi { get; set; }
        public int SchoolId { get; set; }
        public string LocalCourseCode { get; set; }
        public short SchoolYear { get; set; }
        public string UniqueSectionCode { get; set; }
        public string SessionName { get; set; }
        public string ClassPeriodName { get; set; }

    }
}
