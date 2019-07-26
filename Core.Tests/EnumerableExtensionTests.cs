using System;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
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

      [TestMethod]
      public void LinqTerminologyTest()
      {
         var source = 0.UpUntil(10).ToArray();

         var mapped = source.Map(i => i * 10);
         writeEnumerable("Map", mapped);

         var filtered = source.If(i => i % 2 == 0);
         writeEnumerable("If", filtered);
      }
   }
}