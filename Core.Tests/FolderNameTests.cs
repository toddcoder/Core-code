using System;
using Core.Computers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class FolderNameTests
   {
      [TestMethod]
      public void RelativeToTest()
      {
         FolderName baseFolder = @"C:\Users\tebennett\src\Estream\Source\Estream.MigrationTests\bin";
         var result = baseFolder.RelativeTo(@"C:\Users\tebennett\src\Estream\Source\Estream.MigrationTests\Configuration");
         Console.WriteLine(result);
      }
   }
}