using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class LazyRepeatingMonads
{
   protected static Maybe<LazyRepeatingMonads> _function;

   static LazyRepeatingMonads()
   {
      _function = nil;
   }

   public static LazyRepeatingMonads lazyRepeating
   {
      get
      {
         if (!_function)
         {
            _function = new LazyRepeatingMonads();
         }

         return ~_function;
      }
   }

   public LazyMaybe<T> maybe<T>() => new() { Repeating = true };

   public LazyResult<T> result<T>() => new() { Repeating = true };

   public LazyResponding<T> responding<T>() => new() { Repeating = true };

   public LazyCompletion<T> completion<T>() => new() { Repeating = true };
}