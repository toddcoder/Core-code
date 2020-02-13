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

      [TestMethod]
      public void CamelAndPascalCaseTest()
      {
         var name = "SetSQL_nameForUser_ID";
         var camel = name.ToCamel();
         var pascal = name.ToPascal();

         Console.WriteLine($"camel:  {camel}");
         Console.WriteLine($"pascal: {pascal}");
         assert(() => camel).Must().Equal("setSQLNameForUserID").OrThrow();
         assert(() => pascal).Must().Equal("SetSQLNameForUserID").OrThrow();

         name = "TARGET";
         camel = name.ToCamel();
         pascal = name.ToPascal();

         Console.WriteLine($"camel:  {camel}");
         Console.WriteLine($"pascal: {pascal}");
         assert(() => camel).Must().Equal("target").OrThrow();
         assert(() => pascal).Must().Equal("Target").OrThrow();
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
   }
}