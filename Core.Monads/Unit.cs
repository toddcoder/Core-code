namespace Standard.Types.Monads
{
   public class Unit
   {
      public static Unit Value => new Unit();

      public static IMaybe<Unit> Some() => Value.Some<Unit>();

      public static IResult<Unit> Success() => Value.Success<Unit>();

      public static IMatched<Unit> Matched() => Value.Matched<Unit>();
   }
}