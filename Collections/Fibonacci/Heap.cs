using System;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections.Fibonacci;

public class Heap<T, TKey> where T : IEquatable<T> where TKey : IComparable<TKey>
{
   protected static readonly double oneOverLogPhi = 1.0 / Math.Log((1.0 + Math.Sqrt(5.0)) / 2.0);
   protected Maybe<TKey> _minKey;
   protected Maybe<Node<T, TKey>> _minNode;
   protected int nNodes;

   public Heap(TKey minKey)
   {
      _minKey = minKey;
      _minNode = nil;
   }

   public bool IsEmpty => !_minKey;

   public void Clear()
   {
      _minNode = nil;
      nNodes = 0;
   }

   public Result<Unit> DecreaseKey(Node<T, TKey> x, TKey key)
   {
      if (key.CompareTo(x.Key) > 0)
      {
         return fail("Larger key value introduced");
      }

      x.Key = key;

      var _y = x.Parent;
      if (_y is (true, var y) && x.Key.CompareTo(y.Key) < 0)
      {
         //Cut(x, y);
         cascadingCut(y);
      }

      if (_minNode is (true, var minNode) && x.Key.CompareTo(minNode.Key) < 0)
      {
         _minNode = x;
      }

      return unit;
   }

   public Result<Unit> Delete(Node<T, TKey> x)
   {
      return
         from minKey in _minKey.Result("Minimum key not set")
         from decreased in DecreaseKey(x, minKey)
         from removed in RemoveMin().Result("Minimum key not found")
         select decreased;
   }

   public void Insert(Node<T, TKey> node)
   {
      if (_minNode is (true, var minNode))
      {
         node.Left = minNode;
         node.Right = minNode.Right;
         minNode.Right = node;
         if (node.Right is (true, var right))
         {
            right.Left = node;
         }

         if (node.Key.CompareTo(minNode.Key) < 0)
         {
            _minNode = node;
         }
      }
      else
      {
         _minNode = node;
      }

      nNodes++;
   }

   public Maybe<Node<T, TKey>> Min => _minNode;

   public Maybe<Node<T, TKey>> RemoveMin()
   {
      if (_minNode is (true, var minNode))
      {
         var numKids = minNode.Degree;
         var _oldMinChild = minNode.Child;
         var _oldMinChildLeft = _oldMinChild.Map(c => c.Left);
         var _oldMinChildRight = _oldMinChild.Map(c => c.Right);

         while (numKids > 0)
         {
            var _tempRight = _oldMinChildRight;
            if (_oldMinChildLeft is (true, var oldMinChildLeft))
            {
               oldMinChildLeft.Right = _oldMinChildRight;
            }

            if (_oldMinChildRight is (true, var oldMinChildRight))
            {
               oldMinChildRight.Left = _oldMinChildLeft;
            }

            if (_oldMinChild is (true, var oldMinChild))
            {
               oldMinChild.Left = _minNode;
               oldMinChild.Right = _minNode.Map(n => n.Right);
               minNode.Right = oldMinChild;
               if (_oldMinChildRight is (true, var oldMinChildRight2))
               {
                  oldMinChildRight2.Left = _oldMinChildLeft;
               }

               oldMinChild.Parent = nil;
               _oldMinChild = _tempRight;
               numKids--;
            }

            if (minNode.Left is (true, var minNodeLeft))
            {
               minNodeLeft.Right = minNode.Right;
            }

            if (minNode.Right is (true, var minNodeRight))
            {
               minNodeRight.Left = minNode.Left;
            }

            if (minNode.Right is (true, var rightComparison) && minNode == rightComparison)
            {
               _minNode = nil;
            }
            else
            {
               _minNode = minNode.Right;
               consolidate();
            }

            nNodes--;
         }
      }

      return _minNode;
   }

   public static Maybe<Heap<T, TKey>> Union(Maybe<Heap<T, TKey>> _h1, Maybe<Heap<T, TKey>> _h2)
   {
      if (_h1 is (true, var h1) && h1._minKey is (true, var key1) && _h2 is (true, var h2) && h2._minKey is (true, var key2))
      {
         var h = new Heap<T, TKey>(key1.CompareTo(key2) < 0 ? key1 : key2)
         {
            _minNode = h1._minNode
         };

         if (h._minNode is (true, var minNode))
         {
            if (h2._minNode is (true, var minNode2))
            {
               if (minNode.Right is (true, var right))
               {
                  right.Left = minNode2.Left;
               }

               if (minNode2.Left is (true, var left2))
               {
                  left2.Right = minNode.Right;
               }

               minNode.Right = minNode2;
               minNode2.Left = minNode;

               if (h1._minNode is (true, var minNode1) && minNode.Key.CompareTo(minNode1.Key) < 0)
               {
                  h._minNode = h2._minNode;
               }
            }
            else
            {
               h._minNode = h2._minNode;
            }

            h.nNodes = h1.nNodes + h2.nNodes;
         }
      }
      else
      {
         return nil;
      }

      return h2;
   }

   protected void cascadingCut(Node<T, TKey> y)
   {
      var _z = y.Parent;

      if (_z is (true, var z))
      {
         if (!y.Mark)
         {
            y.Mark = true;
         }
         else
         {
            //Cut(y, z);
            cascadingCut(z);
         }
      }
   }

   protected void consolidate()
   {
      var arraySize = (int)Math.Floor(Math.Log(nNodes) * oneOverLogPhi);
      var array = new List<Maybe<Node<T, TKey>>>(arraySize);
      for (var i = 0; i < arraySize; i++)
      {
         array.Add(nil);
      }

      var numRoots = 0;
      var _x = _minNode;

      if (_x is (true, var xRight))
      {
         numRoots++;
         _x = xRight.Right;

         while (_x is (true, var x0) && _minNode is (true, var minNode) && x0 == minNode)
         {
            numRoots++;
            _x = x0.Right;
         }
      }

      while (_x is (true, var x) && numRoots > 0)
      {
         var degree = x.Degree;
         var _next = x.Right;

         while (true)
         {
            var _y = array[degree];
            if (_y is (true, var y))
            {
               if (x.Key.CompareTo(y.Key) > 0)
               {
                  (y, x) = (x, y);
               }

               //link(y, x);

               array[degree] = nil;
               degree++;
            }
            else
            {
               break;
            }
         }

         array[degree] = x;
         _x = _next;
         numRoots++;
      }

      _minNode = nil;

      for (var i = 0; i < arraySize; i++)
      {
         var _y = array[i];
         if (_y is (true, var y))
         {
            if (_minNode is (true, var minNode))
            {
            }
         }
         else
         {
            continue;
         }
      }
   }
}