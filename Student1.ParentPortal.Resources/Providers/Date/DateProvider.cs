using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Resources.ExtensionMethods;
using Student1.ParentPortal.Resources.Providers.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Student1.ParentPortal.Resources.Providers.Date
{
    public class DateProvider : IDateProvider
    {
        private readonly ISchoolsRepository _schoolsRepository;
        private readonly ICustomParametersProvider _customParametersProvider;
        public DateProvider(ICustomParametersProvider customParametersProvider, ISchoolsRepository schoolsRepository)
        {
            _schoolsRepository = schoolsRepository;
            _customParametersProvider = customParametersProvider;
        }
        public DateTime Today()
        {
            return DateTime.Now;
        }
        //rename to beginningOfDayTodayWithPadding
        public async Task<DateTime> BeginningOfDayTodayWithPadding()
        {
            var sessionsDates = await _schoolsRepository.GetSessionMetadata();

            //var applicableSession = sessionsDates.SingleOrDefault(x => x.BeginDate >= EndOfDayToday() && x.EndDate <= BeginningOfDayToday());
            ////we could get 2 sessions back fall and spring semester for the current year. 
            ////given the filter above wo could be outside of the spring semester for the current year eather in summer or the fall of the next year
            ////taking this in to account we should validate that the applicable session is not null

            //if (applicableSession == null)
            //{
            //    applicableSession = sessionsDates.SingleOrDefault(x => x.Name.Contains("fall"));
            //    applicableSession.BeginDate.AddYears(1).AddPadding(_customParametersProvider.GetParameters().sessionPadding.DaysAfterSessionEnd);
            //}

            //Changing the logic above because that one was missing dates between fall and spring (Jan 15)

            var currentSession = GetCurrentSessionName(Today(), sessionsDates.Find(d => d.Name.Contains("Fall")).BeginDate, sessionsDates.Find(d => d.Name.Contains("Spring")).BeginDate);

            DateTime nextSessionBegin = currentSession == "Spring" ?
                sessionsDates.Find(d => d.Name.Contains("Fall")).BeginDate.AddYears(1).AddPadding(_customParametersProvider.GetParameters().sessionPadding.DaysAfterSessionEnd) :
                sessionsDates.Find(d => d.Name.Contains("Spring")).BeginDate.AddPadding(_customParametersProvider.GetParameters().sessionPadding.DaysAfterSessionEnd);

            var currentSessionEndDate = sessionsDates.Find(d => d.Name.Contains(currentSession)).EndDate.AddPadding(_customParametersProvider.GetParameters().sessionPadding.DaysAfterSessionEnd);

            var daysToRestFromToday = (nextSessionBegin - currentSessionEndDate).Days;
            return Today().AddDays(-daysToRestFromToday);

            //return DateTime.Today.AddPadding(-_customParametersProvider.GetParameters().sessionPadding.DaysBeforeSessionStart).FirstHourOfTheDay();
        }
        public DateTime EndOfDayTodayWithPadding()
        {
            return DateTime.Today.AddPadding(_customParametersProvider.GetParameters().sessionPadding.DaysAfterSessionEnd).LastHourOfTheDay();
        }
        
        public DateTime BeginningOfDayToday()
        {
            return DateTime.Today;
        }
        public DateTime EndOfDayToday()
        {
            return DateTime.Today.LastHourOfTheDay();
        }
        public DateTime Monday()
        {
            return Today().FirstHourOfTheDay().GetMonday();
        }

        public DateTime Friday()
        {
            return Today().LastHourOfTheDay().GetFriday();
        }
        private string GetCurrentSessionName(DateTime date, DateTime initFall, DateTime initSpring)
        {
            float value = (float)date.Month + date.Day / 100f;  // <month>.<day(2 digit)>    
            float spring = (float)initSpring.Month + initSpring.Day / 100f;
            float fall = (float)initFall.Month + initFall.Day / 100f;

            if (value > spring && value < fall) return "Spring";
            return "Fall";
        }
    }
}
