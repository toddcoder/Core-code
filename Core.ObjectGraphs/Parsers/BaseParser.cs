using System;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.ObjectGraphs.Parsers
{
   public abstract class BaseParser
   {
      public const string REGEX_NAME = "/(([/w '$@.'] [/w '-']* | '//(' /d+ ')') '*'?)";
      public const string REGEX_TYPE = "(/s* ':' /s* /([/w '$@.'] [/w '-']* (['(<{['] -[']}>)']* [']}>)'])?))?";
      public const string REGEX_VALUE = "/(.*)";
      public const string REGEX_SERVICE = "/(-[';']+) ';'";
      public const string REGEX_OBJECT_SERVICE_REFERENCE = "^ /s* " + REGEX_NAME + " '(' /(-[')']*) ')' $";
      public const string REGEX_STEM = "(['-='] '>')";

      public static int TabCount;

      static BaseParser() => TabCount = -1;

      public static void Assert(bool test, string[] source, int position, string message)
      {
         if (test)
         {
            return;
         }

         message = $"line '{source[position]}' at {position + 1}: {message}";
         throw new ApplicationException(message);
      }

      protected Matcher matcher;
      protected IMaybe<int> overridePosition;

      public BaseParser() => matcher = new Matcher();

      public ObjectGraph Result { get; set; }

      public int Position { get; set; }

      public string[] Source { get; set; }

      public abstract string Pattern { get; }

      public abstract bool Scan(string[] tokens, int position, Replacer replacer);

      public bool Parse(string[] source, int position, Replacer replacer)
      {
         var line = source[position];
         line = replacer.ReplaceVariables(line);
         var pattern = Pattern.Replace("{sp}", "/t".Repeat(TabCount));
         if (matcher.IsMatch(line, pattern))
         {
            var tokens = matcher.Groups(0);
            overridePosition = none<int>();
            if (Scan(tokens, position, replacer))
            {
               Position = overridePosition.DefaultTo(() => position + 1);
               return true;
            }
            else
            {
               return false;
            }
         }
         else
         {
            return false;
         }
      }
   }
}