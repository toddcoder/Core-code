using System;
using System.Collections.Generic;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Completed<T> : ICompletion<T>, IEquatable<Completed<T>>
   {
      public static implicit operator bool(Completed<T> _) => true;

      protected T value;

      internal Completed(T value) => this.value = value;

      public T Value => value;

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted) => ifCompleted(value);

      public override ICompletion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => ifCompleted(value).Completed();

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled)
      {
         return ifCompleted(value);
      }

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifCompleted(value);
      }

      public override ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifCompleted(value);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted)
      {
         return ifCompleted(value);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifCompleted(value);

      public override ICompletion<T> If(Action<T> action)
      {
         action(value);
         return this;
      }

      public override ICompletion<T> Else(Action action) => this;

      public override ICompletion<T> Else(Action<Exception> action) => this;

      public override ICompletion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
      {
         ifCompleted(value);
         return this;
      }

      public override ICompletion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
      {
         ifCompleted(value);
         return this;
      }

      public override ICompletion<TOther> InterruptedAs<TOther>() => throw "There is no exception".Throws();

      public override ICompletion<T> Or(ICompletion<T> other) => this;

      public override ICompletion<T> Or(Func<ICompletion<T>> other) => this;

      public override ICompletion<TResult> SelectMany<TResult>(Func<T, ICompletion<TResult>> projection) => projection(value);

      public override ICompletion<T2> SelectMany<T1, T2>(Func<T, ICompletion<T1>> func, Func<T, T1, T2> projection)
      {
         return func(value).Map(t1 => projection(value, t1).Completed(), cancelled<T2>, interrupted<T2>);
      }

      public override ICompletion<TResult> SelectMany<TResult>(Func<T, TResult> func) => func(value).Completed();

      public override ICompletion<TResult> Select<TResult>(ICompletion<T> result, Func<T, TResult> func) => func(value).Completed();

      public override bool If(out T value)
      {
         value = this.value;
         return true;
      }

      public override bool IfCancelled() => false;

      public override bool IfInterrupted(out Exception exception)
      {
         exception = default;
         return false;
      }

      public override bool If(out T value, out Maybe<Exception> _exception)
      {
         value = this.value;
         _exception = none<Exception>();

         return true;
      }

      public override bool IfNot(out Maybe<Exception> _exception)
      {
         _exception = none<Exception>();
         return false;
      }

      public override bool Else<TOther>(out ICompletion<TOther> result)
      {
         result = default;
         return false;
      }

      public  override ICompletion<TOther> NotCompleted<TOther>() => cancelled<TOther>();

      public override  bool IsCompleted(out ICompletion<T> completed)
      {
         completed = this;
         return true;
      }

      public  override bool NotCompleted(out ICompletion<T> notCompleted)
      {
         notCompleted = this;
         return false;
      }

      public override  void Force()
      {
      }

      public  override T ForceValue() => value;

      public override  ICompletion<T> CancelledOnly() => cancelled<T>();

      public override ICompletion<TOther> CancelledOnly<TOther>() => cancelled<TOther>();

      public override ICompletion<TOther> NotCompletedOnly<TOther>() => cancelled<TOther>();

      public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> _exception)
      {
         value = this.value.Some();
         _exception = none<Exception>();
      }

      public override ICompletion<T> OnCompleted(Action<T> action)
      {
         action(value);
         return this;
      }

      public override ICompletion<T> OnCancelled(Action action) => this;

      public override ICompletion<T> OnInterrupted(Action<Exception> action) => this;

      public override bool ValueOrOriginal(out T value, out ICompletion<T> original)
      {
         value = this.value;
         original = this;

         return true;
      }

      public override bool ValueOrCast<TCompletion>(out T value, out ICompletion<TCompletion> completion)
      {
         value = this.value;
         completion = "Do not use this".Interrupted<TCompletion>();

         return true;
      }

      public override bool ValueEqualTo(ICompletion<T> otherCompletion) => otherCompletion.If(out var otherValue) && EqualToValueOf(otherValue);

      public override bool EqualToValueOf(T otherValue) => value.Equals(otherValue);

      public ICompletion<object> AsObject() => value.Completed<object>();

      public override ICompletion<TResult> CastAs<TResult>()
      {
         if (value is TResult result)
         {
            return result.Completed();
         }
         else
         {
            return $"Invalid cast from {typeof(T).Name} to {typeof(TResult).Name}".Interrupted<TResult>();
         }
      }

      public override ICompletion<T> Where(Predicate<T> predicate) => predicate(value) ? this : cancelled<T>();

      public override ICompletion<T> Where(Predicate<T> predicate, string exceptionMessage) => predicate(value) ? this : exceptionMessage.Interrupted<T>();

      public override ICompletion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage)
      {
         return predicate(value) ? this : exceptionMessage().Interrupted<T>();
      }

      public bool Equals(Completed<T> other)
      {
         return other is not null && (ReferenceEquals(this, other) || EqualityComparer<T>.Default.Equals(value, other.value));
      }

      public override bool Equals(object obj) => obj is Completed<T> other && Equals(other);

      public override int GetHashCode() => value.GetHashCode();

      public override string ToString() => $"Completed({value})";
   }
}