using System;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Core.Numbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

namespace Core.Tests
{
   [TestClass]
   public class EnumerableExtensionTests
   {
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
   }
}