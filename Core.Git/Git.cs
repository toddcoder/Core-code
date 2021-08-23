using System;
using System.Diagnostics;
using Core.Assertions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Git
{
   public class Git
   {
      protected internal const string MESSAGE_DIDNT_UNDERSTAND_ARGUMENT = "Didn't understand argument";
      private const string MESSAGE_BRANCH_MUST_HAVE_A_VALUE = "Branch must have a value";

      protected static Either<string[], string> executeGit(string arguments)
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
               return error;
            }
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

      public Either<string[], string> Fetch(bool all) => executeGit(all ? "fetch --all" : "fetch");

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
               return executeGit($"branch {arguments}");
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
               return executeGit($"checkout {arguments}");
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

            return executeGit($"merge {branch}");
         }
         catch (Exception exception)
         {
            return exception.Message;
         }
      }

      public Either<string[], string> Pull() => executeGit("pull");

      public Either<string[], string> PushFirst(string branch)
      {
         try
         {
            branch.Must().Not.BeNullOrEmpty().OrThrow(MESSAGE_BRANCH_MUST_HAVE_A_VALUE);
            return executeGit($"branch --set-upstream {origin} {branch}");
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
               return executeGit(arguments);
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

      public Either<string[], string> Log(string arguments) => executeGit($"log {arguments}");
   }
}