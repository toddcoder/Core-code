using System;
using Core.Computers;
using Core.Strings.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class TextComparisonTest
{
   protected static void test(string oldFileName, string newFileName)
   {
      FolderName folder = @"C:\Enterprise\Projects\Core\Core.Tests\test-data";
      var oldFile = folder + oldFileName;
      var newFile = folder + newFileName;
      var oldLines = oldFile.Lines;
      var newLines = newFile.Lines;

      var diff = new Differentiator(oldLines, newLines, true, false);
      var _model = diff.BuildModel();
      if (_model is (true, var model))
      {
         Console.WriteLine(model);

         var oldDifferences = model.OldDifferences();
         var newDifferences = model.NewDifferences();

         Console.WriteLine("old differences:");
         foreach (var difference in oldDifferences)
         {
            Console.WriteLine(difference);
         }

         Console.WriteLine();

         Console.WriteLine("new differences:");
         foreach (var difference in newDifferences)
         {
            Console.WriteLine(difference);
         }

         Console.WriteLine("merged differences:");
         foreach (var line in model.MergedDifferences())
         {
            Console.WriteLine(line);
         }
      }
      else
      {
         Console.WriteLine($"exception: {_model.Exception.Message}");
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