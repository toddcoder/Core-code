using System;
using static Core.Maybe.MaybeFunctions;

namespace Core.Maybe
{
   public static class Maybe<T>
   {
      public static IMaybe<T> DefaultTo(bool isSome, Func<T> func,
         T defaultValue) => isSome ? new Some<T>(func()) : new Some<T>(defaultValue);
   }

   public static class Maybe
   {
      public static IMaybe<Func<T>> Some<T>(Func<T> func) => new Some<Func<T>>(func);

      public static IMaybe<Func<T1, T2>> Some<T1, T2>(Func<T1, T2> func) => new Some<Func<T1, T2>>(func);

      public static IMaybe<Func<T1, T2, T3>> Some<T1, T2, T3>(Func<T1, T2, T3> func) => new Some<Func<T1, T2, T3>>(func);

      public static IMaybe<Func<T1, T2, T3, T4>> Some<T1, T2, T3, T4>(Func<T1, T2, T3, T4> func) => new Some<Func<T1, T2, T3, T4>>(func);

      public static IMaybe<Func<T1, T2, T3, T4, T5>> Some<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> func) => new Some<Func<T1, T2, T3, T4, T5>>(func);

      public static IMaybe<Func<T1, T2, T3, T4, T5, T6>> Some<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> func) => new Some<Func<T1, T2, T3, T4, T5, T6>>(func);

      public static IMaybe<T> when<T>(bool test, Func<T> ifTrue) => test ? ifTrue().Some() : none<T>();

      public static IMaybe<T> when<T>(bool test, Func<IMaybe<T>> ifTrue) => test ? ifTrue() : none<T>();
   }
}