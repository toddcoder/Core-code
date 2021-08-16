using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Core.Monads;

namespace Core.Tests
{
   [TestClass]
   public class EitherTests
   {
      [TestMethod]
      public void CreatingTest()
      {
         var _left = 'a'.Either<char, string>();
         var _right = "a".Either<char, string>();
         if (_left.IfLeft(out var left))
         {
            Console.WriteLine($"char {left}");
         }

         if (_right.IfRight(out var right))
         {
            Console.WriteLine($"string {right}");
         }
      }

      [TestMethod]
      public void MappingTest()
      {
         var left = 10.Either<int, double>();
         var dLeft = left.Map(i => (double)i, d => (int)d);
         if (dLeft.IfLeft(out var @double, out var @int))
         {
            Console.WriteLine($"double {@double} is good");
         }
         else
         {
            Console.WriteLine($"Unexpected int {@int}");
         }

         var right = 7.0.Either<int, double>();
         var iRight = right.Map(i => i / 2.0, d => (int)d / 2);
         if (iRight.IfLeft(out @double, out @int))
         {
            Console.WriteLine($"Unexpected double {@double}");
         }
         else
         {
            Console.WriteLine($"int {@int} is good");
         }
      }

      [TestMethod]
      public void ResultTest()
      {
         var charResult = 'a'.Either<char, int>().ResultFromLeft(i => $"Expected char; found {i}");
         if (charResult.If(out var @char, out var exception))
         {
            Console.WriteLine($"char {@char} is good");
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }

         var intResult = 153.Either<char, int>().ResultFromLeft(i => $"Expected char; found {i}");
         if (intResult.If(out @char, out exception))
         {
            Console.WriteLine($"char {@char} is good");
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }

      [TestMethod]
      public void DefaultTest()
      {
         var left = 'a'.Either<int, char>().DefaultToLeft(() => 'a');
         Console.WriteLine($"{left}: {left.GetType().Name}");
      }

      [TestMethod]
      public void ImplicitTest()
      {
         Either<int, char> either = 'a'.Right();
         Console.WriteLine(either);

         either = 10.Left();
         Console.WriteLine(either);
      }
   }
}
