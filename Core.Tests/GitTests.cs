using System;
using Core.Assertions;
using Core.Computers;
using Core.Git;
using Core.Monads;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class GitTests
   {
      protected const string CORE_REPO = @"~\source\repos\toddcoder\Core";

      protected static void onSuccess(string[] lines)
      {
         foreach (var line in lines)
         {
            Console.WriteLine(line);
         }
      }

      protected static void onFailure(Exception exception) => Console.WriteLine($"Exception: {exception.Message}");

      [TestMethod]
      public void LogTest()
      {
         FolderName.Current = CORE_REPO;
         Git.Git.TryTo.Log("origin/develop..origin/master --pretty=format:\"%h %an %cn %s\"").OnSuccess(onSuccess).OnFailure(onFailure);
      }

      [TestMethod]
      public void LogTest2()
      {
         FolderName.Current = CORE_REPO;
         foreach (var result in Git.Git.Log("origin/develop..origin/master --pretty=format:\"%h %an %cn %s\""))
         {
            switch (result)
            {
               case GitSuccess:
                  Console.WriteLine("SUCCESS");
                  break;
               case GitError:
                  Console.WriteLine("ERROR");
                  break;
               case GitLine gitLine:
                  Console.WriteLine(gitLine);
                  break;

            }
            /*switch (result)
            {
               case Left<string, int> left:
                  Console.WriteLine(left.Value);
                  break;
               case Right<string, int> right when right.Value != Git.Git.GOOD_EXIT_CODE:
                  Console.WriteLine($"Error: {right.Value}");
                  break;
            }*/
         }
      }

      [TestMethod]
      public void FetchTest()
      {
         FolderName.Current = CORE_REPO;
         Git.Git.TryTo.Fetch().OnSuccess(onSuccess).OnFailure(onFailure);
      }

      [TestMethod]
      public void BadFetchTest()
      {
         FolderName.Current = @"C:\Temp";
         Git.Git.TryTo.Fetch().OnSuccess(onSuccess).OnFailure(onFailure);
      }

      [TestMethod]
      public void IsCurrentFolderInGitTest()
      {
         FolderName.Current = CORE_REPO;
         Git.Git.IsCurrentFolderInGit().Must().BeTrue().OrThrow();

         FolderName.Current = @"C:\Temp";
         Git.Git.IsCurrentFolderInGit().Must().Not.BeTrue().OrThrow();
      }

      [TestMethod]
      public void IsBranchOnRemoteTest()
      {
         FolderName.Current = CORE_REPO;
         var branch = GitBranch.Current;
         branch.IsOnRemote().Must().BeTrue().OrThrow();
      }

      [TestMethod]
      public void DifferentFromCurrentTest()
      {
         FolderName.Current = CORE_REPO;
         GitBranch master = "master";
         master.TryTo.DifferentFromCurrent().OnSuccess(onSuccess).OnFailure(onFailure);
      }

      [TestMethod]
      public void DifferentFromParentTest()
      {
         FolderName.Current = CORE_REPO;
         GitBranch develop = "origin/develop";
         var currentBranch = GitBranch.Current;
         currentBranch.TryTo.DifferentFrom(develop, true).OnSuccess(onSuccess).OnFailure(onFailure);
      }

      [TestMethod]
      public void ShowMergeCommitTest()
      {
         FolderName.Current = @"\\vmdvw10estm57\Estream";
         GitCommit commit = "2b7c75bdd0f9c186f21d5cf8f317a1f9a77559c3";
         commit.ShowMerge().OnSuccess(onSuccess).OnFailure(onFailure);
      }
   }
}