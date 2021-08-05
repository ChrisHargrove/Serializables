using System;
using System.Globalization;
using UnityEngine;

namespace BatteryAcid.Serializables
{
    [Serializable]
    public class SerializableDateTime : ISerializationCallbackReceiver, IComparable, IComparable<SerializableDateTime>, IConvertible, IEquatable<SerializableDateTime>, IFormattable
    {
        public SerializableDateTime(DateTime dateTime)
            => this.dateTime = dateTime;

        public SerializableDateTime(long ticks)
            => dateTime = new DateTime(ticks);

        public SerializableDateTime(long ticks, DateTimeKind kind)
            => dateTime = new DateTime(ticks, kind);

        public SerializableDateTime(int year, int month, int day)
            => dateTime = new DateTime(year, month, day);

        public SerializableDateTime(int year, int month, int day, Calendar calender)
            => dateTime = new DateTime(year, month, day, calender);

        public SerializableDateTime(int year, int month, int day, int hour, int minute, int second)
            => dateTime = new DateTime(year, month, day, hour, minute, second);

        public SerializableDateTime(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
            => dateTime = new DateTime(year, month, day, hour, minute, second, kind);

        public SerializableDateTime(int year, int month, int day, int hour, int minute, int second, Calendar calendar)
            => dateTime = new DateTime(year, month, day, hour, minute, second, calendar);

        public SerializableDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
            => dateTime = new DateTime(year, month, day, hour, minute, second, millisecond);
        public SerializableDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, DateTimeKind kind)
            => dateTime = new DateTime(year, month, day, hour, minute, second, millisecond, kind);

        public SerializableDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar)
            => dateTime = new DateTime(year, month, day, hour, minute, second, millisecond, calendar);
        public SerializableDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, DateTimeKind kind)
            => dateTime = new DateTime(year, month, day, hour, minute, second, millisecond, calendar, kind);

        private DateTime dateTime;

        [SerializeField, HideInInspector]
        private long ticks;
        [SerializeField, HideInInspector]
        private DateTimeKind kind;

        public long Ticks => dateTime.Ticks;
        public int Second => dateTime.Second;
        public DateTime Date => dateTime.Date;
        public int Month => dateTime.Month;
        public int Minute => dateTime.Minute;
        public int Millisecond => dateTime.Millisecond;
        public DateTimeKind Kind => dateTime.Kind;
        public int Hour => dateTime.Hour;
        public int DayOfYear => dateTime.DayOfYear;
        public DayOfWeek DayOfWeek => dateTime.DayOfWeek;
        public int Day => dateTime.Day;
        public TimeSpan TimeOfDay => dateTime.TimeOfDay;
        public int Year => dateTime.Year;

        public SerializableDateTime Add(TimeSpan value)
            => dateTime.Add(value);

        public SerializableDateTime AddDays(double value)
            => dateTime.AddDays(value);

        public SerializableDateTime AddHours(double value)
            => dateTime.AddHours(value);

        public SerializableDateTime AddMilliseconds(double value)
            => dateTime.AddMilliseconds(value);

        public SerializableDateTime AddMinutes(double value)
            => dateTime.AddMinutes(value);

        public SerializableDateTime AddMonths(int value)
            => dateTime.AddMonths(value);

        public SerializableDateTime AddSeconds(double value)
            => dateTime.AddSeconds(value);

        public SerializableDateTime AddTicks(long value)
            => dateTime.AddTicks(value);

        public SerializableDateTime AddYears(int value)
            => dateTime.AddYears(value);

        public string[] GetDateTimeFormats(char format, IFormatProvider provider)
            => dateTime.GetDateTimeFormats(format, provider);

        public string[] GetDateTimeFormats(char format)
            => dateTime.GetDateTimeFormats(format);

        public string[] GetDateTimeFormats()
            => dateTime.GetDateTimeFormats();

        public string[] GetDateTimeFormats(IFormatProvider provider)
            => dateTime.GetDateTimeFormats(provider);

        public override int GetHashCode()
            => dateTime.GetHashCode();

        public bool IsDaylightSavingTime()
            => dateTime.IsDaylightSavingTime();

        public TimeSpan Substract(DateTime value)
            => dateTime.Subtract(value);

        public TimeSpan Substract(SerializableDateTime value)
            => dateTime.Subtract(value.dateTime);

        public SerializableDateTime Subtract(TimeSpan value)
            => dateTime.Subtract(value);

        public long ToBinary()
            => dateTime.ToBinary();

        public long ToFileTime()
            => dateTime.ToFileTime();

        public long ToFileTimeUtc()
            => dateTime.ToFileTimeUtc();

        public DateTime ToLocalTime()
            => dateTime.ToLocalTime();

        public string ToLongDateString()
            => dateTime.ToLongDateString();

        public string ToLongTimeString()
            => dateTime.ToLongTimeString();

        public double ToOADate()
            => dateTime.ToOADate();

        public string ToShortDateString()
            => dateTime.ToShortDateString();

        public string ToShortTimeString()
            => dateTime.ToShortTimeString();

        public SerializableDateTime ToUniversalTime()
            => dateTime.ToUniversalTime();

        #region Operators

        public static implicit operator DateTime(SerializableDateTime serializable)
            => serializable.dateTime;
        public static implicit operator SerializableDateTime(DateTime dateTime)
            => new SerializableDateTime(dateTime);

        public static SerializableDateTime operator +(SerializableDateTime sd, TimeSpan t)
            => new SerializableDateTime(sd.dateTime + t);
        public static SerializableDateTime operator -(SerializableDateTime sd, TimeSpan t)
            => new SerializableDateTime(sd.dateTime - t);

        public static TimeSpan operator -(SerializableDateTime sd1, SerializableDateTime sd2)
            => sd1.dateTime - sd2.dateTime;
        public static TimeSpan operator -(DateTime d, SerializableDateTime sd)
            => d - sd.dateTime;
        public static TimeSpan operator -(SerializableDateTime sd, DateTime d)
            => sd.dateTime - d;

        public static bool operator ==(SerializableDateTime sd1, SerializableDateTime sd2)
            => sd1.dateTime == sd2.dateTime;
        public static bool operator ==(DateTime d, SerializableDateTime sd)
            => sd.dateTime == d;
        public static bool operator ==(SerializableDateTime sd, DateTime d)
            => sd.dateTime == d;

        public static bool operator !=(SerializableDateTime sd1, SerializableDateTime sd2)
            => sd1.dateTime != sd2.dateTime;
        public static bool operator !=(DateTime d, SerializableDateTime sd)
            => sd.dateTime != d;
        public static bool operator !=(SerializableDateTime sd, DateTime d)
            => sd.dateTime != d;

        public static bool operator <(SerializableDateTime sd1, SerializableDateTime sd2)
            => sd1.dateTime < sd2.dateTime;
        public static bool operator <(SerializableDateTime sd, DateTime d)
            => sd.dateTime < d;
        public static bool operator <(DateTime d, SerializableDateTime sd)
            => d < sd.dateTime;

        public static bool operator >(SerializableDateTime sd1, SerializableDateTime sd2)
            => sd1.dateTime > sd2.dateTime;
        public static bool operator >(SerializableDateTime sd, DateTime d)
            => sd.dateTime > d;
        public static bool operator >(DateTime d, SerializableDateTime sd)
            => d > sd.dateTime;

        public static bool operator <=(SerializableDateTime sd1, SerializableDateTime sd2)
            => sd1.dateTime <= sd2.dateTime;
        public static bool operator <=(SerializableDateTime sd, DateTime d)
            => sd.dateTime <= d;
        public static bool operator <=(DateTime d, SerializableDateTime sd)
            => d <= sd.dateTime;

        public static bool operator >=(SerializableDateTime sd1, SerializableDateTime sd2)
            => sd1.dateTime >= sd2.dateTime;
        public static bool operator >=(SerializableDateTime sd, DateTime d)
            => sd.dateTime >= d;
        public static bool operator >=(DateTime d, SerializableDateTime sd)
            => d >= sd.dateTime;

        #endregion

        #region ISerializationCallbackReceiver

        public void OnAfterDeserialize()
        {
            dateTime = new DateTime(ticks, kind);
        }

        public void OnBeforeSerialize()
        {
            ticks = dateTime.Ticks;
            kind = dateTime.Kind;
        }

        #endregion

        #region IComparable

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is SerializableDateTime serialized)
            {
                return dateTime.CompareTo(serialized.dateTime);
            }
            if (obj is DateTime date)
            {
                return dateTime.CompareTo(date);
            }
            throw new ArgumentException("Object is neither a SerializableDateTime or DateTime");
        }

        public int CompareTo(SerializableDateTime other)
        {
            return dateTime.CompareTo(other.dateTime);
        }

        #endregion

        #region IConvertible

        public TypeCode GetTypeCode()
        {
            return dateTime.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToBoolean(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToByte(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToChar(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToDateTime(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToDecimal(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToDouble(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToInt64(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToSByte(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToSingle(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return dateTime.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToType(conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToUInt16(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToUInt32(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)dateTime).ToUInt64(provider);
        }

        #endregion

        #region IEquatable

        public override bool Equals(object obj)
        {
            if (obj is SerializableDateTime serialized)
            {
                return dateTime.Equals(serialized.dateTime);
            }
            if (obj is DateTime date)
            {
                return dateTime.Equals(date);
            }
            return false;
        }

        public bool Equals(SerializableDateTime other)
        {
            return dateTime.Equals(other.dateTime);
        }

        #endregion

        #region IFormattable

        public override string ToString()
        {
            return dateTime.ToString();
        }

        public string ToString(string format)
        {
            return dateTime.ToString(format);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return dateTime.ToString(format, formatProvider);
        }

        #endregion
    }
}