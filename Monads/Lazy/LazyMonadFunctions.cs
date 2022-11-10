using System;

namespace Core.Monads.Lazy;

public class LazyMonadFunctions
{
   protected static Monads.Maybe<LazyMonadFunctions> _function;

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

   public Maybe<T> maybe<T>(Func<Monads.Maybe<T>> func) => new(func);

   public Result<T> result<T>(Func<Monads.Result<T>> func) => new(func);

   public Responding<T> responding<T>(Func<Monads.Responding<T>> func) => new(func);

   public Completion<T> completion<T>(Func<Monads.Completion<T>> func) => new(func);
}