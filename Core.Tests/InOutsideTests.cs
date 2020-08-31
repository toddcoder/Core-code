using System;
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
         foreach (var (segment, index, status) in inOutside.Enumerable("SELECT foobar as 'can''t';"))
         {
            switch (status)
            {
               case InOutsideStatus.Outside:
                  Console.Write("   Inside: ");
                  break;
               case InOutsideStatus.Inside:
                  Console.Write("Outside: ");
                  break;
               case InOutsideStatus.BeginDelimiter:
                  Console.Write("[");
                  break;
               case InOutsideStatus.EndDelimiter:
                  Console.Write("]");
                  break;
            }

            Console.WriteLine($"<<{segment}>>@{index}");
         }
      }

      [TestMethod]
      public void UnusualDelimiterTest()
      {

      }
   }
}