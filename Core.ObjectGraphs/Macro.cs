using System.Collections.Generic;
using Core.RegularExpressions;
using Core.Strings;

namespace Core.ObjectGraphs
{
   public class Macro
   {
      protected Formatter formatter;
      protected string replacementText;
      protected List<string> parameters;
      protected string pattern;

      public Macro(string name, string replacementText)
      {
         this.replacementText = replacementText;
         formatter = new Formatter();
         parameters = new List<string>();
         pattern = "/b '" + name.Replace("'", "`'") + "' '(' /(-[')']+) ')'";
      }

      public void AddParameter(string parameterName) => parameters.Add(parameterName);

      public string Invoke(string sourceText)
      {
         var matcher = new Matcher();
         var delimitedText = DelimitedText.BothQuotes();
         var source = delimitedText.Destringify(sourceText, true);
         if (matcher.IsMatch(source, pattern))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               var arguments = matcher[i, 1].Split("/s* ',' /s*");
               for (var j = 0; j < arguments.Length; j++)
               {
                  arguments[j] = delimitedText.Restringify(arguments[j], RestringifyQuotes.None);
               }

               matcher[i] = Invoke(arguments);
            }

            return delimitedText.Restringify(matcher.ToString(), RestringifyQuotes.None);
         }

         return sourceText;
      }

      public string Invoke(params string[] arguments)
      {
         var argumentsLength = arguments.Length;
         for (var i = 0; i < parameters.Count; i++)
         {
            if (i < argumentsLength)
            {
               formatter[parameters[i]] = arguments[i];
            }
         }

         return formatter.Format(replacementText);
      }
   }
}