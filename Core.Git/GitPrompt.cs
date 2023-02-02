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

namespace Core.Git;

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
         var _lines = Git.TryTo.ShortStatus();
         if (_lines is (true, var lines))
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
            var _tripleDotsIndex = firstLine.Find("...");
            if (_tripleDotsIndex)
            {
               var local = firstLine.Keep(_tripleDotsIndex).TrimRight();
               var remote = firstLine.Drop(_tripleDotsIndex + 3).TrimLeft();
               hasRemote = true;

               var _branch = local.Matches("^ '##' /s+ /(.+); f").Map(r => r.FirstGroup);
               if (_branch)
               {
                  branch = _branch;

                  var _aheadBehind = remote.Matches("'[' /(-[ ']' ]+) ']'; f").Map(r => r.FirstGroup);
                  if (_aheadBehind)
                  {
                     aheadBehind = _aheadBehind;
                  }
               }
               else
               {
                  return fail($"Couldn't determine branch from {firstLine}");
               }
            }
            else
            {
               var _branch = firstLine.Matches("^ '##' /s+ /(.+); f").Map(r => r.FirstGroup);
               if (_branch)
               {
                  branch = _branch;
               }
               else
               {
                  return fail($"Couldn't determine branch from {firstLine}");
               }
            }

            prompt.Add($"{branch}");

            if (aheadBehind.IsNotEmpty())
            {
               var aheadCount = 0;
               var _aheadCount = aheadBehind.Matches("'ahead' /s+ /(/d+); f").Map(r => r.FirstGroup).Map(Maybe.Int32);
               if (_aheadCount)
               {
                  aheadCount = _aheadCount;
               }

               var behindCount = 0;
               var _behindCount = aheadBehind.Matches("'behind' /s+ /(/d+); f").Map(r => r.FirstGroup).Map(Maybe.Int32);
               if (_behindCount)
               {
                  behindCount = _behindCount;
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
               var _result = line.Matches("^ /(.) /(.); f");
               if (_result is (true, var (staged, unstaged)))
               {
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
            return _lines.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}