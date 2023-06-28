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
         cut(x, y);
         cascadingCut(y);
      }

      if (_minNode is (true, var minNode) && x.Key.CompareTo(minNode.Key) < 0)
      {
         _minNode = x;
      }

      return unit;
   }

   public Maybe<T> Delete(Node<T, TKey> x)
   {
      return
         from minKey in _minKey
         from decreased in DecreaseKey(x, minKey).Maybe()
         from removed in RemoveMin()
         select removed.Data;
   }

   public void Insert(Node<T, TKey> node)
   {
      if (_minNode is (true, var minNode))
      {
         node.Left = minNode;
         node.Right = minNode.Right;
         minNode.Right = node;
         node.Right.MapOf(r => r.Left = node);

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

   public void Enqueue(TKey key, T value) => Insert(new Node<T, TKey>(value, key));

   public Maybe<T> Dequeue() => RemoveMin().Map(n => n.Data);

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
            _oldMinChildLeft.MapOf(l => l.Right = _oldMinChildRight);
            _oldMinChildRight.MapOf(r => r.Left = _oldMinChildLeft);

            if (_oldMinChild is (true, var oldMinChild))
            {
               oldMinChild.Left = _minNode;
               oldMinChild.Right = _minNode.Map(n => n.Right);
               minNode.Right = oldMinChild;
               _oldMinChildRight.MapOf(r => r.Left = oldMinChild);

               oldMinChild.Parent = nil;
               _oldMinChild = _tempRight;
               numKids--;
            }

            minNode.Left.MapOf(l => l.Right = minNode.Right);
            minNode.Right.MapOf(r => r.Left = minNode.Left);

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

         h._minNode = h1._minNode;

         if (h._minNode is (true, var hMinNode))
         {
            if (h2._minNode is (true, var h2MinNode))
            {
               hMinNode.Right.MapOf(r => r.Left = h2MinNode.Left);
               h2MinNode.Left.MapOf(l => l.Right = hMinNode.Right);
               hMinNode.Right = h2MinNode;
               h2MinNode.Left = hMinNode;

               if (h1._minNode is (true, var h1MinNode) && h2MinNode.Key.CompareTo(h1MinNode.Key) < 0)
               {
                  h._minNode = h2._minNode;
               }
            }
         }
         else
         {
            h._minNode = h2._minNode;
         }

         h.nNodes = h1.nNodes + h2.nNodes;

         return h;
      }
      else
      {
         return nil;
      }
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
            cut(y, z);
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

      while (_x is (true, var x0) && _minNode is (true, var minNode) && x0 != minNode)
      {
         numRoots++;
         _x = x0.Right;
      }

      while (numRoots > 0)
      {
         if (_x is (true, var x))
         {
            var degree = x.Degree;
            var _next = x.Right;

            while (array[degree] is (true, var y))
            {
               if (x.Key.CompareTo(y.Key) > 0)
               {
                  (y, x) = (x, y);
               }

               link(y, x);

               array[degree] = nil;
               degree++;
            }

            array[degree] = x;
            _x = _next;
            numRoots++;
         }
      }

      for (var i = 0; i < arraySize; i++)
      {
         if (array[i] is (true, var y))
         {
            if (_minNode is (true, var minNode))
            {
               y.Left.MapOf(l => l.Right = y.Right);
               y.Right.MapOf(r => r.Left = y.Left);

               y.Left = minNode;
               y.Right = minNode.Right;
               minNode.Right = y;
               y.Right.MapOf(r => r.Left = y);

               if (y.Key.CompareTo(minNode.Key) < 0)
               {
                  _minNode = y;
               }
            }
            else
            {
               _minNode = y;
            }
         }
      }
   }

   protected void cut(Node<T, TKey> x, Node<T, TKey> y)
   {
      x.Left.MapOf(l => l.Right = x.Right);
      x.Right.MapOf(r => r.Left = x.Left);
      y.Degree--;

      if (y.Child is (true, var child) && child == y)
      {
         y.Child = x.Right;
      }

      if (y.Degree == 0)
      {
         y.Child = nil;
      }

      x.Left = _minNode;
      y.Right = _minNode.Map(n => n.Right);
      _minNode.MapOf(n => n.Right = x);
      x.Right.MapOf(n => n.Left = x);

      x.Parent = nil;
      x.Mark = false;
   }

   protected void link(Node<T, TKey> newChild, Node<T, TKey> newParent)
   {
      newChild.Left.MapOf(l => l.Right = newChild.Right);
      newChild.Right.MapOf(r => r.Left = newChild.Left);

      newChild.Parent = newParent;

      if (newParent.Child is (true, var newParentChild))
      {
         newChild.Left = newParent.Child;
         newChild.Right = newParent.Child.Map(c => c.Right);
         newParentChild.Right = newChild;
         newChild.Right.MapOf(r => r.Left = newChild);
      }
      else
      {
         newParent.Child = newChild;
         newChild.Right = newChild;
         newChild.Left = newChild;
      }

      newParent.Degree++;
      newChild.Mark = false;
   }
}