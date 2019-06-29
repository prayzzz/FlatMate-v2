using System;
using FlatMate.Module.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatMate.Module.Common.Test.Extensions
{
    [TestClass]
    public class DateTimeExtensionTest
    {
        [TestMethod]
        public void GetNextWeekday()
        {
            // Monday
            var date = new DateTime(2017, 09, 25);

            var nextFriday = date.GetNextWeekday(DayOfWeek.Friday);

            Assert.AreEqual(DayOfWeek.Monday, date.DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, nextFriday.DayOfWeek);
            Assert.AreEqual(29, nextFriday.Day);
            Assert.AreEqual(09, nextFriday.Month);
            Assert.AreEqual(2017, nextFriday.Year);
        }

        [TestMethod]
        public void GetNextWeekdayAll()
        {
            // Monday
            var date = new DateTime(2017, 09, 25);

            for (var i = 0; i < 7; i++)
            {
                date = date.AddDays(1);

                foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
                {
                    var next = date.GetNextWeekday(dayOfWeek);
                    Assert.AreEqual(dayOfWeek, next.DayOfWeek);
                }
            }
        }

        [TestMethod]
        public void GetPreviousWeekday()
        {
            // Monday
            var date = new DateTime(2017, 09, 25);

            var previousFriday = date.GetPreviousWeekday(DayOfWeek.Friday);

            Assert.AreEqual(DayOfWeek.Monday, date.DayOfWeek);
            Assert.AreEqual(DayOfWeek.Friday, previousFriday.DayOfWeek);
            Assert.AreEqual(22, previousFriday.Day);
            Assert.AreEqual(09, previousFriday.Month);
            Assert.AreEqual(2017, previousFriday.Year);
        }

        [TestMethod]
        public void GetPreviousWeekdayAll()
        {
            // Monday
            var date = new DateTime(2017, 09, 25);

            for (var i = 0; i < 7; i++)
            {
                date = date.AddDays(1);

                foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
                {
                    var previous = date.GetPreviousWeekday(dayOfWeek);
                    Assert.AreEqual(dayOfWeek, previous.DayOfWeek);
                }
            }
        }
    }
}