﻿using System;
using Core.Applications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   class Program : CommandLine
   {
      public override void Execute(Arguments arguments)
      {
         Run("/", ":", "/command: 'foobar' /code: 153 /amount: 153.69 /attributeTargets: all");
      }

      public void Main(string command, int code, double amount, AttributeTargets attributeTargets)
      {
         Console.WriteLine($"command: '{command}'");
         Console.WriteLine($"code: {code}");
         Console.WriteLine($"amount: {amount}");
         Console.WriteLine($"attribute targets: {attributeTargets}");
      }
   }

   [TestClass]
   public class ApplicationTests
   {
      [TestMethod]
      public void RunTest()
      {
         var program = new Program();
         program.Execute(new Arguments());
      }
   }
}