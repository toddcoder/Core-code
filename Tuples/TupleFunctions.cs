using System;

namespace Standard.Types.Tuples
{
   public static class TupleFunctions
   {
      [Obsolete("Use TupleValue")]
      public static Tuple<T> tuple<T>(T item) => Tuple.Create(item);

      [Obsolete("Use TupleValue")]
      public static Tuple<T1, T2> tuple<T1, T2>(T1 item1, T2 item2) => Tuple.Create(item1, item2);

      [Obsolete("Use TupleValue")]
      public static Tuple<T1, T2, T3> tuple<T1, T2, T3>(T1 item1, T2 item2, T3 item3) => Tuple.Create(item1, item2, item3);

      [Obsolete("Use TupleValue")]
      public static Tuple<T1, T2, T3, T4> tuple<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
      {
         return Tuple.Create(item1, item2, item3, item4);
      }

      [Obsolete("Use TupleValue")]
      public static Tuple<T1, T2, T3, T4, T5> tuple<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
      {
         return Tuple.Create(item1, item2, item3, item4, item5);
      }

      [Obsolete("Use TupleValue")]
      public static Tuple<T1, T2, T3, T4, T5, T6> tuple<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4,
         T5 item5, T6 item6)
      {
         return Tuple.Create(item1, item2, item3, item4, item5, item6);
      }

      [Obsolete("Use TupleValue")]
      public static Tuple<T1, T2, T3, T4, T5, T6, T7> tuple<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3,
         T4 item4, T5 item5, T6 item6, T7 item7)
      {
         return Tuple.Create(item1, item2, item3, item4, item5, item6, item7);
      }
   }
}