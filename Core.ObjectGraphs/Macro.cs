using System.Collections.Generic;
using Core.RegularExpressions;
using Core.Strings;

namespace Core.ObjectGraphs
{
   public class Macro
   {
      Formatter formatter;
      string replacementText;
      List<string> parameters;
      string pattern;

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
         var destringifier = new Destringifier(sourceText);
         var source = destringifier.Parse();
         if (matcher.IsMatch(source, pattern))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               var arguments = matcher[i, 1].Split("/s* ',' /s*");
               for (var j = 0; j < arguments.Length; j++)
               {
                  arguments[j] = destringifier.Restring(arguments[j], false);
               }

               matcher[i] = Invoke(arguments);
            }

            return destringifier.Restring(matcher.ToString(), true);
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