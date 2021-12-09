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
      protected Hash<PromptColor, int> backColorMap;
      protected Hash<PromptColor, int> foreColorMap;

      public GitPrompt()
      {
         PromptColor = PromptColor.Normal;

         backColorMap = new AutoHash<PromptColor, int>(0xffffff);
         backColorMap[PromptColor.Normal] = 0x00ff00;
         backColorMap[PromptColor.Ahead] = 0xff00ff;
         backColorMap[PromptColor.Behind] = 0x00ffff;
         backColorMap[PromptColor.AheadBehind] = 0xffff00;
         backColorMap[PromptColor.Modified] = 0xffff00;

         foreColorMap = new AutoHash<PromptColor, int>(0xffffff);
      }

      public PromptColor PromptColor { get; set; }

      public int BackColor => backColorMap[PromptColor];

      public int ForeColor => foreColorMap[PromptColor];

      public Result<string> Prompt()
      {
         try
         {
            var prompt = new List<string>();
            if (Git.ShortStatus().If(out var lines, out var exception))
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
               if (firstLine.Matches("^ '##' /s+ /([/w '-']+) ('...' /([/w '//-']+))? (/s+ '[' /(-[']']+) ']')?; f").If(out var result))
               {
                  branch = result.FirstGroup;
                  hasRemote = result.SecondGroup.IsNotEmpty();
                  aheadBehind = result.ThirdGroup;
               }
               else
               {
                  return fail($"Couldn't determine branch from {firstLine}");
               }

               prompt.Add($"{branch}");

               if (aheadBehind.IsNotEmpty())
               {
                  var aheadCount = 0;
                  if (aheadBehind.Matches("'ahead' /s+ /(/d+)").If(out result))
                  {
                     aheadCount = Value.Int32(result.FirstGroup);
                  }

                  var behindCount = 0;
                  if (aheadBehind.Matches("'behind' /s+ /(/d+)").If(out result))
                  {
                     behindCount = Value.Int32(result.FirstGroup);
                  }

                  if (aheadCount > 0 && behindCount > 0)
                  {
                     prompt.Add($"{aheadCount}↕{behindCount}");
                     PromptColor = PromptColor.AheadBehind;
                  }
                  else if (aheadCount > 0)
                  {
                     prompt.Add($"↑{aheadCount}");
                     PromptColor = PromptColor.Ahead;
                  }
                  else if (behindCount > 0)
                  {
                     prompt.Add($"↓{behindCount}");
                     PromptColor = PromptColor.Behind;
                  }
               }
               else
               {
                  prompt.Add(hasRemote ? "≡" : "≢");
               }

               var indexedCounter = new FileCounter(true);
               var unindexedCounter = new FileCounter(false);

               foreach (var line in lines.Skip(1))
               {
                  if (line.Matches("^ /(.) /(.)").If(out result))
                  {
                     var indexed = result.FirstGroup;
                     var unindexed = result.SecondGroup;
                     indexedCounter.Increment(indexed);
                     unindexedCounter.Increment(unindexed);
                  }
               }

               var item = indexedCounter.ToString();
               if (item.IsNotEmpty())
               {
                  prompt.Add("|");
                  prompt.Add(item);
                  PromptColor = PromptColor.Modified;
               }

               item = unindexedCounter.ToString();
               if (item.IsNotEmpty())
               {
                  prompt.Add("|");
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