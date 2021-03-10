using System;
using System.Collections.Generic;
using System.Linq;
using Core.Assertions;
using Core.Computers;
using Core.Enumerables;
using Core.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Assertions.AssertionFunctions;

namespace Core.Tests
{
   [TestClass]
   public class FolderNameTests
   {
      protected const string FOLDER_ESTREAM = @"~\src\Estream\Source\Estream.MigrationTests";
      protected const string FOLDER_APP_DATA = @"~\AppData\Local\TSqlCop";
      protected const string FOLDER_PROGRAM_FILES = @"C:\Program Files (x86)\";
      protected const string FOLDER_VISUAL_STUDIO = FOLDER_PROGRAM_FILES +
         @"Microsoft Visual Studio\2017\Professional\Common7\IDE\Extensions\Enterprise\TSqlCop";
      protected const string FOLDER_SSMS130 = FOLDER_PROGRAM_FILES + @"Microsoft SQL Server\130\Tools\Binn\ManagementStudio\Extensions\TSqlCop";
      protected const string FOLDER_SSMS140 = FOLDER_PROGRAM_FILES + @"Microsoft SQL Server\140\Tools\Binn\ManagementStudio\Extensions\TSqlCop";

      protected static IEnumerable<FolderName> defaultFolderNames()
      {
         yield return FOLDER_ESTREAM;
         yield return FOLDER_APP_DATA;
         yield return FOLDER_VISUAL_STUDIO;
         yield return FOLDER_SSMS130;
         yield return FOLDER_SSMS140;
      }

      [TestMethod]
      public void RelativeToTest()
      {
         FolderName baseFolder = @"~\src\Estream\Source\Estream.MigrationTests\bin";
         var result = baseFolder.RelativeTo((FileName)@"~\src\Estream\Source\Estream.MigrationTests\Configurations\configuration.json");
         Console.WriteLine(result);
         var absolute = baseFolder.AbsoluteFolder(result);
         Console.WriteLine(absolute);
      }

      [TestMethod]
      public void RelativeFolderConversionToAbsolute()
      {
         var folderName = @"..\..\Estream.Migrations\_DDL";
         FolderName baseFolder = @"~\src\Estream\Source\Estream.MigrationTests\Configurations";
         var folder = baseFolder.AbsoluteFolder(folderName);
         Console.WriteLine(folder);
      }

      [TestMethod]
      public void LocalAndParentFilesTest1()
      {
         FolderName folder = @"~\src\Estream\Source\Estream.Measurements\WellSearchDomain\ValueObjects\MasterData";
         FolderName baseFolder = @"~\src\Estream\Source";
         foreach (var file in folder.LocalAndParentFiles.Where(f => f.Extension == ".cs"))
         {
            var relative = baseFolder.RelativeTo(file);
            Console.WriteLine(relative);
         }
      }

      [TestMethod]
      public void LocalAndParentFilesTest2()
      {
         FolderName folder = @"C:\Enterprise\Projects\TSqlCop\SqlConformance.Library\SqlContainment";
         FolderName baseFolder = @"C:\Enterprise\Projects\TSqlCop";
         foreach (var file in folder.LocalAndParentFiles.Where(f => f.Extension == ".cs" && f.Name.IsMatch("'sql'", true)))
         {
            var relative = baseFolder.RelativeTo(file);
            Console.WriteLine(relative);
         }
      }

      [TestMethod]
      public void LocalAndParentFilesTest3()
      {
         FolderName folder = @"C:\Enterprise\Temp";
         foreach (var file in folder.LocalAndParentFiles)
         {
            Console.WriteLine(file);
         }
      }

      [TestMethod]
      public void LocalAndParentFilesTest4()
      {
         FolderName folder = @"C:\Enterprise\Projects\TSqlCop\TSqlCop.Ssms\bin\Debug";
         FolderName baseFolder = @"C:\Enterprise\Projects";
         foreach (var file in folder.LocalAndParentFiles)
         {
            var relative = baseFolder.RelativeTo(file);
            Console.WriteLine(relative);
         }
      }

      [TestMethod]
      public void LocalAndParentFoldersTest1()
      {
         FolderName folder = @"~\src\Estream\Source\Estream.Measurements\WellSearchDomain\ValueObjects\MasterData";
         foreach (var subFolder in folder.LocalAndParentFolders)
         {
            Console.WriteLine(subFolder);
         }
      }

      [TestMethod]
      public void LocalAndParentFoldersTest2()
      {
         FolderName folder = @"C:\Enterprise\Temp";
         foreach (var subFolder in folder.LocalAndParentFolders)
         {
            Console.WriteLine(subFolder);
         }
      }

      [TestMethod]
      public void MultipleLocalAndParentFilesTest()
      {
         var result = defaultFolderNames().LocalAndParentFiles().Where(f => f.NameExtension == "tsqlcop.sql.format.options.xml")
            .FirstOrFail("failed");
         Console.WriteLine(result.Map(f => f.FullPath).Recover(e => e.Message));
      }

      [TestMethod]
      public void ContainsFolderTest()
      {
         FolderName parentFolder = @"C:\Enterprise\Projects";
         FolderName subFolder = @"C:\Enterprise\Projects\Core";
         FolderName alienFolder = @"C:\Enterprise\Working";

         assert(() => parentFolder.ContainsFolder(subFolder)).Must().BeTrue().OrThrow();
         assert(() => parentFolder.ContainsFolder(alienFolder)).Must().Not.BeTrue().OrThrow();
      }

      [TestMethod]
      public void ImmediateSubFolderTest()
      {
         FolderName folder = @"C:\Program Files (x86)\Microsoft Visual Studio\2019";
         assert(() => folder.ContainsImmediateFolderName("Professional")).Must().BeTrue().OrThrow();
         assert(() => folder.ContainsImmediateFolderName("Enterprise")).Must().Not.BeTrue().OrThrow();
      }
   }
}