using System.Collections.Generic;
using System.Diagnostics;
using Core.Computers;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Git
{
   public static class Git
   {
      public const int GOOD_EXIT_CODE = 0;

      public static GitTry TryTo => new();

      public static IEnumerable<GitResult> Execute(string arguments)
      {
         var errors = new List<string>();
         using var process = new Process
         {
            StartInfo = new ProcessStartInfo("git.exe", $"--no-pager {arguments}")
            {
               UseShellExecute = false,
               CreateNoWindow = true,
               RedirectStandardOutput = true,
               RedirectStandardError = true,
               WorkingDirectory = FolderName.Current.FullPath
            }
         };

         process.ErrorDataReceived += (_, e) => errors.Add(e.Data);
         process.Start();
         process.BeginErrorReadLine();

         var _line = Maybe<string>.nil;
         _line = process.StandardOutput.ReadLine();
         if (_line.IsNone)
         {
            yield break;
         }

         while (!process.HasExited)
         {
            System.Threading.Thread.Sleep(100);
            process.WaitForExit(100);
         }

         var isGood = process.ExitCode == GOOD_EXIT_CODE;
         yield return isGood ? GitResult.Success : GitResult.Error;

         if (isGood)
         {
            if (_line.If(out var firstLine))
            {
               yield return firstLine;

               _line = nil;
            }

            while (true)
            {
               var line = process.StandardOutput.ReadLine();
               if (line == null)
               {
                  break;
               }

               yield return line;
            }
         }

         foreach (var error in errors)
         {
            yield return error;
         }
      }

      public static IEnumerable<GitResult> Log(string arguments) => Execute($"log {arguments}");

      public static IEnumerable<GitResult> Fetch() => Execute("fetch --all");

      public static bool IsCurrentFolderInGit()
      {
         foreach (var result in Execute("rev-parse --is-inside-work-tree"))
         {
            if (result is GitLine gitLine)
            {
               if (gitLine.Text.Contains("true"))
               {
                  return true;
               }
            }
         }

         return false;
      }

      public static IEnumerable<GitResult> ShortStatus() => Execute("status -s -b");
   }
}