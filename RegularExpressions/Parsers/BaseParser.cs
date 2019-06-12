using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.RegularExpressions.Parsers
{
   public abstract class BaseParser
   {
      public const string REGEX_IDENTIFIER = "[a-zA-Z_][a-zA-Z_0-9]*";
      public const string REGEX_BAL_IDENTIFIER = "[a-zA-Z_][a-zA-Z_0-9-]*";

      protected Matcher matcher;
      protected string[] tokens;

      public BaseParser()
      {
         matcher = new Matcher(false);
         tokens = new string[0];
      }

      public abstract string Pattern { get; }

      public IMaybe<string> Scan(string source, ref int index)
      {
         var input = source.Substring(index);
         if (matcher.IsMatch(input, Pattern))
         {
            var oldIndex = index;
            index += matcher[0].Length;
            tokens = matcher.Groups(0);
            var result = Parse(source, ref index);
            if (result.IsSome)
               return result;
            else
            {
               index = oldIndex;
               return result;
            }
         }
         else
            return none<string>();
      }

      public abstract IMaybe<string> Parse(string source, ref int index);
   }
}