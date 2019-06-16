using BusinessDaysCounter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessDayCounter
{
    public class BusinessDayCounter
    {
        public int WeekdaysBetweenTwoDates(DateTime firstDate, DateTime secondDate)
        {
            if (!CheckForValidDateRange(firstDate, secondDate))
                return 0;

            var firstDateInUTC = NormalizeDateToUTC(firstDate);
            var secondDateInUTC = NormalizeDateToUTC(secondDate);

            var weekdays = CalculateWeekdaysBetweenTwoDates(firstDateInUTC, secondDateInUTC);

            return weekdays.Count;
        }

        public int BusinessDaysBetweenTwoDates(
            DateTime firstDate, DateTime secondDate, IList<DateTime> publicHolidays)
        {
            if (!CheckForValidDateRange(firstDate, secondDate))
                return 0;

            var businessDaysCount = 0;

            var firstDateInUTC = NormalizeDateToUTC(firstDate);
            var secondDateInUTC = NormalizeDateToUTC(secondDate);

            var weekdays = CalculateWeekdaysBetweenTwoDates(firstDateInUTC, secondDateInUTC);
            businessDaysCount = weekdays.Count();

            if (weekdays.Count > 0 && publicHolidays != null && publicHolidays.Count > 0)
            {
                var publicHolidaysDateInUTC = publicHolidays.Select(d => NormalizeDateToUTC(d));
                var weekdaysExceptHolidays = weekdays.Except(publicHolidaysDateInUTC, new DateComparer()).ToList();
                businessDaysCount = weekdaysExceptHolidays.Count();
            }

            return businessDaysCount;
        }

        public int BusinessDaysBetweenTwoDates(
            DateTime firstDate, DateTime secondDate, IList<IPublicHoliday> publicHolidays)
        {
            if (!CheckForValidDateRange(firstDate, secondDate))
                return 0;

            var businessDaysCount = 0;

            var firstDateInUTC = NormalizeDateToUTC(firstDate);
            var secondDateInUTC = NormalizeDateToUTC(secondDate);

            var weekdays = CalculateWeekdaysBetweenTwoDates(firstDateInUTC, secondDateInUTC);
            businessDaysCount = weekdays.Count();

            if (weekdays.Count > 0 && publicHolidays != null && publicHolidays.Count > 0)
            {
                var rangeInYears = new List<int>();
                var currentYear = firstDate.Year;
                while (currentYear <= secondDate.Year)
                {
                    rangeInYears.Add(currentYear);
                    currentYear += 1;
                }
                var publicHolidaysDates = rangeInYears
                    .SelectMany(y => publicHolidays.Select(h => h.GetHolidayDate(y))).ToList();
                var weekdaysExceptHolidays = weekdays.Except(publicHolidaysDates, new DateComparer()).ToList();
                businessDaysCount = weekdaysExceptHolidays.Count();
            }

            return businessDaysCount;
        }

        private List<DateTime> CalculateWeekdaysBetweenTwoDates(DateTime firstDate, DateTime secondDate)
        {
            var weekdays = new List<DateTime>();

            var currentDate = firstDate.AddDays(1);
            currentDate = DateTime.SpecifyKind(currentDate, DateTimeKind.Utc);


            while (currentDate < secondDate)
            {
                if (currentDate.DayOfWeek == DayOfWeek.Monday
                    || currentDate.DayOfWeek == DayOfWeek.Tuesday
                    || currentDate.DayOfWeek == DayOfWeek.Wednesday
                    || currentDate.DayOfWeek == DayOfWeek.Thursday
                    || currentDate.DayOfWeek == DayOfWeek.Friday)
                {
                    weekdays.Add(currentDate);
                }
                currentDate = currentDate.AddDays(1);
                currentDate = DateTime.SpecifyKind(currentDate, DateTimeKind.Utc);
            }

            return weekdays;
        }

        private DateTime NormalizeDateToUTC(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;

            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        private bool CheckForValidDateRange(DateTime firstDate, DateTime secondDate)
        {
            if (firstDate >= secondDate)
                return false;

            return true;
        }
    }

    class DateComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime d1, DateTime d2)
        {
            return d1.Date == d2.Date;
        }

        public int GetHashCode(DateTime date)
        {
            return date.GetHashCode();
        }
    }
}
