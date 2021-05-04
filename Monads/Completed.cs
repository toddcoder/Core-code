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

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted) => ifCompleted(value);

      public ICompletion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => ifCompleted(value).Completed();

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled)
      {
         return ifCompleted(value);
      }

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifCompleted(value);
      }

      public ICompletion<TResult> Map<TResult>(Func<T, ICompletion<TResult>> ifCompleted, Func<ICompletion<TResult>> ifCancelled,
         Func<Exception, ICompletion<TResult>> ifInterrupted)
      {
         return ifCompleted(value);
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted)
      {
         return ifCompleted(value);
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifCompleted(value);

      public ICompletion<T> If(Action<T> action)
      {
         action(value);
         return this;
      }

      public ICompletion<T> Else(Action action) => this;

      public ICompletion<T> Else(Action<Exception> action) => this;

      public ICompletion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
      {
         ifCompleted(value);
         return this;
      }

      public ICompletion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
      {
         ifCompleted(value);
         return this;
      }

      public ICompletion<TOther> InterruptedAs<TOther>() => throw "There is no exception".Throws();

      public ICompletion<T> Or(ICompletion<T> other) => this;

      public ICompletion<T> Or(Func<ICompletion<T>> other) => this;

      public ICompletion<TResult> SelectMany<TResult>(Func<T, ICompletion<TResult>> projection) => projection(value);

      public ICompletion<T2> SelectMany<T1, T2>(Func<T, ICompletion<T1>> func, Func<T, T1, T2> projection)
      {
         return func(value).Map(t1 => projection(value, t1).Completed(), cancelled<T2>, interrupted<T2>);
      }

      public ICompletion<TResult> SelectMany<TResult>(Func<T, TResult> func) => func(value).Completed();

      public ICompletion<TResult> Select<TResult>(ICompletion<T> result, Func<T, TResult> func) => func(value).Completed();

      public bool If(out T value)
      {
         value = this.value;
         return true;
      }

      public bool IfCancelled() => false;

      public bool IfInterrupted(out Exception exception)
      {
         exception = default;
         return false;
      }

      public bool If(out T value, out IMaybe<Exception> _exception)
      {
         value = this.value;
         _exception = none<Exception>();

         return true;
      }

      public bool IfNot(out IMaybe<Exception> _exception)
      {
         _exception = none<Exception>();
         return false;
      }

      public bool Else<TOther>(out ICompletion<TOther> result)
      {
         result = default;
         return false;
      }

      public ICompletion<TOther> NotCompleted<TOther>() => cancelled<TOther>();

      public bool IsCompleted(out ICompletion<T> completed)
      {
         completed = this;
         return true;
      }

      public bool NotCompleted(out ICompletion<T> notCompleted)
      {
         notCompleted = this;
         return false;
      }

      public void Force()
      {
      }

      public T ForceValue() => value;

      public ICompletion<T> CancelledOnly() => cancelled<T>();

      public ICompletion<TOther> CancelledOnly<TOther>() => cancelled<TOther>();

      public ICompletion<TOther> NotCompletedOnly<TOther>() => cancelled<TOther>();

      public void Deconstruct(out IMaybe<T> value, out IMaybe<Exception> _exception)
      {
         value = this.value.Some();
         _exception = none<Exception>();
      }

      public ICompletion<T> OnCompleted(Action<T> action)
      {
         action(value);
         return this;
      }

      public ICompletion<T> OnCancelled(Action action) => this;

      public ICompletion<T> OnInterrupted(Action<Exception> action) => this;

      public bool ValueOrOriginal(out T value, out ICompletion<T> original)
      {
         value = this.value;
         original = this;

         return true;
      }

      public bool ValueOrCast<TCompletion>(out T value, out ICompletion<TCompletion> completion)
      {
         value = this.value;
         completion = "Do not use this".Interrupted<TCompletion>();

         return true;
      }

      public bool ValueEqualTo(ICompletion<T> otherCompletion) => otherCompletion.If(out var otherValue) && EqualToValueOf(otherValue);

      public bool EqualToValueOf(T otherValue) => value.Equals(otherValue);

      public ICompletion<object> AsObject() => value.Completed<object>();

      public ICompletion<TResult> CastAs<TResult>()
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

      public ICompletion<T> Where(Predicate<T> predicate) => predicate(value) ? this : cancelled<T>();

      public ICompletion<T> Where(Predicate<T> predicate, string exceptionMessage) => predicate(value) ? this : exceptionMessage.Interrupted<T>();

      public ICompletion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage)
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