using System;
using System.Threading.Tasks;

namespace Student1.ParentPortal.Resources.Providers.Date
{
    public interface IDateProvider
    {
        DateTime Today();
        DateTime Monday();
        DateTime Friday();
        Task<DateTime> BeginningOfDayTodayWithPadding();
        DateTime EndOfDayTodayWithPadding();
        DateTime BeginningOfDayToday();
        DateTime EndOfDayToday();
    }
}
