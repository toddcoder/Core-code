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

      [TestMethod]
      public void WriterTest()
      {
         using var writer = new JsonWriter();

         writer.BeginObject();
         writer.BeginObject("metadata");
         writer.Write("id", "Core");
         writer.Write("version", "1.4.4.6");
         writer.Write("title", "Core");
         writer.Write("authors", "Todd Bennett");
         writer.Write("copyright", 2021);
         writer.BeginArray("tags");
         writer.Write("Core");
         writer.Write("Async");
         writer.Write("Types");
         writer.EndArray();
         writer.EndObject();
         writer.EndObject();

         Console.WriteLine(writer);
      }
   }
}