/*
The MIT License (MIT)
Copyright (c) 2015 Clay Anderson
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace BasicPreprocess
{
    /// <summary>
    /// A container for date-related members.
    /// </summary>
    [Serializable]
    public readonly struct Date : IComparable, IFormattable, ISerializable, IComparable<Date>, IEquatable<Date>
    {
        private readonly DateTime m_dt;

        /// <summary>
        /// Max date value
        /// </summary>
        public static readonly Date MaxValue = new Date(DateTime.MaxValue);
        /// <summary>
        /// Min date value
        /// </summary>
        public static readonly Date MinValue = new Date(DateTime.MinValue);

        /// <summary>
        /// Constructor by year, month, day.
        /// </summary>
        /// <param name="year">The 4-digit year</param>
        /// <param name="month">The month</param>
        /// <param name="day">The day</param>
        public Date(int year, int month, int day)
        {
            m_dt = new DateTime(year, month, day);
        }

        /// <summary>
        /// Conversion constructor from datetime.
        /// </summary>
        /// <param name="dateTime"></param>
        public Date(DateTime dateTime)
        {
            m_dt = dateTime.AddTicks(-dateTime.Ticks % TimeSpan.TicksPerDay);
        }

        private Date(SerializationInfo info, StreamingContext context)
        {
            m_dt = DateTime.FromFileTime(info.GetInt64("ticks"));
        }

        /// <summary>
        /// operator overload for date - date = timespan
        /// </summary>
        /// <param name="d1">The later date</param>
        /// <param name="d2">The earlier date</param>
        /// <returns></returns>
        public static TimeSpan operator -(Date d1, Date d2) => d1.m_dt - d2.m_dt;

        /// <summary>
        /// operator overload for date - timespan = date
        /// </summary>
        /// <param name="d">The date</param>
        /// <param name="t">The timespan</param>
        /// <returns></returns>
        public static Date operator -(Date d, TimeSpan t) => new Date(d.m_dt - t);

        /// <summary>
        /// Operator overload for date != date = true/false
        /// </summary>
        /// <param name="d1">A date</param>
        /// <param name="d2">Another date</param>
        /// <returns></returns>
        public static bool operator !=(Date d1, Date d2) => d1.m_dt != d2.m_dt;

        /// <summary>
        /// Operator overload for Date + Timespan = Date
        /// </summary>
        /// <param name="d">The date</param>
        /// <param name="t">The timespan</param>
        /// <returns></returns>
        public static Date operator +(Date d, TimeSpan t) => new Date(d.m_dt + t);

        /// <summary>
        /// Operator overload for Date LT Date = true/false, 
        /// where later dates are higher values than earlier dates.
        /// </summary>
        /// <param name="d1">A date</param>
        /// <param name="d2">Another date</param>
        /// <returns></returns>
        public static bool operator <(Date d1, Date d2) => d1.m_dt < d2.m_dt;

        /// <summary>
        /// Operator overload for Date LTE Date = true/false, 
        /// where later dates are higher values than earlier dates.
        /// </summary>
        /// <param name="d1">A date</param>
        /// <param name="d2">Another date</param>
        public static bool operator <=(Date d1, Date d2) => d1.m_dt <= d2.m_dt;

        /// <summary>
        /// Operator overload for Date == Date = true/false, 
        /// where later dates are higher values than earlier dates.
        /// </summary>
        /// <param name="d1">A date</param>
        /// <param name="d2">Another date</param>
        public static bool operator ==(Date d1, Date d2) => d1.m_dt == d2.m_dt;

        /// <summary>
        /// Operator overload for Date > Date = true/false, 
        /// where later dates are higher values than earlier dates.
        /// </summary>
        /// <param name="d1">A date</param>
        /// <param name="d2">Another date</param>
        public static bool operator >(Date d1, Date d2) => d1.m_dt > d2.m_dt;

        /// <summary>
        /// Operator overload for Date >= Date = true/false, 
        /// where later dates are higher values than earlier dates.
        /// </summary>
        /// <param name="d1">A date</param>
        /// <param name="d2">Another date</param>
        public static bool operator >=(Date d1, Date d2) => d1.m_dt >= d2.m_dt;

        /// <summary>
        /// Implicit operator overload for DateTime to be used like Date.
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator DateTime(Date d) => d.m_dt;

        /// <summary>
        /// Explicit operator overload for Date to be used like Datetime.
        /// </summary>
        /// <param name="d"></param>
        public static explicit operator Date(DateTime d) => new Date(d);

        /// <summary>
        /// Get day member of date
        /// </summary>
        public int Day => m_dt.Day;

        /// <summary>
        /// Get day of week enumeration
        /// </summary>
        public DayOfWeek DayOfWeek => m_dt.DayOfWeek;

        /// <summary>
        /// Get day of year value
        /// </summary>
        public int DayOfYear => m_dt.DayOfYear;

        /// <summary>
        /// get month of year value
        /// </summary>
        public int Month => m_dt.Month;

        /// <summary>
        /// Get today's date.
        /// </summary>
        public static Date Today => new Date(DateTime.Today);

        /// <summary>
        /// Get the year value.
        /// </summary>
        public int Year => m_dt.Year;

        /// <summary>
        /// Get the tick value.
        /// </summary>
        public long Ticks => m_dt.Ticks;

        /// <summary>
        /// Add days to a date.
        /// </summary>
        /// <param name="value">The number of days to add.</param>
        /// <returns></returns>
        public Date AddDays(int value) => new Date(m_dt.AddDays(value));

        /// <summary>
        /// Add months to a date.
        /// </summary>
        /// <param name="value">The number of months to add.</param>
        /// <returns></returns>
        public Date AddMonths(int value) => new Date(m_dt.AddMonths(value));

        /// <summary>
        /// Add years to a date.
        /// </summary>
        /// <param name="value">The number of years to add.</param>
        /// <returns></returns>
        public Date AddYears(int value) => new Date(m_dt.AddYears(value));

        /// <summary>
        /// Compare 2 dates, and output the int difference.
        /// </summary>
        /// <param name="d1">A date</param>
        /// <param name="d2">Another date</param>
        /// <returns></returns>
        public static int Compare(Date d1, Date d2) => d1.CompareTo(d2);

        /// <summary>
        /// Compare another date to this date
        /// </summary>
        /// <param name="value">Another date</param>
        /// <returns></returns>
        public int CompareTo(Date value) => m_dt.CompareTo(value.m_dt);

        /// <summary>
        /// Compare another value to this date
        /// </summary>
        /// <param name="value">The other value</param>
        /// <returns></returns>
        public int CompareTo(object value) => m_dt.CompareTo(value);

        /// <summary>
        /// Get the number of days in a given month.
        /// </summary>
        /// <param name="year">The year of the month in question (in case of february in a leap year)</param>
        /// <param name="month">The month in question</param>
        /// <returns></returns>
        public static int DaysInMonth(int year, int month) => DateTime.DaysInMonth(year, month);

        /// <summary>
        /// Are two dates equal?
        /// </summary>
        /// <param name="value">The date in question</param>
        /// <returns></returns>
        public bool Equals(Date value) => m_dt.Equals(value.m_dt);

        /// <summary>
        /// Are a date and a value equal?
        /// </summary>
        /// <param name="value">The object in question</param>
        /// <returns></returns>
        public override bool Equals(object value) => value is Date date && m_dt.Equals(date.m_dt);

        /// <summary>
        /// Get the int hashcode of a date.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => m_dt.GetHashCode();

        /// <summary>
        /// Are two dates equal?
        /// </summary>
        /// <param name="d1">A date</param>
        /// <param name="d2">Another date</param>
        /// <returns></returns>
        public static bool Equals(Date d1, Date d2) => d1.m_dt.Equals(d2.m_dt);

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            => info.AddValue("ticks", m_dt.Ticks);

        /// <summary>
        /// Is this a leap year?
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static bool IsLeapYear(int year) => DateTime.IsLeapYear(year);

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Date Parse(string s)
            => new Date(DateTime.Parse(s));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Date Parse(string s, IFormatProvider provider)
            => new Date(DateTime.Parse(s, provider));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="provider"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static Date Parse(string s, IFormatProvider provider, DateTimeStyles style)
            => new Date(DateTime.Parse(s, provider, style));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Date ParseExact(string s, string format, IFormatProvider provider)
            => new Date(DateTime.ParseExact(s, format, provider));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static Date ParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style)
            => new Date(DateTime.ParseExact(s, format, provider, style));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="formats"></param>
        /// <param name="provider"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static Date ParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style)
            => new Date(DateTime.ParseExact(s, formats, provider, style));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TimeSpan Subtract(Date value) => this - value;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Date Subtract(TimeSpan value) => this - value;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToLongString() => m_dt.ToLongDateString();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToShortString() => m_dt.ToShortDateString();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToShortString();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public string ToString(IFormatProvider provider) => m_dt.ToString(provider);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            if (format == "O" || format == "o" || format == "s")
                return ToString("yyyy-MM-dd");

            return m_dt.ToString(format);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider provider) => m_dt.ToString(format, provider);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out Date result)
        {
            bool success = DateTime.TryParse(s, out DateTime d);
            result = new Date(d);
            return success;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="provider"></param>
        /// <param name="style"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, IFormatProvider provider, DateTimeStyles style, out Date result)
        {
            bool success = DateTime.TryParse(s, provider, style, out DateTime d);
            result = new Date(d);
            return success;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <param name="style"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style,
            out Date result)
        {
            bool success = DateTime.TryParseExact(s, format, provider, style, out DateTime d);
            result = new Date(d);
            return success;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="formats"></param>
        /// <param name="provider"></param>
        /// <param name="style"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style,
            out Date result)
        {
            bool success = DateTime.TryParseExact(s, formats, provider, style, out DateTime d);
            result = new Date(d);
            return success;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Date ToDate(this DateTime dt) => new Date(dt);
    }
}