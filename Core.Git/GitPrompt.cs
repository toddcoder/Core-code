using System;
using System.Text;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Git
{
   public class GitPrompt
   {
      public Result<string> Prompt()
      {
         try
         {
            var prompt = new StringBuilder();
            if (Git.ShortStatus().If(out var lines, out var exception))
            {
               if (lines.Length == 0)
               {
                  return fail("Nothing returned");
               }

               var firstLine = lines[0];
               var branch = "";
               var hasRemote = false;
               if (firstLine.Matches("^ '##' /s+ /(-/s+) ('...' /(-/s+))?; f").If(out var result))
               {
                  branch = result.FirstGroup;
                  hasRemote = result.SecondGroup.IsNotEmpty();
               }
               else
               {
                  return fail($"Couldn't determine branch from {firstLine}");
               }

               prompt.Append(branch);
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