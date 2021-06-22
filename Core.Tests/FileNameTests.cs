using System;
using Core.Computers;
using Core.Computers.Synchronization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class FileNameTests
   {
      protected const string SOURCE_FOLDER0 = @"C:\Enterprise\Working\_DDL";
      protected const string TARGET_FOLDER0 = @"C:\Enterprise\Working\_DDL_target";
      protected const string SOURCE_FOLDER1 = @"C:\Enterprise\Projects\TSqlCop.libs\lib\net45";
      protected const string TARGET_FOLDER1 = @"C:\Enterprise\Working\libs\lib\net45";

      [TestMethod]
      public void TruncateBySubfolderTest()
      {
         FileName file = @"C:\Enterprise\Test\Sql Data\test.sql";
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
         var synchronizer = new Synchronizer(SOURCE_FOLDER0, TARGET_FOLDER0, ".+ '.sql' $; f");

         synchronizer.Success += (sender, args) => Console.WriteLine($"Success: {args.Message}");
         synchronizer.Failure += (sender, args) => Console.WriteLine($"Failure: {args.Message}");
         synchronizer.NewFolderSuccess += (sender, args) => Console.WriteLine($"New folder success: {args.Message}");
         synchronizer.NewFolderFailure += (sender, args) => Console.WriteLine($"New folder failure: {args.Message}");

         synchronizer.Synchronize();
      }

      [TestMethod]
      public void SynchronizePickedFilesTest()
      {
         var synchronizer = new Synchronizer(SOURCE_FOLDER1, TARGET_FOLDER1, "'.dll' $; f");

         synchronizer.Success += (sender, args) => Console.WriteLine($"Success: {args.Message}");
         synchronizer.Failure += (sender, args) => Console.WriteLine($"Failure: {args.Message}");
         synchronizer.NewFolderSuccess += (sender, args) => Console.WriteLine($"New folder success: {args.Message}");
         synchronizer.NewFolderFailure += (sender, args) => Console.WriteLine($"New folder failure: {args.Message}");

         synchronizer.Synchronize("ApexSQL.Activation.dll", "ApexSQL.Common.Formatting.dll", "ApexSQL.Common.GrammarParser.dll",
            "ApexSQL.Common.Shared.dll", "ApexSQL.Common.UI.dll", "ApexSql.Refactor.dll", "Core.dll", "Microsoft.SqlServer.TransactSql.ScriptDom.dll",
            "Newtonsoft.Json.dll", "SqlConformance.Library.dll", "SqlConformance.Util.dll");
      }

      [TestMethod]
      public void IndexedFileTest()
      {
         FileName originalFile = @"C:\Enterprise\Working\_DDL\tsqlcop.json";
         if (originalFile.Indexed().If(out var file))
         {
            originalFile.CopyTo(file);
         }
         else
         {
            Console.WriteLine("Out of indexes");
         }
      }
   }
}