using System;

namespace Core.Lambdas;

public static class LambdaFunctions
{
   public static Func<T> func<T>(Func<T> function) => function;

   public static Func<T, TResult> func<T, TResult>(Func<T, TResult> function) => function;

   public static Func<T1, T2, TResult> func<T1, T2, TResult>(Func<T1, T2, TResult> function) => function;

   public static Func<T1, T2, T3, TResult> func<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function) => function;

   public static Func<T1, T2, T3, T4, TResult> func<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function)
   {
      return function;
   }

   public static Func<T1, T2, T3, T4, T5, TResult> func<T1, T2, T3, T4, T5, TResult>
      (Func<T1, T2, T3, T4, T5, TResult> function)
   {
      return function;
   }

   public static Func<T1, T2, T3, T4, T5, T6, TResult> func<T1, T2, T3, T4, T5, T6, TResult>
      (Func<T1, T2, T3, T4, T5, T6, TResult> function)
   {
      return function;
   }

   public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> func<T1, T2, T3, T4, T5, T6, T7, TResult>
      (Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
   {
      return function;
   }

   public static Action action(Action action) => action;

   public static Action<T> action<T>(Action<T> action) => action;

   public static Action<T1, T2> action<T1, T2>(Action<T1, T2> action) => action;

   public static Action<T1, T2, T3> action<T1, T2, T3>(Action<T1, T2, T3> action) => action;

   public static Action<T1, T2, T3, T4> action<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action) => action;

   public static Action<T1, T2, T3, T4, T5> action<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action) => action;

   public static Action<T1, T2, T3, T4, T5, T6> action<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
   {
      return action;
   }

   public static Action<T1, T2, T3, T4, T5, T6, T7> action<T1, T2, T3, T4, T5, T6, T7>
      (Action<T1, T2, T3, T4, T5, T6, T7> action)
   {
      return action;
   }

   public static T Identity<T>(T value) => value;
}