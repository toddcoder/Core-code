﻿using System;
using System.Text;
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

      [TestMethod]
      public void SlicedTest()
      {
         var source = "Throw the old woman out!";
         Slicer slicer = source;

         Console.WriteLine(slicer[6, 13]);
         slicer[6, 13] = "Lucy";

         Console.WriteLine(slicer[0, 5]);
         slicer[0, 5] = "Toss";

         Console.WriteLine(slicer[23, 1]);
         slicer[23, 1] = "?!";

         var builder = new StringBuilder(source);

         foreach (var (index, length, text) in slicer)
         {
            builder.Remove(index, length);
            builder.Insert(index, text);
         }

         Console.WriteLine(builder);
      }
   }
}