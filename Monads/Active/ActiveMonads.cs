using static Core.Monads.MonadFunctions;

namespace Core.Monads.Active;

public class ActiveMonads
{
   protected static Maybe<ActiveMonads> _function;

   static ActiveMonads()
   {
      _function = nil;
   }

   public static ActiveMonads monads
   {
      get
      {
         if (!_function)
         {
            _function = new ActiveMonads();
         }

         return _function;
      }
   }

   public Maybe<T> maybe<T>() => nil;

   public Result<T> result<T>() => nil;

   public Optional<T> optional<T>() => nil;

   public Completion<T> completion<T>() => nil;
}