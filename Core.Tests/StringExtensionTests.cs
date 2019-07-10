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
         var message = "(is,are) book(s)";
         Console.WriteLine(message.Plural(1));
         Console.WriteLine(message.Plural(2));
      }
   }
}