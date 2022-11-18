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
using static Core.Monads.Lazy.LazyMonads;
using static Core.Strings.StringFunctions;

namespace Core.Tests;

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

   protected class ReleaseTarget
   {
      public string[] ReleaseTargets { get; set; } = Array.Empty<string>();
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

      var _setting = Setting.FromString(source);
      if (_setting)
      {
         var _result =
            from connections in (~_setting).Maybe.Setting("connections")
            from connection1 in connections.Maybe.Setting("connection1")
            from server in connection1.Maybe.String("server")
            from database in connection1.Maybe.String("database")
            select (server, database);
         if (_result)
         {
            var (server, database) = ~_result;
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
         Console.WriteLine(_setting.Exception.Message);
      }
   }

   [TestMethod]
   public void FlatTest()
   {
      var resources = new Resources<ConfigurationTests>();
      var source = resources.String("TestData.connections2.txt");

      var _setting = Setting.FromString(source);
      if (_setting)
      {
         var _result =
            from connections in (~_setting).Maybe.Setting("connections")
            from connection1 in connections.Maybe.Setting("connection1")
            from server in connection1.Maybe.String("server")
            from database in connection1.Maybe.String("database")
            select (server, database);
         if (_result)
         {
            var (server, database) = ~_result;
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
         Console.WriteLine(_setting.Exception.Message);
      }
   }

   [TestMethod]
   public void MultilineArrayTest()
   {
      var resources = new Resources<ConfigurationTests>();
      var source = resources.String("TestData.Arrays.txt");
      var _setting = Setting.FromString(source);
      if (_setting)
      {
         Console.WriteLine(~_setting);
      }
      else
      {
         Console.WriteLine(_setting.Exception.Message);
      }
   }

   [TestMethod]
   public void ToStringTest()
   {
      var resources = new Resources<ConfigurationTests>();
      var source = resources.String("TestData.connections.txt");
      var _setting = Setting.FromString(source);
      if (_setting)
      {
         Console.Write(~_setting);
      }
      else
      {
         Console.WriteLine(_setting.Exception.Message);
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
      var _setting = Setting.Serialize(test, "test");
      if (_setting)
      {
         Console.Write(_setting);
      }
      else
      {
         Console.WriteLine(_setting.Exception.Message);
      }
   }

   [TestMethod]
   public void BinarySerializationTest()
   {
      var resources = new Resources<ConfigurationTests>();
      var binary = resources.Bytes("TestData.guids.pdf");
      var package = new BinaryPackage { Payload = binary };
      var _newPackage =
         from setting in Setting.Serialize(package, "guids").OnSuccess(Console.WriteLine)
         from newPackage in setting.Deserialize<BinaryPackage>()
         select newPackage;
      if (_newPackage)
      {
         package.Must().Equal(~_newPackage).OrThrow();
      }
      else
      {
         Console.WriteLine(_newPackage.Exception.Message);
      }
   }

   [TestMethod]
   public void DeserializationTest()
   {
      var source = @"enum: Bravo; intValue: 153; stringValue: foobar; file: C:\temp\temp.txt; doubles: 1.0, 5.0, 3.0; isTrue: true; " +
         @"escape: ""`r `t \ foobar""";
      var _object =
         from setting in Setting.FromString(source)
         from obj in setting.Deserialize<object>()
         select obj;
      if (_object)
      {
         Console.WriteLine(~_object);
      }
      else
      {
         Console.WriteLine(_object.Exception.Message);
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

      var _container =
         from setting in Setting.Serialize(container, "data").OnSuccess(Console.WriteLine)
         from deserializedContainer in setting.Deserialize<Container>()
         select deserializedContainer;
      if (_container)
      {
         foreach (var test in (~_container).Tests)
         {
            Console.WriteLine(test);
         }
      }
      else
      {
         Console.WriteLine(_container.Exception.Message);
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
      var _setting = hash.ToSetting();
      if (_setting)
      {
         Console.WriteLine(~_setting);
      }
      else
      {
         Console.WriteLine($"Exception: {_setting.Exception.Message}");
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
      var _stringHash =
         from setting in hash.ToSetting()
         let file = (FileName)$@"C:\Temp\{uniqueID()}.txt"
         from _ in file.TryTo.SetText(setting.ToString())
         from source in file.TryTo.Text
         from setting2 in Setting.FromString(source)
         select setting.ToStringHash();
      if (_stringHash)
      {
         foreach (var (key, value) in ~_stringHash)
         {
            Console.WriteLine($"{key}: {value}");
         }
      }
      else
      {
         Console.WriteLine($"Exception: {_stringHash.Exception.Message}");
      }
   }

   [TestMethod]
   public void EmptyStringItemTest()
   {
      var hash = new StringHash(true) { ["release"] = "", ["build"] = "http" };
      var _setting = hash.ToSetting();
      if (_setting)
      {
         var setting = ~_setting;
         var source = setting.ToString();
         Console.WriteLine(source);
         Console.WriteLine(setting["release"]);
         Console.WriteLine(setting["build"]);

         _setting = Setting.FromString(source);
         if (_setting)
         {
            setting = ~_setting;
            Console.WriteLine(setting["release"]);
            Console.WriteLine(setting["build"]);
         }
         else
         {
            Console.WriteLine($"Exception: {_setting.Exception.Message}");
         }
      }
      else
      {
         Console.WriteLine($"Exception: {_setting.Exception.Message}");
      }
   }

   [TestMethod]
   public void WritingTest()
   {
      var setting = new Setting
      {
         ["repository"] = @"\\vmdvw10estm57",
         ["server"] = ".",
         ["database"] = "local_tebennett"
      };

      Console.WriteLine(setting);
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
      var _setting = Setting.FromString(source);
      if (_setting)
      {
         foreach (var (key, innerSetting) in (~_setting).Settings())
         {
            Console.WriteLine($"{key} [");
            Console.WriteLine($"   value1: {innerSetting.Value.String("value1")}");
            Console.WriteLine($"   value2: {innerSetting.Value.String("value2")}");
            Console.WriteLine("]");
         }

         Console.WriteLine("=".Repeat(80));

         Console.WriteLine(~_setting);
      }
      else
      {
         throw _setting.Exception;
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

      var setting = ~Setting.FromString(source);
      var (_, innerSetting) = ~setting.Settings().FirstOrFail("No outer group");
      foreach (var (key, value) in innerSetting.Items())
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

      var setting = ~Setting.FromString(source);
      foreach (var (key, value) in setting.Items())
      {
         Console.WriteLine($"{key}: \"{value}\"");
      }
   }

   [TestMethod]
   public void QuoteTest()
   {
      using var writer = new StringWriter();
      writer.WriteLine("[");
      writer.WriteLine(@"   bravo: ""^(Enqueuing task `""\[)[^\]]+(\]`"").+$; u""");
      writer.WriteLine("]");
      var source = writer.ToString();
      _ = ~Setting.FromString(source);
   }

   [TestMethod]
   public void MultiLineArraySavingTest()
   {
      using var writer = new StringWriter();
      writer.WriteLine("releaseTargets: {");
      writer.WriteLine("   Monthly - 6.34 - March");
      writer.WriteLine("   Monthly - 6.35 - April");
      writer.WriteLine("}");
      var source = writer.ToString();

      var _setting = Setting.FromString(source);
      if (_setting)
      {
         Console.WriteLine(~_setting);
      }
      else
      {
         Console.WriteLine(_setting.Exception.Message);
      }

      var _releaseTarget = (~_setting).Deserialize<ReleaseTarget>();
      if (_releaseTarget)
      {
         _setting = Setting.Serialize(typeof(ReleaseTarget), ~_releaseTarget);
         if (_setting)
         {
            Console.WriteLine(~_setting);
         }
         else
         {
            Console.WriteLine(_setting.Exception.Message);
         }
      }
      else
      {
         Console.WriteLine(_releaseTarget.Exception.Message);
      }
   }

   protected class NonConformanceInfo
   {
      public NonConformanceInfo()
      {
         Message = "";
         Rule = "";
      }

      public int Index { get; set; }

      public int Length { get; set; }

      public int Line { get; set; }

      public int Column { get; set; }

      public int EndLine { get; set; }

      public int EndColumn { get; set; }

      public string Message { get; set; }

      public string Rule { get; set; }

      public override string ToString() => $"{Message}: {Rule} ({Index}, {Length})";
   }

   protected class NonConformanceInfoContainer
   {
      public NonConformanceInfoContainer()
      {
         NonConformanceInfos = Array.Empty<NonConformanceInfo>();
      }

      public NonConformanceInfo[] NonConformanceInfos { get; set; }
   }

   [TestMethod]
   public void Bug1Test()
   {
      var resources = new Resources<ConfigurationTests>();
      var source = resources.String("usesForeignKey.txt");
      var _setting = lazy.result<Setting>();
      var _container = lazy.result<NonConformanceInfoContainer>();
      if (_setting.ValueOf(Setting.FromString(source)))
      {
         var setting = ~_setting;
         Console.WriteLine(setting);
         if (_container.ValueOf(setting.Deserialize<NonConformanceInfoContainer>()))
         {
            var container = ~_container;
            foreach (var info in container.NonConformanceInfos)
            {
               Console.WriteLine(info);
            }
         }
      }
   }
}