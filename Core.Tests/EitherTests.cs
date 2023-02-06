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
      if (_left is ((true, var left), _))
      {
         Console.WriteLine($"char {left}");
      }

      if (_right is (_, (true, var right)))
      {
         Console.WriteLine($"string {right}");
      }
   }

   [TestMethod]
   public void MappingTest()
   {
      Either<int, double> _left = 10;
      var dLeft = _left.Map(i => (double)i, d => (int)d);
      switch (dLeft)
      {
         case ((true, var @double), _):
            Console.WriteLine($"double {@double} is good");
            break;
         case ((_, (true, var @int))):
            Console.WriteLine($"int {@int} is good");
            break;
      }

      Either<int, double> _right = 7.0;
      var iRight = _right.Map(i => i / 2.0, d => (int)d / 2);
      switch (iRight)
      {
         case ((true, var @double), _):
            Console.WriteLine($"double {@double} is good");
            break;
         case ((_, (true, var @int))):
            Console.WriteLine($"int {@int} is good");
            break;
      }
   }

   [TestMethod]
   public void DefaultTest()
   {
      var left = 'a'.Either<int, char>() | 'a';
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