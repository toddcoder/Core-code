using System;
using System.Diagnostics;
using Core.Exceptions;
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
            var error = string.Empty;
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

            process.ErrorDataReceived += (_, e) => error += e.Data;
            process.Start();
            process.BeginErrorReadLine();
            var enumerable = process.StandardOutput.ReadToEnd().TrimEnd().Lines();
            process.WaitForExit(1000);

            if (error.IsEmpty())
            {
               return enumerable;
            }
            else
            {
               return fail(error);
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public static Result<string[]> Log(string arguments) => Execute($"log {arguments}");

      public static Result<string[]> Fetch() => Execute("fetch --all");
   }
}