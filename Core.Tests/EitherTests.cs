using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Core.Monads;

namespace Core.Tests;

[TestClass]
public class EitherTests
{
   [TestMethod]
   public void CreatingTest()
   {
      Either<char, string> _left = 'a';
      Either<char, string> _right = "a";
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
      Either<int, double> _left = 10;
      var dLeft = _left.Map(i => (double)i, d => (int)d);
      if (dLeft.IfLeft(out var @double, out var @int))
      {
         Console.WriteLine($"double {@double} is good");
      }
      else
      {
         Console.WriteLine($"Unexpected int {@int}");
      }

      Either<int, double> _right = 7.0;
      var iRight = _right.Map(i => i / 2.0, d => (int)d / 2);
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
      var _char = ((Either<char, int>)'a').ResultFromLeft(i => $"Expected char; found {i}");
      if (_char)
      {
         Console.WriteLine($"char {_char} is good");
      }
      else
      {
         Console.WriteLine($"Exception: {_char.Exception.Message}");
      }

      _char = ((Either<char, int>)153).ResultFromLeft(i => $"Expected char; found {i}");
      if (_char)
      {
         Console.WriteLine($"char {_char} is good");
      }
      else
      {
         Console.WriteLine($"Exception: {_char.Exception.Message}");
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
      Either<int, char> either = 'a';
      Console.WriteLine(either);

      either = 10;
      Console.WriteLine(either);
   }
}