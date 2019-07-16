using System;
using Core.Computers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class FileNameTests
   {
      [TestMethod]
      public void TruncateBySubfolderTest()
      {
         FileName file = @"C:\Enterprise\Test\Sql Data\foobar.sql";
         var name = file.TruncateBySubfolder(2);
         Console.WriteLine(name);
         name = file.TruncateBySubfolder(0);
         Console.WriteLine(name);
         name = file.TruncateBySubfolder(10);
         Console.WriteLine(name);
      }
   }
}