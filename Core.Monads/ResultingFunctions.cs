using System;

namespace Standard.Types.Maybe
{
   [Obsolete("Use switch")]
   public static class ResultingFunctions
   {
      public static IResult<Resulting<T>> When<T>(bool test, Func<T> whenTrue) => new Resulting<T>().When(test, whenTrue);

      public static IResult<Resulting<T>> When<TMatch, T>(object value, Func<TMatch, T> whenTrue) => new ResultingContext<TMatch>(value).When(whenTrue);

      public static IResult<ResultingIn<T>> WhenIsA<T>(bool test, Func<IResult<T>> whenTrue) =>
         new ResultingIn<T>().WhenIsA(test, whenTrue);

      public static IResult<ResultingIn<T>> WhenIsA<TMatch, T>(object value, Func<TMatch, IResult<T>> whenTrue) =>
         new ResultingInContext<TMatch>(value).WhenIsA(whenTrue);
   }

   [Obsolete("Use switch")]
   public class ResultingContext<TMatch>
   {
      object value;

      public ResultingContext(object value) => this.value = value;

      public IResult<Resulting<T>> When<T>(Func<TMatch, T> whenTrue) => new Resulting<T>().When(value, whenTrue);
   }

   [Obsolete("Use switch")]
   public class ResultingInContext<TMatch>
   {
      object value;

      public ResultingInContext(object value) => this.value = value;

      public IResult<ResultingIn<T>> WhenIsA<T>(Func<TMatch, IResult<T>> whenTrue) => new ResultingIn<T>().WhenIsA(value, whenTrue);
   }
}