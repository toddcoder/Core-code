using System;
using Core.Applications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   internal class Program : CommandLineInterface
   {
      [EntryPoint]
      public void Main(string command, int code, double amount, AttributeTargets attributeTargets, string text = "", bool recursive = true)
      {
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
         program.Run("/command: 'foobar' /code: 153 /amount: 153.69 /attributeTargets: all", "/", ":");
      }

      [TestMethod]
      public void GreedyTest()
      {
         var program = new Program();
         program.Run("/command: 'C:\\Enterprise\\Temp' /code: 153 /amount: 153.69 /attributeTargets: all /text: 'foo' /recursive: false", "/", ":");
      }

      [TestMethod]
      public void AlternateSyntax()
      {
         var program = new Program();
         program.Run("--command 'C:\\Enterprise\\Temp' --code 153 --recursive --amount 153.69 --attribute-targets all --text 'foo'", "--", " ");
      }
   }
}