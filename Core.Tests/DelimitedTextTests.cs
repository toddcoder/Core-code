using System;
using System.Linq;
using Core.Applications;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.ObjectGraphs;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Lambdas.LambdaFunctions;
using static Core.RegularExpressions.RegexExtensions;

namespace Core.Tests
{
   [TestClass]
   public class DelimitedTextTests
   {
      [TestMethod]
      public void BasicDelimitedTextTest()
      {
         var delimitedText = DelimitedText.AsSql(friendly: false);
         delimitedText.ExceptReplacement = "'".Some();
         var source = "SELECT foobar as 'can''t';";
         Console.WriteLine(source);

         foreach (var (text, index, status) in delimitedText.Enumerable(source))
         {
            switch (status)
            {
               case DelimitedTextStatus.Inside:
                  Console.Write("   Inside: ");
                  break;
               case DelimitedTextStatus.Outside:
                  Console.Write("Outside: ");
                  break;
               case DelimitedTextStatus.BeginDelimiter:
                  Console.Write("[");
                  break;
               case DelimitedTextStatus.EndDelimiter:
                  Console.Write("]");
                  break;
            }

            Console.WriteLine($"<<{text}>>@{index}");
         }
      }

      [TestMethod]
      public void UnusualDelimiterTest()
      {
         var delimitedText = new DelimitedText("'('", "')'", @"'\)'");
         foreach (var (text, _, _) in delimitedText.Enumerable("foo(bar)baz").Where(i => i.status == DelimitedTextStatus.Outside))
         {
            Console.Write(text);
         }
      }

      [TestMethod]
      public void SwapTest()
      {
         var delimitedText = DelimitedText.AsSql();
         var source = "'a = b' != 'b = a'";
         var result = delimitedText.Enumerable(source)
            .Where(t => t.status == DelimitedTextStatus.Outside && t.text.Contains("="))
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
         var delimitedText = DelimitedText.AsSql();
         var source = "'a', '', 'b'";
         foreach (var (text, _, _) in delimitedText.Enumerable(source).Where(t => t.status == DelimitedTextStatus.Inside))
         {
            Console.WriteLine($"<<{text}>>");
         }

         Console.WriteLine("---");

         var strings = delimitedText.StringsOnly(source).Select(t => t.text);
         foreach (var @string in strings)
         {
            Console.WriteLine($"<<{@string}>>");
         }
      }

      [TestMethod]
      public void Transforming1Test()
      {
         var delimitedText = DelimitedText.AsSql();
         var result = delimitedText.Transform("a BETWEEN 0 AND 100", "$0 BETWEEN $1 AND $2", "$0 >= $1 AND $0 <= $2");
         Console.WriteLine(result);
      }

      [TestMethod]
      public void Transforming2Test()
      {
         var delimitedText = DelimitedText.AsSql();
         var result = delimitedText.Transform("'9/1/2020' BETWEEN '1/1/2020' AND '12/31/2020'", "$0 BETWEEN $1 AND $2", "$0 >= $1 AND $0 <= $2");
         Console.WriteLine(result);
      }

      [TestMethod]
      public void Transforming3Test()
      {
         var delimitedText = DelimitedText.AsSql();
         var result = delimitedText.Transform("(111 + 123       - 153) / 3", "($0+$1-$2) / 3", "sum('$0', '$1', '$2') / n('$0', '$1', '$2')");
         Console.WriteLine(result);

         delimitedText.TransformingMap = func<string, string>(s => s.Trim()).Some();
         result = delimitedText.Transform("(111 + 123       - 153) / 3", "($0+$1-$2) / 3", "sum('$0', '$1', '$2') / n('$0', '$1', '$2')");
         Console.WriteLine(result);

         delimitedText.TransformingMap = func<string, string>(s1 => s1.Split("/s* '+' /s*").Select(s2 => s2.Trim().Quotify()).ToString(", ")).Some();
         result = delimitedText.Transform("(111 + 123       + 153) / 3", "($0) / 3", "sum($0) / n($0)");
         Console.WriteLine(result);
      }

      [TestMethod]
      public void SlicerTest()
      {
         var source = "'foobar' ELSE-1 'ELSE-1'";
         var delimitedText = DelimitedText.AsSql();

         foreach (var (text, index, _) in delimitedText.Enumerable(source).Where(t => t.status == DelimitedTextStatus.Outside))
         {
            foreach (var (_, sliceIndex, length) in text.FindAllByRegex("/b 'ELSE' ['-+']"))
            {
               var fullIndex = index + sliceIndex;
               delimitedText[fullIndex, length] = "ELSE -";
            }
         }

         Console.WriteLine(delimitedText);

         delimitedText.Status[DelimitedTextStatus.Inside] = true;
         delimitedText.Replace(source, "/b 'ELSE' ['-+'] /d+", "?");
         Console.WriteLine(delimitedText);
      }

      [TestMethod]
      public void SplitTest()
      {
         var source = "'foobar' AND 'foobaz' OR 'foo' AND 'bar'";
         var delimitedText = DelimitedText.AsSql(true);
         var slices = delimitedText.Split(source, "/s+ 'OR' /s+").ToArray();
         foreach (var slice in slices)
         {
            Console.WriteLine($"<{slice.Text}>");
         }

         var withParentheses = slices.Select(s => $"({s.Text})").ToString(" OR ");
         Console.WriteLine("---");
         Console.WriteLine(withParentheses);
      }

      [TestMethod]
      public void ReplacementTest()
      {
         var source = "'foobar' AND 'foobaz' OR 'foo' AND 'bar'";
         var delimitedText = DelimitedText.AsSql(true);
         delimitedText.Replace(source, "/b 'AND' | 'OR' /b", slice => slice.Text.Same("AND") ? "OR" : "AND");
         Console.WriteLine(delimitedText);

         source = "'foobar' && 'foobaz' || 'foo' && 'bar'";
         delimitedText.Replace(source, "/b 'AND' | 'OR' /b", slice => slice.Text.Same("AND") ? "OR" : "AND");
         Console.WriteLine(delimitedText);

         delimitedText.Replace(source, "'&&' | '||'", slice => slice.Text == "&&" ? "||" : "&&");
         Console.WriteLine(delimitedText);

         Console.WriteLine("---");

         delimitedText.Status = DelimitedTextStatus.Inside;
         delimitedText.Replace(source, "/w+", slice => slice.Text.ToUpper());
         Console.WriteLine(delimitedText);

         delimitedText.Replace(source, "/w+", "?");
         Console.WriteLine(delimitedText);
      }

      [TestMethod]
      public void EnumerationTest()
      {
         var source = "'foobar' AND 'foobaz' OR 'foo' AND 'bar'";
         var delimitedText = DelimitedText.AsSql(true);
         foreach (var (index, _) in delimitedText.Substrings(source, "AND"))
         {
            delimitedText[index, 3] = "&&";
         }

         source = delimitedText.ToString();
         delimitedText.Status[DelimitedTextStatus.Outside] = false;
         delimitedText.Status[DelimitedTextStatus.BeginDelimiter] = true;
         delimitedText.Status[DelimitedTextStatus.EndDelimiter] = true;

         foreach (var (index, status) in delimitedText.Substrings(source, "'"))
         {
            switch (status)
            {
               case DelimitedTextStatus.BeginDelimiter:
                  delimitedText[index, 1] = "<<";
                  break;
               case DelimitedTextStatus.EndDelimiter:
                  delimitedText[index, 1] = ">>";
                  break;
            }
         }

         Console.WriteLine(delimitedText);

         source = delimitedText.ToString();
         delimitedText.BeginPattern = "'<<'";
         delimitedText.EndPattern = "'>>'".Some();
         delimitedText.Status[DelimitedTextStatus.Outside] = true;
         delimitedText.Status[DelimitedTextStatus.Inside] = true;

         foreach (var (text, index, status) in delimitedText.Matches(source, "'&&' | '<<' | '>>' | 'foo'"))
         {
            switch (status)
            {
               case DelimitedTextStatus.Outside:
                  delimitedText[index, text.Length] = "AND";
                  break;
               case DelimitedTextStatus.Inside:
                  delimitedText[index, text.Length] = "???";
                  break;
               case DelimitedTextStatus.BeginDelimiter:
               case DelimitedTextStatus.EndDelimiter:
                  delimitedText[index, text.Length] = "\"";
                  break;
            }
         }

         Console.WriteLine(delimitedText);
      }

      [TestMethod]
      public void DestringifyingAndRestringifyingTest()
      {
         var source = "'a' and 'b' equals 'a' plus 'b'";
         var delimitedText = DelimitedText.AsSql();

         var destringified = delimitedText.Destringify(source);
         Console.WriteLine(destringified);

         foreach (var (index, item) in delimitedText.Strings.IndexedEnumerable())
         {
            Console.WriteLine($"{index}: {item}");
         }

         destringified = destringified.ToUpper();

         Console.WriteLine("---");

         var restringed = delimitedText.Restringify(destringified, RestringifyQuotes.DoubleQuote);
         Console.WriteLine(restringed);
      }

      [TestMethod]
      public void EmbeddedStringsTest()
      {
         var source = "find -P\"^ 'A'\" -F '.'";
         var delimitedText = DelimitedText.BothQuotes();
         foreach (var (text, _, status) in delimitedText.Enumerable(source))
         {
            switch (status)
            {
               case DelimitedTextStatus.Outside:
                  Console.WriteLine($"outside: {text.Guillemetify()}");
                  break;
               case DelimitedTextStatus.Inside:
                  Console.WriteLine($"inside: {text.Guillemetify()}");
                  break;
               case DelimitedTextStatus.BeginDelimiter:
                  Console.WriteLine($"begin delimiter: {text.Guillemetify()}");
                  break;
               case DelimitedTextStatus.EndDelimiter:
                  Console.WriteLine($"end delimiter: {text.Guillemetify()}");
                  break;
            }
         }
      }

      [TestMethod]
      public void DependentClassesTest()
      {
         ObjectGraph objectGraph = "foobar{a->'alpha';$b->\"bravo\";@c->charlie}";
         Console.WriteLine(objectGraph);

         var arguments = new Arguments("find -P\"^ 'A'\" -F '.'");
         foreach (var argument in arguments)
         {
            Console.WriteLine(argument.Text.Guillemetify());
         }

         StringStringHash stringHash = "a? -> 'foobar', '$ b' -> \"bravo\", @c->charlie";
         foreach (var (key, value) in stringHash)
         {
            Console.WriteLine($"{key}=>{value}");
         }

         CSV csv = "A,Alpha,180.00\r\nB,Bravo,\"1,982.18\"\r\n,C,Charlie,\"2,020.00\"";
         foreach (var record in csv)
         {
            foreach (var field in record)
            {
               Console.Write(field.Guillemetify());
            }
            Console.WriteLine();
         }
      }
   }
}