namespace Core.Matching.MultiMatching;

public static class MultiMatchingFunctions
{
   public static MultiMatcher<T> match<T>() => new();

   public static MultiMatcher match() => new();
}