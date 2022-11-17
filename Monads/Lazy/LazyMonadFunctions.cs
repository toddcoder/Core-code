using System;

namespace Core.Monads.Lazy;

public class LazyMonadFunctions
{
   protected static Maybe<LazyMonadFunctions> _function;

   static LazyMonadFunctions()
   {
      _function = MonadFunctions.nil;
   }

   public static LazyMonadFunctions lazy
   {
      get
      {
         if (!_function)
         {
            _function = new LazyMonadFunctions();
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