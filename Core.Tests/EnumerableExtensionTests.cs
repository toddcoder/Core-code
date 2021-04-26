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
      [TestMethod]
      public void FirstOrNoneTest()
      {
         var testArray = 'f'.DownTo('a');
         if (testArray.FirstOrNone().If(out var ch))
         {
            ch.ToString().Must().Equal("f").OrThrow();
            Console.WriteLine($"{ch} == 'f'");
         }
      }

      [TestMethod]
      public void FirstOrFailTest()
      {
         var testArray = 0.UpUntil(10).ToArray();
         if (testArray.FirstOrFail("Not found").If(out var first, out var exception))
         {
            Console.WriteLine(first);
         }
         else
         {
            Console.WriteLine(exception.Message);
         }

         testArray = array<int>();
         if (testArray.FirstOrFail("Not found").If(out first, out exception))
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
         Console.WriteLine(source.IndexOfMax().If(out var index) ? source[index] : "empty");

         Console.WriteLine();

         Console.WriteLine("Index of max length");
         Console.WriteLine(source.IndexOfMax(s => s.Length).If(out index) ? source[index] : "empty");

         Console.WriteLine();

         Console.WriteLine("Index of min");
         Console.WriteLine(source.IndexOfMin().If(out index) ? source[index] : "empty");

         Console.WriteLine();

         Console.WriteLine("Index of min length");
         Console.WriteLine(source.IndexOfMin(s => s.Length).If(out index) ? source[index] : "empty");
      }

      [TestMethod]
      public void MonadMinMaxTest()
      {
         var strings = array("foobar", "foo", "a", "bar");
         if (strings.MaxOrNone().If(out var max))
         {
            Console.WriteLine($"Max value: {max}");
         }

         if (strings.MaxOrNone(s => s.Length).If(out max))
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
   }
}