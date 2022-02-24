using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using Core.Applications;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Configurations;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
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
            Tests = Array.Empty<Test>();
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

         if (Group.FromString(source).If(out var group, out var exception))
         {
            var result =
               from connections in @group.GetGroup("connections")
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

         if (Group.FromString(source).If(out var group, out var exception))
         {
            var result =
               from connections in @group.GetGroup("connections")
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
      public void MultilineArrayTest()
      {
         var resources = new Resources<ConfigurationTests>();
         var source = resources.String("TestData.Arrays.txt");
         if (Group.FromString(source).If(out var group, out var exception))
         {
            Console.WriteLine(group);
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
         if (Group.FromString(source).If(out var group, out var exception))
         {
            Console.Write(group);
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
         if (Group.Serialize(test, "test").If(out var group, out var exception))
         {
            Console.Write(group);
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
         if (Group.Serialize(package, "guids").If(out var configuration, out var exception))
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
         if (Group.FromString(source).If(out var configuration, out var exception))
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
         if (Group.Serialize(container, "data").If(out var configuration, out var exception))
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
         if (hash.ToGroup().If(out var group, out var exception))
         {
            Console.WriteLine(group);
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
            from @group in hash.ToGroup()
            let file = (FileName)$@"C:\Temp\{uniqueID()}.txt"
            from _ in file.TryTo.SetText(@group.ToString())
            from source in file.TryTo.Text
            from group2 in Group.FromString(source)
            select @group.ToStringHash();
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
         if (hash.ToGroup().If(out var group, out var exception))
         {
            var source = group.ToString();
            Console.WriteLine(source);
            Console.WriteLine(group["release"]);
            Console.WriteLine(group["build"]);

            if (Group.FromString(source).If(out group, out exception))
            {
               Console.WriteLine(group["release"]);
               Console.WriteLine(group["build"]);
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

      [TestMethod]
      public void WritingTest()
      {
         var group = new Group
         {
            ["repository"] = @"\\vmdvw10estm57",
            ["server"] = ".",
            ["database"] = "local_tebennett"
         };

         Console.WriteLine(group);
      }

      [TestMethod]
      public void ArrayTest()
      {
         using var writer = new StringWriter();
         writer.WriteLine("[");
         writer.WriteLine("   value1: 111");
         writer.WriteLine("   value2: \"$1\"");
         writer.WriteLine("]");
         writer.WriteLine("[");
         writer.WriteLine("   value1: 123");
         writer.WriteLine("   value2: \"$2\"");
         writer.WriteLine("]");
         writer.WriteLine("[");
         writer.WriteLine("   value1: 153");
         writer.WriteLine("   value2: \"$3\"");
         writer.WriteLine("]");
         var source = writer.ToString();
         if (Group.FromString(source).If(out var group, out var exception))
         {
            foreach (var (key, innerGroup) in group.Groups())
            {
               Console.WriteLine($"{key} [");
               Console.WriteLine($"   value1: {innerGroup.ValueAt("value1")}");
               Console.WriteLine($"   value2: {innerGroup.ValueAt("value2")}");
               Console.WriteLine("]");
            }

            Console.WriteLine("=".Repeat(80));

            Console.WriteLine(group);
         }
         else
         {
            throw exception;
         }
      }

      [TestMethod]
      public void AnonymousTest()
      {
         using var writer = new StringWriter();
         writer.WriteLine("[");
         writer.WriteLine("   alpha");
         writer.WriteLine("   bravo");
         writer.WriteLine("   charlie");
         writer.WriteLine("]");
         var source = writer.ToString();

         var group = Group.FromString(source).ForceValue();
         var (_, innerGroup) = group.Groups().FirstOrFail("No outer group").ForceValue();
         foreach (var (key, value) in innerGroup.Values())
         {
            Console.WriteLine($"{key}: \"{value}\"");
         }
      }

      [TestMethod]
      public void AnonymousTest2()
      {
         using var writer = new StringWriter();
         writer.WriteLine("alpha");
         writer.WriteLine("bravo");
         writer.WriteLine("charlie");
         var source = writer.ToString();

         var group = Group.FromString(source).ForceValue();
         foreach (var (key, value) in group.Values())
         {
            Console.WriteLine($"{key}: \"{value}\"");
         }
      }

      [TestMethod]
      public void QuoteTest()
      {
         using var writer = new StringWriter();
         writer.WriteLine("[");
         //writer.WriteLine("   alpha: foobar\"baz");
         writer.WriteLine(@"   bravo: ""^(Enqueuing task `""\[)[^\]]+(\]`"").+$; u""");
         writer.WriteLine("]");
         var source = writer.ToString();
         _ = Group.FromString(source).ForceValue();
      }
   }
}