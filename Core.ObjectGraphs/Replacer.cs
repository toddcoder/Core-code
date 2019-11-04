﻿using System.Collections.Generic;
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

      Destringifier destringifier;
      Formatter formatter;
      List<Macro> macros;

      public Replacer(string source)
      {
         destringifier = new Destringifier(source);
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
         text = destringifier.Restring(text, !text.IsMatch("^ '//(' /d+ ')' $"));
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
               FileName file = destringifier.Restring(matcher[i, 1], false);
               matcher[i] = file.Exists() ? file.Text : "";
            }
         }

         return matcher.ToString();
      }

      public string Parse() => destringifier.Parse();

      public void Copy(Replacer replacer)
      {
         formatter = replacer.formatter;
         macros = replacer.macros;
      }
   }
}