using System;
using System.Collections.Generic;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Match<T> : Matched<T>, IEquatable<Match<T>>
   {
      public static implicit operator bool(Match<T> _) => true;

      protected T value;

      internal Match(T value) => this.value = value;

      public T Value => value;

      public override Matched<T> Do(Action<T> ifMatched, Action ifNotOrFailed)
      {
         ifMatched(value);
         return this;
      }

      public override Matched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch)
      {
         ifMatched(value);
         return this;
      }

      public override Matched<TOther> ExceptionAs<TOther>() => throw "There is no exception".Throws();

      public override Matched<T> Or(Matched<T> other) => this;

      public override Matched<T> Or(Func<Matched<T>> other) => this;

      public override Matched<TResult> SelectMany<TResult>(Func<T, Matched<TResult>> projection) => projection(value);

      public override Matched<T2> SelectMany<T1, T2>(Func<T, Matched<T1>> func, Func<T, T1, T2> projection)
      {
         return func(value).Map(t1 => projection(value, t1).Match(), noMatch<T2>, failedMatch<T2>);
      }

      public override Matched<TResult> SelectMany<TResult>(Func<T, TResult> func) => func(value).Match();

      public override Matched<TResult> Select<TResult>(Matched<T> result, Func<T, TResult> func) => func(value).Match();

      public override bool If(out T value)
      {
         value = this.value;
         return true;
      }

      public override bool IfNotMatched() => false;

      public override bool Failed(out Exception exception)
      {
         exception = "There is no exception".Throws();
         return false;
      }

      public override bool ValueOrOriginal(out T value, out Matched<T> original)
      {
         value = this.value;
         original = this;

         return true;
      }

      public override bool ValueOrCast<TMatched>(out T value, out Matched<TMatched> matched)
      {
         value = this.value;
         matched = "Do not use this".FailedMatch<TMatched>();

         return true;
      }

      public override bool If(out T value, out Maybe<Exception> exception)
      {
         value = this.value;
         exception = none<Exception>();

         return true;
      }

      public override bool IfNot(out Maybe<Exception> anyException)
      {
         anyException = none<Exception>();
         return false;
      }

      public override bool Else<TOther>(out Matched<TOther> result)
      {
         result = noMatch<TOther>();
         return false;
      }

      public override Matched<TOther> Unmatched<TOther>() => noMatch<TOther>();

      public override bool WasMatched(out Matched<T> matched)
      {
         matched = this;
         return true;
      }

      public override void Force()
      {
      }

      public override T ForceValue() => value;

      public override Matched<T> UnmatchedOnly() => noMatch<T>();

      public override Matched<TOther> UnmatchedOnly<TOther>() => noMatch<TOther>();

      public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> exception)
      {
         value = this.value.Some();
         exception = none<Exception>();
      }

      public override bool EqualToValueOf(Matched<T> otherMatched) => otherMatched.If(out var otherValue) && ValueEqualTo(otherValue);

      public override bool ValueEqualTo(T otherValue) => value.Equals(otherValue);

      public override Matched<TResult> CastAs<TResult>()
      {
         if (value is TResult result)
         {
            return result.Match();
         }
         else
         {
            return $"Invalid cast from {typeof(T).Name} to {typeof(TResult).Name}".FailedMatch<TResult>();
         }
      }

      public override Matched<T> Where(Predicate<T> predicate) => predicate(value) ? this : noMatch<T>();

      public override Matched<T> Where(Predicate<T> predicate, string exceptionMessage) =>
         predicate(value) ? this : exceptionMessage.FailedMatch<T>();

      public override Matched<T> Where(Predicate<T> predicate, Func<string> exceptionMessage)
      {
         return predicate(value) ? this : exceptionMessage().FailedMatch<T>();
      }

      public override Matched<T> ExceptionMessage(string message) => this;

      public override Matched<T> ExceptionMessage(Func<Exception, string> message) => this;

      public override bool IsMatched => true;

      public override bool IsNotMatched => false;

      public override bool IsFailedMatch => false;

      public override Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched) => ifMatched(value);

      public override Matched<TResult> Map<TResult>(Func<T, TResult> ifMatched) => ifMatched(value).Match();

      public override Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched, Func<Matched<TResult>> ifNotMatched)
      {
         return ifMatched(value);
      }

      public override Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched, Func<Exception, Matched<TResult>> ifFailedMatch)
      {
         return ifMatched(value);
      }

      public override Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched, Func<Matched<TResult>> ifNotMatched,
         Func<Exception, Matched<TResult>> ifFailedMatch)
      {
         return ifMatched(value);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched,
         Func<Exception, TResult> ifFailedMatch)
      {
         return ifMatched(value);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed) => ifMatched(value);

      public override Matched<T> If(Action<T> action)
      {
         action(value);
         return this;
      }

      public override Matched<T> Else(Action action) => this;

      public override Matched<T> Else(Action<Exception> action) => this;

      public bool Equals(Match<T> other)
      {
         return other is not null && ReferenceEquals(this, other) || EqualityComparer<T>.Default.Equals(value, other.value);
      }

      public override bool Equals(object obj) => obj is Match<T> other && Equals(other);

      public override int GetHashCode() => value.GetHashCode();

      public override string ToString() => $"Matched({value})";
   }
}