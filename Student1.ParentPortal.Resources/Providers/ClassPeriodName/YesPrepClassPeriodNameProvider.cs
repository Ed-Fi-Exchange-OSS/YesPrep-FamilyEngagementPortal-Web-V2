using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student1.ParentPortal.Resources.Providers.ClassPeriodName
{
    public class YesPrepClassPeriodNameProvider : IClassPeriodNameProvider
    {
        public string ParseClassPeriodName(string classPeriodName)
        {
            // In the original data from eSchool Plus the class Periods where '10/12_F_1', '10/12_F_2'
            // We wanted only the class period number so we are parsing out the last chunk after the _
            //var classParts = classPeriodName.Split('_');
            //return classParts[classParts.Length - 1];
            // In the new Skyward SIS the class period is different. Its at the beginning.
            // For example: '1 - A','1 - B',...,'2 - A','2 - B',...,'10 - A'
            //var classParts = classPeriodName.Split('-');
            return classPeriodName.Replace(" ","");
        }
    }
}
