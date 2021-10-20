using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Response<T> : Responding<T>, IEquatable<Response<T>>
   {
      internal Response(T value)
      {
         Value = value;
      }

      public T Value { get; }

      public override bool IsResponse => true;

      public override bool IsNoResponse => false;

      public override bool IsFailedResponse => false;

      public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse)
      {
         try
         {
            return ifResponse(Value);
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public override Responding<TResult> Map<TResult>(Func<T, TResult> ifResponse)
      {
         try
         {
            return ifResponse(Value);
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse, Func<Responding<TResult>> ifNoResponse,
         Func<Exception, Responding<TResult>> ifFailedResponse)
      {
         return ifResponse(Value);
      }

      public override Responding<T> OnResponse(Action<T> action)
      {
         action(Value);
         return this;
      }

      public override Responding<T> OnNoResponse(Action action) => this;

      public override Responding<T> OnFailedResponse(Action<Exception> action) => this;

      public override Responding<TResult> SelectMany<TResult>(Func<T, Responding<TResult>> projection) => projection(Value);

      public override Responding<T2> SelectMany<T1, T2>(Func<T, Responding<T1>> func, Func<T, T1, T2> projection)
      {
         return func(Value).Map(t1 => projection(Value, t1).Response(), noResponse<T2>, failedResponse<T2>);
      }

      public override Responding<TResult> SelectMany<TResult>(Func<T, TResult> func) => func(Value).Response();

      public override Responding<TResult> Select<TResult>(Responding<T> result, Func<T, TResult> func) => func(Value).Response();

      public override bool If(out T value)
      {
         value = Value;
         return true;
      }

      public override bool IfNoResponse() => false;

      public override bool IfFailedResponse(out Exception exception)
      {
         exception = fail("There is no exception");
         return false;
      }

      public override bool If(out T value, out Maybe<Exception> _exception)
      {
         value = Value;
         _exception = Maybe<Exception>.nil;

         return true;
      }

      public override T Force() => Value;

      public override T DefaultTo(Func<Maybe<Exception>, T> func) => Value;

      public override void Deconstruction(out T value, out Maybe<Exception> _exception)
      {
         value = Value;
         _exception = Maybe<Exception>.nil;
      }

      public override Maybe<T> Maybe() => Value;

      public override Result<T> Result() => Value;

      public override Matched<T> Matched() => Value;

      public override Completion<T> Completion() => Value;

      public bool Equals(Response<T> other) => Value.Equals(other.Value);

      public override bool Equals(object obj) => obj is Response<T> other && Equals(other);

      public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Value);

      public static bool operator ==(Response<T> left, Response<T> right) => Equals(left, right);

      public static bool operator !=(Response<T> left, Response<T> right) => !Equals(left, right);
   }
}