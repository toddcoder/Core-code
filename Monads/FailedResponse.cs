﻿using System;
using Core.Strings;

namespace Core.Monads
{
   public class FailedResponse<T> : Responding<T>, IEquatable<FailedResponse<T>>
   {
      public FailedResponse(Exception exception)
      {
         Exception = exception is FullStackException ? exception : new FullStackException(exception);
      }

      public Exception Exception { get; }

#pragma warning disable CS0672
      public override bool IsResponse => false;
#pragma warning restore CS0672

#pragma warning disable CS0672
      public override bool IsNoResponse => false;
#pragma warning restore CS0672

#pragma warning disable CS0672
      public override bool IsFailedResponse => true;
#pragma warning restore CS0672

      public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse) => new FailedResponse<TResult>(Exception);

      public override Responding<TResult> Map<TResult>(Func<T, TResult> ifResponse) => new FailedResponse<TResult>(Exception);

      public override Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse, Func<Responding<TResult>> ifNoResponse,
         Func<Exception, Responding<TResult>> ifFailedResponse)
      {
         return ifFailedResponse(Exception);
      }

      public override Responding<T> OnResponse(Action<T> action) => this;

      public override Responding<T> OnNoResponse(Action action) => this;

      public override Responding<T> OnFailedResponse(Action<Exception> action)
      {
         try
         {
            action(Exception);
            return this;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public override Responding<TResult> SelectMany<TResult>(Func<T, Responding<TResult>> projection) => new FailedResponse<TResult>(Exception);

      public override Responding<T2> SelectMany<T1, T2>(Func<T, Responding<T1>> func, Func<T, T1, T2> projection)
      {
         return new FailedResponse<T2>(Exception);
      }

      public override Responding<TResult> SelectMany<TResult>(Func<T, TResult> func) => new FailedResponse<TResult>(Exception);

      public override Responding<TResult> Select<TResult>(Responding<T> result, Func<T, TResult> func) => new FailedResponse<TResult>(Exception);

      public override bool Map(out T value)
      {
         value = default;
         return false;
      }

      public override bool IfNoResponse() => false;

      public override bool IfFailedResponse(out Exception exception)
      {
         exception = Exception;
         return true;
      }

      public override bool Map(out T value, out Maybe<Exception> _exception)
      {
         value = default;
         _exception = Exception;

         return false;
      }

      public override T Force() => throw Exception;

      public override T DefaultTo(Func<Maybe<Exception>, T> func) => func(Exception);

      public override void Deconstruction(out T value, out Maybe<Exception> _exception)
      {
         value = default;
         _exception = Exception;
      }

      public override Maybe<T> Maybe() => new None<T>();

      public override Result<T> Result() => new Failure<T>(Exception);

      public override Matched<T> Matched() => new FailedMatch<T>(Exception);

      public override Completion<T> Completion() => new Interrupted<T>(Exception);

      public bool Equals(FailedResponse<T> other) => Equals(Exception, other.Exception);

      public override bool Equals(object obj) => obj is FailedResponse<T> other && Equals(other);

      public override int GetHashCode() => Exception != null ? Exception.GetHashCode() : 0;

      public static bool operator ==(FailedResponse<T> left, FailedResponse<T> right) => Equals(left, right);

      public static bool operator !=(FailedResponse<T> left, FailedResponse<T> right) => !Equals(left, right);

      public override string ToString() => $"FailedResponse({Exception.Message.Elliptical(60, ' ')})";
   }
}