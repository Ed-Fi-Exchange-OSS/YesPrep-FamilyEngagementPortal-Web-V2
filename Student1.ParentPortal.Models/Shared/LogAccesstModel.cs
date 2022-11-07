using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student1.ParentPortal.Models.Shared
{
    public class LogAccessModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime EventDate { get; set; }
        public int? USI { get; set; }
        public string UniqueId { get; set; }
        public int? PersonType { get; set; }
        public string Platform { get; set; }
        public string PlatformInfo { get; set; }
    }
}
