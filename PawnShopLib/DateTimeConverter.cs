using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    internal static class DateTimeConverter
    {
        internal static int DateTimeToDays(DateTime time)
        {
            int days = 0;
            days += 366;
            for (int i = 1; i < time.Year; i++)
            {
                if (DateTime.IsLeapYear(i))
                    days += 366;
                else
                    days += 365;
            }
            for (int i = 1; i < time.Month; i++)
            {
                days += DateTime.DaysInMonth(time.Year, i);
            }
            days += time.Day;
            return days;
        }
        internal static DateTime DaysToDateTime(int days)
        {
            int year = 1;
            int month = 1;
            days -= 366;
            for (int i = 1; days > (DateTime.IsLeapYear(i) ? 366 : 365); i++)
            {
                if (DateTime.IsLeapYear(i))
                    days -= 366;
                else
                    days -= 365;
                year++;
            }
            for (int i = 1; days > DateTime.DaysInMonth(year, i); i++)
            {
                days -= DateTime.DaysInMonth(year, i);
                month++;
            }
            return new DateTime(year, month, days);
        }
    }
}
