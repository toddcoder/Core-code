using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Completed<T> : Completion<T>, IEquatable<Completed<T>>
   {
      public static implicit operator bool(Completed<T> _) => true;

      protected T value;

      internal Completed(T value) => this.value = value;

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted) => ifCompleted(value);

      public override Completion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => ifCompleted(value).Completed();

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled)
      {
         return ifCompleted(value);
      }

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted,
         Func<Exception, Completion<TResult>> ifInterrupted)
      {
         return ifCompleted(value);
      }

      public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled,
         Func<Exception, Completion<TResult>> ifInterrupted)
      {
         return ifCompleted(value);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted)
      {
         return ifCompleted(value);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifCompleted(value);

      public override Completion<T> Map(Action<T> action)
      {
         action(value);
         return this;
      }

      public override Completion<T> UnMap(Action action) => this;

      public override Completion<T> UnMap(Action<Exception> action) => this;

      public override Completion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
      {
         ifCompleted(value);
         return this;
      }

      public override Completion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
      {
         ifCompleted(value);
         return this;
      }

     public override Completion<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection) => projection(value);

      public override Completion<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection)
      {
#pragma warning disable CS0618
         return func(value).Map(t1 => projection(value, t1).Completed(), cancelled<T2>, interrupted<T2>);
#pragma warning restore CS0618
      }

      public override Completion<TResult> SelectMany<TResult>(Func<T, TResult> func) => func(value).Completed();

      public override Completion<TResult> Select<TResult>(Completion<T> result, Func<T, TResult> func) => func(value).Completed();

      public override bool Map(out T value)
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

      public override bool Map(out T value, out Maybe<Exception> _exception)
      {
         value = this.value;
         _exception = nil;

         return true;
      }

      public override bool UnMap(out Maybe<Exception> _exception)
      {
         _exception = nil;
         return false;
      }

      public override bool UnMap<TOther>(out Completion<TOther> result)
      {
         result = default;
         return false;
      }

      public override Completion<TOther> NotCompleted<TOther>() => nil;

      public override  bool IsCompleted(out Completion<T> completed)
      {
         completed = this;
         return true;
      }

      public  override bool NotCompleted(out Completion<T> notCompleted)
      {
         notCompleted = this;
         return false;
      }

      public override  void Force()
      {
      }

      public  override T ForceValue() => value;

      public override Completion<T> CancelledOnly() => nil;

      public override Completion<TOther> CancelledOnly<TOther>() => nil;

      public override Completion<TOther> NotCompletedOnly<TOther>() => nil;

      public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> _exception)
      {
         value = this.value.Some();
         _exception = nil;
      }

      public override Completion<T> OnCompleted(Action<T> action)
      {
         action(value);
         return this;
      }

      public override Completion<T> OnCancelled(Action action) => this;

      public override Completion<T> OnInterrupted(Action<Exception> action) => this;

      public override bool ValueEqualTo(Completion<T> otherCompletion) => otherCompletion.Map(out var otherValue) && EqualToValueOf(otherValue);

      public override bool EqualToValueOf(T otherValue) => value.Equals(otherValue);

      public Completion<object> AsObject() => value.Completed<object>();

      public override Completion<TResult> CastAs<TResult>()
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

      public override Completion<T> Where(Predicate<T> predicate) => predicate(value) ? this : nil;

      public override Completion<T> Where(Predicate<T> predicate, string exceptionMessage) => predicate(value) ? this : exceptionMessage.Interrupted<T>();

      public override Completion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage)
      {
         return predicate(value) ? this : exceptionMessage().Interrupted<T>();
      }

      public override T DefaultTo(Func<Maybe<Exception>, T> defaultFunc) => Value;

      public override Maybe<T> Maybe() => Value;

      public override Result<T> Result() => Value;

      public override Responding<T> Responding() => Value;

      public override T Value => value;

      public override Exception Exception => throw fail("Completed has no Exception");

      public bool Equals(Completed<T> other)
      {
         return other is not null && (ReferenceEquals(this, other) || EqualityComparer<T>.Default.Equals(value, other.value));
      }

      public override bool Equals(object obj) => obj is Completed<T> other && Equals(other);

      public override int GetHashCode() => value.GetHashCode();

      public override string ToString() => $"Completed({value})";
   }
}