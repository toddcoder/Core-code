using System;
using Core.Applications;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Cancelled<T> : Completion<T>, IEquatable<Cancelled<T>>
{
   public static implicit operator bool(Cancelled<T> _) => false;

   internal Cancelled()
   {
   }

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted) => nil;

   public override Completion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => nil;

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled)
   {
      return ifCancelled();
   }

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted,
      Func<Exception, Completion<TResult>> ifInterrupted)
   {
      return nil;
   }

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled,
      Func<Exception, Completion<TResult>> ifInterrupted)
   {
      return ifCancelled();
   }

   public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted)
   {
      return ifCancelled();
   }

   public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifNotCompleted();

   public override Completion<T> Map(Action<T> action) => this;

   public override Completion<T> UnMap(Action action)
   {
      action();
      return this;
   }

   public override Completion<T> UnMap(Action<Exception> action) => this;

   public override Completion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
   {
      ifNotCompleted();
      return this;
   }

   public override Completion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
   {
      ifCancelled();
      return this;
   }

   public override Completion<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection) => nil;

   public override Completion<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection) => nil;

   public override Completion<TResult> SelectMany<TResult>(Func<T, TResult> func) => nil;

   public override Completion<TResult> Select<TResult>(Completion<T> result, Func<T, TResult> func) => nil;

   public override bool IfCancelled() => true;

   public override Completion<TOther> NotCompleted<TOther>() => nil;

   public override void Force()
   {
   }

   public override T ForceValue() => throw fail("There is no value");

   public override Completion<T> CancelledOnly() => this;

   public override Completion<TOther> CancelledOnly<TOther>() => nil;

   public override Completion<TOther> NotCompletedOnly<TOther>() => nil;

   public override void Deconstruct(out bool isCompleted, out T value)
   {
      isCompleted = false;
      value = default;
   }

   public override Completion<T> OnCompleted(Action<T> action) => this;

   public override Completion<T> OnCancelled(Action action)
   {
      action();
      return this;
   }

   public override Completion<T> OnInterrupted(Action<Exception> action) => this;

   public override bool ValueEqualTo(Completion<T> otherCompletion) => false;

   public override bool EqualToValueOf(T otherValue) => false;

   public Completion<object> AsObject() => nil;

   public override Completion<TResult> CastAs<TResult>() => nil;

   public override Completion<T> Where(Predicate<T> predicate) => this;

   public override Completion<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

   public override Completion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

   public override T DefaultTo(Func<Maybe<Exception>, T> defaultFunc) => defaultFunc(nil);

   public override Maybe<T> Maybe() => nil;

   public override Result<T> Result() => new Failure<T>(new CancelException());

   public override Optional<T> Optional() => nil;

   public override Exception Exception => throw fail("Cancelled has no Exception");

   public override Maybe<Exception> AnyException => nil;

   public override object ToObject() => nil;

   public override Completion<T> Initialize(Func<T> initializer) => initializer();

   public bool Equals(Cancelled<T> other) => true;

   public override bool Equals(object obj) => obj is Cancelled<T> other && Equals(other);

   public override int GetHashCode() => false.GetHashCode();

   public override string ToString() => $"cancelled<{typeof(T).Name}>";
}