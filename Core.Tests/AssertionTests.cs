using System;
using System.Collections.Generic;
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
            from positive in value.MustAs(nameof(value)).BePositive().Try()
            from greater in value.MustAs(nameof(value)).BeGreaterThan(100).Try()
            from less in value.MustAs(nameof(value)).BeLessThanOrEqual(200).Try()
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

      [TestMethod]
      public void TypeAssertionTest()
      {
         0.GetType().Must().EqualToTypeOf(1).Assert();
         0.GetType().Must().BeConvertibleTo(1L.GetType()).Assert();
         0.GetType().Must().BeValue().Assert();
         "".GetType().Must().BeClass().Assert();
         typeof(DayOfWeek).Must().BeEnumeration().Assert();
         var listType = new List<string>().GetType();
         listType.Must().BeGeneric().ContainGenericArgument(typeof(string)).Assert();
      }
   }
}