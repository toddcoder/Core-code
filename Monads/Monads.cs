using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Monads
{
   protected static Maybe<Monads> _function;

   static Monads()
   {
      _function = nil;
   }

   public static Monads monads
   {
      get
      {
         if (!_function)
         {
            _function = new Monads();
         }

         return _function;
      }
   }

   public Maybe<T> maybe<T>() => nil;

   public Result<T> result<T>() => nil;

   public Optional<T> optional<T>() => nil;

   public Completion<T> completion<T>() => nil;
}