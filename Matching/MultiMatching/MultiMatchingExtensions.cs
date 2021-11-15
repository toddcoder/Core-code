namespace Core.Matching.MultiMatching
{
   public static class MultiMatchingExtensions
   {
      public static MultiMatcher Matching(this string input) => new(input);
   }
}