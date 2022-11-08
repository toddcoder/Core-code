namespace Core.Monads.MultiMatching;

public static class MonadMatcherFunctions
{
   public static MaybeMatcher<T, TResult> maybeMatcher<T, TResult>() => new();

   public static MaybeMatcher<>
}