using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class LazyRepeatingMonads
{
   protected static Optional<LazyRepeatingMonads> _function;

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

         return _function;
      }
   }

   public LazyOptional<T> maybe<T>() => new() { Repeating = true };

   public LazyOptional<T> result<T>() => new() { Repeating = true };

   public LazyOptional<T> optional<T>() => new() { Repeating = true };

   public LazyCompletion<T> completion<T>() => new() { Repeating = true };
}