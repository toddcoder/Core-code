using System;
using Core.Assertions;
using Core.Enumerables;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

namespace Core.Tests
{
   [TestClass]
   public class StringExtensionTests
   {
      [TestMethod]
      public void PluralTest()
      {
         var message = "There (is,are) # book(s)";
         Console.WriteLine(message.Plural(1));
         Console.WriteLine(message.Plural(2));

         message = "child(ren)";
         Console.WriteLine(message.Plural(1));
         Console.WriteLine(message.Plural(2));

         message = @"\#G(OO,EE)SE";
         Console.WriteLine(message.Plural(1));
         Console.WriteLine(message.Plural(2));
      }

      protected static void test(string name, string camelResult, string pascalResult)
      {
         var camel = name.ToCamel();
         var pascal = name.ToPascal();

         Console.WriteLine($"camel:  {camel}");
         Console.WriteLine($"pascal: {pascal}");
         camel.Must().Equal(camelResult).OrThrow();
         pascal.Must().Equal(pascalResult).OrThrow();
      }

      [TestMethod]
      public void CamelAndPascalCaseTest()
      {
         test("SetSQL_nameForUser_ID", "setSQLNameForUserId", "SetSQLNameForUserId");
         test("TARGET", "target", "Target");
         test("TARGET_REPORT", "targetReport", "TargetReport");
         test("C1", "c1", "C1");
      }

      [TestMethod]
      public void EllipticalTest()
      {
         var text = "'We had joy, we had fun, we had seasons in the sun' -- the second most depressing song in the world";
         foreach (var limit in 80.DownTo(-10, -10))
         {
            Console.WriteLine(limit);
            Console.WriteLine(text.Elliptical(limit, ' '));
            Console.WriteLine();
         }

         Console.WriteLine();

         text = "Now Is The Time For All Good Men To Come To The Aid Of Their Party";
         foreach (var limit in 80.DownTo(-10, -10))
         {
            Console.WriteLine(limit);
            Console.WriteLine(text.Elliptical(limit, " ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Console.WriteLine();
         }
      }

      [TestMethod]
      public void TruncateTest()
      {
         var text = "'We had joy, we had fun, we had seasons in the sun' -- the second most depressing song in the world";
         foreach (var limit in 80.DownTo(-10, -10))
         {
            Console.WriteLine(limit);
            Console.WriteLine(text.Truncate(limit));
            Console.WriteLine();
         }
      }

      [TestMethod]
      public void JustificationTest()
      {
         var text = "Apples and oranges";
         Console.WriteLine($"|{text.LeftJustify(80)}|");
         Console.WriteLine($"|{text.RightJustify(80)}|");
         Console.WriteLine($"|{text.Center(80)}|");
      }

      [TestMethod]
      public void NormalizeWhitespaceTest()
      {
         var text = "foobar  \t \r\n \tis okay";
         Console.WriteLine(text.NormalizeWhitespace());
      }

      [TestMethod]
      public void ExactlyTest()
      {
         var empty = string.Empty;
         var tooLittle = "the cat jumped over the dog";
         var tooMuch = $"{tooLittle}, but the dog didn't care.";
         var needsNormalization = "the cat\tthe dog\r\nThey need help.";

         Console.WriteLine(" 0000000001111111111222222222233333333334");
         Console.WriteLine(" 1234567890123456789012345678901234567890");
         Console.WriteLine($"|{empty.Exactly(40)}|");
         Console.WriteLine($"|{tooLittle.Exactly(40)}|");
         Console.WriteLine($"|{tooMuch.Exactly(40)}|");
         Console.WriteLine($"|{needsNormalization.Exactly(40)}|");
      }

      [TestMethod]
      public void LinesTest()
      {
         var text = "foobar\r\nfoobaz\rfoo\nbar";
         var lines = text.Lines();
         foreach (var line in lines)
         {
            Console.WriteLine(line);
         }
      }

      protected static void quoteString(string text, bool indent = true)
      {
         var indentation = indent ? "  " : "";
         Console.WriteLine($"{indentation}\"{text}\"");
      }

      protected void writeWords(string text)
      {
         quoteString(text, false);
         foreach (var word in text.Words())
         {
            quoteString(word);
         }

         Console.WriteLine("=".Repeat(80));
      }

      [TestMethod]
      public void WordsTest()
      {
         foreach (var text in array("maryMaryContrary", "ExamplesRemarksConstructorsProperties", "alpha-bravo-charlie"))
         {
            writeWords(text);
         }

         writeWords("LEFT OUTER JOIN");
      }
   }
}