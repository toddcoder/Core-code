using System;
using Core.Assertions;
using Core.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Assertions.AssertionFunctions;

namespace Core.Tests
{
   internal enum Enum1
   {
      Alpha,
      Bravo,
      Charlie
   }

   [Flags]
   internal enum Enum2
   {
      Add = 1,
      Overwrite = 2,
      Delete = 4
   }

   [TestClass]
   public class EnumTests
   {
      [TestMethod]
      public void InTest()
      {
         assert(() => Enum1.Alpha.In(Enum1.Alpha, Enum1.Charlie)).Must().BeTrue().OrThrow();

         var @enum = Enum2.Add | Enum2.Delete;
         assert(() => @enum.In(Enum2.Overwrite, Enum2.Add)).Must().BeTrue().OrThrow();

         if (assert(() => @enum.In(Enum2.Overwrite)).Must().BeTrue().OrFailure().IfNot(out var exception))
         {
            Console.WriteLine(exception.Message);
         }
      }
   }
}