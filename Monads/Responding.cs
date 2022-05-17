using System;

namespace Core.Monads
{
   public abstract class Responding<T>
   {
      public static Responding<T> nil => new NoResponse<T>();

      public static Responding<T> Nil(string message) => new FailedResponse<T>(new Exception(message));

      public static Responding<T> operator |(Responding<T> left, Responding<T> right)
      {
         if (left.IsResponse)
         {
            return left;
         }
         else if (right.IsResponse)
         {
            return right;
         }
         else
         {
            return left;
         }
      }

      public static Responding<T> operator |(Responding<T> left, Func<Responding<T>> rightFunc)
      {
         if (left.IsResponse)
         {
            return left;
         }
         else
         {
            var right = rightFunc();
            if (right.IsResponse)
            {
               return right;
            }
         }

         return left;
      }

      public static implicit operator Responding<T>(T value) => new Response<T>(value);

      public static implicit operator Responding<T>(Exception exception) => new FailedResponse<T>(exception);

      public static implicit operator Responding<T>(Nil _) => new NoResponse<T>();

      public static implicit operator Responding<T>(Maybe<Exception> _exception)
      {
         return _exception.Map(e => (Responding<T>)new FailedResponse<T>(e)).DefaultTo(() => new NoResponse<T>());
      }

      public abstract bool IsResponse { get; }

      public abstract bool IsNoResponse { get; }

      public abstract bool IsFailedResponse { get; }

      public abstract Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse);

      public abstract Responding<TResult> Map<TResult>(Func<T, TResult> ifResponse);

      public abstract Responding<TResult> Map<TResult>(Func<T, Responding<TResult>> ifResponse, Func<Responding<TResult>> ifNoResponse,
         Func<Exception, Responding<TResult>> ifFailedResponse);

      public abstract Responding<T> OnResponse(Action<T> action);

      public abstract Responding<T> OnNoResponse(Action action);

      public abstract Responding<T> OnFailedResponse(Action<Exception> action);

      public abstract Responding<TResult> SelectMany<TResult>(Func<T, Responding<TResult>> projection);

      public abstract Responding<T2> SelectMany<T1, T2>(Func<T, Responding<T1>> func, Func<T, T1, T2> projection);

      public abstract Responding<TResult> SelectMany<TResult>(Func<T, TResult> func);

      public abstract Responding<TResult> Select<TResult>(Responding<T> result, Func<T, TResult> func);

      public abstract bool Map(out T value);

      public abstract bool IfNoResponse();

      public abstract bool IfFailedResponse(out Exception exception);

      public abstract bool Map(out T value, out Maybe<Exception> _exception);

      public abstract T Force();

      public abstract T DefaultTo(Func<Maybe<Exception>, T> func);

      public abstract void Deconstruction(out T value, out Maybe<Exception> _exception);

      public abstract Maybe<T> Maybe();

      public abstract Result<T> Result();

      public abstract Matched<T> Matched();

      public abstract Completion<T> Completion();
   }
}