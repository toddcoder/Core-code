using System;
using Core.Assertions;
using Core.Computers;
using Core.Git;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class GitTests
   {
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
         FolderName.Current = @"C:\Enterprise\Projects\Core";
         Git.Git.Log("origin/develop..origin/master --pretty=format:\"%h %an %cn %s\"").OnSuccess(onSuccess).OnFailure(onFailure);
      }

      [TestMethod]
      public void FetchTest()
      {
         FolderName.Current = @"C:\Enterprise\Projects\Core";
         Git.Git.Fetch().OnSuccess(onSuccess).OnFailure(onFailure);
      }

      [TestMethod]
      public void IsCurrentFolderInGitTest()
      {
         FolderName.Current = @"~\source\repos\toddcoder\Core";
         Git.Git.IsCurrentFolderInGit().Must().BeTrue().OrThrow();

         FolderName.Current = @"C:\Temp";
         Git.Git.IsCurrentFolderInGit().Must().Not.BeTrue().OrThrow();
      }

      [TestMethod]
      public void IsBranchOnRemoteTest()
      {
         FolderName.Current = @"~\source\repos\toddcoder\Core";
         var branch = GitBranch.Current;
         branch.IsOnRemote().Must().BeTrue().OrThrow();
      }

      [TestMethod]
      public void DifferentFromCurrentTest()
      {
         FolderName.Current = @"~\source\repos\toddcoder\Core";
         GitBranch master = "master";
         master.DifferentFromCurrent().OnSuccess(onSuccess).OnFailure(onFailure);
      }
   }
}