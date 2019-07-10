using System;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
   }
}