using System;

namespace BusinessDaysCounter
{
    public interface IPublicHoliday
    {
        DateTime GetHolidayDate(int year);
    }

    public class FixedDatePublicHoliday : IPublicHoliday
    {
        protected readonly int _day;
        protected readonly int _month;

        public FixedDatePublicHoliday(int day, int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException("Invalid day");
            if (day < 1 || day > 31)
                throw new ArgumentOutOfRangeException("Invalid month");

            // Handle February month  
            if ((month == 2) && (day > 29))
            {
                throw new ArgumentOutOfRangeException("Invalid day");
            }

            // Months of April, June, Sept and Nov must have  
            // number of days less than or equal to 30. 
            if ((month == 4 || month == 6 || month == 9 || month == 11)
                    && (day > 30))
                throw new ArgumentOutOfRangeException("Invalid day");

            _day = day;
            _month = month;
        }

        public virtual DateTime GetHolidayDate(int year)
        {
            if (_day > DateTime.DaysInMonth(year, _month))
            {
                return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
            }
            var date = new DateTime(year, _month, _day);
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            return date;
        }
    }

    public class SlidingDatePublicHoliday : FixedDatePublicHoliday, IPublicHoliday
    {
        public SlidingDatePublicHoliday(int day, int month) : 
            base(day, month)
        {
            
        }

        public override DateTime GetHolidayDate(int year)
        {
            var holidayDate = base.GetHolidayDate(year);
            if (holidayDate == DateTime.MinValue)
            {
                return holidayDate;
            }

            if (holidayDate.DayOfWeek == DayOfWeek.Sunday)
                return holidayDate.AddDays(1);

            if (holidayDate.DayOfWeek == DayOfWeek.Saturday)
                return holidayDate.AddDays(2);

            return holidayDate;
        }
    }

    public class OccuranceOfDayPublicHoliday : IPublicHoliday
    {
        private readonly int _occurance;
        private readonly DayOfWeek _dayOfWeek;
        private readonly int _month;

        public OccuranceOfDayPublicHoliday(int occurance, DayOfWeek dayOfWeek, int month)
        {
            if (occurance == 0 && occurance > 5)
                throw new ArgumentOutOfRangeException("Invalid Occurance");

            _occurance = occurance;
            _dayOfWeek = dayOfWeek;
            _month = month;
        }

        public DateTime GetHolidayDate(int year)
        {
            var month = _month;

            DateTime workingDate = new DateTime(year, _month, 1);
            DateTime lastDay = new DateTime(year += (_month + 1 > 12) ? 1 : 0,
                                            month += (_month + 1 > 12) ? -11 : 1,
                                            1).AddDays(-1);

            workingDate = DateTime.SpecifyKind(workingDate, DateTimeKind.Utc);
            lastDay = DateTime.SpecifyKind(lastDay, DateTimeKind.Utc);

            var maxDays = (lastDay - workingDate).Days + 1;

            int gap = 0;

            // if the the day of week of the first day is NOT the specified day of week
            if (workingDate.DayOfWeek != _dayOfWeek)
            {
                // determine the number of days between the first of the month and
                // the first instance of the specified day of week
                gap = (int)workingDate.DayOfWeek - (int)_dayOfWeek;
                gap = (gap < 0) ? Math.Abs(gap) : 7 - gap;

                // and set the date to the first instance of the specified day of week
                workingDate = workingDate.AddDays(gap);
            }

            // if we want something later than the first instance
            if (_occurance > 1)
            {
                // determine how many days we're going to add to the working date to
                // satisfy the specified ordinal
                int daysToAdd = 7 * (_occurance - 1);

                // finally we adjust the date by the number of days to add
                workingDate = workingDate.AddDays(daysToAdd);
            }

            if (workingDate.Month == _month)
                return workingDate;

            return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
        }
    }
}
