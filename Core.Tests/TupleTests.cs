using System;
using Core.Assertions;
using Core.Enumerables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

namespace Core.Tests
{
   [TestClass]
   public class TupleTests
   {
      [TestMethod]
      public void SplitTest()
      {
         var enumerable = array(("alpha", 1), ("bravo", 2), ("charlie", 3));
         var (enumerable1, enumerable2) = enumerable.Split();
         var text1 = enumerable1.ToString(", ");
         var text2 = enumerable2.ToString(", ");

         text1.Must().Equal("alpha, bravo, charlie").OrThrow();
         Console.WriteLine($"First enumerable = {text1}");

         text2.Must().Equal("1, 2, 3").OrThrow();
         Console.WriteLine($"Second enumerable = {text2}");
      }
   }
}