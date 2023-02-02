using System;
using System.Linq;
using Core.Assertions;
using Core.Computers;
using Core.Monads;
using Core.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class ZipTest
{
   protected const string SOURCE_FOLDER = @"C:\Enterprise\Working\Zips\Source";
   protected const string ZIPS_FOLDER = @"C:\Enterprise\Working\Zips";

   [TestInitialize]
   public void Initialize()
   {
      FolderName zipFolder = SOURCE_FOLDER;
      var _zipFile = zipFolder.TryToZip("tsqlcop");
      if (_zipFile)
      {
         Console.WriteLine($"Zipped to {_zipFile}");
      }
      else
      {
         Console.WriteLine($"exception: {_zipFile.Exception.Message}");
      }
   }

   [TestMethod]
   public void ZipFolderTest()
   {
      FolderName zipFolder = ZIPS_FOLDER;
      var _folder =
         from files in zipFolder.TryTo.Files
         from zipFile in files.Where(f => f.Extension == ".zip").MaxOrFail(f => f.CreationTime, () => "No file found")
         from unzippedFolder in zipFile.TryToUnzip()
         select unzippedFolder;
      if (_folder is (true, var folder))
      {
         folder.Must().Exist().OrThrow();
         Console.WriteLine(folder.FullPath);
      }
      else
      {
         Console.WriteLine(_folder.Exception.Message);
      }
   }
}