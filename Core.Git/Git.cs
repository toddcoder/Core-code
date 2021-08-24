using System;
using System.Diagnostics;
using Core.Assertions;
using Core.Exceptions;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Git
{
   public class Git
   {
      protected internal const string MESSAGE_DIDNT_UNDERSTAND_ARGUMENT = "Didn't understand argument";

      protected static Either<string[], string> executeNoBranchGit(string arguments)
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

      protected string branch;
      private string origin;

      public Git()
      {
         branch = CurrentBranch;
         origin = "origin";
      }

      public Git(string branch)
      {
         branch.Must().Not.BeNullOrEmpty().OrThrow("Branch must have a value");

         this.branch = branch;
         origin = "origin";
      }

      public string Origin
      {
         get => origin;
         set
         {
            origin.Must().Not.BeNullOrEmpty().OrThrow("Origin must have a value");
            origin = value;
         }
      }

      public static string CurrentBranch
      {
         get
         {
            if (executeNoBranchGit("rev-parse --abbrev-ref HEAD").IfLeft(out var lines, out var message))
            {
               return lines.Length > 0 ? lines[0].Trim() : string.Empty;
            }
            else
            {
               throw message.Fail();
            }
         }
      }

      public string WorkingBranch
      {
         get => branch;
         set => branch = value;
      }

      protected Either<string[], string> executeGit(string arguments)
      {
         try
         {
            var error = string.Empty;
            using var process = new Process
            {
               StartInfo = new ProcessStartInfo("git.exe", arguments.Substitute("'$branch' /b; f", branch))
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

      public Either<string[], string> Fetch(bool all) => executeGit(all ? "fetch --all" : "fetch");

      public Either<string[], string> Branch(BranchArguments branchArguments = BranchArguments.List)
      {
         try
         {
            Maybe<string> _arguments = branchArguments switch
            {
               BranchArguments.List => "--list",
               BranchArguments.Delete => $"-d {branch}",
               BranchArguments.DeleteForce => $"-D {branch}",
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

      public Either<string[], string> Checkout(CheckoutArguments checkoutArguments = CheckoutArguments.Checkout)
      {
         try
         {
            Maybe<string> _arguments = checkoutArguments switch
            {
               CheckoutArguments.Checkout => branch,
               CheckoutArguments.Force => $"{branch} --force",
               CheckoutArguments.NewBranch => $"{branch} -b",
               _ => nil
            };
            if (_arguments.If(out var arguments))
            {
               return executeGit($"checkout {arguments}").OnLeft(_ => branch = CurrentBranch);
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

      public Either<string[], string> Merge()
      {
         try
         {
            return executeGit($"merge {branch}");
         }
         catch (Exception exception)
         {
            return exception.Message;
         }
      }

      public Either<string[], string> Pull() => executeGit("pull");

      public Either<string[], string> PushFirst()
      {
         try
         {
            return executeGit($"branch --set-upstream {Origin} {branch}");
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