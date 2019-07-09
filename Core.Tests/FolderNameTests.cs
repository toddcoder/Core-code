﻿using System;
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
         var result = baseFolder.RelativeTo((FileName)@"C:\Users\tebennett\src\Estream\Source\Estream.MigrationTests\Configurations\configuration.json");
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
   }
}