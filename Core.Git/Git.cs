using System.Collections.Generic;
using System.Diagnostics;
using Core.Computers;

namespace Core.Git;

public static class Git
{
   public const int GOOD_EXIT_CODE = 0;

   public static GitTry TryTo => new();

   public static IEnumerable<GitResult> Execute(string arguments)
   {
      var lines = new List<string>();
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

      process.OutputDataReceived += (_, e) => lines.Add(e.Data);
      process.ErrorDataReceived += (_, e) => errors.Add(e.Data);
      process.Start();
      process.BeginErrorReadLine();
      process.BeginOutputReadLine();
      process.WaitForExit();

      var isGood = process.ExitCode == GOOD_EXIT_CODE;
      yield return isGood ? GitResult.Success : GitResult.Error;

      if (isGood)
      {
         foreach (var line in lines)
         {
            yield return line;
         }
      }

      foreach (var error in errors)
      {
         yield return error;
      }
   }

   public static IEnumerable<GitResult> Log(string arguments, bool oneLine = false)
   {
      return Execute($"log {arguments}{(oneLine ? " --oneline" : "")}");
   }

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

   public static IEnumerable<GitResult> Revert(string commit) => Execute($"revert --no-edit -m 1 {commit}");

   public static IEnumerable<GitResult> CherryPick(string reference) => Execute($"cherry-pick {reference}");
}