using System;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class StringClassTests
   {
      [TestMethod]
      public void SlicerTest()
      {
         Slicer slicer = "Throw the old woman out!";

         Console.WriteLine(slicer[6, 13]);
         slicer[6, 13] = "Lucy";

         Console.WriteLine(slicer[0, 5]);
         slicer[0, 5] = "Toss";

         Console.WriteLine(slicer[23, 1]);
         slicer[23, 1] = "?!";
         Console.WriteLine(slicer);
      }
   }
}
