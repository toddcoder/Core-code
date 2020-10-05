using System;
using System.Text;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class StringClassTests
   {
      [TestMethod]
      public void SlicerTest()
      {
         Slicer slicer = "Throw the old woman out!";

         Console.WriteLine(slicer[6, 13]);
         slicer[6, 13] = "Lucy";

         Console.WriteLine(slicer[0, 5]);
         slicer[0, 5] = "Toss";

         Console.WriteLine(slicer[23, 1]);
         slicer[23, 1] = "?!";
         Console.WriteLine(slicer);
      }

      [TestMethod]
      public void SlicedTest()
      {
         var source = "Throw the old woman out!";
         Slicer slicer = source;

         Console.WriteLine(slicer[6, 13]);
         slicer[6, 13] = "Lucy";

         Console.WriteLine(slicer[0, 5]);
         slicer[0, 5] = "Toss";

         Console.WriteLine(slicer[23, 1]);
         slicer[23, 1] = "?!";

         var builder = new StringBuilder(source);

         foreach (var (index, length, text) in slicer)
         {
            builder.Remove(index, length);
            builder.Insert(index, text);
         }

         Console.WriteLine(builder);
      }

      [TestMethod]
      public void ListStringTest()
      {
         var listString = new ListString("alpha", "; ", true) { Text = "bravo" };
         listString.Text = "charlie";
         Console.WriteLine(listString);
         listString.Text = "alpha";
         Console.WriteLine(listString);
      }

      [TestMethod]
      public void DestringifyAsSqlTest()
      {
         var source = "SELECT 'I can''t do this' from foobar --yes you can\r\nprint ''";
         var delimitedText = DelimitedText.AsSql();
         var parsed = delimitedText.Destringify(source);
         Console.WriteLine(parsed);
         Console.WriteLine(delimitedText.Restringify(parsed, RestringifyQuotes.SingleQuote));
      }

      [TestMethod]
      public void DestringifyAsSqlTest2()
      {
         var source = "UPDATE Foobar SET A = -A, B = 'This is a test' /*a test*/;";
         var delimitedText = DelimitedText.AsSql();
         var parsed = delimitedText.Destringify(source);
         Console.WriteLine(parsed);
         Console.WriteLine(delimitedText.Restringify(parsed, RestringifyQuotes.SingleQuote));

         var inOutside = new DelimitedText("'", "'", "''", friendly: false);
         foreach (var (text, _, _) in inOutside.Enumerable(source))
         {
            Console.WriteLine($"<{text}>");
         }
      }
   }
}