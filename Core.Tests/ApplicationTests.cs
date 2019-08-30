﻿using System;
using Core.Applications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   internal class Parameters
   {
      public bool Push { get; set; }

      public bool Pull { get; set; }

      public bool Show { get; set; }

      public int Code { get; set; }

      public double Amount { get; set; }

      public AttributeTargets AttributeTargets { get; set; } = AttributeTargets.All;

      public string Text { get; set; } = "";

      public bool Recursive { get; set; } = true;
   }

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

   internal class ObjectProgram : CommandLineInterface
   {
      [EntryPoint(true)]
      public void Main(Parameters parameters)
      {
         var command = "?";
         if (parameters.Pull)
         {
            command = "pull";
         }
         else if (parameters.Push)
         {
            command = "push";
         }
         else if (parameters.Show)
         {
            command = "show";
         }

         Console.WriteLine($"command: '{command}'");
         Console.WriteLine($"code: {parameters.Code}");
         Console.WriteLine($"amount: {parameters.Amount}");
         Console.WriteLine($"attribute targets: {parameters.AttributeTargets}");
         Console.WriteLine($"text: '{parameters.Text}'");
         Console.WriteLine($"recursive: {parameters.Recursive.ToString().ToLower()}");
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

         Console.WriteLine();

         var objectProgram = new ObjectProgram();
         objectProgram.Run("foo.exe pull /code: 153 /amount: 153.69 /attributeTargets: all", "/", ":");
      }

      [TestMethod]
      public void GreedyTest()
      {
         var program = new Program();
         program.Run("\"foo.exe\" push /code: 153 /amount: 153.69 /attributeTargets: all /text: 'foo' /recursive: false", "/", ":");

         Console.WriteLine();

         var objectProgram = new ObjectProgram();
         objectProgram.Run("\"foo.exe\" push /code: 153 /amount: 153.69 /attributeTargets: all /text: 'foo' /recursive: false", "/", ":");
      }

      [TestMethod]
      public void AlternateSyntax()
      {
         var program = new Program();
         program.Run("show --code 153 --amount 153.69 --attribute-targets all --text 'foo' --recursive", "--", " ");

         Console.WriteLine();

         var objectProgram = new ObjectProgram();
         objectProgram.Run("show --code 153 --amount 153.69 --attribute-targets all --text 'foo' --recursive", "--", " ");
      }

      [TestMethod]
      public void Aliases()
      {
         var program = new Program { Application = "test" };
         program.Run("alias std show --code 153 --amount 153.69 --attribute-targets all --text 'foo' --recursive", "--", " ");
         program.Run("std", "--", " ");

         Console.WriteLine();

         var objectProgram = new ObjectProgram { Application = "test.object" };
         objectProgram.Run("alias std show --code 153 --amount 153.69 --attribute-targets all --text 'foo' --recursive", "--", " ");
         objectProgram.Run("std", "--", " ");
      }
   }
}