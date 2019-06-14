using System;
using System.Collections.Generic;
using Core.Arrays;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.RegularExpressions;
using Core.Strings;
using static System.Math;
using static Core.Booleans.Assertions;
using static Core.Monads.MonadFunctions;

namespace Core.Dates
{
	public class Time : IComparable<DateTime>, IComparable<Time>
	{
		const int HOURS_IN_MILLISECONDS = 86400000;

		static (int, int, int, int) deriveFromMilliseconds(long value)
		{
			Assert(value >= 0, "Milliseconds must be positive");

			var milliseconds = 0;
			var seconds = 0;
			var minutes = 0;
			var dividend = (double)value;
			var factor = 3600000d;

			var (division, remainder) = dividend.DivRem(factor);
			var hours = (int)division;
			if (remainder != 0)
			{
				dividend = remainder * factor;
				factor /= 60;
				(division, remainder) = dividend.DivRem(factor);
				minutes = (int)division;

				if (remainder != 0)
				{
					dividend = remainder * factor;
					factor /= 60;
					(division, remainder) = dividend.DivRem(factor);
					seconds = (int)division;

					if (remainder != 0)
					{
						dividend = remainder * factor;
						milliseconds = (int)dividend;
					}
				}
			}

			return (hours, minutes, seconds, milliseconds);
		}

		static (int, int, int, int) parseParts(string text)
		{
			var matcher = new Matcher();
			text = text.RemoveWhitespace();

			matcher.RequiredMatch(text, "^ /(/d1%2) ( ':' /(/d1%2))? ( ':' /(/d1%2))? ( '.' /(/d1%3))? $",
				"Couldn't determine parts of time to parse");

			var (hour, minute, second, millisecond) = matcher;
			return (hour.ToInt(), minute.ToInt(), second.ToInt(), millisecond.ToInt());
		}

		static int absoluteDifference(int value1, int value2) => Sign(value1 - value2);

		public static string ToLongString(int days, int hours, int minutes, int seconds, IMaybe<int> milliseconds)
		{
			var builder = new List<string>();

			if (days > 0)
				if (days == 1)
					builder.Add("1 day");
				else
					builder.Add($"{days} days");

			if (hours > 0)
				if (hours == 1)
					builder.Add("1 hour");
				else
					builder.Add($"{hours} hours");

			if (minutes > 0)
				if (minutes == 1)
					builder.Add("1 minute");
				else
					builder.Add($"{minutes} minutes");

			if (seconds > 0)
				if (seconds == 1)
					builder.Add("1 second");
				else
					builder.Add($"{seconds} seconds");

			if (milliseconds.If(out var ms) && ms > 0)
				if (ms == 1)
					builder.Add("1 millisecond");
				else
					builder.Add($"{ms} milliseconds");

			return builder.ToArray().Andify();
		}

		public static string ToLongString(TimeSpan timeSpan, bool includeMilliseconds)
		{
			return ToLongString(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds,
				maybe(includeMilliseconds, () => timeSpan.Milliseconds));
		}

		public static string ToShortString(TimeSpan timeSpan, bool includeMilliseconds)
		{
			return ToShortString(timeSpan.Days, timeSpan.Hours,
				timeSpan.Minutes, timeSpan.Seconds, maybe(includeMilliseconds, () => timeSpan.Milliseconds));
		}

		public static string ToShortString(int days, int hours, int minutes, int seconds, IMaybe<int> milliseconds)
		{
			var list = new List<string>();

			if (days > 0)
				list.Add("day(s)".Plural(days));
			if (hours > 0)
				list.Add("hr(s)".Plural(hours));
			if (minutes > 0)
				list.Add("min(s)".Plural(minutes));
			if (seconds > 0)
				list.Add("sec(s)".Plural(seconds));
			if (milliseconds.If(out var ms) && ms > 0)
				list.Add("msec(s)".Plural(ms));

			if (list.Count == 0)
				list.Add("0 secs");

			return list.Stringify(" ");
		}

		public static IEnumerator<Time> Enumerable(Time startingTime, Time endingTime, Time incrementTime)
		{
			for (var currentTime = startingTime; currentTime <= endingTime; currentTime += incrementTime)
				yield return currentTime;
		}

		public static bool operator ==(Time time1, Time time2) => time1.Equals(time2);

		public static bool operator !=(Time time1, Time time2) => !time1.Equals(time2);

		public static bool operator <(Time time1, Time time2) => time1.CompareTo(time2) < 0;

		public static bool operator >(Time time1, Time time2) => time1.CompareTo(time2) > 0;

		public static bool operator <=(Time time1, Time time2) => time1.CompareTo(time2) <= 0;

		public static bool operator >=(Time time1, Time time2) => time1.CompareTo(time2) >= 0;

		public static bool operator ==(Time time1, DateTime time2) => time1.CompareTo(time2) == 0;

		public static bool operator !=(Time time1, DateTime time2) => time1.CompareTo(time2) != 0;

		public static bool operator <(Time time1, DateTime time2) => time1.CompareTo(time2) < 0;

		public static bool operator >(Time time1, DateTime time2) => time1.CompareTo(time2) > 0;

		public static bool operator <=(Time time1, DateTime time2) => time1.CompareTo(time2) <= 0;

		public static bool operator >=(Time time1, DateTime time2) => time1.CompareTo(time2) >= 0;

		public static Time operator +(Time time1, Time time2) => new Time(time1.Milliseconds + time2.Milliseconds);

		public static Time operator +(Time time, TimeSpan interval) => new Time(time.Milliseconds + interval.TotalMilliseconds);

		public static DateTime operator +(Time time, DateTime dateTime) => dateTime.Truncate() + (TimeSpan)time;

		public static DateTime operator +(DateTime dateTime, Time time) => dateTime.Truncate() + (TimeSpan)time;

		public static Time operator -(Time time1, Time time2) => new Time(time1.Milliseconds - time2.Milliseconds);

		public static Time operator -(Time time, TimeSpan interval) => new Time(time.Milliseconds - interval.TotalMilliseconds);

		public static implicit operator Time(string text)
		{
			var (hour, minute, second, millisecond) = parseParts(text);
			return new Time(hour, minute, second, millisecond);
		}

		public static implicit operator Time(DateTime date) => new Time(date);

		public static implicit operator TimeSpan(Time time) => time.ToTimeSpan();

		int hour;
		int minute;
		int second;
		int millisecond;

		public Time() : this(0, 0, 0, 0) { }

		public Time(DateTime date) : this(date.Hour, date.Minute, date.Second, date.Millisecond) { }

		public Time(int hour, int minute, int second, int millisecond)
		{
			setHour(hour);
			setMinute(minute);
			setSecond(second);
			setMillisecond(millisecond);
		}

		public Time(double hours)
		{
			var (division, remainder) = hours.DivRem(1);
			var hr = (int)division;

			(division, remainder) = (remainder * 60).DivRem(1);
			var min = (int)Floor(division);
			var sec = (int)Floor(remainder * 60);

			setHour(hr);
			setMinute(min);
			setSecond(sec);
			setMillisecond(0);
		}

		public Time(TimeSpan timeSpan)
		{
			setHour(timeSpan.Hours);
			setMinute(timeSpan.Minutes);
			setSecond(timeSpan.Seconds);
			setMillisecond(timeSpan.Milliseconds);
		}

		public Time(string time)
		{
			var (hr, min, sec, mill) = parseParts(time);

			setHour(hr);
			setMinute(min);
			setSecond(sec);
			setMillisecond(mill);
		}

		public Time(long milliseconds)
		{
			var (hr, min, sec, mill) = deriveFromMilliseconds(milliseconds);

			setHour(hr);
			setMinute(min);
			setSecond(sec);
			setMillisecond(mill);
		}

		public int Hour
		{
			get => hour;
			set => setHour(value);
		}

		public int Minute
		{
			get => minute;
			set => setMinute(value);
		}

		public int Second
		{
			get => second;
			set => setSecond(value);
		}

		public int Millisecond
		{
			get => millisecond;
			set => setMillisecond(value);
		}

		public long Milliseconds
		{
			get
			{
				long result = millisecond;
				var factor = 1000;
				result += second * factor;
				factor *= 60;
				result += minute * factor;
				factor *= 60;
				result += hour * factor;
				return result;
			}
			set
			{
				Assert(value < HOURS_IN_MILLISECONDS, "Milliseconds must be under 24 hours");

				(hour, minute, second, millisecond) = deriveFromMilliseconds(value);
			}
		}

		void setHour(int newHour)
		{
			Assert(newHour.Between(0).And(23), $"The value of hour ({newHour}) is invalid");

			hour = newHour;
		}

		void setMinute(int newMinute)
		{
			Assert(newMinute.Between(0).And(59), $"The value of minute ({newMinute}) is invalid");

			minute = newMinute;
		}

		void setSecond(int newSecond)
		{
			Assert(newSecond.Between(0).And(59), $"The value of second ({newSecond}) is invalid");

			second = newSecond;
		}

		void setMillisecond(int newMillisecond)
		{
			Assert(newMillisecond.Between(0).And(999), $"The value of millisecond ({newMillisecond}) is invalid");

			millisecond = newMillisecond;
		}

		public override string ToString() => $"{hour:00}:{minute:00}:{second:00}.{millisecond:000}";

		public override int GetHashCode() => ToString().GetHashCode();

		public override bool Equals(object obj)
		{
			if (obj is Time comparisand)
				return hour == comparisand.Hour && minute == comparisand.Minute && second == comparisand.Second &&
					millisecond == comparisand.Millisecond;
			else
				return false;
		}

		public int CompareTo(Time time)
		{
			var current = absoluteDifference(hour, time.hour);
			if (current == 0)
			{
				current = absoluteDifference(minute, time.minute);
				if (current == 0)
				{
					current = absoluteDifference(second, time.second);
					if (current == 0)
					{
						current = absoluteDifference(millisecond, time.millisecond);
						return current;
					}
					else
						return current;
				}
				else
					return current;
			}
			else
				return current;
		}

		public int CompareTo(DateTime date)
		{
			var current = absoluteDifference(hour, date.Hour);
			if (current == 0)
			{
				current = absoluteDifference(minute, date.Minute);
				if (current == 0)
				{
					current = absoluteDifference(second, date.Second);
					if (current == 0)
					{
						current = absoluteDifference(millisecond, date.Millisecond);
						return current;
					}
					else
						return current;
				}
				else
					return current;
			}
			else
				return current;
		}

		public TimeSpan Difference(Time comparisand)
		{
			var result = Abs(Milliseconds - comparisand.Milliseconds);
			var (hours, minutes, seconds, milliseconds) = deriveFromMilliseconds(result);

			return new TimeSpan(0, hours, minutes, seconds, milliseconds);
		}

		public bool Within(Time time, TimeSpan allowed) => Difference(time) <= allowed;

		public string ToLongString(bool includeMilliseconds)
		{
			return ToLongString(0, hour, minute, second, maybe(includeMilliseconds, () => millisecond));
		}

		public DateTime ToDate()
		{
			return DateTime.MinValue.AddHours(hour).AddMinutes(minute).AddSeconds(second).AddMilliseconds(millisecond);
		}

		public TimeSpan ToTimeSpan() => new TimeSpan(0, hour, minute, second, millisecond);
	}
}