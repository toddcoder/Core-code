using System;
using Core.Computers;
using Core.Computers.Synchronization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class FileNameTests
   {
      const string SOURCE_FOLDER0 = @"C:\Enterprise\Working\_DDL";
      const string TARGET_FOLDER0 = @"C:\Enterprise\Working\_DDL_target";
      const string SOURCE_FOLDER1 = @"C:\Enterprise\Projects\TSqlCop.libs\lib\net45";
      const string TARGET_FOLDER1 = @"C:\Enterprise\Working\libs\lib\net45";

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
         var synchronizer = new Synchronizer(SOURCE_FOLDER0, TARGET_FOLDER0, ".+ '.sql' $");

         synchronizer.Success += (sender, args) => Console.WriteLine($"Success: {args.Message}");
         synchronizer.Failure += (sender, args) => Console.WriteLine($"Failure: {args.Message}");
         synchronizer.NewFolderSuccess += (sender, args) => Console.WriteLine($"New folder success: {args.Message}");
         synchronizer.NewFolderFailure += (sender, args) => Console.WriteLine($"New folder failure: {args.Message}");

         synchronizer.Synchronize();
      }

      [TestMethod]
      public void SynchronizePickedFilesTest()
      {
         var synchronizer=new Synchronizer(SOURCE_FOLDER1, TARGET_FOLDER1, "'.dll' $");

         synchronizer.Success += (sender, args) => Console.WriteLine($"Success: {args.Message}");
         synchronizer.Failure += (sender, args) => Console.WriteLine($"Failure: {args.Message}");
         synchronizer.NewFolderSuccess += (sender, args) => Console.WriteLine($"New folder success: {args.Message}");
         synchronizer.NewFolderFailure += (sender, args) => Console.WriteLine($"New folder failure: {args.Message}");

         synchronizer.Synchronize("ApexSQL.Activation.dll", "ApexSQL.Common.Formatting.dll", "ApexSQL.Common.GrammarParser.dll",
            "ApexSQL.Common.Shared.dll", "ApexSQL.Common.UI.dll", "ApexSql.Refactor.dll", "Core.dll", "Microsoft.SqlServer.TransactSql.ScriptDom.dll",
            "Newtonsoft.Json.dll", "SqlConformance.Library.dll", "SqlConformance.Util.dll");
      }
   }
}