using System;
using System.Linq;
using Core.Enumerables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

namespace Core.Tests
{
   [TestClass]
   public class EnumerableExtensionTests
   {
      [TestMethod]
      public void FirstOrFailTest()
      {
         var testArray = 0.UpUntil(10).ToArray();
         var anyFirst = testArray.FirstOrFail("Not found");
         if (anyFirst.If(out var first, out var exception))
         {
            Console.WriteLine(first);
         }
         else
         {
            Console.WriteLine(exception.Message);
         }

         testArray = array<int>();
         anyFirst = testArray.FirstOrFail("Not found");
         if (anyFirst.If(out first, out exception))
         {
            Console.WriteLine(first);
         }
         else
         {
            Console.WriteLine(exception.Message);
         }
      }
   }
}