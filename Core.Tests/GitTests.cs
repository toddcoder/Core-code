using System;
using Core.Computers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class GitTests
   {
      protected static void onLeft(string[] lines)
      {
         foreach (var line in lines)
         {
            Console.WriteLine(line);
         }
      }

      protected static void onRight(string message) => Console.WriteLine($"Exception: {message}");

      [TestMethod]
      public void LogTest()
      {
         FolderName.Current = @"C:\Enterprise\Projects\Core";
         var git = new Git.Git("master");
         git.Log("origin/develop..origin/master").OnLeft(onLeft).OnRight(onRight);
      }

      [TestMethod]
      public void FetchTest()
      {
         FolderName.Current = @"C:\Enterprise\Projects\Core";
         var git = new Git.Git();
         git.Fetch(true).OnLeft(onLeft).OnRight(onRight);
      }
   }
}