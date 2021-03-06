﻿using System;
using System.Collections.Generic;
using Core.Assertions;
using Core.Collections.Infix;
using Core.DataStructures;
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
   }
}