using System;
using System.Linq;
using Core.Assertions;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

namespace Core.Tests
{
   [TestClass]
   public class EnumerableExtensionTests
   {
      protected class ItemToSort
      {
         public ItemToSort(string key)
         {
            Key = key;
            Ordinal = key[0];
         }

         public string Key { get; }

         public int Ordinal { get; }

         public override string ToString() => $"{Key}: {Ordinal}";
      }

      protected class SortableItem
      {
         public SortableItem()
         {
            Branch = string.Empty;
            Order = string.Empty;
         }

         public string Branch { get; set; }

         public string Order { get; set; }
      }

      [TestMethod]
      public void FirstOrNoneTest()
      {
         var testArray = 'f'.DownTo('a');
         var _char = testArray.FirstOrNone();
         if (_char)
         {
            _char.Value.ToString().Must().Equal("f").OrThrow();
            Console.WriteLine($"{_char} == 'f'");
         }
      }

      [TestMethod]
      public void FirstOrFailTest()
      {
         var testArray = 0.UpUntil(10).ToArray();
         var _first = testArray.FirstOrFail("Not found");
         if (_first)
         {
            Console.WriteLine(_first.Value);
         }
         else
         {
            Console.WriteLine(_first.Exception.Message);
         }

         testArray = array<int>();
         _first = testArray.FirstOrFail("Not found");
         if (_first)
         {
            Console.WriteLine(_first.Value);
         }
         else
         {
            Console.WriteLine(_first.Exception.Message);
         }
      }

      [TestMethod]
      public void IndexesOfMinMaxTest()
      {
         var source = array("alpha", "apples", "brat", "IP");

         Console.WriteLine("Index of max");
         Console.WriteLine(source.IndexOfMax().Map(out var index) ? source[index] : "empty");

         Console.WriteLine();

         Console.WriteLine("Index of max length");
         Console.WriteLine(source.IndexOfMax(s => s.Length).Map(out index) ? source[index] : "empty");

         Console.WriteLine();

         Console.WriteLine("Index of min");
         Console.WriteLine(source.IndexOfMin().Map(out index) ? source[index] : "empty");

         Console.WriteLine();

         Console.WriteLine("Index of min length");
         Console.WriteLine(source.IndexOfMin(s => s.Length).Map(out index) ? source[index] : "empty");
      }

      [TestMethod]
      public void MonadMinMaxTest()
      {
         var strings = array("foobar", "foo", "a", "bar");
         var _max = strings.MaxOrNone();
         if (_max)
         {
            Console.WriteLine($"Max value: {_max}");
         }

         _max = strings.MaxOrNone(s => s.Length);
         if (_max)
         {
            Console.WriteLine($"Max length: {_max}");
         }
      }

      [TestMethod]
      public void AllMatchTest()
      {
         var left = array("foobar", "foo", "a", "bar");
         var right = array(6, 3, 1, 3);
         left.AllMatch(right, (s, i) => s.Length == i).Must().BeTrue().OrThrow();

         var right2 = array(5, 3, 1, 3);
         left.AllMatch(right2, (s, i) => s.Length == i).Must().BeTrue().OrThrow();
      }

      [TestMethod]
      public void IntegerEnumerableTest1()
      {
         foreach (var i in 10.Times())
         {
            Console.Write($"{i,3}");
         }
      }

      [TestMethod]
      public void IntegerEnumerableTest2()
      {
         foreach (var i in 10.To(20))
         {
            Console.Write($"{i,3}");
         }
      }

      [TestMethod]
      public void IntegerEnumerableTest3()
      {
         foreach (var i in 20.To(10).By(2))
         {
            Console.Write($"{i,3}");
         }
      }

      [TestMethod]
      public void ByTest()
      {
         var numbers = 1.To(10).ToArray();
         foreach (var by2 in numbers.By(2))
         {
            Console.WriteLine("----------");
            foreach (var value in by2)
            {
               Console.WriteLine(value);
            }
         }
      }

      [TestMethod]
      public void SortByListTest()
      {
         ItemToSort[] array = { new("a"), new("b"), new("c"), new("d"), new("e"), new("f") };
         var enumerable = array.SortByList(i => i.Key, "f", "e", "b", "a", "c", "d");
         foreach (var itemToSort in enumerable)
         {
            Console.WriteLine(itemToSort);
         }
      }

      [TestMethod]
      public void ListOrderByTest()
      {
         SortableItem[] array =
         {
            new() { Branch = "alpha", Order = "Good" }, new() { Branch = "bravo", Order = "Good" },
            new() { Branch = "charlie", Order = "Ugly" }, new() { Branch = "delta", Order = "Bad" },
            new() { Branch = "echo", Order = "Good" }, new() { Branch = "foxtrot", Order = "Ugly" }
         };
         var sorted = array.OrderBy(i => i.Order, new[] { "Good", "Bad", "Ugly" });
         foreach (var sortableItem in sorted)
         {
            Console.WriteLine($"{sortableItem.Branch}: {sortableItem.Order}");
         }

         Console.WriteLine("=".Repeat(80));

         sorted = array.OrderByDescending(i => i.Order, new[] { "Good", "Bad", "Ugly" });
         foreach (var sortableItem in sorted)
         {
            Console.WriteLine($"{sortableItem.Branch}: {sortableItem.Order}");
         }
      }
   }
}