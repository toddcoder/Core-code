using System;
using Standard.Types.Maybe;

namespace Standard.Types.Tuples
{
   public static class TupleExtensions
   {
      [Obsolete("Use TupleValue")]
      public static void Assign<T1, T2>(this Tuple<T1, T2> tuple, out T1 variable1, out T2 variable2)
      {
         variable1 = tuple.Item1;
         variable2 = tuple.Item2;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2>(this IMaybe<Tuple<T1, T2>> tuple, out T1 variable1, out T2 variable2)
      {
         if (tuple.IsSome)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
         }
         return tuple.IsSome;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2>(this IResult<Tuple<T1, T2>> tuple, out T1 variable1, out T2 variable2)
      {
         if (tuple.IsSuccessful)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
         }
         return tuple.IsSuccessful;
      }

      [Obsolete("Use TupleValue")]
      public static void Assign<T1, T2, T3>(this Tuple<T1, T2, T3> tuple, out T1 variable1, out T2 variable2,
         out T3 variable3)
      {
         variable1 = tuple.Item1;
         variable2 = tuple.Item2;
         variable3 = tuple.Item3;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3>(this IMaybe<Tuple<T1, T2, T3>> tuple, out T1 variable1, out T2 variable2,
         out T3 variable3)
      {
         if (tuple.IsSome)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
         }
         return tuple.IsSome;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3>(this IResult<Tuple<T1, T2, T3>> tuple, out T1 variable1, out T2 variable2,
         out T3 variable3)
      {
         if (tuple.IsSuccessful)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
         }
         return tuple.IsSuccessful;
      }

      [Obsolete("Use TupleValue")]
      public static void Assign<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple, out T1 variable1, out T2 variable2,
         out T3 variable3, out T4 variable4)
      {
         variable1 = tuple.Item1;
         variable2 = tuple.Item2;
         variable3 = tuple.Item3;
         variable4 = tuple.Item4;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3, T4>(this IMaybe<Tuple<T1, T2, T3, T4>> tuple, out T1 variable1,
         out T2 variable2, out T3 variable3, out T4 variable4)
      {
         if (tuple.IsSome)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
            variable4 = tuple.Value.Item4;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
            variable4 = default(T4);
         }
         return tuple.IsSome;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3, T4>(this IResult<Tuple<T1, T2, T3, T4>> tuple, out T1 variable1,
         out T2 variable2, out T3 variable3, out T4 variable4)
      {
         if (tuple.IsSuccessful)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
            variable4 = tuple.Value.Item4;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
            variable4 = default(T4);
         }
         return tuple.IsSuccessful;
      }

      [Obsolete("Use TupleValue")]
      public static void Assign<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple, out T1 variable1,
         out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5)
      {
         variable1 = tuple.Item1;
         variable2 = tuple.Item2;
         variable3 = tuple.Item3;
         variable4 = tuple.Item4;
         variable5 = tuple.Item5;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3, T4, T5>(this IMaybe<Tuple<T1, T2, T3, T4, T5>> tuple, out T1 variable1,
         out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5)
      {
         if (tuple.IsSome)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
            variable4 = tuple.Value.Item4;
            variable5 = tuple.Value.Item5;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
            variable4 = default(T4);
            variable5 = default(T5);
         }
         return tuple.IsSome;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3, T4, T5>(this IResult<Tuple<T1, T2, T3, T4, T5>> tuple, out T1 variable1,
         out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5)
      {
         if (tuple.IsSuccessful)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
            variable4 = tuple.Value.Item4;
            variable5 = tuple.Value.Item5;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
            variable4 = default(T4);
            variable5 = default(T5);
         }
         return tuple.IsSuccessful;
      }

      [Obsolete("Use TupleValue")]
      public static void Assign<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5, T6> tuple, out T1 variable1,
         out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6)
      {
         variable1 = tuple.Item1;
         variable2 = tuple.Item2;
         variable3 = tuple.Item3;
         variable4 = tuple.Item4;
         variable5 = tuple.Item5;
         variable6 = tuple.Item6;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3, T4, T5, T6>(this IMaybe<Tuple<T1, T2, T3, T4, T5, T6>> tuple,
         out T1 variable1, out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6)
      {
         if (tuple.IsSome)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
            variable4 = tuple.Value.Item4;
            variable5 = tuple.Value.Item5;
            variable6 = tuple.Value.Item6;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
            variable4 = default(T4);
            variable5 = default(T5);
            variable6 = default(T6);
         }
         return tuple.IsSome;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3, T4, T5, T6>(this IResult<Tuple<T1, T2, T3, T4, T5, T6>> tuple,
         out T1 variable1, out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6)
      {
         if (tuple.IsSuccessful)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
            variable4 = tuple.Value.Item4;
            variable5 = tuple.Value.Item5;
            variable6 = tuple.Value.Item6;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
            variable4 = default(T4);
            variable5 = default(T5);
            variable6 = default(T6);
         }
         return tuple.IsSuccessful;
      }

      [Obsolete("Use TupleValue")]
      public static void Assign<T1, T2, T3, T4, T5, T6, T7>(this Tuple<T1, T2, T3, T4, T5, T6, T7> tuple,
         out T1 variable1, out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6,
         out T7 variable7)
      {
         variable1 = tuple.Item1;
         variable2 = tuple.Item2;
         variable3 = tuple.Item3;
         variable4 = tuple.Item4;
         variable5 = tuple.Item5;
         variable6 = tuple.Item6;
         variable7 = tuple.Item7;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3, T4, T5, T6, T7>(this IMaybe<Tuple<T1, T2, T3, T4, T5, T6, T7>> tuple,
         out T1 variable1, out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6,
         out T7 variable7)
      {
         if (tuple.IsSome)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
            variable4 = tuple.Value.Item4;
            variable5 = tuple.Value.Item5;
            variable6 = tuple.Value.Item6;
            variable7 = tuple.Value.Item7;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
            variable4 = default(T4);
            variable5 = default(T5);
            variable6 = default(T6);
            variable7 = default(T7);
         }
         return tuple.IsSome;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3, T4, T5, T6, T7>(this IResult<Tuple<T1, T2, T3, T4, T5, T6, T7>> tuple,
         out T1 variable1, out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6,
         out T7 variable7)
      {
         if (tuple.IsSuccessful)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
            variable4 = tuple.Value.Item4;
            variable5 = tuple.Value.Item5;
            variable6 = tuple.Value.Item6;
            variable7 = tuple.Value.Item7;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
            variable4 = default(T4);
            variable5 = default(T5);
            variable6 = default(T6);
            variable7 = default(T7);
         }
         return tuple.IsSuccessful;
      }

      [Obsolete("Use TupleValue")]
      public static void Assign<T1, T2, T3, T4, T5, T6, T7, TRest>(this Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple,
         out T1 variable1, out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6,
         out T7 variable7, out TRest rest)
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

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3, T4, T5, T6, T7, TRest>(this IMaybe<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
         tuple, out T1 variable1, out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6,
         out T7 variable7, out TRest rest)
      {
         if (tuple.IsSome)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
            variable4 = tuple.Value.Item4;
            variable5 = tuple.Value.Item5;
            variable6 = tuple.Value.Item6;
            variable7 = tuple.Value.Item7;
            rest = tuple.Value.Rest;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
            variable4 = default(T4);
            variable5 = default(T5);
            variable6 = default(T6);
            variable7 = default(T7);
            rest = default(TRest);
         }
         return tuple.IsSome;
      }

      [Obsolete("Use TupleValue")]
      public static bool Assign<T1, T2, T3, T4, T5, T6, T7, TRest>(this IResult<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
         tuple, out T1 variable1, out T2 variable2, out T3 variable3, out T4 variable4, out T5 variable5, out T6 variable6,
         out T7 variable7, out TRest rest)
      {
         if (tuple.IsSuccessful)
         {
            variable1 = tuple.Value.Item1;
            variable2 = tuple.Value.Item2;
            variable3 = tuple.Value.Item3;
            variable4 = tuple.Value.Item4;
            variable5 = tuple.Value.Item5;
            variable6 = tuple.Value.Item6;
            variable7 = tuple.Value.Item7;
            rest = tuple.Value.Rest;
         }
         else
         {
            variable1 = default(T1);
            variable2 = default(T2);
            variable3 = default(T3);
            variable4 = default(T4);
            variable5 = default(T5);
            variable6 = default(T6);
            variable7 = default(T7);
            rest = default(TRest);
         }
         return tuple.IsSuccessful;
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T, TResult>(this Tuple<T> tuple, Func<T, TResult> mapping)
      {
         return mapping(tuple.Item1);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, TResult>(this Tuple<T1, T2> tuple, Func<T1, T2, TResult> mapping)
      {
         return mapping(tuple.Item1, tuple.Item2);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, TResult>(this Tuple<T1, T2, T3> tuple, Func<T1, T2, T3, TResult> mapping)
      {
         return mapping(tuple.Item1, tuple.Item2, tuple.Item3);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, T4, TResult>(this Tuple<T1, T2, T3, T4> tuple,
         Func<T1, T2, T3, T4, TResult> mapping)
      {
         return mapping(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, T4, T5, TResult>(this Tuple<T1, T2, T3, T4, T5> tuple,
         Func<T1, T2, T3, T4, T5, TResult> mapping)
      {
         return mapping(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, T4, T5, T6, TResult>(this Tuple<T1, T2, T3, T4, T5, T6> tuple,
         Func<T1, T2, T3, T4, T5, T6, TResult> mapping)
      {
         return mapping(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, T4, T5, T6, T7, TResult>(this Tuple<T1, T2, T3, T4, T5, T6, T7> tuple,
         Func<T1, T2, T3, T4, T5, T6, T7, TResult> mapping)
      {
         return mapping(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, T4, T5, T6, T7, TRest, TResult>(this Tuple<T1, T2, T3, T4, T5, T6, T7,
         TRest> tuple, Func<T1, T2, T3, T4, T5, T6, T7, TRest, TResult> mapping)
      {
         return mapping(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7,
            tuple.Rest);
      }

      [Obsolete("Use TupleValue")]
      public static IMaybe<TResult> Map<T, TResult>(this IMaybe<Tuple<T>> tuple, Func<T, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1));
      }

      [Obsolete("Use TupleValue")]
      public static IResult<TResult> Map<T, TResult>(this IResult<Tuple<T>> tuple, Func<T, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1));
      }

      [Obsolete("Use TupleValue")]
      public static IMaybe<TResult> Map<T1, T2, TResult>(this IMaybe<Tuple<T1, T2>> tuple, Func<T1, T2, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2));
      }

      [Obsolete("Use TupleValue")]
      public static IResult<TResult> Map<T1, T2, TResult>(this IResult<Tuple<T1, T2>> tuple, Func<T1, T2, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2));
      }

      [Obsolete("Use TupleValue")]
      public static IMaybe<TResult> Map<T1, T2, T3, TResult>(this IMaybe<Tuple<T1, T2, T3>> tuple,
         Func<T1, T2, T3, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3));
      }

      [Obsolete("Use TupleValue")]
      public static IResult<TResult> Map<T1, T2, T3, TResult>(this IResult<Tuple<T1, T2, T3>> tuple,
         Func<T1, T2, T3, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3));
      }

      [Obsolete("Use TupleValue")]
      public static IMaybe<TResult> Map<T1, T2, T3, T4, TResult>(this IMaybe<Tuple<T1, T2, T3, T4>> tuple,
         Func<T1, T2, T3, T4, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      [Obsolete("Use TupleValue")]
      public static IResult<TResult> Map<T1, T2, T3, T4, TResult>(this IResult<Tuple<T1, T2, T3, T4>> tuple,
         Func<T1, T2, T3, T4, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4));
      }

      [Obsolete("Use TupleValue")]
      public static IMaybe<TResult> Map<T1, T2, T3, T4, T5, TResult>(this IMaybe<Tuple<T1, T2, T3, T4, T5>> tuple,
         Func<T1, T2, T3, T4, T5, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));
      }

      [Obsolete("Use TupleValue")]
      public static IResult<TResult> Map<T1, T2, T3, T4, T5, TResult>(this IResult<Tuple<T1, T2, T3, T4, T5>> tuple,
         Func<T1, T2, T3, T4, T5, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));
      }

      [Obsolete("Use TupleValue")]
      public static IMaybe<TResult> Map<T1, T2, T3, T4, T5, T6, TResult>(this IMaybe<Tuple<T1, T2, T3, T4, T5, T6>> tuple,
         Func<T1, T2, T3, T4, T5, T6, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6));
      }

      [Obsolete("Use TupleValue")]
      public static IResult<TResult> Map<T1, T2, T3, T4, T5, T6, TResult>(this IResult<Tuple<T1, T2, T3, T4, T5, T6>> tuple,
         Func<T1, T2, T3, T4, T5, T6, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6));
      }

      [Obsolete("Use TupleValue")]
      public static IMaybe<TResult> Map<T1, T2, T3, T4, T5, T6, T7, TResult>(this IMaybe<Tuple<T1, T2, T3, T4, T5, T6,
         T7>> tuple, Func<T1, T2, T3, T4, T5, T6, T7, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7));
      }

      [Obsolete("Use TupleValue")]
      public static IResult<TResult> Map<T1, T2, T3, T4, T5, T6, T7, TResult>(this IResult<Tuple<T1, T2, T3, T4, T5, T6,
         T7>> tuple, Func<T1, T2, T3, T4, T5, T6, T7, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7));
      }

      [Obsolete("Use TupleValue")]
      public static IMaybe<TResult> Map<T1, T2, T3, T4, T5, T6, T7, TRest, TResult>(this IMaybe<Tuple<T1, T2, T3, T4, T5,
         T6, T7, TRest>> tuple, Func<T1, T2, T3, T4, T5, T6, T7, TRest, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Rest));
      }

      [Obsolete("Use TupleValue")]
      public static IResult<TResult> Map<T1, T2, T3, T4, T5, T6, T7, TRest, TResult>(this IResult<Tuple<T1, T2, T3, T4, T5,
         T6, T7, TRest>> tuple, Func<T1, T2, T3, T4, T5, T6, T7, TRest, TResult> mapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Rest));
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T, TResult>(this IMaybe<Tuple<T>> tuple, Func<T, TResult> mapping,
         Func<TResult> elseMapping)
      {
         return tuple.Map(t => mapping(t.Item1), elseMapping);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, TResult>(this IMaybe<Tuple<T1, T2>> tuple, Func<T1, T2, TResult> mapping,
         Func<TResult> elseMapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2), elseMapping);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, TResult>(this IMaybe<Tuple<T1, T2, T3>> tuple,
         Func<T1, T2, T3, TResult> mapping, Func<TResult> elseMapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3), elseMapping);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, T4, TResult>(this IMaybe<Tuple<T1, T2, T3, T4>> tuple,
         Func<T1, T2, T3, T4, TResult> mapping, Func<TResult> elseMapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4), elseMapping);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, T4, T5, TResult>(this IMaybe<Tuple<T1, T2, T3, T4, T5>> tuple,
         Func<T1, T2, T3, T4, T5, TResult> mapping, Func<TResult> elseMapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5), elseMapping);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, T4, T5, T6, TResult>(this IMaybe<Tuple<T1, T2, T3, T4, T5, T6>> tuple,
         Func<T1, T2, T3, T4, T5, T6, TResult> mapping, Func<TResult> elseMapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6), elseMapping);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, T4, T5, T6, T7, TResult>(this IMaybe<Tuple<T1, T2, T3, T4, T5, T6,
         T7>> tuple, Func<T1, T2, T3, T4, T5, T6, T7, TResult> mapping, Func<TResult> elseMapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7), elseMapping);
      }

      [Obsolete("Use TupleValue")]
      public static TResult Map<T1, T2, T3, T4, T5, T6, T7, TRest, TResult>(this IMaybe<Tuple<T1, T2, T3, T4, T5,
         T6, T7, TRest>> tuple, Func<T1, T2, T3, T4, T5, T6, T7, TRest, TResult> mapping, Func<TResult> elseMapping)
      {
         return tuple.Map(t => mapping(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Rest), elseMapping);
      }
   }
}