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
         FileName oldFile = @"C:\Enterprise\Projects\Core\Core.Tests\test-data\old.txt";
         FileName newFile = @"C:\Enterprise\Projects\Core\Core.Tests\test-data\new.txt";

         var oldLines = oldFile.Lines;
         var newLines = newFile.Lines;

         var diff = new TextDiff(oldLines, newLines, false, false);
         if (diff.Build().If(out var model, out var exception))
         {
            Console.WriteLine(model);
         }
         else
         {
            Console.WriteLine($"exception: {exception.Message}");
         }
      }
   }
}
