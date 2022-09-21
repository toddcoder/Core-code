using System;
using System.Collections.Generic;
using System.Linq;
using Core.Assertions;
using Core.Collections;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

namespace Core.Tests
{
   [TestClass]
   public class HashTest
   {
      protected static void test(AutoHash<string, int> autoHash, string message, Func<string, int> newLambda)
      {
         var keys = array("alpha", "bravo", "charlie");

         Console.WriteLine("-".Repeat(80));
         Console.WriteLine(message.LeftJustify(80, '-'));
         foreach (var key in keys)
         {
            Console.WriteLine($"{key}: {autoHash[key]}");
         }

         autoHash.DefaultLambda = newLambda;

         Console.WriteLine("-".Repeat(80));
         foreach (var key in keys)
         {
            Console.WriteLine($"{key}: {autoHash[key]}");
         }
      }

      [TestMethod]
      public void AutoHashTest()
      {
         test(new AutoHash<string, int>(_ => -1), "No auto-add", _ => -2);
         test(new AutoHash<string, int>(_ => -1, true), "Auto-add", _ => -2);

         var autoHash = new AutoHash<string, int>(k => k.Length);
         autoHash.AddKeys(new List<string> { "alpha", "bravo" });
         test(autoHash, "AddKeys", k => -k.Length);
      }

      [TestMethod]
      public void StringHashTest()
      {
         var hash = new StringHash<int>(true) { ["alpha"] = 0, ["bravo"] = 1, ["charlie"] = 2 };
         hash.Must().HaveKeyOf("Bravo").OrThrow();
      }

      [TestMethod]
      public void GroupToHashTest()
      {
         var random = new Random(153);
         var array = Enumerable.Range(0, 1000).Select(_ => random.Next(10)).ToArray();
         var _hash = array.GroupToHash(i => i < 5 ? "lower" : "upper");
         if (_hash)
         {
            foreach (var (key, value) in _hash.Value)
            {
               Console.WriteLine($"key {key}: {value.Length}");
            }
         }
      }
   }
}