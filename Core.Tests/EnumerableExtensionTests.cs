using System;
using System.Collections.Generic;
using System.Linq;
using Core.Assertions;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;
using static Core.Assertions.AssertionFunctions;

namespace Core.Tests
{
   [TestClass]
   public class EnumerableExtensionTests
   {
      [TestMethod]
      public void FirstOrNoneTest()
      {
         var testArray = 'f'.DownTo('a');
         if (testArray.FirstOrNone().If(out var ch))
         {
            assert(() => ch.ToString()).Must().Equal("f").OrThrow();
            Console.WriteLine($"{ch} == 'f'");
         }
      }

      [TestMethod]
      public void FirstOrFailTest()
      {
         var testArray = 0.UpUntil(10).ToArray();
         var anyFirst = testArray.FirstOrFail("Not found");
         if (anyFirst.If(out var first, out var exception))
         {
            Console.WriteLine(first);
         }
         else
         {
            Console.WriteLine(exception.Message);
         }

         testArray = array<int>();
         anyFirst = testArray.FirstOrFail("Not found");
         if (anyFirst.If(out first, out exception))
         {
            Console.WriteLine(first);
         }
         else
         {
            Console.WriteLine(exception.Message);
         }
      }

      static void writeEnumerable<T>(string title, IEnumerable<T> array) => Console.WriteLine($"{title}: {array.Stringify()}");

      static void writeScalar(string title, object scalar) => Console.WriteLine($"{title}: {scalar}");

      static void writeHash<TKey, TValue>(string title, Hash<TKey, TValue[]> hash)
      {
         Console.WriteLine($"{title}: {hash.Map(kv => $"{kv.Key}: [{kv.Value.Stringify()}]").Stringify()}");
      }

      [TestMethod]
      public void LinqTerminologyTest()
      {
         var source = 0.UpUntil(10).ToArray();

         var mapped = source.Map(i => i * 10);
         writeEnumerable("Map", mapped);

         var filtered = source.If(i => i % 2 == 0);
         writeEnumerable("If", filtered);

         var sum = source.FoldLeft((a, v) => a + v);
         writeScalar("FoldLeft (no init)", sum);

         var enumerable = 1.UpTo(10);
         sum = enumerable.FoldLeft(1, (a, v) => a * 2 * v);
         writeScalar("FoldLeft", sum);

         sum = enumerable.FoldRight((v, a) => a - v);
         writeScalar("FoldRight (no init)", sum);

         sum = enumerable.FoldRight(35, (v, a) => a - v);
         writeScalar("FoldRight", sum);

         var sourceArray = array("alpha", "apple", "bravo", "charlie", "chuck");
         var hash = sourceArray.Group(v => v[0]);
         writeHash("Group", hash);

         var (isTrue, isFalse) = source.Partition(i => i.IsEven());
         writeEnumerable("is true", isTrue);
         writeEnumerable("is false", isFalse);
      }

      [TestMethod]
      public void IndexesOfMinMaxTest()
      {
         var source = array("alpha", "apples", "brat", "IP");

         Console.WriteLine("Index of max");
         if (source.IndexOfMax().If(out var index))
         {
            Console.WriteLine(source[index]);
         }
         else
         {
            Console.WriteLine("empty");
         }

         Console.WriteLine();

         Console.WriteLine("Index of max length");
         if (source.IndexOfMax(s => s.Length).If(out index))
         {
            Console.WriteLine(source[index]);
         }
         else
         {
            Console.WriteLine("empty");
         }

         Console.WriteLine();

         Console.WriteLine("Index of min");
         if (source.IndexOfMin().If(out index))
         {
            Console.WriteLine(source[index]);
         }
         else
         {
            Console.WriteLine("empty");
         }

         Console.WriteLine();

         Console.WriteLine("Index of min length");
         if (source.IndexOfMin(s => s.Length).If(out index))
         {
            Console.WriteLine(source[index]);
         }
         else
         {
            Console.WriteLine("empty");
         }
      }

      [TestMethod]
      public void MonadMinMaxTest()
      {
         var strings = array("foobar", "foo", "a", "bar");
         var anyMax = strings.MaxOrNone();
         if (anyMax.If(out var max))
         {
            Console.WriteLine($"Max value: {max}");
         }

         anyMax = strings.MaxOrNone(s => s.Length);
         if (anyMax.If(out max))
         {
            Console.WriteLine($"Max length: {max}");
         }
      }
   }
}