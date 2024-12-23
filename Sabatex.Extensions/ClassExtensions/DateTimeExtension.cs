using System;
using System.ComponentModel;
using System.Globalization;

namespace Sabatex.Extensions.ClassExtensions;

/// <summary>
/// DateTime extended as BeginOfDay ...
/// </summary>
public static class DateTimeExtension
{
    public static DateTime BeginOfDay(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
    }
    public static DateTime EndOfDay(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
    }

    public static DateTime BeginOfWeek(this DateTime dt)
    {
        return BeginOfDay(dt.AddDays(1 - (int)(dt.DayOfWeek)));
    }

    public static DateTime EndOfWeek(this DateTime dt)
    {
        return EndOfDay(dt.AddDays(7 - (int)(dt.DayOfWeek)));
    }

    public static DateTime BeginOfMonth(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);
    }

    public static DateTime EndOfMonth(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month), 23, 59, 59);
    }

    public static DateTime BeginOfYear(this DateTime dt)
    {
        return new DateTime(dt.Year, 1, 1, 0, 0, 0);
    }

    public static DateTime EndOfYear(this DateTime dt)
    {
        return new DateTime(dt.Year, 12, 31, 23, 59, 59);
    }

    public static int Quarter(this DateTime dt)
    {
        switch (dt.Month)
        {
            case 1:
            case 2:
            case 3:
                return 1;
            case 4:
            case 5:
            case 6:
                return 2;
            case 7:
            case 8:
            case 9:
                return 3;
            default:
                return 4;
        }
    }

    public static DateTime BeginOfQuarter(this DateTime dt)
    {
        switch (dt.Quarter())
        {
            case 1:
                return new DateTime(dt.Year, 1, 1, 0, 0, 0);
            case 2:
                return new DateTime(dt.Year, 4, 1, 0, 0, 0);
            case 3:
                return new DateTime(dt.Year, 7, 1, 0, 0, 0);
            default:
                return new DateTime(dt.Year, 10, 1, 0, 0, 0);

        }
    }

    public static DateTime EndOfQuarter(this DateTime dt)
    {
        switch (dt.Quarter())
        {
            case 1:
                return new DateTime(dt.Year, 3, 31, 23, 59, 49);
            case 2:
                return new DateTime(dt.Year, 6, 30, 23, 59, 49);
            case 3:
                return new DateTime(dt.Year, 9, 30, 23, 59, 49);
            default:
                return new DateTime(dt.Year, 12, 31, 23, 59, 49);

        }
    }

    public static int WeekOfYear(this DateTime dt)
    {
        Calendar cl = CultureInfo.InvariantCulture.Calendar;
        return cl.GetWeekOfYear(dt, DateTimeFormatInfo.CurrentInfo.CalendarWeekRule, DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
    }
}
