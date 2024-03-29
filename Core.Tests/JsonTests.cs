﻿using System;
using System.Diagnostics;
using Core.Computers;
using Core.Dates;
using Core.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class JsonTests
{
   [TestMethod]
   public void DeserializationTest()
   {
      FileName jsonFile = @"..\..\TestData\work-item.json";
      var source = jsonFile.Text;
      var deserializer = new Deserializer(source);
      var _setting = deserializer.Deserialize();
      if (_setting is (true, var setting))
      {
         Console.WriteLine(setting);
      }
      else
      {
         Console.WriteLine($"Exception: {_setting.Exception.Message}");
      }
   }

   [TestMethod]
   public void Deserialization2Test()
   {
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      FileName jsonFile = @"..\..\TestData\builds.json";
      var source = jsonFile.Text;
      var deserializer = new Deserializer(source);
      var _setting = deserializer.Deserialize();

      stopwatch.Stop();


      if (_setting is (true, var setting))
      {
         Console.WriteLine(setting.Count);
         Console.WriteLine(setting);
         Console.WriteLine(stopwatch.Elapsed.ToString(true));
      }
      else
      {
         Console.WriteLine($"Exception: {_setting.Exception.Message}");
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

   [TestMethod]
   public void PatchTest()
   {
      var branchFilters = new[] { "+refs/heads/master", "+refs/heads/r-6.43.0*" };
      using var writer = new JsonWriter();
      writer.BeginArray();

      writer.BeginObject();
      writer.Write("op", "replace");
      writer.Write("path", "/triggers/branchFilters");

      writer.BeginArray("value");

      foreach (var branchFilter in branchFilters)
      {
         writer.Write(branchFilter);
      }

      writer.EndArray();
      writer.EndObject();

      writer.EndArray();
      var json = writer.ToString();
      Console.WriteLine(json);
   }

   [TestMethod]
   public void ArrayTest()
   {
      using var writer = new JsonWriter();
      writer.BeginObject();
      writer.Write("array", new[] { "alpha", "bravo", "charlie" });
      writer.EndObject();
      Console.WriteLine(writer);
   }
}