using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Core.Applications;
using Core.Configurations;
using Core.Monads;

namespace Core.Tests
{
   [TestClass]
   public class ConfigurationTests
   {
      protected class Test
      {

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
   }
}