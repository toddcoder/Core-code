using System;

namespace Core.Collections
{
	public static class TupleExtensions
	{
		public static void Assign<T1, T2>(this Tuple<T1, T2> tuple, out T1 variable1, out T2 variable2)
		{
			variable1 = tuple.Item1;
			variable2 = tuple.Item2;
		}

		public static void Assign<T1, T2, T3>(this Tuple<T1, T2, T3> tuple, out T1 variable1, out T2 variable2, out T3 variable3)
		{
			variable1 = tuple.Item1;
			variable2 = tuple.Item2;
			variable3 = tuple.Item3;
		}

		public static void Assign<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple, out T1 variable1, out T2 variable2, out T3 variable3,
			out T4 variable4)
		{
			variable1 = tuple.Item1;
			variable2 = tuple.Item2;
			variable3 = tuple.Item3;
			variable4 = tuple.Item4;
		}

		public static void Assign<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple, out T1 variable1, out T2 variable2,
			out T3 variable3, out T4 variable4, out T5 variable5)
		{
			variable1 = tuple.Item1;
			variable2 = tuple.Item2;
			variable3 = tuple.Item3;
			variable4 = tuple.Item4;
			variable5 = tuple.Item5;
		}

		public static void Assign<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5, T6> tuple, out T1 variable1, out T2 variable2,
			out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6)
		{
			variable1 = tuple.Item1;
			variable2 = tuple.Item2;
			variable3 = tuple.Item3;
			variable4 = tuple.Item4;
			variable5 = tuple.Item5;
			variable6 = tuple.Item6;
		}

		public static void Assign<T1, T2, T3, T4, T5, T6, T7>(this Tuple<T1, T2, T3, T4, T5, T6, T7> tuple, out T1 variable1,
			out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6, out T7 variable7)
		{
			variable1 = tuple.Item1;
			variable2 = tuple.Item2;
			variable3 = tuple.Item3;
			variable4 = tuple.Item4;
			variable5 = tuple.Item5;
			variable6 = tuple.Item6;
			variable7 = tuple.Item7;
		}

		public static void Assign<T1, T2, T3, T4, T5, T6, T7, TRest>(this Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple, out T1 variable1,
			out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6, out T7 variable7, out TRest rest)
		{
			variable1 = tuple.Item1;
			variable2 = tuple.Item2;
			variable3 = tuple.Item3;
			variable4 = tuple.Item4;
			variable5 = tuple.Item5;
			variable6 = tuple.Item6;
			variable7 = tuple.Item7;
			rest = tuple.Rest;
		}
	}
}