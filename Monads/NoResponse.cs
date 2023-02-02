using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class NoResponse<T> : Responding<T>, IEquatable<NoResponse<T>>
{
   protected Lazy<int> hashCode;

   public NoResponse()
   {
      hashCode = new Lazy<int>(() => typeof(T).GetHashCode());
   }

   [Obsolete("Use deconstruction")]
   public override T Value => throw fail("NoResponse has no value");

   public override Exception Exception => throw fail("NoResponse has no Exception");

   public override Maybe<Exception> AnyException => nil;

   public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse) => nil;

   public override Responding<TResult> Map<TResult>(Func<T, TResult> ifResponse) => nil;

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

   public override Responding<TResult> SelectMany<TResult>(Func<T, Responding<TResult>> projection) => nil;

   public override Responding<T2> SelectMany<T1, T2>(Func<T, Responding<T1>> func, Func<T, T1, T2> projection) => nil;

   public override Responding<TResult> SelectMany<TResult>(Func<T, TResult> func) => nil;

   public override Responding<TResult> Select<TResult>(Responding<T> result, Func<T, TResult> func) => nil;

   public override bool IfNoResponse() => true;

   public override bool IfFailedResponse(out Exception exception)
   {
      exception = fail("There is no exception");
      return false;
   }

   public override T Force() => throw fail("There is no value");

   public override T DefaultTo(Func<Maybe<Exception>, T> func) => func(nil);

   public override void Deconstruct(out bool isResponding, out T value)
   {
      isResponding = false;
      value = default;
   }

   public override Maybe<T> Maybe() => nil;

   public override Result<T> Result() => fail("There is no value");

   public override Completion<T> Completion() => nil;

   public bool Equals(NoResponse<T> other) => true;

   public override bool Equals(object obj) => obj is NoResponse<T>;

   public override int GetHashCode() => hashCode.Value;

   public static bool operator ==(NoResponse<T> left, NoResponse<T> right) => Equals(left, right);

   public static bool operator !=(NoResponse<T> left, NoResponse<T> right) => !Equals(left, right);

   public override string ToString() => "NoResponse()";
}