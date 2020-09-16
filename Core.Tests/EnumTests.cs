using System;
using Core.Assertions;
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
         assert(() => Enum1.Alpha).Must().HaveAnyOf(Enum1.Alpha, Enum1.Charlie).OrThrow();

         var @enum = Enum2.Add | Enum2.Delete;
         assert(() => @enum).Must().HaveAnyOf(Enum2.Overwrite, Enum2.Add).OrThrow();

         if (assert(() => @enum).Must().HaveAnyOf(Enum2.Overwrite).OrFailure().IfNot(out var exception))
         {
            Console.WriteLine(exception.Message);
         }

         assert(() => @enum).Must().HaveAllOf(Enum2.Add, Enum2.Delete).OrFailure();
         if (assert(() => @enum).Must().HaveAllOf(Enum2.Add, Enum2.Delete, Enum2.Overwrite).OrFailure().IfNot(out exception))
         {
            Console.WriteLine(exception.Message);
         }
      }
   }
}