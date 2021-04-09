using System;
using System.Linq;
using Core.Assertions;
using Core.Collections;
using Core.Numbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;
using static Core.Assertions.AssertionFunctions;

namespace Core.Tests
{
   [TestClass]
   public class SetTests
   {
      [TestMethod]
      public void SetOfStringVsStringSetTest()
      {
         var setOfString = new Set<string> { "Case" };
         var stringSet = new StringSet(true) { "Case" };

         setOfString.Remove("case");
         assert(() => setOfString.Count).Must().Equal(1).OrThrow();

         stringSet.Remove("case");
         assert(() => stringSet.Count).Must().Equal(0).OrThrow();
      }

      [TestMethod]
      public void SetOperatorsTest()
      {
         var union = Set<int>.Of(0.Until(5).Select(i => i * 2)) | Set<int>.Of(0.Until(5).Select(i => i * 2 + 1));
         var intersection = Set<int>.Of(3, 4) & Set<int>.Of(1, 3, 5, 2, 4);
         var exception = Set<int>.Of(3.Until(10)) - Set<int>.Of(0.Until(6));
         var symmetricException = Set<int>.Of(3, 4) ^ Set<int>.Of(1, 3, 5, 2, 4);

         Console.WriteLine($"Union: {enumerableImage(union)}");
         Console.WriteLine($"Intersection: {enumerableImage(intersection)}");
         Console.WriteLine($"Exception: {enumerableImage(exception)}");
         Console.WriteLine($"Symmetric exception: {enumerableImage(symmetricException)}");
      }
   }
}
