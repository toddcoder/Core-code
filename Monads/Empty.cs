using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Empty<T> : Optional<T>, IEquatable<Empty<T>>
{
   protected Lazy<int> hashCode;

   public Empty()
   {
      hashCode = new Lazy<int>(() => typeof(T).GetHashCode());
   }

   public override Exception Exception => throw fail("Empty has no Exception");

   public override Maybe<Exception> AnyException => nil;

   public override Optional<TResult> Map<TResult>(Func<T, Optional<TResult>> ifJust) => nil;

   public override Optional<TResult> Map<TResult>(Func<T, TResult> ifJust) => nil;

   public override Optional<TResult> Map<TResult>(Func<T, Optional<TResult>> ifJust, Func<Optional<TResult>> ifEmpty,
      Func<Exception, Optional<TResult>> ifFailed)
   {
      return ifEmpty();
   }

   public override Optional<T> OnJust(Action<T> action) => this;

   public override Optional<T> OnEmpty(Action action)
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

   public override Optional<T> OnFailed(Action<Exception> action) => this;

   public override Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> projection) => nil;

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Optional<T1>> func, Func<T, T1, T2> projection) => nil;

   public override Optional<TResult> SelectMany<TResult>(Func<T, TResult> func) => nil;

   public override Optional<TResult> Select<TResult>(Optional<T> result, Func<T, TResult> func) => nil;

   public override bool IfEmpty() => true;

   public override bool IfFailed(out Exception exception)
   {
      exception = fail("There is no exception");
      return false;
   }

   public override T Force() => throw fail("There is no value");

   public override T DefaultTo(Func<Maybe<Exception>, T> func) => func(nil);

   public override void Deconstruct(out bool isJust, out T value)
   {
      isJust = false;
      value = default;
   }

   public override Maybe<T> Maybe() => nil;

   public override Result<T> Result() => fail("There is no value");

   public override Completion<T> Completion() => nil;

   public override object ToObject() => nil;

   public override Optional<T> Initialize(Func<T> initializer) => initializer();

   public bool Equals(Empty<T> other) => true;

   public override bool Equals(object obj) => obj is Empty<T>;

   public override int GetHashCode() => hashCode.Value;

   public static bool operator ==(Empty<T> left, Empty<T> right) => Equals(left, right);

   public static bool operator !=(Empty<T> left, Empty<T> right) => !Equals(left, right);

   public override string ToString() => "Empty()";
}