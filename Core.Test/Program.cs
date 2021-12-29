using System;
using Core.Applications.CommandProcessing;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Git;
using Core.Monads;
using static Core.Applications.ConsoleFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Test
{
   public class Program : CommandProcessor
   {
      public static void Main()
      {
         var program = new Program();
         program.Run();
      }

      public Program() : base("core.test")
      {
         Repo = nil;
         GitArguments = string.Empty;
      }

      [Switch("test", "boolean", "Test mode")]
      public bool Testing
      {
         get => Test;
         set => Test = value;
      }

      [Switch("repo", "Folder", "Repository")]
      public Maybe<string> Repo { get; set; }

      [Switch("alt-sym", "boolean", "Use alternate symbols for ASCII strings")]
      public bool AlternateSymbols { get; set; }

      [Command("git-prompt", "Display a git prompt", "$repo?")]
      public void GitPrompt()
      {
         if (Repo.If(out var repo))
         {
            FolderName.Current = repo;
         }

         var prompt = new GitPrompt();
         if (AlternateSymbols)
         {
            prompt.ConnectedSymbol = "[|]";
            prompt.NotConnectedSymbol = "[ ]";
            prompt.StagedSymbol = "[*]";
            prompt.UnstagedSymbol = "[ ]";
         }

         prompt.Prompt().OnSuccess(p => writePrompt(p, prompt)).OnFailure(e => Console.WriteLine($"Exception: {e.Message}"));
      }

      protected static void writePrompt(string message, GitPrompt prompt)
      {
         var backColor = Console.BackgroundColor;
         var foreColor = Console.ForegroundColor;

         try
         {
            Console.BackgroundColor = consoleColorFromName(prompt.BackColor).DefaultTo(() => backColor);
            Console.ForegroundColor = consoleColorFromName(prompt.ForeColor).DefaultTo(() => foreColor);
            Console.WriteLine(message);
         }
         catch (Exception exception)
         {
            Console.WriteLine(exception);
         }
         finally
         {
            Console.BackgroundColor = backColor;
            Console.ForegroundColor = foreColor;
         }
      }

      [Switch("args", "string", "Arguments supplied by user", "a")]
      public string GitArguments { get; set; }

      [Command("log", "Execute a log in git with supplied arguments", "$args")]
      public void Log()
      {
         try
         {
            GitArguments.Must().Not.BeEmpty().OrThrow();

            foreach (var result in Git.Git.Log(GitArguments))
            {
               switch (result)
               {
                  case GitError:
                     Console.WriteLine("Error");
                     break;
                  case GitLine gitLine:
                     Console.WriteLine($"   {gitLine}");
                     break;
               }
            }
         }
         catch (Exception exception)
         {
            HandleException(exception);
         }
      }

      public override StringHash GetConfigurationDefaults() => new(true);

      public override StringHash GetConfigurationHelp() => new(true);
   }
}