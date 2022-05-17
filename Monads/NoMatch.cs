using System;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class NoMatch<T> : Matched<T>, IEquatable<NoMatch<T>>
   {
      public static implicit operator bool(NoMatch<T> _) => false;

      internal NoMatch()
      {
      }

      public override Matched<T> Do(Action<T> ifMatched, Action ifNotOrFailed)
      {
         ifNotOrFailed();
         return this;
      }

      public override Matched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch)
      {
         ifNotMatched();
         return this;
      }

      [Obsolete("Use exception")]
      public override Matched<TOther> ExceptionAs<TOther>() => throw "There is no exception".Throws();

      public override Matched<T> Or(Matched<T> other) => other;

      public override Matched<T> Or(Func<Matched<T>> other) => other();

      public override Matched<TResult> SelectMany<TResult>(Func<T, Matched<TResult>> projection) => noMatch<TResult>();

      public override Matched<T2> SelectMany<T1, T2>(Func<T, Matched<T1>> func, Func<T, T1, T2> projection)
      {
         return noMatch<T2>();
      }

      public override Matched<TResult> SelectMany<TResult>(Func<T, TResult> func) => noMatch<TResult>();

      public override Matched<TResult> Select<TResult>(Matched<T> result, Func<T, TResult> func) => noMatch<TResult>();

      public override bool Map(out T value)
      {
         value = default;
         return false;
      }

      public override bool IfNotMatched() => true;

      public override bool Failed(out Exception exception)
      {
         exception = "There is no exception".Throws();
         return false;
      }

      [Obsolete("Use If")]
      public override bool ValueOrOriginal(out T value, out Matched<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      [Obsolete("Use If")]
      public override bool ValueOrCast<TMatched>(out T value, out Matched<TMatched> matched)
      {
         value = default;
         matched = noMatch<TMatched>();

         return false;
      }

      public override bool Map(out T value, out Maybe<Exception> exception)
      {
         value = default;
         exception = none<Exception>();

         return false;
      }

      public override bool IfNot(out Maybe<Exception> anyException)
      {
         anyException = none<Exception>();
         return true;
      }

      public override bool UnMap<TOther>(out Matched<TOther> result)
      {
         result = noMatch<TOther>();
         return true;
      }

      public override Matched<TOther> Unmatched<TOther>() => noMatch<TOther>();

      public override bool WasMatched(out Matched<T> matched)
      {
         matched = this;
         return false;
      }

      public override void Force()
      {
      }

      public override T ForceValue() => throw "There is no value".Throws();

      public override Matched<T> UnmatchedOnly() => this;

      public override Matched<TOther> UnmatchedOnly<TOther>() => noMatch<TOther>();

      public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> exception)
      {
         value = none<T>();
         exception = none<Exception>();
      }

      public override bool EqualToValueOf(Matched<T> otherMatched) => false;

      public override bool ValueEqualTo(T otherValue) => false;

      public override Matched<TResult> CastAs<TResult>() => noMatch<TResult>();

      public override Matched<T> Where(Predicate<T> predicate) => this;

      public override Matched<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public override Matched<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public override Matched<T> ExceptionMessage(string message) => this;

      public override Matched<T> ExceptionMessage(Func<Exception, string> message) => this;

      public override Matched<T> OnMatch(Action<T> onMatched) => this;

      public override Matched<T> OnFailed(Action<Exception> onFailed) => this;

      public override Matched<T> OnNotMatched(Action onNotMatched)
      {
         try
         {
            onNotMatched();
         }
         catch (Exception exception)
         {
            return exception;
         }

         return this;
      }

      public override Maybe<T> Maybe() => new None<T>();

      public override bool IsMatched => false;

      public override bool IsNotMatched => true;

      public override bool IsFailedMatch => false;

      public override Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched) => noMatch<TResult>();

      public override Matched<TResult> Map<TResult>(Func<T, TResult> ifMatched) => noMatch<TResult>();

      public override Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched, Func<Matched<TResult>> ifNotMatched)
      {
         return ifNotMatched();
      }

      public override Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched, Func<Exception, Matched<TResult>> ifFailedMatch)
      {
         return noMatch<TResult>();
      }

      public override Matched<TResult> Map<TResult>(Func<T, Matched<TResult>> ifMatched, Func<Matched<TResult>> ifNotMatched,
         Func<Exception, Matched<TResult>> ifFailedMatch)
      {
         return noMatch<TResult>();
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched, Func<Exception, TResult> ifFailedMatch)
      {
         return ifNotMatched();
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed) => ifNotOrFailed();

      public override Matched<T> Map(Action<T> action) => this;

      public override Matched<T> UnMap(Action action)
      {
         action();
         return this;
      }

      public override Matched<T> UnMap(Action<Exception> action) => this;

      public bool Equals(NoMatch<T> other) => true;

      public override bool Equals(object obj) => obj is NoMatch<T>;

      public override int GetHashCode() => false.GetHashCode();

      public override string ToString() => $"notMatched<{typeof(T).Name}>";
   }
}