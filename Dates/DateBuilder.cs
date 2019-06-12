using System;
using Core.Numbers;
using Core.Objects;

namespace Core.Dates
{
	public class DateBuilder : IComparable<DateTime>
	{
		public class DateBuilderException : ApplicationException
		{
			static string getMessage(DateBuilder builder, Exception innerException)
			{
				return string.Format("Y:{0} M:{1} D:{2} H:{3} M:{4} S:{5} M:{6} [{7}]", builder.Year, builder.Month, builder.Day, builder.Hour,
					builder.Minute, builder.Second, builder.Millisecond, innerException.Message);
			}
			public DateBuilderException(DateBuilder builder, Exception innerException)
				: base(getMessage(builder, innerException), innerException)
			{
			}
		}

		const int LIMIT_MILLISECOND = 1000;
		const int LIMIT_SECOND = 60;
		const int LIMIT_MINUTE = 60;
		const int LIMIT_HOUR = 24;
		const int LIMIT_MONTH = 12;

		public static implicit operator DateBuilder(DateTime date)
		{
			return new DateBuilder(date);
		}

		public static implicit operator DateTime(DateBuilder builder)
		{
			return builder.ToDate();
		}

		public static implicit operator DateBuilder(string source)
		{
			return new DateBuilder(source);
		}

		public static bool operator ==(DateBuilder left, DateTime right)
		{
			return left.CompareTo(right) == 0;
		}

		public static bool operator !=(DateBuilder left, DateTime right)
		{
			return left.CompareTo(right) != 0;
		}

		public static bool operator <(DateBuilder left, DateTime right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator <=(DateBuilder left, DateTime right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >(DateBuilder left, DateTime right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator >=(DateBuilder left, DateTime right)
		{
			return left.CompareTo(right) >= 0;
		}

		public static DateBuilder operator +(DateBuilder builder, TimeSpan increment)
		{
			return (DateTime)builder + increment;
		}

		public static DateBuilder operator -(DateBuilder builder, TimeSpan increment)
		{
			return (DateTime)builder - increment;
		}

		static void increment(int initialValue, int modulo, out int newValue, out int carryover, bool offset = false)
		{
			if (offset)
				initialValue--;
			newValue = initialValue % modulo;
			carryover = (initialValue - newValue) / modulo;
			if (offset)
				newValue++;
		}

		int year;
		int month;
		int day;
		int hour;
		int minute;
		int second;
		int millisecond;
		bool adjustDay;

		public DateBuilder(bool adjustDay = false)
		{
			Clear();
			this.adjustDay = adjustDay;
		}

		public DateBuilder(int year, int month, int day, int hour, int minute, int second = 0, int millisecond = 0, bool adjustDay = false)
		{
			this.adjustDay = adjustDay;
			Year = year;
			Month = month;
			Day = day;
			Hour = hour;
			Minute = minute;
			Second = second;
			Millisecond = millisecond;
		}

		public DateBuilder(DateTime date, bool adjustDate = false)
			: this(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, adjustDate)
		{
		}

		public DateBuilder(string source, bool adjustDate = false)
			: this(DateTime.Parse(source), adjustDate)
		{
		}

		public void Clear()
		{
			month = day = 1;
			year = hour = minute = second = millisecond = 0;
		}

		void fixDay()
		{
			if (month == 0)
				return;

			var lastDayOfMonth = month.LastOfMonth(year);
			if (adjustDay && day > lastDayOfMonth)
				day = lastDayOfMonth;
		}

		public int Year
		{
			get
			{
				return year;
			}
			set
			{
				year = value;
				fixDay();
			}
		}

		public int Month
		{
			get
			{
				return month;
			}
			set
			{
				if (value >= LIMIT_MONTH)
				{
					int carryover;
					increment(value, LIMIT_MONTH, out month, out carryover, true);
					if (carryover > 0)
						Year += carryover;
				}
				else
					month = value;
				fixDay();
			}
		}

		public int Day
		{
			get
			{
				return day;
			}
			set
			{
				var limitDay = month.LastOfMonth(year);
				if (value >= limitDay && adjustDay)
				{
					int carryover;
					increment(value, limitDay, out day, out carryover, true);
					if (carryover > 0)
						Month += carryover;
				}
				else
					day = value;
				fixDay();
			}
		}

		public int Hour
		{
			get
			{
				return hour;
			}
			set
			{
				if (value >= LIMIT_HOUR)
				{
					int carryover;
					increment(value, 24, out hour, out carryover);
					if (carryover > 0)
						Day += carryover;
				}
				else
					hour = value;
				fixDay();
			}
		}

		public int Minute
		{
			get
			{
				return minute;
			}
			set
			{
				if (value >= LIMIT_MINUTE)
				{
					int carryover;
					increment(value, LIMIT_MINUTE, out minute, out carryover);
					if (carryover > 0)
						Hour += carryover;
				}
				else
					minute = value;
				fixDay();
			}
		}

		public int Second
		{
			get
			{
				return second;
			}
			set
			{
				if (value >= LIMIT_SECOND)
				{
					int carryover;
					increment(value, LIMIT_SECOND, out second, out carryover);
					if (carryover > 0)
						Minute += carryover;
				}
				else
					second = value;
				fixDay();
			}
		}

		public int Millisecond
		{
			get
			{
				return millisecond;
			}
			set
			{
				if (value >= LIMIT_MILLISECOND)
				{
					int carryover;
					increment(value, LIMIT_MILLISECOND, out millisecond, out carryover);
					if (carryover > 0)
						Second += carryover;
				}
				else
					millisecond = value;
				fixDay();
			}
		}

		public bool AdjustDay
		{
			get
			{
				return adjustDay;
			}
			set
			{
				adjustDay = value;
				fixDay();
			}
		}

		public DateTime ToDate()
		{
			try
			{
				return new DateTime(year, month, day, hour, minute, second, millisecond);
			}
			catch (Exception ex)
			{
				throw new DateBuilderException(this, ex);
			}
		}

		public int CompareTo(DateTime other)
		{
			return year.ComparedTo(other.Year)
				.And(month).ComparedTo(other.Month)
				.And(day).ComparedTo(other.Day)
				.And(hour).ComparedTo(other.Hour)
				.And(minute).ComparedTo(other.Minute)
				.And(second).ComparedTo(other.Second)
				.And(millisecond).ComparedTo(other.Millisecond);
		}

		public override string ToString()
		{
			return ToDate().ToString("yyyy-MM-dd hh:mm:ss");
		}

		public override bool Equals(object obj)
		{
			var builder = obj.As<DateBuilder>();
			if (builder.IsSome)
				return this == builder.Value;
			var date = obj.As<DateTime>();
			if (date.IsSome)
				return this == date.Value;
			return false;
		}

		public override int GetHashCode()
		{
			return ToDate().GetHashCode();
		}

		public DayOfWeek DayOfWeek
		{
			get
			{
				return ToDate().DayOfWeek;
			}
		}

		public DateBuilder Clone()
		{
			return new DateBuilder(year, month, day, hour, minute, second, millisecond, adjustDay);
		}
	}
}