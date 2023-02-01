using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Response<T> : Responding<T>, IEquatable<Response<T>>
{
   protected T value;

   internal Response(T value)
   {
      this.value = value;
   }

   public override T Value => value;

   public override Exception Exception => throw fail("Response has no Exception");

   public override Maybe<Exception> AnyException => nil;

   public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse)
   {
      try
      {
         return ifResponse(value);
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
         return ifResponse(value);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse, Func<Responding<TResult>> ifNoResponse,
      Func<Exception, Responding<TResult>> ifFailedResponse)
   {
      return ifResponse(value);
   }

   public override Responding<T> OnResponse(Action<T> action)
   {
      action(value);
      return this;
   }

   public override Responding<T> OnNoResponse(Action action) => this;

   public override Responding<T> OnFailedResponse(Action<Exception> action) => this;

   public override Responding<TResult> SelectMany<TResult>(Func<T, Responding<TResult>> projection) => projection(value);

   public override Responding<T2> SelectMany<T1, T2>(Func<T, Responding<T1>> func, Func<T, T1, T2> projection)
   {
      return func(value).Map(t1 => projection(value, t1).Response(), () => nil, e => e);
   }

   public override Responding<TResult> SelectMany<TResult>(Func<T, TResult> func) => func(value);

   public override Responding<TResult> Select<TResult>(Responding<T> result, Func<T, TResult> func) => func(value);

   public override bool IfNoResponse() => false;

   public override bool IfFailedResponse(out Exception exception)
   {
      exception = fail("There is no exception");
      return false;
   }

   public override T Force() => value;

   public override T DefaultTo(Func<Maybe<Exception>, T> func) => value;

   public override void Deconstruct(out bool isResponding, out T value)
   {
      isResponding = true;
      value = this.value;
   }

   public override Maybe<T> Maybe() => value;

   public override Result<T> Result() => value;

   public override Completion<T> Completion() => value;

   public bool Equals(Response<T> other) => value.Equals(other.value);

   public override bool Equals(object obj) => obj is Response<T> other && Equals(other);

   public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(value);

   public static bool operator ==(Response<T> left, Response<T> right) => Equals(left, right);

   public static bool operator !=(Response<T> left, Response<T> right) => !Equals(left, right);

   public override string ToString() => value.ToString();
}