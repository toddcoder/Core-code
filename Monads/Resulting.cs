using System;
using static Core.Lambdas.LambdaFunctions;
using static Core.Maybe.AttemptFunctions;

namespace Core.Maybe
{
   [Obsolete("Use switch")]
   public class Resulting<T>
   {
      IResult<Func<T>> action;

      public Resulting() => action = "No case matched".Failure<Func<T>>();

      public IResult<Resulting<T>> When(bool test, Func<T> whenTrue)
      {
         if (action.IsFailed && test)
            action = whenTrue.Success();
         return this.Success();
      }

      public IResult<Resulting<T>> When<TMatch>(object value, Func<TMatch, T> whenTrue)
      {
         if (action.IsFailed && value is TMatch test)
         {
            var t = test;
            action = func(() => whenTrue(t)).Success();
         }
         return this.Success();
      }

      public IResult<Resulting<T>> Otherwise(Func<T> otherwise)
      {
         if (action.IsFailed)
            action = otherwise.Success();

         return this.Success();
      }

      public IResult<T> WhenFailed(Func<string> message)
      {
         if (action.IsFailed)
            return message().Failure<T>();

         return Result();
      }

      public IResult<T> Result() =>
         from validAction in action
         from result in tryTo(validAction)
         select result;
   }

   public class ResultingIn<T>
   {
      IResult<Func<IResult<T>>> action;

      public ResultingIn() => action = "No case matched".Failure<Func<IResult<T>>>();

      public IResult<ResultingIn<T>> WhenIsA(bool test, Func<IResult<T>> whenTrue)
      {
         if (action.IsFailed && test)
            action = whenTrue.Success();
         return this.Success();
      }

      public IResult<ResultingIn<T>> WhenIsA<TMatch>(object value, Func<TMatch, IResult<T>> whenTrue)
      {
         if (action.IsFailed && value is TMatch test)
         {
            var t = test;
            action = func(() => whenTrue(t)).Success();
         }
         return this.Success();
      }

      public IResult<ResultingIn<T>> Otherwise(Func<IResult<T>> otherwise)
      {
         if (action.IsFailed)
            action = otherwise.Success();

         return this.Success();
      }

      public IResult<T> WhenFailed(Func<string> message)
      {
         if (action.IsFailed)
            return message().Failure<T>();

         return Result();
      }

      public IResult<T> Result() =>
         from validAction in action
         from result in tryTo(() => validAction().Value)
         select result;
   }
}