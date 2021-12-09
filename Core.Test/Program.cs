using System;
using Core.Applications.CommandProcessing;
using Core.Collections;
using Core.Computers;
using Core.Git;
using Core.Monads;
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
         prompt.Prompt().OnSuccess(Console.WriteLine).OnFailure(e => Console.WriteLine($"Exception: {e.Message}"));
      }

      public override StringHash GetConfigurationDefaults() => new StringHash(true);

      public override StringHash GetConfigurationHelp() => new StringHash(true);
   }
}
