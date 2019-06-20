using Core.Dates.Relative.DateOperations;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Dates.Relative.Parsers
{
   public abstract class Parser
   {
      protected Matcher matcher;

      public Parser() => matcher = new Matcher();

      public abstract string Pattern { get; }

      public abstract IMatched<DateOperation> Parse(string source, string[] tokens);

      public IMatched<DateOperation> Scan(string source)
      {
         var formatter = new Formatter
         {
            ["unit"] = "/('month' | 'year' | 'day')",
            ["units"] = "/('months'? | 'years'? | 'days'?)"
         };
         var pattern = formatter.Format(Pattern);
         if (matcher.IsMatch(source, pattern, true))
         {
            var tokens = matcher.Groups(0);
            return Parse(source, tokens);
         }
         else
         {
            return notMatched<DateOperation>();
         }
      }
   }
}