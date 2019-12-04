using System;
using Core.Computers;
using Core.Strings.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class TextComparisonTest
   {
      [TestMethod]
      public void BasicComparisonTest()
      {
         FileName oldFile = @"C:\Enterprise\Working\test\big file.bad.sql";
         FileName newFile = @"C:\Enterprise\Working\test\big file.sql";

         var oldLines = oldFile.Lines;
         var newLines = newFile.Lines;

         var differ = new TextDiffer();
         if (differ.CreateDiffs(oldLines, newLines, false, false).If(out var result, out var exception))
         {
            Console.WriteLine(result);
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }
   }
}
