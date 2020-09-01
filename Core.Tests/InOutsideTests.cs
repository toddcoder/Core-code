using System;
using System.Linq;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class InOutsideTests
   {
      [TestMethod]
      public void BasicInOutsideTest()
      {
         var inOutside = new InOutside("'", "'", "''", "'", friendly: false);
         foreach (var (text, index, status) in inOutside.Enumerable("SELECT foobar as 'can''t';"))
         {
            switch (status)
            {
               case InOutsideStatus.Inside:
                  Console.Write("   Inside: ");
                  break;
               case InOutsideStatus.Outside:
                  Console.Write("Outside: ");
                  break;
               case InOutsideStatus.BeginDelimiter:
                  Console.Write("[");
                  break;
               case InOutsideStatus.EndDelimiter:
                  Console.Write("]");
                  break;
            }

            Console.WriteLine($"<<{text}>>@{index}");
         }
      }

      [TestMethod]
      public void UnusualDelimiterTest()
      {
         var inOutside = new InOutside("'('", "')'", @"'\)'");
         foreach (var (text, _, _) in inOutside.Enumerable("foo(bar)baz").Where(i => i.status == InOutsideStatus.Outside))
         {
            Console.Write(text);
         }
      }

      [TestMethod]
      public void SwapTest()
      {
         var inOutside = new InOutside("'", "'", "''", "'", friendly: false);
         var source = "'a = b' != 'b = a'";
         var result = inOutside.Enumerable(source)
            .Where(t => t.status == InOutsideStatus.Outside && t.text.Contains("="))
            .ToArray();
         if (result.Length == 1 && result[0].text.FindByRegex("/s+ ['!=<>'] '=' /s+").If(out var slice))
         {
            var index = slice.Index + result[0].index;
            var length = slice.Length;
            Console.Write(source.Drop(index + length));
            Console.Write(source.Drop(index).Keep(length));
            Console.WriteLine(source.Keep(index));
         }
      }

      [TestMethod]
      public void EmptyStringTest()
      {
         var inOutside = new InOutside("'", "'", "''", "'", friendly: false);
         var source = "'a', '', 'b'";
         foreach (var (text, _, _)  in inOutside.Enumerable(source).Where(t=>t.status==InOutsideStatus.Inside))
         {
            Console.WriteLine($"<<{text}>>");
         }
      }
   }
}