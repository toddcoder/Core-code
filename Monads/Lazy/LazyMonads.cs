using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class LazyMonads
{
   protected static Maybe<LazyMonads> _function;

   static LazyMonads()
   {
      _function = nil;
   }

   public static LazyMonads lazy
   {
      get
      {
         if (!_function)
         {
            _function = new LazyMonads();
         }

         return ~_function;
      }
   }

   public LazyMaybe<T> maybe<T>(Func<Maybe<T>> func) => new(func);

   public LazyMaybe<T> maybe<T>() => new();

   public LazyResult<T> result<T>(Func<Result<T>> func) => new(func);

   public LazyResult<T> result<T>() => new();

   public LazyResponding<T> responding<T>(Func<Responding<T>> func) => new(func);

   public LazyResponding<T> responding<T>() => new();

   public LazyCompletion<T> completion<T>(Func<Completion<T>> func) => new(func);

   public LazyCompletion<T> completion<T>() => new();
}