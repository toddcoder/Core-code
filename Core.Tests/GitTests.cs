using System;
using Core.Computers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class GitTests
   {
      [TestMethod]
      public void LogTest()
      {
         FolderName.Current = @"C:\Enterprise\Projects\Core";
         var git = new Git.Git();
         var _result = git.Log("origin/develop..origin/master");
         if (_result.IfLeft(out var lines, out var message))
         {
            foreach (var line in lines)
            {
               Console.WriteLine(line);
            }
         }
         else
         {
            Console.WriteLine(message);
         }
      }

      [TestMethod]
      public void FetchTest()
      {
         FolderName.Current = @"C:\Enterprise\Projects\Core";
         var git = new Git.Git();
         var _result = git.Fetch(true);
         if (_result.IfLeft(out var lines, out var message))
         {
            foreach (var line in lines)
            {
               Console.WriteLine(line);
            }
         }
         else
         {
            Console.WriteLine(message);
         }
      }
   }
}