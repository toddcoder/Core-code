using System;
using Core.Assertions;
using Core.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

namespace Core.Tests
{
   [TestClass]
   public class AssertionTests
   {
      [TestMethod]
      public void ComparableTest()
      {
         if (153.Must().BeBetween(1).And(200))
         {
            Console.WriteLine("153 between 1 and 200");
         }

         if (153.Must().BeBetween(0).Until(154))
         {
            Console.WriteLine("153 between 0 and 154 exclusively");
         }

         if (0.Must().BeBetween(1).And(153))
         {
            Console.WriteLine("0 between 1 and 153");
         }
         else
         {
            Console.WriteLine("0 outside of 1...153");
         }

         if (0.Must().Not.BePositive())
         {
            Console.WriteLine("0 not positive");
         }

         var value = 153;
         var result =
            from positive in value.Must().BePositive().Try()
            from greater in value.Must().BeGreaterThan(100).Try()
            from less in value.Must().BeLessThanOrEqual(200).Try()
            select less;
         if (result.If(out var integer, out var exception))
         {
            Console.WriteLine($"integer is {integer}");
         }
         else
         {
            Console.WriteLine(exception.Message);
         }
      }

      [TestMethod]
      public void StringAssertionTest()
      {
         foreach (var str in array("foobar", "", null))
         {
            if (str.Must().Not.BeNull())
            {
               Console.WriteLine($"{str ?? "null"} is not null");
            }
            else
            {
               Console.WriteLine("is null");
            }

            if (str.Must().Not.BeEmpty())
            {
               Console.WriteLine($"{str ?? "null"} is not empty");
            }
            else
            {
               Console.WriteLine("is empty");
            }
         }
      }

      [TestMethod]
      public void DictionaryAssertionTest()
      {
         var hash = new Hash<char, string> { ['a'] = "alfa", ['b'] = "bravo", ['c'] = "charlie" };
         hash.Must().Not.BeNullOrEmpty().Assert();
         hash.Must().HaveKeyOf('b').Assert();
      }
   }
}