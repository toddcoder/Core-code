using System.Collections.Generic;
using System.Linq;
using Core.Computers;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.ObjectGraphs
{
   public class Replacer
   {
      public static Replacer Duplicate(IMaybe<Replacer> replacer, string source)
      {
         if (replacer.If(out var oldReplacer))
         {
            var newReplacer = new Replacer(source);
            newReplacer.Copy(oldReplacer);
            return newReplacer;
         }

         return new Replacer(source);
      }

      protected string source;
      protected DelimitedText delimitedText;
      protected Formatter formatter;
      protected List<Macro> macros;

      public Replacer(string source)
      {
         this.source = source;
         delimitedText = DelimitedText.BothQuotes();
         formatter = new Formatter();
         macros = new List<Macro>();
      }

      public string this[string variable]
      {
         get => formatter[variable];
         set => formatter[variable] = value;
      }

      public void AddMacro(Macro macro) => macros.Add(macro);

      public string Replace(string text)
      {
         text = TextFromFile(text);
         text = delimitedText.Restringify(text, RestringifyQuotes.None);
         text = formatter.Format(text);

         return macros.Aggregate(text, (current, macro) => macro.Invoke(current));
      }

      public static IMaybe<ObjectGraphReference> ResolveAsterisk(string name) => maybe(name.EndsWith("*"), () =>
      {
         name = name.Front();
         return new ObjectGraphReference(name, Key(name));
      });

      public static string Key(string name) => $"{name}{uniqueID()}";

      public string ReplaceVariables(string text) => formatter.Format(text);

      public string TextFromFile(string text)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(text, "'{@' /(-['}']+) '}'"))
         {
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               FileName file = delimitedText.Restringify(matcher[i, 1], RestringifyQuotes.None);
               matcher[i] = file.Exists() ? file.Text : "";
            }
         }

         return matcher.ToString();
      }

      public string Parse() => delimitedText.Destringify(source);

      public void Copy(Replacer replacer)
      {
         formatter = replacer.formatter;
         macros = replacer.macros;
      }
   }
}