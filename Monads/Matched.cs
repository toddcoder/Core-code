using System;
using System.Collections.Generic;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Matched<T> : IMatched<T>, IEquatable<Matched<T>>
   {
      public static implicit operator bool(Matched<T> _) => true;

      public static bool operator &(Matched<T> x, IHasValue y) => y.HasValue;

      public static bool operator |(Matched<T> x, IHasValue y) => true;

      public static bool operator !(Matched<T> _) => false;

      protected T value;

      internal Matched(T value) => this.value = value;

      public IMatched<T> Do(Action<T> ifMatched, Action ifNotOrFailed)
      {
         ifMatched(value);
         return this;
      }

      public IMatched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch)
      {
         ifMatched(value);
         return this;
      }

      public IMatched<TOther> ExceptionAs<TOther>() => throw "There is no exception".Throws();

      public IMatched<T> Or(IMatched<T> other) => this;

      public IMatched<T> Or(Func<IMatched<T>> other) => this;

      public IMatched<TResult> SelectMany<TResult>(Func<T, IMatched<TResult>> projection) => projection(value);

      public IMatched<T2> SelectMany<T1, T2>(Func<T, IMatched<T1>> func, Func<T, T1, T2> projection)
      {
         return func(value).Map(t1 => projection(value, t1).Matched(), notMatched<T2>, failedMatch<T2>);
      }

      public IMatched<TResult> SelectMany<TResult>(Func<T, TResult> func) => func(value).Matched();

      public IMatched<TResult> Select<TResult>(IMatched<T> result, Func<T, TResult> func) => func(value).Matched();

      public bool If(out T value)
      {
         value = this.value;
         return true;
      }

      public bool IfNotMatched() => false;

      public bool Failed(out Exception exception)
      {
         exception = "There is no exception".Throws();
         return false;
      }

      public bool ValueOrOriginal(out T value, out IMatched<T> original)
      {
         value = this.value;
         original = this;

         return true;
      }

      public bool ValueOrCast<TMatched>(out T value, out IMatched<TMatched> matched)
      {
         value = this.value;
         matched = "Do not use this".FailedMatch<TMatched>();

         return true;
      }

      public bool If(out T value, out IMaybe<Exception> exception)
      {
         value = this.value;
         exception = none<Exception>();

         return true;
      }

      public bool IfNot(out IMaybe<Exception> anyException)
      {
         anyException = none<Exception>();
         return false;
      }

      public bool Else<TOther>(out IMatched<TOther> result)
      {
         result = notMatched<TOther>();
         return false;
      }

      public IMatched<TOther> Unmatched<TOther>() => notMatched<TOther>();

      public bool WasMatched(out IMatched<T> matched)
      {
         matched = this;
         return true;
      }

      public void Force() { }

      public T ForceValue() => value;

      public IMatched<T> UnmatchedOnly() => notMatched<T>();

      public IMatched<TOther> UnmatchedOnly<TOther>() => notMatched<TOther>();

      public void Deconstruct(out IMaybe<T> value, out IMaybe<Exception> exception)
      {
         value = this.value.Some();
         exception = none<Exception>();
      }

      public bool EqualToValueOf(IMatched<T> otherMatched) => otherMatched.If(out var otherValue) && ValueEqualTo(otherValue);

      public bool ValueEqualTo(T otherValue) => value.Equals(otherValue);

      public IMatched<object> AsObject() => value.Matched<object>();

      public IMatched<TResult> CastAs<TResult>()
      {
         if (value is TResult result)
         {
            return result.Matched();
         }
         else
         {
            return $"Invalid cast from {typeof(T).Name} to {typeof(TResult).Name}".FailedMatch<TResult>();
         }
      }

      public IMatched<T> Where(Predicate<T> predicate) => predicate(value) ? this : notMatched<T>();

      public IMatched<T> Where(Predicate<T> predicate, string exceptionMessage) => predicate(value) ? this : exceptionMessage.FailedMatch<T>();

      public IMatched<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => predicate(value) ? this : exceptionMessage().FailedMatch<T>();

      public bool IsMatched => true;

      public bool IsNotMatched => false;

      public bool IsFailedMatch => false;

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched) => ifMatched(value);

      public IMatched<TResult> Map<TResult>(Func<T, TResult> ifMatched) => ifMatched(value).Matched();

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched)
      {
         return ifMatched(value);
      }

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched,
         Func<Exception, IMatched<TResult>> ifFailedMatch)
      {
         return ifMatched(value);
      }

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched,
         Func<Exception, IMatched<TResult>> ifFailedMatch)
      {
         return ifMatched(value);
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched,
         Func<Exception, TResult> ifFailedMatch)
      {
         return ifMatched(value);
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed) => ifMatched(value);

      public IMatched<T> If(Action<T> action)
      {
         action(value);
         return this;
      }

      public IMatched<T> Else(Action action) => this;

      public IMatched<T> Else(Action<Exception> action) => this;

      public bool HasValue => true;

      public bool Equals(Matched<T> other)
      {
         if (ReferenceEquals(null, other))
         {
            return false;
         }

         if (ReferenceEquals(this, other))
         {
            return true;
         }

         return EqualityComparer<T>.Default.Equals(value, other.value);
      }

      public override bool Equals(object obj) => obj is Matched<T> other && Equals(other);

      public override int GetHashCode() => value.GetHashCode();
   }
}