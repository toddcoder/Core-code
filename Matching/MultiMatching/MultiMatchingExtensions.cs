namespace Core.Matching.MultiMatching
{
   public static class MultiMatchingExtensions
   {
      public static MultiMatcher<T> Matching<T>(this string input) => new(input);
   }
}