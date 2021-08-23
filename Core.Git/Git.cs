using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Core.Assertions;
using Core.Dates.DateIncrements;
using Core.Exceptions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using Timer = System.Timers.Timer;

namespace Core.Git
{
   public class Git
   {
      protected internal const string MESSAGE_DIDNT_UNDERSTAND_ARGUMENT = "Didn't understand argument";
      private const string MESSAGE_BRANCH_MUST_HAVE_A_VALUE = "Branch must have a value";

      protected static Result<T> timedOutCall<T>(Func<T> func, TimeSpan timeout)
      {
         try
         {
            var timer = new Timer(100.0);
            var stopwatch = new Stopwatch();
            timer.Elapsed += (_, _) =>
            {
               if (stopwatch.Elapsed > timeout)
               {
                  throw "Function call exceed timeout".Fail<TimeoutException>();
               }

               Thread.Sleep(100);
            };

            timer.Start();
            stopwatch.Start();
            var call = func();
            timer.Stop();

            return call;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      protected Result<string> executeGit(string arguments)
      {
         try
         {
            using var process = new Process
            {
               StartInfo = new ProcessStartInfo("git.exe", arguments)
               {
                  UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true
               }
            };

            process.Start();
            process.WaitForExit(1000);

            return timedOutCall(() => process.StandardOutput.ReadToEnd().TrimEnd(), 1.Minute());
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      protected Either<string[], string> executeGitEnumerable(string arguments)
      {
         try
         {
            using var process = new Process
            {
               StartInfo = new ProcessStartInfo("git.exe", arguments)
               {
                  UseShellExecute = false,
                  CreateNoWindow = true,
                  RedirectStandardOutput = true
               }
            };

            IEnumerable<string> linesFromProcess()
            {
               while (true)
               {
                  var line = process.StandardOutput.ReadLine();
                  if (line.NotNull(out var notNullLine))
                  {
                     yield return notNullLine;
                  }
                  else
                  {
                     break;
                  }
               }
            }

            process.Start();
            process.WaitForExit(1000);

            var enumerable = linesFromProcess();
            return enumerable.ToArray();
         }
         catch (Exception exception)
         {
            return exception.Message;
         }
      }

      protected string origin;

      public Git(string origin = "origin")
      {
         this.origin = origin;
      }

      public Either<string[], string> Fetch(bool all) => executeGitEnumerable(all ? "--all" : "");

      public Either<string[], string> Branch(string branch = "", BranchArguments branchArguments = BranchArguments.List)
      {
         try
         {
            Maybe<string> _arguments = branchArguments switch
            {
               BranchArguments.List => "--list",
               BranchArguments.Delete => $"-d {branch.Must().Not.BeNullOrEmpty().Value}",
               BranchArguments.DeleteForce => $"-D {branch.Must().Not.BeNullOrEmpty().Value}",
               _ => nil
            };

            if (_arguments.If(out var arguments))
            {
               return executeGitEnumerable($"branch {arguments}");
            }
            else
            {
               return MESSAGE_DIDNT_UNDERSTAND_ARGUMENT;
            }
         }
         catch (Exception exception)
         {
            return exception.Message;
         }
      }

      public Either<string[], string> Checkout(string branch, CheckoutArguments checkoutArguments = CheckoutArguments.Checkout)
      {
         try
         {
            branch.Must().Not.BeNullOrEmpty().OrThrow(MESSAGE_BRANCH_MUST_HAVE_A_VALUE);

            Maybe<string> _arguments = checkoutArguments switch
            {
               CheckoutArguments.Checkout => branch,
               CheckoutArguments.Force => $"{branch} --force",
               CheckoutArguments.NewBranch => $"{branch} -b",
               _ => nil
            };
            if (_arguments.If(out var arguments))
            {
               return executeGitEnumerable($"checkout {arguments}");
            }
            else
            {
               return MESSAGE_DIDNT_UNDERSTAND_ARGUMENT;
            }
         }
         catch (Exception exception)
         {
            return exception.Message;
         }
      }

      public Either<string[], string> Merge(string branch)
      {
         try
         {
            branch.Must().Not.BeNullOrEmpty().OrThrow(MESSAGE_BRANCH_MUST_HAVE_A_VALUE);

            return executeGitEnumerable($"merge {branch}");
         }
         catch (Exception exception)
         {
            return exception.Message;
         }
      }

      public Either<string[], string> Pull() => executeGitEnumerable("pull");

      public Either<string[], string> PushFirst(string branch)
      {
         try
         {
            branch.Must().Not.BeNullOrEmpty().OrThrow(MESSAGE_BRANCH_MUST_HAVE_A_VALUE);
            return executeGitEnumerable($"branch --set-upstream {origin} {branch}");
         }
         catch (Exception exception)
         {
            return exception.Message;
         }
      }

      public Either<string[], string> Push(PushArguments pushArguments = PushArguments.All)
      {
         try
         {
            Maybe<string> _arguments = pushArguments switch
            {
               PushArguments.All => "push",
               PushArguments.Tags => "push --tags",
               _ => nil
            };
            if (_arguments.If(out var arguments))
            {
               return executeGitEnumerable(arguments);
            }
            else
            {
               return MESSAGE_DIDNT_UNDERSTAND_ARGUMENT;
            }
         }
         catch (Exception exception)
         {
            return exception.Message;
         }
      }

      public Either<string[], string> Log(string arguments) => executeGitEnumerable($"log {arguments}");
   }
}