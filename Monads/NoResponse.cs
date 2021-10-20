using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class NoResponse<T> : Responding<T>, IEquatable<NoResponse<T>>
   {
      protected Lazy<int> hashCode;

      public NoResponse()
      {
         hashCode = new Lazy<int>(() => typeof(T).GetHashCode());
      }

      public override bool IsResponse => false;

      public override bool IsNoResponse => true;

      public override bool IsFailedResponse => false;

      public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse) => new NoResponse<TResult>();

      public override Responding<TResult> Map<TResult>(Func<T, TResult> ifResponse) => new NoResponse<TResult>();

      public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse, Func<Responding<TResult>> ifNoResponse,
         Func<Exception, Responding<TResult>> ifFailedResponse)
      {
         return ifNoResponse();
      }

      public override Responding<T> OnResponse(Action<T> action) => this;

      public override Responding<T> OnNoResponse(Action action)
      {
         try
         {
            action();
            return this;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public override Responding<T> OnFailedResponse(Action<Exception> action) => this;

      public override Responding<TResult> SelectMany<TResult>(Func<T, Responding<TResult>> projection) => new NoResponse<TResult>();

      public override Responding<T2> SelectMany<T1, T2>(Func<T, Responding<T1>> func, Func<T, T1, T2> projection) => new NoResponse<T2>();

      public override Responding<TResult> SelectMany<TResult>(Func<T, TResult> func) => new NoResponse<TResult>();

      public override Responding<TResult> Select<TResult>(Responding<T> result, Func<T, TResult> func) => new NoResponse<TResult>();

      public override bool If(out T value)
      {
         value = default;
         return false;
      }

      public override bool IfNoResponse() => true;

      public override bool IfFailedResponse(out Exception exception)
      {
         exception = fail("There is no exception");
         return false;
      }

      public override bool If(out T value, out Maybe<Exception> _exception)
      {
         value = default;
         _exception = Maybe<Exception>.nil;

         return false;
      }

      public override T Force() => throw fail("There is no value");

      public override T DefaultTo(Func<Maybe<Exception>, T> func) => func(Maybe<Exception>.nil);

      public override void Deconstruction(out T value, out Maybe<Exception> _exception)
      {
         value = default;
         _exception = Maybe<Exception>.nil;
      }

      public override Maybe<T> Maybe() => new None<T>();

      public override Result<T> Result() => new Failure<T>(fail("There is no value"));

      public override Matched<T> Matched() => new NoMatch<T>();

      public override Completion<T> Completion() => new Cancelled<T>();

      public bool Equals(NoResponse<T> other) => true;

      public override bool Equals(object obj) => obj is NoResponse<T>;

      public override int GetHashCode() => hashCode.Value;

      public static bool operator ==(NoResponse<T> left, NoResponse<T> right) => Equals(left, right);

      public static bool operator !=(NoResponse<T> left, NoResponse<T> right) => !Equals(left, right);
   }
}