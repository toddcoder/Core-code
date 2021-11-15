using System;

namespace Core.Matching.MultiMatching
{
   public class Case
   {
      protected MultiMatcher multiMatcher;

      public Case(MultiMatcher multiMatcher, Pattern pattern)
      {
         this.multiMatcher = multiMatcher;
         Pattern = pattern;
      }

      public Pattern Pattern { get; }

      public MultiMatcher Then(Action<MatchResult> action)
      {
         multiMatcher.AddPattern(Pattern, action);
         return multiMatcher;
      }
   }
}