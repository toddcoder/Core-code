﻿using System;
using Core.Applications.CommandProcessing;
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
         var program = new Program
         {
            Test = true
         };
         program.Run();
      }

      public Program() : base("core.test")
      {
         Repo = nil;
      }

      [Switch("repo","Folder","Repository")]
      public Maybe<string> Repo { get; set; }

      [Command("git-prompt","Display a git prompt","$repo?")]
      public void GitPrompt()
      {
         if (Repo.If(out var repo))
         {
            FolderName.Current = repo;
         }

         var prompt = new GitPrompt();
         prompt.Prompt().OnSuccess(p => writePrompt(p, prompt)).OnFailure(e => Console.WriteLine($"Exception: {e.Message}"));
      }

      protected void writePrompt(string message, GitPrompt prompt)
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

      public override StringHash GetConfigurationDefaults() => new StringHash(true);

      public override StringHash GetConfigurationHelp() => new StringHash(true);
   }
}
