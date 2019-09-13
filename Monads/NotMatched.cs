using System;
using Core.Exceptions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class NotMatched<T> : IMatched<T>
   {
      internal NotMatched() { }

      public IMatched<T> Do(Action<T> ifMatched, Action ifNotOrFailed)
      {
         ifNotOrFailed();
         return this;
      }

      public IMatched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch)
      {
         ifNotMatched();
         return this;
      }

      public IMatched<TOther> ExceptionAs<TOther>() => throw "There is no exception".Throws();

      public IMatched<T> Or(IMatched<T> other) => other;

      public IMatched<T> Or(Func<IMatched<T>> other) => other();

      public IMatched<TResult> SelectMany<TResult>(Func<T, IMatched<TResult>> projection) => notMatched<TResult>();

      public IMatched<T2> SelectMany<T1, T2>(Func<T, IMatched<T1>> func, Func<T, T1, T2> projection)
      {
         return notMatched<T2>();
      }

      public IMatched<TResult> SelectMany<TResult>(Func<T, TResult> func) => notMatched<TResult>();

      public IMatched<TResult> Select<TResult>(IMatched<T> result, Func<T, TResult> func) => notMatched<TResult>();

      public bool If(out T value)
      {
         value = default;
         return false;
      }

      public bool IfNotMatched() => true;

      public bool Failed(out Exception exception)
      {
         exception = "There is no exception".Throws();
         return false;
      }

      public bool Out(out T value, out IMatched<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      public bool If(out T value, out bool isNotMatched, out Exception exception)
      {
         value = default;
         isNotMatched = true;
         exception = "There is no exception".Throws();

         return false;
      }

      public bool If(out T value, out IMaybe<Exception> exception)
      {
         value = default;
         exception = none<Exception>();

         return false;
      }

      public bool Else<TOther>(out IMatched<TOther> result)
      {
         result = notMatched<TOther>();
         return true;
      }

      public IMatched<TOther> Unmatched<TOther>() => notMatched<TOther>();

      public bool WasMatched(out IMatched<T> matched)
      {
         matched = this;
         return false;
      }

      public void Force() { }

      public T ForceValue() => throw "There is no value".Throws();

      public IMatched<T> UnmatchedOnly() => this;

      public IMatched<TOther> UnmatchedOnly<TOther>() => notMatched<TOther>();

      public void Deconstruct(out IMaybe<T> value, out IMaybe<Exception> exception)
      {
         value = none<T>();
         exception = none<Exception>();
      }

      public bool IsMatched => false;

      public bool IsNotMatched => true;

      public bool IsFailedMatch => false;

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched) => notMatched<TResult>();

      public IMatched<TResult> Map<TResult>(Func<T, TResult> ifMatched) => notMatched<TResult>();

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched)
      {
         return ifNotMatched();
      }

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched,
         Func<Exception, IMatched<TResult>> ifFailedMatch)
      {
         return notMatched<TResult>();
      }

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched,
         Func<Exception, IMatched<TResult>> ifFailedMatch)
      {
         return notMatched<TResult>();
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched,
         Func<Exception, TResult> ifFailedMatch)
      {
         return ifNotMatched();
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed) => ifNotOrFailed();

      public IMatched<T> If(Action<T> action) => this;

      public IMatched<T> Else(Action action)
      {
         action();
         return this;
      }

      public IMatched<T> Else(Action<Exception> action) => this;
   }
}