using System;
using Core.Enumerables;
using Core.ObjectGraphs;
using Core.ObjectGraphs.Configurations.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;
using static Core.Monads.MonadExtensions;

namespace Core.Tests
{
   internal enum TestEnum
   {
      Ignore,
      PayAttention
   }

   internal class TestClass
   {
      public TestClass()
      {
         Array = array(1, 5, 3);
         TestEnum = TestEnum.Ignore;
         Name = "";
      }

      public string Name { get; set; }

      public int Index { get; set; }

      public bool IsTrue { get; set; }

      public TestEnum TestEnum { get; set; }

      public int[] Array { get; set; }
   }

   [TestClass]
   public class ObjectGraphTest
   {
      [TestMethod]
      public void SaveTest()
      {
         ObjectGraph graph = "foo{a->'alpha';b->\"bravo\";c->charlie}";
         Console.WriteLine(graph);
      }

      [TestMethod]
      public void AccessTest()
      {
         ObjectGraph graph = "a->\"alpha\";b->'bravo';c->charlie";
         Console.WriteLine(graph["a"].Value);
         Console.WriteLine(graph["b"].Value);
         Console.WriteLine(graph["c"].Value);
      }

      [TestMethod]
      public void SerializationTest()
      {
         var test = new TestClass { Name = "foobar", Index = 153, IsTrue = true, TestEnum = TestEnum.PayAttention, Array = array(111, 123, 153) };
         var graph = ObjectGraph.Serialize(test);
         Console.WriteLine(graph);

         if (graph.Object<TestClass>().If(out var test2, out var exception))
         {
            Console.WriteLine(test2.Name);
            Console.WriteLine(test2.Index);
            Console.WriteLine(test2.IsTrue);
            Console.WriteLine(test2.TestEnum);
            Console.WriteLine(test2.Array.ToString(", "));
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }

      [TestMethod]
      public void JsonToObjectGraphTest()
      {
         var json = "{name:\"foobar\",index:153,\"isTrue\":true,array:[111,123,153]," +
            "obj:{id:0,name:\"name1\",array2:[111, 222, 333]}, foo_bar: 'xxx'}";
         var parser = new JsonToObjectGraphParser(json);
         if (parser.ParseBoth().If(out var jsonObject, out var objectGraph, out var exception))
         {
            var writer = new JsonWriter();
            jsonObject.Generate(writer);
            Console.WriteLine(writer);
            Console.WriteLine(objectGraph);
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }

      [TestMethod]
      public void BuildingObjectGraphTest()
      {
         var objectGraph = ObjectGraph.RootObjectGraph();
         for (var i = 0; i < 5; i++)
         {
            objectGraph.Subgraph = $"${i}->{{i->{i};iSq->{i * i}}}";
         }

         Console.WriteLine(objectGraph);

         for (var i = 0; i < 5; i++)
         {
            var name = $"${i}";
            var childGraph = objectGraph[name];
            var index = childGraph.ToInt("i");
            var indexSquared = childGraph.ToInt("iSq");
            Console.WriteLine($"{name}: index: {index}, index^2: {indexSquared}");
         }
      }

      [TestMethod]
      public void NewSyntaxTest()
      {
         ObjectGraph objectGraph = "$0->{segment->{index->1236;length->35;line->30;column->22;endLine->30;endColumn->57};" +
            "message->\"Uses foreign key FK_FeeDelta_FeeArcId (FeeDelta.FeeArcId) -> (FeeArc.FeeArcId)\";oldText->\"\";rule->positiveAnalysisInformation}";
         Console.WriteLine(objectGraph);
      }
   }
}