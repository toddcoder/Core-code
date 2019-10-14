using System;
using System.Collections.Generic;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Configurations.Parsing
{
   public class ParsingState
   {
      protected string source;
      protected int index;
      protected Stack<string> indentations;

      public ParsingState(string source)
      {
         this.source = source;
         index = 0;
         indentations = new Stack<string>();
      }

      public string CurrentSource => source.Drop(index);

      public string CurrentIndentations => indentations.IsEmpty() ? string.Empty : indentations.Stringify("");

      public string CurrentIndentation => indentations.PeekIf().DefaultTo(() => "");

      protected string replaceShortcuts(string pattern)
      {
         return pattern.ReplaceAll(("|s|", "/[' ' /t]*"), ("|s+|", "/[' ' /t]+"), ("|t|", CurrentIndentation), ("|t+|", CurrentIndentations));
      }

      public IMatched<Matcher> Advance(string pattern, bool advanceIndex = true)
      {
         try
         {
            var newPattern = replaceShortcuts(pattern);
            var matcher = new Matcher();
            if (matcher.IsMatch(CurrentSource, newPattern))
            {
               if (advanceIndex)
               {
                  index = matcher.Index + matcher.Length;
               }

               return matcher.Matched();
            }
            else
            {
               return notMatched<Matcher>();
            }
         }
         catch (Exception exception)
         {
            return failedMatch<Matcher>(exception);
         }
      }

      public IMatched<Unit> AdvanceOverIndentation()
      {
         while (indentations.Count > 0)
         {
            var matched = Advance("|t|");
            if (matched.If(out _, out var anyException))
            {
               indentations.Pop();
               return AdvanceOverIndentation();
            }
            else if (anyException.If(out var exception))
            {
               return failedMatch<Unit>(exception);
            }
            else
            {
               return Unit.Matched();
            }
         }

         return notMatched<Unit>();
      }
   }
}