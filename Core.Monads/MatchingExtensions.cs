namespace Standard.Types.Monads
{
   public static class MatchingExtensions
   {
      public static MatchingContext<T> Matching<T>(this IMatched<T> matched) => new MatchingContext<T>(matched);
   }
}