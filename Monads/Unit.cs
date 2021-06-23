using System.Threading;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Unit
   {
      public static Unit Value => new Unit();

      public static Maybe<Unit> Some() => Value.Some();

      public static Result<Unit> Success() => Value.Success();

      public static IMatched<Unit> Matched() => Value.Matched();

      public static Completion<Unit> Completed() => Value.Completed();

      public static Completion<Unit> Completed(CancellationToken token)
      {
         if (token.IsCancellationRequested)
         {
            return cancelled<Unit>();
         }
         else
         {
            return Completed();
         }
      }
   }
}