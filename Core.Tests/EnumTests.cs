using System;
using Core.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
         Enum1.Alpha.Must().HaveAnyOf(Enum1.Alpha, Enum1.Charlie).OrThrow();

         var @enum = Enum2.Add | Enum2.Delete;
         @enum.Must().HaveAnyOf(Enum2.Overwrite, Enum2.Add).OrThrow();

         if (@enum.Must().HaveAnyOf(Enum2.Overwrite).OrFailure().IfNot(out var exception))
         {
            Console.WriteLine(exception.Message);
         }

         @enum.Must().HaveAllOf(Enum2.Add, Enum2.Delete).OrFailure();
         if (@enum.Must().HaveAllOf(Enum2.Add, Enum2.Delete, Enum2.Overwrite).OrFailure().IfNot(out exception))
         {
            Console.WriteLine(exception.Message);
         }
      }
   }
}