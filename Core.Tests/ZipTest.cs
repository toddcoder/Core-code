using System;
using System.Linq;
using Core.Assertions;
using Core.Computers;
using Core.Monads;
using Core.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class ZipTest
   {
      const string SOURCE_FOLDER = @"C:\Enterprise\Working\Zips\Source";
      const string ZIPS_FOLDER = @"C:\Enterprise\Working\Zips";

      [TestInitialize]
      public void Initialize()
      {
         FolderName zipFolder = SOURCE_FOLDER;
         if (zipFolder.TryToZip("tsqlcop").If(out var zipFile, out var exception))
         {
            Console.WriteLine($"Zipped to {zipFile}");
         }
         else
         {
            Console.WriteLine($"exception: {exception.Message}");
         }
      }

      [TestMethod]
      public void ZipFolderTest()
      {
         FolderName zipFolder = ZIPS_FOLDER;
         var anyFiles = zipFolder.TryTo.Files;
         if (anyFiles.If(out var files, out var exception))
         {
            var lastFile = files.Where(f => f.Extension == ".zip").MaxOrFail(f => f.CreationTime, () => "No file found");
            if (lastFile.If(out var zipFile, out exception))
            {
               if (zipFile.TryToUnzip().If(out var folder, out exception))
               {
                  folder.Must().Exist().Assert($"{nameof(folder)} must exist");
                  Console.WriteLine(folder.FullPath);
               }
               else
               {
                  Console.WriteLine($"exception: {exception.Message}");
               }
            }
            else
            {
               Console.WriteLine($"exception: {exception.Message}");
            }
         }
         else
         {
            Console.WriteLine($"exception: {exception.Message}");
         }
      }
   }
}