using System;
using System.Linq;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Lambdas.LambdaFunctions;
using static Core.RegularExpressions.RegexExtensions;

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
         foreach (var (text, _, _) in inOutside.Enumerable(source).Where(t => t.status == InOutsideStatus.Inside))
         {
            Console.WriteLine($"<<{text}>>");
         }
      }

      [TestMethod]
      public void Transformer1Test()
      {
         var transformer = new Transformer("'", "'", "''", friendly: false);
         var result = transformer.Transform("a BETWEEN 0 AND 100", "$0 BETWEEN $1 AND $2", "$0 >= $1 AND $0 <= $2");
         Console.WriteLine(result);
      }

      [TestMethod]
      public void Transformer2Test()
      {
         var transformer = new Transformer("'", "'", "''", friendly: false);
         var result = transformer.Transform("'9/1/2020' BETWEEN '1/1/2020' AND '12/31/2020'", "$0 BETWEEN $1 AND $2", "$0 >= $1 AND $0 <= $2");
         Console.WriteLine(result);
      }

      [TestMethod]
      public void Transformer3Test()
      {
         var transformer = new Transformer("'", "'", "''", friendly: false);
         var result = transformer.Transform("(111 + 123       - 153) / 3", "($0+$1-$2) / 3", "sum('$0', '$1', '$2') / n('$0', '$1', '$2')");
         Console.WriteLine(result);

         transformer.Map = func<string, string>(s => s.Trim()).Some();
         result = transformer.Transform("(111 + 123       - 153) / 3", "($0+$1-$2) / 3", "sum('$0', '$1', '$2') / n('$0', '$1', '$2')");
         Console.WriteLine(result);

         transformer.Map = func<string, string>(s1 => s1.Split("/s* '+' /s*").Select(s2 => s2.Trim().Quotify()).ToString(", ")).Some();
         result = transformer.Transform("(111 + 123       + 153) / 3", "($0) / 3", "sum($0) / n($0)");
         Console.WriteLine(result);
      }

      [TestMethod]
      public void SlicerTest()
      {
         var source = "'foobar' ELSE-1 'foobaz'";
         var inOutside = new InOutside("'", "'", "''", friendly: false);

         foreach (var (text, index, _) in inOutside.Enumerable(source).Where(t => t.status == InOutsideStatus.Outside))
         {
            foreach (var (_, sliceIndex, length) in text.FindAllByRegex(@"\bELSE[-+]", friendly: false))
            {
               var fullIndex = index + sliceIndex;
               inOutside[fullIndex, length] = "ELSE -";
            }
         }

         Console.WriteLine(inOutside);
      }
   }
}