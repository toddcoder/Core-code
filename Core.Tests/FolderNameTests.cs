using System;
using System.Linq;
using Core.Computers;
using Core.RegularExpressions;
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
         var result = baseFolder.RelativeTo(
            (FileName)@"C:\Users\tebennett\src\Estream\Source\Estream.MigrationTests\Configurations\configuration.json");
         Console.WriteLine(result);
         var absolute = baseFolder.AbsoluteFolder(result);
         Console.WriteLine(absolute);
      }

      [TestMethod]
      public void RelativeFolderConversionToAbsolute()
      {
         var folderName = @"..\..\Estream.Migrations\_DDL";
         FolderName baseFolder = @"C:\Users\tebennett\src\Estream\Source\Estream.MigrationTests\Configurations";
         var folder = baseFolder.AbsoluteFolder(folderName);
         Console.WriteLine(folder);
      }

      [TestMethod]
      public void LocalAndParentFilesTest1()
      {
         FolderName folder = @"C:\Users\tebennett\src\Estream\Source\Estream.Measurements\WellSearchDomain\ValueObjects\MasterData";
         FolderName baseFolder = @"C:\Users\tebennett\src\Estream\Source";
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
         FolderName folder = @"C:\Users\tebennett\src\Estream\Source\Estream.Measurements\WellSearchDomain\ValueObjects\MasterData";
         foreach (var subFolder in folder.LocalAndParentFolders /*.Where(f => f.Name.IsMatch("/b 'Source' /b"))*/)
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
   }
}