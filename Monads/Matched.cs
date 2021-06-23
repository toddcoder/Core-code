using System;
using System.Collections.Generic;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Matched<T> : IMatched<T>, IEquatable<Matched<T>>
   {
      public static implicit operator bool(Matched<T> _) => true;

      protected T value;

      internal Matched(T value) => this.value = value;

      public T Value => value;

      public override IMatched<T> Do(Action<T> ifMatched, Action ifNotOrFailed)
      {
         ifMatched(value);
         return this;
      }

      public override IMatched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch)
      {
         ifMatched(value);
         return this;
      }

      public override IMatched<TOther> ExceptionAs<TOther>() => throw "There is no exception".Throws();

      public override IMatched<T> Or(IMatched<T> other) => this;

      public override IMatched<T> Or(Func<IMatched<T>> other) => this;

      public override IMatched<TResult> SelectMany<TResult>(Func<T, IMatched<TResult>> projection) => projection(value);

      public override IMatched<T2> SelectMany<T1, T2>(Func<T, IMatched<T1>> func, Func<T, T1, T2> projection)
      {
         return func(value).Map(t1 => projection(value, t1).Matched(), notMatched<T2>, failedMatch<T2>);
      }

      public override IMatched<TResult> SelectMany<TResult>(Func<T, TResult> func) => func(value).Matched();

      public override IMatched<TResult> Select<TResult>(IMatched<T> result, Func<T, TResult> func) => func(value).Matched();

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

      public override bool ValueOrOriginal(out T value, out IMatched<T> original)
      {
         value = this.value;
         original = this;

         return true;
      }

      public override bool ValueOrCast<TMatched>(out T value, out IMatched<TMatched> matched)
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

      public override bool Else<TOther>(out IMatched<TOther> result)
      {
         result = notMatched<TOther>();
         return false;
      }

      public override IMatched<TOther> Unmatched<TOther>() => notMatched<TOther>();

      public override bool WasMatched(out IMatched<T> matched)
      {
         matched = this;
         return true;
      }

      public override void Force()
      {
      }

      public override T ForceValue() => value;

      public override IMatched<T> UnmatchedOnly() => notMatched<T>();

      public override IMatched<TOther> UnmatchedOnly<TOther>() => notMatched<TOther>();

      public override void Deconstruct(out Maybe<T> value, out Maybe<Exception> exception)
      {
         value = this.value.Some();
         exception = none<Exception>();
      }

      public override bool EqualToValueOf(IMatched<T> otherMatched) => otherMatched.If(out var otherValue) && ValueEqualTo(otherValue);

      public override bool ValueEqualTo(T otherValue) => value.Equals(otherValue);

      public override IMatched<TResult> CastAs<TResult>()
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

      public override IMatched<T> Where(Predicate<T> predicate) => predicate(value) ? this : notMatched<T>();

      public override IMatched<T> Where(Predicate<T> predicate, string exceptionMessage) =>
         predicate(value) ? this : exceptionMessage.FailedMatch<T>();

      public override IMatched<T> Where(Predicate<T> predicate, Func<string> exceptionMessage)
      {
         return predicate(value) ? this : exceptionMessage().FailedMatch<T>();
      }

      public override IMatched<T> ExceptionMessage(string message) => this;

      public override IMatched<T> ExceptionMessage(Func<Exception, string> message) => this;

      public override bool IsMatched => true;

      public override bool IsNotMatched => false;

      public override bool IsFailedMatch => false;

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched) => ifMatched(value);

      public override IMatched<TResult> Map<TResult>(Func<T, TResult> ifMatched) => ifMatched(value).Matched();

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched)
      {
         return ifMatched(value);
      }

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<Exception, IMatched<TResult>> ifFailedMatch)
      {
         return ifMatched(value);
      }

      public override IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched,
         Func<Exception, IMatched<TResult>> ifFailedMatch)
      {
         return ifMatched(value);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched,
         Func<Exception, TResult> ifFailedMatch)
      {
         return ifMatched(value);
      }

      public override TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed) => ifMatched(value);

      public override IMatched<T> If(Action<T> action)
      {
         action(value);
         return this;
      }

      public override IMatched<T> Else(Action action) => this;

      public override IMatched<T> Else(Action<Exception> action) => this;

      public bool Equals(Matched<T> other)
      {
         return other is not null && ReferenceEquals(this, other) || EqualityComparer<T>.Default.Equals(value, other.value);
      }

      public override bool Equals(object obj) => obj is Matched<T> other && Equals(other);

      public override int GetHashCode() => value.GetHashCode();

      public override string ToString() => $"Matched({value})";
   }
}