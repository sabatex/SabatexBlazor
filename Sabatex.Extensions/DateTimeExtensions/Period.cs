
using sabatex.Extensions.ClassExtensions;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace sabatex.Extensions.DateTimeExtensions
{
    [TypeConverter(typeof(PeriodConverter))]
    public struct Period : IEquatable<Period>
    {
        public DateTime? Begin { get; set; }
        public DateTime? End { get; set; }
        //public virtual DateTime? Begin { get => begin; set => SetProperty(ref begin, value); }
        //public virtual DateTime? End { get=>end; set=>SetProperty(ref end,value);}

        /// <summary>
        /// Max years range
        /// </summary>
        public const int MaxRange = 255;

        public Period(DateTime date) : this(date.BeginOfDay(), date.EndOfDay()) { }
        public Period(DateTime? beginDate, DateTime? endDate)
        {
            Begin = beginDate;
            End = endDate;
        }

        public static Period GetDay(DateTime date) => new Period(date.BeginOfDay(), date.EndOfDay());
        public static Period GetMonth(DateTime date) => new Period(date.BeginOfMonth(), date.EndOfMonth());
        public static Period GetQuarter(DateTime date) => new Period(date.BeginOfQuarter(), date.EndOfQuarter());
        public static Period GetYear(DateTime date) => new Period(date.BeginOfYear(), date.EndOfYear());
        public static Period GetWeek(DateTime date) => new Period(date.BeginOfWeek(), date.EndOfWeek());

        public static Period Parse(string value)
        {
            if (value.Length == 0)
                throw new ArgumentNullException(nameof(value));
            int pos = value.IndexOf(',');
            if (pos != -1)
            {
                   string s1 = value.Substring(0, pos);
                    DateTime? d1 = s1 == "null" ? new DateTime?() : DateTime.Parse(s1).BeginOfDay();
                    string s2 = value.Substring(pos + 1);
                    DateTime? d2 = s2 == "null" ? new DateTime?() : DateTime.Parse(s2).EndOfDay();
                    return new Period(d1, d2);
            }
            return new Period();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Period))
                return false;
            var p = (Period)obj;
            return ((this.Begin == p.Begin) && (this.End == p.End));
        }

        /// <summary>
        /// 14 bit Begin.Year 8 bit (Range years ) 9 bit (range days)
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (Begin == null)
            {
                if (End == null) return 0x4e7e0000;
                return 0x4e5e0000 | (End.Value.Year * 366 + End.Value.DayOfYear);
            }
            else
            {

                if (End == null) return 0x4e3e0000 | (Begin.Value.Year * 366 + Begin.Value.DayOfYear);
                return (Begin.Value.Year << 17) | (((End.Value.Year * 366 + End.Value.DayOfYear) - (Begin.Value.Year * 366 + Begin.Value.DayOfYear)) & 0x1FFFF);
            }
        }

        public static bool operator ==(Period arg1, Period arg2)
        {
            return arg1.Equals(arg2);
        }

        public static bool operator !=(Period arg1, Period arg2)
        {
            return !arg1.Equals(arg2);
        }

        public override string ToString()
        {
            var s1 = Begin == null ? "null" : Begin.Value.ToShortDateString();
            var s2 = End == null ? "null" : End.Value.ToShortDateString();
            return string.Format("Period from {0} to {1}", s1, s2);
        }

        public bool Equals(Period other)
        {
            return Begin == other.Begin && End == other.End;
        }
    }

}

