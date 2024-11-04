using sabatex.Extensions.ClassExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace sabatex.Extensions.DateTimeExtensions
{
    public class PeriodConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var dt = (Period)value;
                var d1 = dt.Begin;
                var s1 = d1 == null ? "null" : d1.Value.ToShortDateString();
                var d2 = dt.End;
                var s2 = d2 == null ? "null" : d2.Value.ToShortDateString();
                return s1 + "," + s2;
            }
            return null;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string s = (string)value;
                if (s != "")
                {
                    int pos = s.IndexOf(',');
                    if (pos != -1)
                    {
                        try
                        {
                            string s1 = s.Substring(0, pos);
                            DateTime? d1 = s1 == "null" ? new DateTime?() : DateTime.Parse(s1).BeginOfDay();
                            string s2 = s.Substring(pos + 1);
                            DateTime? d2 = s2 == "null" ? new DateTime?() : DateTime.Parse(s2).EndOfDay();
                            return new Period(d1, d2);
                        }
                        catch
                        { }
                    }
                }
            }
            return new Period();
        }
    }

}
