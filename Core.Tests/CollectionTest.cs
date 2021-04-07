﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Assertions;
using Core.Collections;
using Core.Collections.Infix;
using Core.DataStructures;
using Core.Dates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;
using static Core.Assertions.AssertionFunctions;

namespace Core.Tests
{
   [TestClass]
   public class CollectionTest
   {
      [TestMethod]
      public void InfixListWithInfixDataTest()
      {
         var list = new InfixList<int, char> { { 1, '+' }, { 2, '-' }, 3 };
         Console.WriteLine(list);

         var intStack = new Stack<int>();
         var charStack = new Stack<char>();

         foreach (var (number, _op) in list)
         {
            intStack.Push(number);

            if (_op.If(out var op))
            {
               charStack.Push(op);
            }
         }

         while (charStack.Count > 0)
         {
            var y = intStack.Pop();
            var x = intStack.Pop();
            var op = charStack.Pop();
            switch (op)
            {
               case '+':
                  intStack.Push(x + y);
                  break;
               case '-':
                  intStack.Push(x - y);
                  break;
            }
         }

         var result = intStack.Pop();
         assert(() => result).Must().Equal(result).OrThrow();
      }

      [TestMethod]
      public void PriorityQueueTest()
      {
         var queue = new PriorityQueue<int>();

         foreach (var item in array(1, 5, 3, 6, 9))
         {
            queue.Enqueue(item);
         }

         while (queue.Dequeue().If(out var item))
         {
            Console.WriteLine(item);
         }
      }

      [TestMethod]
      public void SetOfStringVsStringSetTest()
      {
         var setOfString = new Set<string> { "Case" };
         var stringSet = new StringSet(true) { "Case" };

         setOfString.Remove("case");
         assert(() => setOfString.Count).Must().Equal(1).OrThrow();

         stringSet.Remove("case");
         assert(() => stringSet.Count).Must().Equal(0).OrThrow();

         var fastSetOfString = new FastSet<string>() { "Case" };
         var fastStringSet = new FastStringSet(true) { "Case" };
         fastSetOfString.Remove("case");
         assert(() => fastSetOfString.Count).Must().Equal(1).OrThrow();

         fastStringSet.Remove("case");
         assert(() => fastStringSet.Count).Must().Equal(0).OrThrow();
      }

      protected static string time(Func<Func<string>, string> func)
      {
         var stopwatch = new Stopwatch();
         stopwatch.Start();

         var result = func(() => stopwatch.Elapsed.ToString(true));

         stopwatch.Stop();
         return result;
      }

      [TestMethod]
      public void SetVsFastSetTest()
      {
         var limit = 200_000;
         var set = new Set<int>(Enumerable.Range(0, limit));
         var fastSet = new FastSet<int>(Enumerable.Range(0, limit));

         var message = time(elapsedFunc =>
         {
            var foundCount = 0;
            var notFoundCount = 0;

            for (var i = 0; i < limit; i++)
            {
               if (set.Contains(i))
               {
                  foundCount++;
               }
               else
               {
                  notFoundCount++;
               }
            }

            return $"Set<int> = {elapsedFunc()}, found = {foundCount}, not found = {notFoundCount}";
         });
         Console.WriteLine(message);

         message = time(elapsedFunc =>
         {
            var foundCount = 0;
            var notFoundCount = 0;

            for (var i = 0; i < limit; i++)
            {
               if (fastSet.Contains(i))
               {
                  foundCount++;
               }
               else
               {
                  notFoundCount++;
               }
            }

            return $"FastSet<int> = {elapsedFunc()}, found = {foundCount}, not found = {notFoundCount}";
         });
         Console.WriteLine(message);

         message = time(elapsedFunc =>
         {
            for (var i = 0; i < limit; i++)
            {
               set.Remove(i);
            }

            return $"Set<int> = {elapsedFunc()} removed all";
         });
         Console.WriteLine(message);

         message = time(elapsedFunc =>
         {
            for (var i = 0; i < limit; i++)
            {
               fastSet.Remove(i);
            }

            return $"FastSet<int> = {elapsedFunc()} removed all";
         });
         Console.WriteLine(message);
      }
   }
}