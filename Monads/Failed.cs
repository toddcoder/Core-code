using System;
using Core.Strings;

namespace Core.Monads;

public class Failed<T> : Optional<T>, IEquatable<Failed<T>>
{
   internal readonly Exception exception;

   public Failed(Exception exception)
   {
      this.exception = exception is FullStackException ? exception : new FullStackException(exception);
   }

   public override Maybe<Exception> Exception => exception;

   public override Optional<TResult> Map<TResult>(Func<T, Optional<TResult>> ifJust) => new Failed<TResult>(exception);

   public override Optional<TResult> Map<TResult>(Func<T, TResult> ifJust) => new Failed<TResult>(exception);

   public override Optional<TResult> Map<TResult>(Func<T, Optional<TResult>> ifJust, Func<Optional<TResult>> ifEmpty,
      Func<Exception, Optional<TResult>> ifFailed)
   {
      return ifFailed(exception);
   }

   public override Optional<T> OnJust(Action<T> action) => this;

   public override Optional<T> OnEmpty(Action action) => this;

   public override Optional<T> OnFailed(Action<Exception> action)
   {
      try
      {
         action(exception);
         return this;
      }
      catch (Exception innerException)
      {
         return innerException;
      }
   }

   public override Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> projection) => new Failed<TResult>(exception);

   public override Optional<TResult> SelectMany<TResult>(Func<T, Maybe<TResult>> projection) => new Failed<TResult>(exception);

   public override Optional<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection) => new Failed<TResult>(exception);

   public override Optional<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection) => new Failed<TResult>(exception);

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Optional<T1>> func, Func<T, T1, T2> projection)
   {
      return new Failed<T2>(exception);
   }

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Maybe<T1>> func, Func<T, T1, T2> projection) => new Failed<T2>(exception);

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection) => new Failed<T2>(exception);

   public override Optional<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection) => new Failed<T2>(exception);

   public override Optional<TResult> SelectMany<TResult>(Func<T, TResult> func) => new Failed<TResult>(exception);

   public override Optional<TResult> Select<TResult>(Optional<T> result, Func<T, TResult> func) => new Failed<TResult>(exception);

   public override bool IfEmpty() => false;

   public override bool IfFailed(out Exception exception)
   {
      exception = this.exception;
      return true;
   }

   public override T Force() => throw exception;

   public override T DefaultTo(Func<Maybe<Exception>, T> func) => func(exception);

   public override void Deconstruct(out bool isJust, out T value)
   {
      isJust = false;
      value = default;
   }

   public override Maybe<T> Maybe() => new None<T>();

   public override Result<T> Result() => new Failure<T>(exception);

   public override Completion<T> Completion() => new Interrupted<T>(exception);

   public override object ToObject() => exception;

   public override Optional<T> Initialize(Func<T> initializer) => initializer();

   public override void MapOf(Action<T> action)
   {
   }

   public bool Equals(Failed<T> other) => Equals(exception, other.exception);

   public override bool Equals(object obj) => obj is Failed<T> other && Equals(other);

   public override int GetHashCode() => exception?.GetHashCode() ?? 0;

   public static bool operator ==(Failed<T> left, Failed<T> right) => Equals(left, right);

   public static bool operator !=(Failed<T> left, Failed<T> right) => !Equals(left, right);

   public override string ToString() => $"Failed({exception.Message.Elliptical(60, ' ')})";
}