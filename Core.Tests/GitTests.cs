using System;
using Core.Computers;
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
   }
}