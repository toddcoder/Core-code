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

            if (_op.Map(out var op))
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
         result.Must().Equal(result).OrThrow();
      }

      [TestMethod]
      public void PriorityQueueTest()
      {
         var queue = new PriorityQueue<int>();

         foreach (var item in array(1, 5, 3, 6, 9))
         {
            queue.Enqueue(item);
         }

         while (queue.Dequeue().Map(out var item))
         {
            Console.WriteLine(item);
         }
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
#pragma warning disable 618
         var fastSet = new FastSet<int>(Enumerable.Range(0, limit));
#pragma warning restore 618

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

      [TestMethod]
      public void MaybeStackItemTest()
      {
         var stack = new MaybeStack<string>();
         stack.Push("alpha");
         stack.Push("bravo");
         stack.Push("charlie");

         if (stack.Item(0).Map(out var item, out var exception))
         {
            Console.WriteLine(item);
         }
         else
         {
            Console.WriteLine(exception.Message);
         }

         if (stack.Item(-1).Map(out item, out exception))
         {
            Console.WriteLine(item);
         }
         else
         {
            Console.WriteLine(exception.Message);
         }
      }
   }
}