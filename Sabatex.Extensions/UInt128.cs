// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Copyrigth (c) Serhiy Lakas 2020
namespace sabatex.Extensions
{
#if NET6_0_OR_GREATER
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System;
using System.Collections.Generic;

    //
    // Summary:
    //     Represents a 128-bit unsigned integer.

    public readonly struct UInt128: IComparable, IComparable<UInt128>, IEquatable<UInt128>
    {
        readonly ulong h;
        readonly ulong l;
        UInt128(ulong value)
        {
            h = 0;
            l = value;
        }

        UInt128(ulong h,ulong l)
        {
            this.h = h;
            this.l = l;
        }
        UInt128(UInt128 value)
        {
            h = value.h;
            l = value.l;
        }
        //
        // Summary:
        //     Represents the largest possible value of an sabatex.Extensions.UInt128. This field is constant.
        public static readonly UInt128 MaxValue = new UInt128(ulong.MaxValue, ulong.MaxValue);
        public static readonly UInt128 MinValue = new UInt128(0);
        public static implicit operator UInt128(ulong value)=> new UInt128(value);        
        public static explicit operator ulong(UInt128 value)=>value.l;
        public int CompareTo(UInt128 other)
        {
            if (h > other.h) return 1;
            if (h < other.h) return -1;
            if (l > other.l) return 1;
            if (l < other.l) return -1;
            return 0;
        }
        public int CompareTo(object obj) => CompareTo((UInt128)obj);

        public static bool operator ==(UInt128 a, UInt128 b) => a.CompareTo(b) == 0;
        public static bool operator !=(UInt128 a, UInt128 b) => a.CompareTo(b) != 0;
        public static bool operator >(UInt128 a, UInt128 b) => a.CompareTo(b) > 0;
        public static bool operator <(UInt128 a, UInt128 b) => a.CompareTo(b) < 0;
        public static bool operator >=(UInt128 a, UInt128 b)
        {
            var r = a.CompareTo(b);
            return r == 0 || r > 0;
        }
        public static bool operator <=(UInt128 a, UInt128 b)
        {
            var r = a.CompareTo(b);
            return r == 0 || r < 0;
        }

        public static UInt128 operator >>(UInt128 value, int count)
        {
            if (count >=128) return 0;
            if (count >=64) return value.h >> count;
            return new UInt128(value.h >> count,value.h<<(64-count) | value.l >> count);
 /*            var result = uInt128;
            for (int i =0;i<count;i++)
            {
                result.l >>= 1;
                if ((result.h & 1) == 1) result.l |= 0x8000000000000000;
                result.h >>= 1;
            }

            return result; */
        }
        public static UInt128 operator <<(UInt128 value, int count)
        {
            if (count >=128) return 0;
            if (count >=64) return new UInt128(value.l << count,0);
            return new UInt128(value.h << count | value.l >> (64-count),value.l << count);
 
            // var result = uInt128;
            // for (int i = 0; i < count; i++)
            // {
            //     result.h <<= 1;
            //     if ((result.l & 0x8000000000000000) > 0) result.h |= 1;
            //     result.l <<= 1;
            // }

            // return result;
        }

        public static UInt128 operator &(UInt128 a, UInt128 b) => new UInt128(a.h & b.h, a.l & b.l);
        public static UInt128 operator |(UInt128 a, UInt128 b) => new UInt128(a.h | b.h, a.l | b.l);
        public static UInt128 operator ^(UInt128 a, UInt128 b) => new UInt128(a.h ^ b.h, a.l ^ b.l);
        public static UInt128 operator ++ (UInt128 value)
        {
            var l = value.l+1;
            if (l == 0)
                return  new UInt128(value.h,l);
            else
                return new UInt128(value.h+1,l);
         }
        public static UInt128 operator --(UInt128 value)
        {
            var l = value.l-1;
            if (l == 0)
                return  new UInt128(value.h,l);
            else
                return new UInt128(value.h-1,l);
        }

        public static UInt128 operator +(UInt128 a,UInt128 b)
        {
            var l = a.l + b.l;
            var h = a.h + b.h;
            if (l <a.l || l < b.l)
                return new UInt128(h + 1,l);
            else
                return new UInt128(h,l);
        }

        public static UInt128 operator -(UInt128 a, UInt128 b)
        {
            var l = a.l - b.l;
            var h = a.h - b.h;
            if (a.l < b.l)
                return new UInt128(h - 1,l);
            else
                return new UInt128(h,l);
        }

        public static UInt128 operator *(UInt128 a, UInt128 b)
        {
            UInt128 result = 0;
            for (int i = 0;i<128;i++)
            {
                if ((b & 1) > 0) result += a;
                a <<= 1;
                b >>= 1;
            }
            return result;
        }
        public static (UInt128,UInt128) Div(UInt128 a, UInt128 b)
        {
            if (b == 0)
                throw new DivideByZeroException();
            if (a < b) return (0,a);

            var hiBit = new UInt128(0x8000000000000000, 0);
            var result = new UInt128(0);
            int count = 0;
            // check count shift
            while ((b & hiBit) == 0 && (b << 1) <= a)
            {
                count++;
                b <<= 1;
            }


            while (count > 0)
            {
                count--;
                if (a >= b)
                {
                    result |= 1;
                    a -= b;
                }
                result <<= 1;
                b >>= 1;
            }
            if (a >= b)
            {
                a -= b;
                result |= 1;
            }
            return (result,a);


        }

        public static UInt128 operator /(UInt128 a, UInt128 b) => Div(a, b).Item1;

        public static UInt128 operator %(UInt128 a, UInt128 b) => Div(a,b).Item2;
 
        public bool Equals(UInt128 other) => h == other.h && l == other.l;
        public override bool Equals(object obj) => Equals((UInt128)obj);
        public override int GetHashCode()
        {
            return h.GetHashCode() ^ l.GetHashCode();
        }

        const string hexChars = "0123456789ABCDEF";
        const string decimalChars = "0123456789";
        public override string ToString()
        {
            return ToHexString();
        }
        public string ToDecimalString()
        {
            var result = new StringBuilder();
            var d = this;

            UInt128 temp = d / 10;
            while (temp > 0)
            {
                ulong ind = (ulong)(d % 10);

                result.Insert(0, decimalChars[(int)ind]);
                d = temp;
                temp = temp / 10;
            }
            result.Insert(0, decimalChars[(int)(ulong)(d % 10)]);
            return result.ToString();
        }

        public static UInt128 Parse(string s)
        {
            var result = new UInt128(0);
            switch (s.Substring(0, 2))
            {
                case "0x":
                    foreach (char c in s.Substring(2))
                    {
                        if (c == ' ') continue;
                        result <<= 4;
                        var r = hexChars.IndexOf(Char.ToUpper(c));
                        if (r < 0) throw new FormatException();
                        result |= (ulong)r;
                    }
                    break;
                case "0b":
                    foreach (char c in s.Substring(2))
                    {
                        if (c == ' ') continue;
                        result <<= 1;
                        switch (c)
                        {
                            case '1':
                                result |= 1;
                                break;
                            case '0':
                                break;
                            default:
                                throw new FormatException();
                        }
                     }
                    break;
                default:
                    foreach (char c in s)
                    {
                        result *= 10;
                        var r = decimalChars.IndexOf(Char.ToUpper(c));
                        if (r < 0) throw new FormatException();
                        result += (ulong)r;
                    }
                    break;
            }
            return result;

        }

        public string ToHexString(bool withSpaces = false)
        {
            var result = new StringBuilder("0x");
            if (withSpaces) result.Append(' ');
            for (int i = 60; i >= 0; i -= 4)
            {
                result.Append(hexChars[(int)(h >> i) & 0xF]);
                if (withSpaces)
                {
                    if ((i % 16) == 0) result.Append(' ');
                }
            }
            if (withSpaces)  result.Append(' ');
            for (int i = 60; i >= 0; i -= 4)
            {
                result.Append(hexChars[(int)(l >> i) & 0xF]);
                if (withSpaces)
                {
                    if ((i % 16) == 0) result.Append(' ');
                }

            }

            return result.ToString();

        }

    }
#endif
}
