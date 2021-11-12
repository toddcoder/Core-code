namespace Core.Matching.MultiMatching
{
   public static class MultiMatchingExtensions
   {
      public static MultiMatcher MatchFirst(this string input) => new(input);
   }
}