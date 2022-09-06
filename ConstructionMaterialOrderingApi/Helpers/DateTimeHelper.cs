using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionMaterialOrderingApi.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly List<int> _listOfMonthsWithOnlyHave30Days = new List<int> { 4, 6, 9, 11 };

        public static DateTime GetDueDate(DateTime reg, DateTime now)
        {
            if(now.Month == 2 && reg.Day > 28)
            {
                int day = reg.Day - 28;
                return new DateTime(now.Year, now.AddMonths(1).Month, day);
            }
            else if(_listOfMonthsWithOnlyHave30Days.Any(m => m == now.Month) && reg.Day > 30)
            {
                int day = reg.Day - 30;
                return new DateTime(now.Year, now.AddMonths(1).Month, day);
            }
            else
            {
                DateTime prevDue = new DateTime(now.Year, now.Month, reg.Day);
                prevDue = prevDue.AddDays(-1);
                if(prevDue.Date < now.Date)
                {
                    var calculatedDate = new DateTime(now.Year, now.Month, reg.Day).AddMonths(1);
                    if(calculatedDate.Month == 2 && reg.Day > 28)
                    {
                        int day = prevDue.Day - 28;
                        return new DateTime(now.Year, calculatedDate.AddMonths(1).Month, day);
                    }
                    return new DateTime(now.Year, now.Month, reg.Day).AddMonths(1);
                }
                else
                {
                    return new DateTime(now.Year, now.Month, reg.Day);
                }
            }
        }
    }
}