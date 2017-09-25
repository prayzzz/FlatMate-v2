using System;

namespace FlatMate.Module.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetNextWeekday(this DateTime current, DayOfWeek day)
        {
            var daysToAdd = ((int)day - (int)current.DayOfWeek + 7) % 7;
            return current.AddDays(daysToAdd);
        }

        public static DateTime GetPreviousWeekday(this DateTime current, DayOfWeek day)
        {
            var daysToAdd = ((int)current.DayOfWeek - (int)day + 7) % 7;
            return current.AddDays(-daysToAdd);
        }
    }
}