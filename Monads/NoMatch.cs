using System;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class NoMatch<T> : IMatched<T>, IEquatable<NoMatch<T>>
   {
      public static implicit operator bool(NoMatch<T> _) => false;

      internal NoMatch()
      {
      }

      public override IMatched<T> Do(Action<T> ifMatched, Action ifNotOrFailed)
      {
         ifNotOrFailed();
         return this;
      }

      public override IMatched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch)
      {
         ifNotMatched();
         return this;
      }

      public override IMatched<TOther> ExceptionAs<TOther>() => throw "There is no exception".Throws();

      public override IMatched<T> Or(IMatched<T> other) => other;

      public override IMatched<T> Or(Func<IMatched<T>> other) => other();

      public override IMatched<TResult> SelectMany<TResult>(Func<T, IMatched<TResult>> projection) => noMatch<TResult>();

      public override IMatched<T2> SelectMany<T1, T2>(Func<T, IMatched<T1>> func, Func<T, T1, T2> projection)
      {
         return noMatch<T2>();
      }

      public override IMatched<TResult> SelectMany<TResult>(Func<T, TResult> func) => noMatch<TResult>();

      public override IMatched<TResult> Select<TResult>(IMatched<T> result, Func<T, TResult> func) => noMatch<TResult>();

      public override bool If(out T value)
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

      public override bool ValueOrOriginal(out T value, out IMatched<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      public override bool ValueOrCast<TMatched>(out T value, out IMatched<TMatched> matched)
      {
         value = default;
         matched = noMatch<TMatched>();

         return false;
      }

      public override bool If(out T value, out Maybe<Exception> exception)
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

      public override bool Else<TOther>(out IMatched<TOther> result)
      {
         result = noMatch<TOther>();
         return true;
      }

      public override IMatched<TOther> Unmatched<TOther>() => noMatch<TOther>();

      public override bool WasMatched(out IMatched<T> matched)
      {
         matched = this;
         return false;
      }

      public override void Force()
      {
      }

      public override T ForceValue() => throw "There is no value".Throws();

      public override IMatched<T> UnmatchedOnly() => this;

      public override IMatched<TOther> UnmatchedOnly<TOther>() => noMatch<TOther>();

      public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> exception)
      {
         value = none<T>();
         exception = none<Exception>();
      }

      public override bool EqualToValueOf(IMatched<T> otherMatched) => false;

      public override bool ValueEqualTo(T otherValue) => false;

      public override IMatched<TResult> CastAs<TResult>() => noMatch<TResult>();

      public override IMatched<T> Where(Predicate<T> predicate) => this;

      public override IMatched<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public override IMatched<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public override IMatched<T> ExceptionMessage(string message) => this;

      public override IMatched<T> ExceptionMessage(Func<Exception, string> message) => this;

      public override bool IsMatched => false;

      public override bool IsNotMatched => true;

      public override bool IsFailedMatch => false;

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched) => noMatch<TResult>();

      public override IMatched<TResult> Map<TResult>(Func<T, TResult> ifMatched) => noMatch<TResult>();

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched)
      {
         return ifNotMatched();
      }

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<Exception, IMatched<TResult>> ifFailedMatch)
      {
         return noMatch<TResult>();
      }

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched,
         Func<Exception, IMatched<TResult>> ifFailedMatch)
      {
         return noMatch<TResult>();
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched, Func<Exception, TResult> ifFailedMatch)
      {
         return ifNotMatched();
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed) => ifNotOrFailed();

      public override IMatched<T> If(Action<T> action) => this;

      public override IMatched<T> Else(Action action)
      {
         action();
         return this;
      }

      public override IMatched<T> Else(Action<Exception> action) => this;

      public bool Equals(NoMatch<T> other) => true;

      public override bool Equals(object obj) => obj is NoMatch<T>;

      public override int GetHashCode() => false.GetHashCode();

      public override string ToString() => $"notMatched<{typeof(T).Name}>";
   }
}