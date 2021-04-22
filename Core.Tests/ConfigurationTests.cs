﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Core.Applications;
using Core.Computers;
using Core.Configurations;
using Core.Enumerables;
using Core.Monads;

namespace Core.Tests
{
   [TestClass]
   public class ConfigurationTests
   {
      protected enum TestEnum
      {
         Alpha,
         Bravo,
         Charlie
      }

      protected class Test
      {
         public TestEnum Enum { get; set; }

         public int IntValue { get; set; }

         public string StringValue { get; set; }

         public FileName File { get; set; }

         public double[] Doubles { get; set; }

         public bool IsTrue { get; set; }

         public string Escape { get; set; }

         public override string ToString()
         {
            return $"{Enum}; {IntValue}; {StringValue}; {File}; {Doubles.Select(d => d.ToString()).ToString(", ")}; {IsTrue}; {Escape}";
         }
      }

      [TestMethod]
      public void BasicTest()
      {
         var resources = new Resources<ConfigurationTests>();
         var source = resources.String("TestData.connections.txt");
         var parser = new Parser(source);

         if (parser.Parse().If(out var configuration, out var exception))
         {
            var result =
               from connections in configuration.GetGroup("connections")
               from connection1 in connections.GetGroup("connection1")
               from _server in connection1.GetValue("server")
               from _database in connection1.GetValue("database")
               select (_server, _database);
            if (result.If(out var server, out var database))
            {
               Console.WriteLine($"server: {server}");
               Console.WriteLine($"database: {database}");
            }
            else
            {
               Console.WriteLine("Failed");
            }
         }
         else
         {
            Console.WriteLine(exception.Message);
         }
      }

      [TestMethod]
      public void FlatTest()
      {
         var resources = new Resources<ConfigurationTests>();
         var source = resources.String("TestData.connections2.txt");
         var parser = new Parser(source);

         if (parser.Parse().If(out var configuration, out var exception))
         {
            var result =
               from connections in configuration.GetGroup("connections")
               from connection1 in connections.GetGroup("connection1")
               from _server in connection1.GetValue("server")
               from _database in connection1.GetValue("database")
               select (_server, _database);
            if (result.If(out var server, out var database))
            {
               Console.WriteLine($"server: {server}");
               Console.WriteLine($"database: {database}");
            }
            else
            {
               Console.WriteLine("Failed");
            }
         }
         else
         {
            Console.WriteLine(exception.Message);
         }
      }

      [TestMethod]
      public void ToStringTest()
      {
         var resources = new Resources<ConfigurationTests>();
         var source = resources.String("TestData.connections.txt");
         var parser = new Parser(source);
         if (parser.Parse().If(out var configuration, out var exception))
         {
            Console.Write(configuration);
         }
         else
         {
            Console.WriteLine(exception.Message);
         }
      }

      [TestMethod]
      public void SerializationTest()
      {
         var test = new Test
         {
            Enum = TestEnum.Bravo,
            IntValue = 153,
            StringValue = "foobar",
            File = @"C:\temp\temp.txt",
            Doubles = new[] { 1.0, 5.0, 3.0 },
            IsTrue = true,
            Escape = "\r \t \\ foobar"
         };
         if (Configuration.Serialize(test, "test").If(out var configuration, out var exception))
         {
            Console.Write(configuration);
         }
         else
         {
            Console.WriteLine(exception.Message);
         }
      }

      [TestMethod]
      public void DeserializationTest()
      {
         var source = @"enum: Bravo; intValue: 153; stringValue: foobar; file: C:\temp\temp.txt; doubles: 1.0, 5.0, 3.0; isTrue: true; " +
            @"escape: ""\r \t \\ foobar""";
         var parser = new Parser(source);
         if (parser.Parse().If(out var configuration, out var exception))
         {
            if (configuration.Deserialize<Test>().If(out var obj, out exception))
            {
               Console.WriteLine(obj);
            }
            else
            {
               Console.WriteLine(exception.Message);
            }
         }
         else
         {
            Console.WriteLine(exception.Message);
         }
      }
   }
}