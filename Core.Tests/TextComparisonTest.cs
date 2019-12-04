using System;
using Core.Computers;
using Core.Strings.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class TextComparisonTest
   {
      static void test(string oldFileName, string newFileName)
      {
         FolderName folder = @"C:\Enterprise\Projects\Core\Core.Tests\test-data";
         var oldFile = folder + oldFileName;
         var newFile = folder + newFileName;
         var oldLines = oldFile.Lines;
         var newLines = newFile.Lines;

         var diff = new Differentiator(oldLines, newLines, true, false);
         if (diff.BuildModel().If(out var model, out var exception))
         {
            Console.WriteLine(model);
         }
         else
         {
            Console.WriteLine($"exception: {exception.Message}");
         }
      }

      [TestMethod]
      public void SmallFileTest() => test("old.txt", "new.txt");

      [TestMethod]
      public void LargeFileTest() => test("big file.bad.sql", "big file.sql");

      [TestMethod]
      public void FormattedSqlFile() => test("udtype.sql", "udtype-formatted.sql");

      [TestMethod]
      public void UnrepairedSqlFile() => test("udtype-unrepaired.sql", "udtype.sql");
   }
}