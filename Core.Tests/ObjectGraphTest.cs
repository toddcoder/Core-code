using System;
using Core.Enumerables;
using Core.ObjectGraphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

namespace Core.Tests
{
   internal enum TestEnum
   {
      Ignore,
      PayAttention
   }

   internal class TestClass
   {
      public string Name { get; set; } = "";

      public int Index { get; set; }

      public bool IsTrue { get; set; }

      public TestEnum TestEnum { get; set; } = TestEnum.Ignore;

      public int[] Array { get; set; } = array(1, 5, 3);
   }

   [TestClass]
   public class ObjectGraphTest
   {
      [TestMethod]
      public void SaveTest()
      {
         ObjectGraph graph = "foo{a->alpha;b->bravo;c->charlie}";
         Console.WriteLine(graph);
      }

      [TestMethod]
      public void AccessTest()
      {
         ObjectGraph graph = "a->alpha;b->bravo;c->charlie";
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
            Console.WriteLine(test2.Array.Stringify());
         }
         else
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
      }
   }
}