using System;
using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.RegularExpressions
{
   [Obsolete("Use Pattern and Results")]
   public class MatcherTrying
   {
      protected Matcher matcher;

      public MatcherTrying(Matcher matcher) => this.matcher = matcher;

      public IResult<bool> IsMatch(string input, string pattern, RegexOptions options)
      {
         return tryTo(() => matcher.IsMatch(input, pattern, options));
      }

      public IResult<bool> IsMatch(string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return tryTo(() => matcher.IsMatch(input, pattern, ignoreCase, multiline));
      }

      public IResult<bool> IsMatch(string input, RegexPattern regexPattern)
      {
         return tryTo(() => matcher.IsMatch(input, regexPattern));
      }

      public IResult<Unit> Evaluate(string input, string pattern, RegexOptions options)
      {
         return tryTo(() => matcher.Evaluate(input, pattern, options));
      }

      public IResult<Unit> Evaluate(string input, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         return tryTo(() => matcher.Evaluate(input, pattern, ignoreCase, multiline));
      }

      public IResult<Unit> Evaluate(string input, RegexPattern regexPattern)
      {
         return tryTo(() => matcher.Evaluate(input, regexPattern));
      }
   }
}