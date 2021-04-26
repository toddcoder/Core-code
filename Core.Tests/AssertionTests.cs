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
         var number = 153;
         if (number.Must().BeBetween(1).And(200))
         {
            Console.WriteLine("153 between 1 and 200");
         }

         if (number.Must().BeBetween(0).Until(154))
         {
            Console.WriteLine("153 between 0 and 154 exclusively");
         }

         Console.WriteLine(0.Must().BeBetween(1).And(153) ? "0 between 1 and 153" : "0 outside of 1...153");

         if (0.Must().Not.BePositive())
         {
            Console.WriteLine("0 not positive");
         }

         var value = 153;
         var result =
            from positive in value.Must().BePositive().OrFailure()
            from greater in value.Must().BeGreaterThan(100).OrFailure()
            from less in value.Must().BeLessThanOrEqual(200).OrFailure()
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
            Console.WriteLine(str.Must().Not.BeNull() ? $"{str ?? "null"} is not null" : "is null");

            Console.WriteLine(str.Must().Not.BeEmpty() ? $"{str ?? "null"} is not empty" : "is empty");
         }
      }

      [TestMethod]
      public void DictionaryAssertionTest()
      {
         var hash = new Hash<char, string> { ['a'] = "alfa", ['b'] = "bravo", ['c'] = "charlie" };
         hash.Must().Not.BeNullOrEmpty().OrThrow();
         hash.Must().HaveKeyOf('b').OrThrow();
      }

      [TestMethod]
      public void TypeAssertionTest()
      {
         0.GetType().Must().EqualToTypeOf(1).OrThrow();
         0.GetType().Must().BeConvertibleTo(1L.GetType()).OrThrow();
         0.GetType().Must().BeValue().OrThrow();
         "".GetType().Must().BeClass().OrThrow();
         typeof(DayOfWeek).Must().BeEnumeration().OrThrow();
         var listType = new List<string>().GetType();
         listType.Must().BeGeneric().ContainGenericArgument(typeof(string)).OrThrow();
      }

      protected static int getX() => 3;

      protected class XClass
      {
         protected int x;

         public XClass(int x)
         {
            this.x = x;
         }

         public int X => x;
      }

      [TestMethod]
      public void NamelessAssertionTest1()
      {
         var x = 1;
         x.Must().Equal(2).OrThrow();
      }

      [TestMethod]
      public void NamelessAssertionTest2()
      {
         getX().Must().Equal(2).OrThrow();
      }

      [TestMethod]
      public void NamelessAssertionTest3()
      {
         var xObject = new XClass(10);
         xObject.X.Must().Equal(2).OrThrow();
      }

      [TestMethod]
      public void NamelessAssertionTest4()
      {
         var text = "";
         text.Must().Not.BeNullOrEmpty().OrThrow();
      }

      [TestMethod]
      public void NamelessAssertionTest5()
      {
         10.Must().Equal(2).OrThrow();
      }

      [TestMethod]
      public void CastAssertionTest1()
      {
         10.Must().Equal(2).OrThrow();
      }

      [TestMethod]
      public void CastAssertionTest2()
      {
         ((double)10).Must().Equal(2.0).OrThrow();
      }
   }
}