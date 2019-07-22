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
         slicer[6, 13] = "Lucy";
         slicer[0, 5] = "Toss";
         slicer[23, 1] = "?!";
         Console.WriteLine(slicer);
      }
   }
}
