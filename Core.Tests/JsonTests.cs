using System;
using Core.Computers;
using Core.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class JsonTests
   {
      [TestMethod]
      public void DeserializationTest()
      {
         FileName jsonFile = @"..\..\TestData\work-item.json";
         var source = jsonFile.Text;
         var deserializer = new Deserializer(source);
         if (deserializer.Deserialize().If(out var configuration, out var exception))
         {
            Console.WriteLine(configuration);
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }
   }
}