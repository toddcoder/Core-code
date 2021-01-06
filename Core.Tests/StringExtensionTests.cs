using System;
using Core.Assertions;
using Core.Enumerables;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Assertions.AssertionFunctions;

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

      protected void test(string name, string camelResult, string pascalResult)
      {
         var camel = name.ToCamel();
         var pascal = name.ToPascal();

         Console.WriteLine($"camel:  {camel}");
         Console.WriteLine($"pascal: {pascal}");
         assert(() => camel).Must().Equal(camelResult).OrThrow();
         assert(() => pascal).Must().Equal(pascalResult).OrThrow();
      }

      [TestMethod]
      public void CamelAndPascalCaseTest()
      {
         test("SetSQL_nameForUser_ID", "setSQLNameForUserId", "SetSQLNameForUserId");
         test("TARGET", "target", "Target");
         test("TARGET_REPORT", "targetReport", "TargetReport");
         test("C1","c1", "C1");
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
   }
}