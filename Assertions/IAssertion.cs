using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;

namespace Core.Assertions
{
   public interface IAssertion<T> : ICanBeTrue
   {
      T Value { get; }

      IEnumerable<Constraint> Constraints { get; }

      void Assert();

      void Assert(string message);

      void Assert(Func<string> messageFunc);

      void Assert<TException>(params object[] args) where TException : Exception;

      T Ensure();

      T Ensure(string message);

      T Ensure(Func<string> messageFunc);

      T Ensure<TException>(params object[] args) where TException : Exception;

      TResult Ensure<TResult>();

      TResult Ensure<TResult>(string message);

      TResult Ensure<TResult>(Func<string> messageFunc);

      TResult Ensure<TException, TResult>(params object[] args) where TException : Exception;

      IResult<T> Try();

      IResult<T> Try(string message);

      IResult<T> Try(Func<string> messageFunc);

      Task<ICompletion<T>> TryAsync(CancellationToken token);

      Task<ICompletion<T>> TryAsync(string message, CancellationToken token);

      Task<ICompletion<T>> TryAsync(Func<string> messageFunc, CancellationToken token);
   }
}