using System;
using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.RegularExpressions
{
   public class MatcherTrying
   {
      protected Matcher matcher;

      public MatcherTrying(Matcher matcher) => this.matcher = matcher;

      public Result<bool> IsMatch(string input, string pattern, RegexOptions options)
      {
         return tryTo(() => matcher.IsMatch(input, pattern, options));
      }

      public Result<bool> IsMatch(string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return tryTo(() => matcher.IsMatch(input, pattern, ignoreCase, multiline));
      }

      public Result<bool> IsMatch(string input, RegexPattern regexPattern)
      {
         return tryTo(() => matcher.IsMatch(input, regexPattern));
      }

      public Result<Unit> Evaluate(string input, string pattern, RegexOptions options)
      {
         return tryTo(() => matcher.Evaluate(input, pattern, options));
      }

      public Result<Unit> Evaluate(string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return tryTo(() => matcher.Evaluate(input, pattern, ignoreCase, multiline));
      }

      public Result<Unit> Evaluate(string input, RegexPattern regexPattern)
      {
         return tryTo(() => matcher.Evaluate(input, regexPattern));
      }
   }
}