using System;
using Core.Computers;
using Core.Computers.Synchronization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

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

      synchronizer.Success += (_, args) => Console.WriteLine($"Success: {args.Message}");
      synchronizer.Failure += (_, args) => Console.WriteLine($"Failure: {args.Message}");
      synchronizer.NewFolderSuccess += (_, args) => Console.WriteLine($"New folder success: {args.Message}");
      synchronizer.NewFolderFailure += (_, args) => Console.WriteLine($"New folder failure: {args.Message}");

      synchronizer.Synchronize();
   }

   [TestMethod]
   public void SynchronizePickedFilesTest()
   {
      var synchronizer = new Synchronizer(SOURCE_FOLDER1, TARGET_FOLDER1, "'.dll' $; f");

      synchronizer.Success += (_, args) => Console.WriteLine($"Success: {args.Message}");
      synchronizer.Failure += (_, args) => Console.WriteLine($"Failure: {args.Message}");
      synchronizer.NewFolderSuccess += (_, args) => Console.WriteLine($"New folder success: {args.Message}");
      synchronizer.NewFolderFailure += (_, args) => Console.WriteLine($"New folder failure: {args.Message}");

      synchronizer.Synchronize("ApexSQL.Activation.dll", "ApexSQL.Common.Formatting.dll", "ApexSQL.Common.GrammarParser.dll",
         "ApexSQL.Common.Shared.dll", "ApexSQL.Common.UI.dll", "ApexSql.Refactor.dll", "Core.dll", "Microsoft.SqlServer.TransactSql.ScriptDom.dll",
         "Newtonsoft.Json.dll", "SqlConformance.Library.dll", "SqlConformance.Util.dll");
   }

   [TestMethod]
   public void IndexedFileTest()
   {
      FileName originalFile = @"C:\Enterprise\Working\_DDL\tsqlcop.json";
      var _indexed = originalFile.Indexed();
      if (_indexed)
      {
         originalFile.CopyTo(_indexed);
      }
      else
      {
         Console.WriteLine("Out of indexes");
      }
   }

   [TestMethod]
   public void UniqueFileNameTest()
   {
      FileName file = @"~\source\repos\toddcoder\Core\Core.Tests\TestData\connections.txt";
      var _uniqueFile = file.Unique();
      if (_uniqueFile)
      {
         Console.WriteLine(_uniqueFile);
      }
      else
      {
         Console.WriteLine("Unique name not created");
      }
   }

   [TestMethod]
   public void FileToUrlTest()
   {
      FileName file = @"\\pdfsevolv01corp\data\ProductionSupport\MergePalette\Configuration\Releases\r-6.33.8\" +
         @"Branches\r-6.33.8\extra branches.control";
      var uri = file.Uri();
      Console.WriteLine(uri);
   }
}