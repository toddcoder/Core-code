using System;
using Core.Applications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   internal class Program : CommandLineInterface
   {
      [EntryPoint]
      public void Main(bool push = false, bool pull = false, bool show = false, int code = 0, double amount = 0,
         AttributeTargets attributeTargets = AttributeTargets.All, string text = "", bool recursive = true)
      {
         var command = "?";
         if (pull)
         {
            command = "pull";
         }
         else if (push)
         {
            command = "push";
         }
         else if (show)
         {
            command = "show";
         }

         Console.WriteLine($"command: '{command}'");
         Console.WriteLine($"code: {code}");
         Console.WriteLine($"amount: {amount}");
         Console.WriteLine($"attribute targets: {attributeTargets}");
         Console.WriteLine($"text: '{text}'");
         Console.WriteLine($"recursive: {recursive.ToString().ToLower()}");
      }
   }

   [TestClass]
   public class ApplicationTests
   {
      [TestMethod]
      public void RunTest()
      {
         var program = new Program();
         program.Run("foo.exe pull /code: 153 /amount: 153.69 /attributeTargets: all", "/", ":");
      }

      [TestMethod]
      public void GreedyTest()
      {
         var program = new Program();
         program.Run("\"foo.exe\" push /code: 153 /amount: 153.69 /attributeTargets: all /text: 'foo' /recursive: false", "/", ":");
      }

      [TestMethod]
      public void AlternateSyntax()
      {
         var program = new Program();
         program.Run("show --code 153 --amount 153.69 --attribute-targets all --text 'foo' --recursive", "--", " ");
      }
   }
}