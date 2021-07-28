using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Core.Applications;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Configurations;
using Core.Enumerables;
using Core.Monads;
using static Core.Strings.StringFunctions;

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

      protected class Container
      {
         public Container()
         {
            Tests = new Test[0];
         }

         public Test[] Tests { get; set; }
      }

      protected class BinaryPackage : IEquatable<BinaryPackage>
      {
         public byte[] Payload { get; set; }

         public bool Equals(BinaryPackage other)
         {
            return other is not null && (ReferenceEquals(this, other) || Payload.Zip(other.Payload, (b1, b2) => b1 == b2).All(b => b));
         }

         public override bool Equals(object obj) => obj is BinaryPackage binaryPackage && Equals(binaryPackage);

         public override int GetHashCode() => Payload?.GetHashCode() ?? 0;
      }

      [TestMethod]
      public void BasicTest()
      {
         var resources = new Resources<ConfigurationTests>();
         var source = resources.String("TestData.connections.txt");

         if (Configuration.FromString(source).If(out var configuration, out var exception))
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

         if (Configuration.FromString(source).If(out var configuration, out var exception))
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
         if (Configuration.FromString(source).If(out var configuration, out var exception))
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
      public void BinarySerializationTest()
      {
         var resources = new Resources<ConfigurationTests>();
         var binary = resources.Bytes("TestData.guids.pdf");
         var package = new BinaryPackage { Payload = binary };
         if (Configuration.Serialize(package, "guids").If(out var configuration, out var exception))
         {
            Console.WriteLine(configuration);
            if (configuration.Deserialize<BinaryPackage>().If(out var newPackage, out exception))
            {
               package.Must().Equal(newPackage).OrThrow();
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

      [TestMethod]
      public void DeserializationTest()
      {
         var source = @"enum: Bravo; intValue: 153; stringValue: foobar; file: C:\temp\temp.txt; doubles: 1.0, 5.0, 3.0; isTrue: true; " +
            @"escape: ""`r `t \ foobar""";
         if (Configuration.FromString(source).If(out var configuration, out var exception))
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

      [TestMethod]
      public void ComplexSerializationDeserializationTest()
      {
         FileName file = @"C:\Temp\temp.txt";

         var container = new Container
         {
            Tests = new[]
            {
               new Test
               {
                  Enum = TestEnum.Alpha, Doubles = new double[] { 1, 2, 3 }, Escape = "`1", File = file, IntValue = 123, IsTrue = true,
                  StringValue = "foo"
               },
               new Test
               {
                  Enum = TestEnum.Bravo, Doubles = new[] { 1.0, 5, 3 }, Escape = "`2", File = file, IntValue = 153, IsTrue = false,
                  StringValue = "bar"
               }
            }
         };
         if (Configuration.Serialize(container, "data").If(out var configuration, out var exception))
         {
            Console.WriteLine(configuration);
            if (configuration.Deserialize<Container>().If(out container, out exception))
            {
               foreach (var test in container.Tests)
               {
                  Console.WriteLine(test);
               }
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

      [TestMethod]
      public void HashToConfigurationTest()
      {
         var hash = new StringHash(true)
         {
            ["alpha"] = "Alpha",
            ["bravo"] = "Beta",
            ["charlie"] = "Kappa",
            ["delta"] = "Delta",
            ["echo"] = "Eta",
            ["foxtrot"] = "Phi"
         };
         if (hash.ToConfiguration().If(out var configuration, out var exception))
         {
            Console.WriteLine(configuration);
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }

      [TestMethod]
      public void SerializeStringHashTest()
      {
         var hash = new StringHash(true)
         {
            ["alpha"] = "Alpha",
            ["bravo"] = "Beta",
            ["charlie"] = "Kappa",
            ["delta"] = "Delta",
            ["echo"] = "Eta",
            ["foxtrot"] = "Phi"
         };
         var _result =
            from configuration in hash.ToConfiguration()
            let file = (FileName)$@"C:\Temp\{uniqueID()}.txt"
            from _ in file.TryTo.SetText(configuration.ToString())
            from source in file.TryTo.Text
            from configuration2 in Configuration.FromString(source)
            from hash2 in configuration2.ToStringHash()
            select hash2;
         if (_result.If(out var result, out var exception))
         {
            foreach (var (key, value) in result)
            {
               Console.WriteLine($"{key}: {value}");
            }
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }

      [TestMethod]
      public void EmptyStringItemTest()
      {
         var hash = new StringHash(true) { ["release"] = "", ["build"] = "http" };
         if (hash.ToConfiguration().If(out var configuration, out var exception))
         {
            var source = configuration.ToString();
            Console.WriteLine(source);
            Console.WriteLine(configuration["release"]);
            Console.WriteLine(configuration["build"]);

            if (Configuration.FromString(source).If(out configuration, out exception))
            {
               Console.WriteLine(configuration["release"]);
               Console.WriteLine(configuration["build"]);
            }
            else
            {
               Console.WriteLine($"Exception: {exception.Message}");
            }
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }
   }
}