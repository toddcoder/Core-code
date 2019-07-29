using System;
using Core.Applications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   internal class Program : CommandLineInterface
   {
      [EntryPoint]
      public void Main(string command, int code, double amount, AttributeTargets attributeTargets, string text = "")
      {
         Console.WriteLine($"command: '{command}'");
         Console.WriteLine($"code: {code}");
         Console.WriteLine($"amount: {amount}");
         Console.WriteLine($"attribute targets: {attributeTargets}");
         Console.WriteLine($"text: '{text}'");
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
         program.Run("/command: 'C:\\Enterprise\\Temp' /code: 153 /amount: 153.69 /attributeTargets: all /text: 'foo'", "/", ":");
      }
   }
}