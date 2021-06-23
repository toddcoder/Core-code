namespace Core.Monads
{
   public static class MatchingExtensions
   {
      public static MatchingContext<T> Matching<T>(this Matched<T> matched) => new(matched);
   }
}