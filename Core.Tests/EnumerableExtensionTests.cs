using System;
using System.Linq;
using Core.Assertions;
using Core.Enumerables;
using Core.Monads;
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

      [TestMethod]
      public void FirstOrNoneTest()
      {
         var testArray = 'f'.DownTo('a');
         if (testArray.FirstOrNone().Map(out var ch))
         {
            ch.ToString().Must().Equal("f").OrThrow();
            Console.WriteLine($"{ch} == 'f'");
         }
      }

      [TestMethod]
      public void FirstOrFailTest()
      {
         var testArray = 0.UpUntil(10).ToArray();
         if (testArray.FirstOrFail("Not found").Map(out var first, out var exception))
         {
            Console.WriteLine(first);
         }
         else
         {
            Console.WriteLine(exception.Message);
         }

         testArray = array<int>();
         if (testArray.FirstOrFail("Not found").Map(out first, out exception))
         {
            Console.WriteLine(first);
         }
         else
         {
            Console.WriteLine(exception.Message);
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
         if (strings.MaxOrNone().Map(out var max))
         {
            Console.WriteLine($"Max value: {max}");
         }

         if (strings.MaxOrNone(s => s.Length).Map(out max))
         {
            Console.WriteLine($"Max length: {max}");
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
   }
}