using System;
using Core.Computers;
using Core.Computers.Synchronization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class FileNameTests
   {
      const string SOURCE_FOLDER = @"C:\Enterprise\Working\_DDL";
      const string TARGET_FOLDER = @"C:\Enterprise\Working\_DDL_target";

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

      [TestMethod]
      public void SynchronizeFilesTest()
      {
         var synchronizer = new Synchronizer(SOURCE_FOLDER, TARGET_FOLDER, ".+ '.sql' $");
         synchronizer.Success += (sender, args) => Console.WriteLine($"Success: {args.Message}");
         synchronizer.Failure += (sender, args) => Console.WriteLine($"Failure: {args.Message}");
         //synchronizer.Untouched += (sender, args) => Console.WriteLine($"Untouched: {args.Message}");
         synchronizer.NewFolderSuccess += (sender, args) => Console.WriteLine($"New folder success: {args.Message}");
         synchronizer.NewFolderFailure += (sender, args) => Console.WriteLine($"New folder failure: {args.Message}");

         synchronizer.Synchronize();
      }
   }
}