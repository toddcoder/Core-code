namespace Core.Monads
{
   public class Unit
   {
      public static Unit Value => new Unit();

      public static IMaybe<Unit> Some() => Value.Some();

      public static IResult<Unit> Success() => Value.Success();

      public static IMatched<Unit> Matched() => Value.Matched();
   }
}