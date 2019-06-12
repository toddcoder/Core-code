using System;
using Core.Monads.Maybe;

namespace Core.Dates
{
	public static class TimeSpanExpansions
	{
		public static string ToLongString(this TimeSpan span, bool includeMilliseconds)
		{
			return Time.ToLongString(span, includeMilliseconds);
		}

		public static TimeSpan ToTimeSpan(this string source)
		{
			return TimeSpan.Parse(source);
		}

		public static IMaybe<TimeSpan> AsTimeSpan(this string source)
		{
			TimeSpan span;
			TimeSpan.TryParse(source, out span);
			return (span != TimeSpan.Zero).Maybe(() => span);
		}
	}
}