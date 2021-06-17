using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using RRegex = System.Text.RegularExpressions.Regex;

namespace Core.RegularExpressions.Parsers
{
   public abstract class BaseParser
   {
      public const string REGEX_IDENTIFIER = "[a-zA-Z_][a-zA-Z_0-9]*";
      public const string REGEX_BAL_IDENTIFIER = "[a-zA-Z_][a-zA-Z_0-9-]*";

      protected string[] tokens;
      protected RRegex regexIdentifier;
      protected RRegex regexBalIdentifier;

      public BaseParser()
      {
         tokens = new string[0];
         regexIdentifier = new RRegex(REGEX_IDENTIFIER, RegexOptions.Compiled);
         regexBalIdentifier = new RRegex(REGEX_BAL_IDENTIFIER, RegexOptions.Compiled);
      }

      public abstract string Pattern { get; }

      protected static string escape(string text) => RRegex.Escape(text);

      public IMaybe<string> Scan(string source, ref int index)
      {
         static IEnumerable<Group> getGroups(Match match)
         {
            foreach (Group group in match.Groups)
            {
               yield return group;
            }
         }

         var options = RegexOptions.Compiled;
         var regex = new RRegex(Pattern, options);

         var input = source.Drop(index);
         var matches = regex.Matches(input);
         if (matches.Count > 0)
         {
            var oldIndex = index;

            index += matches[0].Length;
            tokens = getGroups(matches[0]).Select(group => group.Value).ToArray();
            var result = Parse(source, ref index);
            if (result.IsSome)
            {
               return result;
            }
            else
            {
               index = oldIndex;
               return result;
            }
         }
         else
         {
            return none<string>();
         }
      }

      public abstract IMaybe<string> Parse(string source, ref int index);
   }
}