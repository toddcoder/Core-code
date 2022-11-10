using System;
using Core.Strings;

namespace Core.Monads;

public class FailedResponse<T> : Responding<T>, IEquatable<FailedResponse<T>>
{
   protected readonly Exception exception;

   public FailedResponse(Exception exception)
   {
      this.exception = exception is FullStackException ? exception : new FullStackException(exception);
   }

   public override T Value => throw exception;

   public override Exception Exception => exception;

   public override Maybe<Exception> AnyException => exception;

   public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse) => new FailedResponse<TResult>(exception);

   public override Responding<TResult> Map<TResult>(Func<T, TResult> ifResponse) => new FailedResponse<TResult>(exception);

   public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse, Func<Responding<TResult>> ifNoResponse,
      Func<Exception, Responding<TResult>> ifFailedResponse)
   {
      return ifFailedResponse(exception);
   }

   public override Responding<T> OnResponse(Action<T> action) => this;

   public override Responding<T> OnNoResponse(Action action) => this;

   public override Responding<T> OnFailedResponse(Action<Exception> action)
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

   public override Responding<TResult> SelectMany<TResult>(Func<T, Responding<TResult>> projection) => new FailedResponse<TResult>(exception);

   public override Responding<T2> SelectMany<T1, T2>(Func<T, Responding<T1>> func, Func<T, T1, T2> projection)
   {
      return new FailedResponse<T2>(exception);
   }

   public override Responding<TResult> SelectMany<TResult>(Func<T, TResult> func) => new FailedResponse<TResult>(exception);

   public override Responding<TResult> Select<TResult>(Responding<T> result, Func<T, TResult> func) => new FailedResponse<TResult>(exception);

   [Obsolete("Use ~")]
   public override bool Map(out T value)
   {
      value = default;
      return false;
   }

   public override bool IfNoResponse() => false;

   public override bool IfFailedResponse(out Exception exception)
   {
      exception = this.exception;
      return true;
   }

   [Obsolete("Use ~")]
   public override bool Map(out T value, out Maybe<Exception> _exception)
   {
      value = default;
      _exception = exception;

      return false;
   }

   public override T Force() => throw exception;

   public override T DefaultTo(Func<Maybe<Exception>, T> func) => func(exception);

   public override void Deconstruct(out T value, out Maybe<Exception> _exception)
   {
      value = default;
      _exception = exception;
   }

   public override Maybe<T> Maybe() => new None<T>();

   public override Result<T> Result() => new Failure<T>(exception);

   public override Completion<T> Completion() => new Interrupted<T>(exception);

   public bool Equals(FailedResponse<T> other) => Equals(exception, other.exception);

   public override bool Equals(object obj) => obj is FailedResponse<T> other && Equals(other);

   public override int GetHashCode() => exception?.GetHashCode() ?? 0;

   public static bool operator ==(FailedResponse<T> left, FailedResponse<T> right) => Equals(left, right);

   public static bool operator !=(FailedResponse<T> left, FailedResponse<T> right) => !Equals(left, right);

   public override string ToString() => $"FailedResponse({exception.Message.Elliptical(60, ' ')})";
}