using System;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Git
{
   public class GitPrompt
   {
      protected AutoHash<PromptColor, string> backColorMap;
      protected AutoHash<PromptColor, string> foreColorMap;
      protected string aheadSymbol;
      protected string behindSymbol;
      protected string aheadBehindSymbol;
      protected string connectedSymbol;
      protected string notConnectedSymbol;
      protected string stagedSymbol;
      protected string unstagedSymbol;

      public GitPrompt()
      {
         PromptColor = PromptColor.Normal;

         backColorMap = new AutoHash<PromptColor, string>(_ => "Black")
         {
            [PromptColor.Normal] = "DarkGreen",
            [PromptColor.Ahead] = "Magenta",
            [PromptColor.Behind] = "Cyan",
            [PromptColor.AheadBehind] = "Red",
            [PromptColor.Modified] = "Green"
         };

         foreColorMap = new AutoHash<PromptColor, string>(_ => "White")
         {
            [PromptColor.Behind] = "Black"
         };

         aheadSymbol = "↑";
         behindSymbol = "↓";
         aheadBehindSymbol = "↕";
         connectedSymbol = "≡";
         notConnectedSymbol = "≢";
         stagedSymbol = "\u2713";
         unstagedSymbol = "\u2717";
      }

      public PromptColor PromptColor { get; set; }

      public string BackColor => backColorMap[PromptColor];

      public string ForeColor => foreColorMap[PromptColor];

      public string AheadSymbol
      {
         get => aheadSymbol;
         set => aheadSymbol = value;
      }

      public string BehindSymbol
      {
         get => behindSymbol;
         set => behindSymbol = value;
      }

      public string AheadBehindSymbol
      {
         get => aheadBehindSymbol;
         set => aheadBehindSymbol = value;
      }

      public string ConnectedSymbol
      {
         get => connectedSymbol;
         set => connectedSymbol = value;
      }

      public string NotConnectedSymbol
      {
         get => notConnectedSymbol;
         set => notConnectedSymbol = value;
      }

      public string StagedSymbol
      {
         get => stagedSymbol;
         set => stagedSymbol = value;
      }

      public string UnstagedSymbol
      {
         get => unstagedSymbol;
         set => unstagedSymbol = value;
      }

      public Result<string> Prompt()
      {
         try
         {
            var prompt = new List<string>();
            if (Git.TryTo.ShortStatus().If(out var lines, out var exception))
            {
               if (lines.Length == 0)
               {
                  PromptColor = PromptColor.Error;
                  return fail("Nothing returned");
               }

               PromptColor = PromptColor.Normal;

               var firstLine = lines[0];
               var branch = "";
               var hasRemote = false;
               var aheadBehind = "";
               if (firstLine.Find("...").If(out var tripleDotsIndex))
               {
                  var local = firstLine.Keep(tripleDotsIndex).TrimRight();
                  var remote = firstLine.Drop(tripleDotsIndex + 3).TrimLeft();
                  hasRemote = true;

                  if (local.Matches("^ '##' /s+ /(.+); f").If(out var result))
                  {
                     branch = result.FirstGroup;

                     if (remote.Matches("'[' /(-[ ']' ]+) ']'; f").If(out result))
                     {
                        aheadBehind = result.FirstGroup;
                     }
                  }
                  else
                  {
                     return fail($"Couldn't determine branch from {firstLine}");
                  }
               }
               else if (firstLine.Matches("^ '##' /s+ /(.+); f").If(out var result))
               {
                  branch = result.FirstGroup;
               }
               else
               {
                  return fail($"Couldn't determine branch from {firstLine}");
               }

               prompt.Add($"{branch}");

               if (aheadBehind.IsNotEmpty())
               {
                  var aheadCount = 0;
                  if (aheadBehind.Matches("'ahead' /s+ /(/d+); f").If(out var result))
                  {
                     aheadCount = Value.Int32(result.FirstGroup);
                  }

                  var behindCount = 0;
                  if (aheadBehind.Matches("'behind' /s+ /(/d+); f").If(out result))
                  {
                     behindCount = Value.Int32(result.FirstGroup);
                  }

                  if (aheadCount > 0 && behindCount > 0)
                  {
                     prompt.Add($"{aheadCount}{aheadBehindSymbol}{behindCount}");
                     PromptColor = PromptColor.AheadBehind;
                  }
                  else if (aheadCount > 0)
                  {
                     prompt.Add($"{aheadSymbol}{aheadCount}");
                     PromptColor = PromptColor.Ahead;
                  }
                  else if (behindCount > 0)
                  {
                     prompt.Add($"{behindSymbol}{behindCount}");
                     PromptColor = PromptColor.Behind;
                  }
               }
               else
               {
                  prompt.Add(hasRemote ? connectedSymbol : notConnectedSymbol);
               }

               var stagedCounter = new FileCounter(true);
               var unstagedCounter = new FileCounter(false);

               foreach (var line in lines.Skip(1))
               {
                  if (line.Matches("^ /(.) /(.); f").If(out var result))
                  {
                     var staged = result.FirstGroup;
                     var unstaged = result.SecondGroup;
                     stagedCounter.Increment(staged);
                     unstagedCounter.Increment(unstaged);
                  }
               }

               var item = stagedCounter.ToString();
               if (item.IsNotEmpty())
               {
                  prompt.Add(stagedSymbol);
                  prompt.Add(item);
                  PromptColor = PromptColor.Modified;
               }

               item = unstagedCounter.ToString();
               if (item.IsNotEmpty())
               {
                  prompt.Add(unstagedSymbol);
                  prompt.Add(item);
                  PromptColor = PromptColor.Modified;
               }

               return prompt.ToString(" ");
            }
            else
            {
               return exception;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
   }
}