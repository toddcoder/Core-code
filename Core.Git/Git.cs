using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Arrays;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Git
{
   public static class Git
   {
      public static Result<string[]> Execute(string arguments)
      {
         try
         {
            var errors = new List<string>();
            using var process = new Process
            {
               StartInfo = new ProcessStartInfo("git.exe", arguments)
               {
                  UseShellExecute = false,
                  CreateNoWindow = true,
                  RedirectStandardOutput = true,
                  RedirectStandardError = true
               }
            };

            process.ErrorDataReceived += (_, e) => errors.Add(e.Data);
            process.Start();
            process.BeginErrorReadLine();
            var enumerable = process.StandardOutput.ReadToEnd().TrimEnd().Lines();
            process.WaitForExit(1000);

            var error = errors.Where(e => e is not null).ToArray();

            if (process.ExitCode == 0)
            {
               return enumerable.Augment(error);
            }
            else
            {
               return fail(error.ToString(" "));
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public static Result<string[]> Log(string arguments) => Execute($"log {arguments}");

      public static Result<string[]> Fetch() => Execute("fetch --all");

      public static bool IsCurrentFolderInGit()
      {
         try
         {
            return Execute("rev-parse --is-inside-work-tree").Map(s => s.Any(i => i.Contains("true"))).Recover(_ => false);
         }
         catch
         {
            return false;
         }
      }

      public static Result<string[]> ShortStatus() => Execute("status -s -b");
   }
}