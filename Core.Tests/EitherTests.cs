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
   }
}
