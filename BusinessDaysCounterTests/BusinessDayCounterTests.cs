using NUnit.Framework;
using System;
using BusinessDayCounter;
using System.Collections.Generic;
using BusinessDaysCounter;

namespace BusinessDayCounterTests
{
    public class BusinessDaysCounterShould
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Return0WhenSecondDateIsLessThanFirstDate()
        {
            var expectedDaysCount = 0;

            var firstDate = new DateTime(2019, 06, 16);
            var secondDate = new DateTime(2019, 06, 16);

            var sut = new BusinessDayCounter.BusinessDayCounter();

            var count = sut.WeekdaysBetweenTwoDates(firstDate, secondDate);

            Assert.AreEqual(expectedDaysCount, count);
        }

        [TestCaseSource("WeekdaysBetweenTwoDatesTestCases")]
        public int ReturnWeekdaysForDateRange(DateTime firstDate, DateTime secondDate)
        {
            var sut = new BusinessDayCounter.BusinessDayCounter();

            var count = sut.WeekdaysBetweenTwoDates(firstDate, secondDate);

            return count;
        }

        [TestCaseSource("BusinessDaysBetweenTwoDatesTestCases")]
        public int ReturnWeekdaysExceptHolidaysForDateRange(
            DateTime firstDate, DateTime secondDate, IList<DateTime> publicHolidays)
        {
            var sut = new BusinessDayCounter.BusinessDayCounter();

            var count = sut.BusinessDaysBetweenTwoDates(firstDate, secondDate, publicHolidays);

            return count;
        }

        [TestCaseSource("BusinessDaysBetweenTwoDatesWithComplexHolidaysTestCases")]
        public int ReturnWeekdaysExceptHolidaysWithComplexHolidayStructureForDateRange(
            DateTime firstDate, DateTime secondDate, IList<IPublicHoliday> publicHolidays)
        {
            var sut = new BusinessDayCounter.BusinessDayCounter();

            var count = sut.BusinessDaysBetweenTwoDates(firstDate, secondDate, publicHolidays);

            return count;
        }

        [TestCaseSource("FixedDatePublicHolidayTestCases")]
        public DateTime ReturnsFixedDatePublicHoliday(int day, int month, int year)
        {
            var sut = new FixedDatePublicHoliday(day, month);

            var publicHolidayDate = sut.GetHolidayDate(year);

            return publicHolidayDate;
        }

        [TestCaseSource("SlidingDatePublicHolidayTestCases")]
        public DateTime ReturnsSlidingDatePublicHoliday(int day, int month, int year)
        {
            var sut = new SlidingDatePublicHoliday(day, month);

            var publicHolidayDate = sut.GetHolidayDate(year);

            return publicHolidayDate;
        }

        [TestCaseSource("OccuranceOfDayPublicHolidayTestCases")]
        public DateTime ReturnsOccuranceOfDayPublicHoliday(
            int occurance, DayOfWeek dayOfWeek, int month, int year)
        {
            var sut = new OccuranceOfDayPublicHoliday(occurance, dayOfWeek, month);

            var publicHolidayDate = sut.GetHolidayDate(year);

            return publicHolidayDate;
        }

        static IEnumerable<TestCaseData> WeekdaysBetweenTwoDatesTestCases
        {
            get
            {
                yield return new TestCaseData(new DateTime(2013, 10, 07), new DateTime(2013, 10, 09)).Returns(1);
                yield return new TestCaseData(new DateTime(2013, 10, 05), new DateTime(2013, 10, 14)).Returns(5);
                yield return new TestCaseData(new DateTime(2013, 10, 07), new DateTime(2014, 01, 01)).Returns(61);
                yield return new TestCaseData(new DateTime(2013, 10, 07), new DateTime(2013, 10, 05)).Returns(0);
            }
        }

        static IEnumerable<TestCaseData> BusinessDaysBetweenTwoDatesTestCases
        {
            get
            {
                var publicHolidays = new List<DateTime>
                {
                    new DateTime(2013, 12, 25),
                    new DateTime(2013, 12, 26),
                    new DateTime(2014, 01, 01)
                };

                yield return new TestCaseData(
                    new DateTime(2013, 10, 07), new DateTime(2013, 10, 09), publicHolidays)
                    .Returns(1);
                yield return new TestCaseData(
                    new DateTime(2013, 12, 24), new DateTime(2013, 12, 27), publicHolidays)
                    .Returns(0);
                yield return new TestCaseData(
                    new DateTime(2013, 10, 07), new DateTime(2014, 01, 01), publicHolidays)
                    .Returns(59);
            }
        }

        static IEnumerable<TestCaseData> BusinessDaysBetweenTwoDatesWithComplexHolidaysTestCases
        {
            get
            {
                var publicHolidays = new List<IPublicHoliday>
                {
                    new FixedDatePublicHoliday(25, 04),
                    new FixedDatePublicHoliday(29, 02),
                    new SlidingDatePublicHoliday(09, 03),
                    new SlidingDatePublicHoliday(19, 04),
                    new OccuranceOfDayPublicHoliday(02, DayOfWeek.Monday, 06)
                };
                yield return new TestCaseData(
                    new DateTime(2018, 10, 07), new DateTime(2019, 10, 09), publicHolidays)
                    .Returns(258);
            }
        }

        static IEnumerable<TestCaseData> FixedDatePublicHolidayTestCases
        {
            get
            {
                yield return new TestCaseData(25, 04, 2019).Returns(DateTime.SpecifyKind(new DateTime(2019, 04, 25), DateTimeKind.Utc));
                yield return new TestCaseData(29, 02, 2019).Returns(DateTime.MinValue);
            }
        }

        static IEnumerable<TestCaseData> SlidingDatePublicHolidayTestCases
        {
            get
            {
                yield return new TestCaseData(09, 03, 2019).Returns(DateTime.SpecifyKind(new DateTime(2019, 03, 11), DateTimeKind.Utc));
                yield return new TestCaseData(19, 04, 2019).Returns(DateTime.SpecifyKind(new DateTime(2019, 04, 19), DateTimeKind.Utc));
            }
        }

        static IEnumerable<TestCaseData> OccuranceOfDayPublicHolidayTestCases
        {
            get
            {
                yield return new TestCaseData(02, DayOfWeek.Monday, 06, 2019).Returns(DateTime.SpecifyKind(new DateTime(2019, 06, 10), DateTimeKind.Utc));
                yield return new TestCaseData(02, DayOfWeek.Monday, 06, 2018).Returns(DateTime.SpecifyKind(new DateTime(2018, 06, 11), DateTimeKind.Utc));
                yield return new TestCaseData(05, DayOfWeek.Monday, 06, 2018).Returns(DateTime.MinValue);
            }
        }
    }
}