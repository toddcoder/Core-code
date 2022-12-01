using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class LinkedList<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>
{
   public class Node
   {
      protected Maybe<LinkedList<T>> _list;
      protected Maybe<Node> _next;
      protected Maybe<Node> _previous;
      protected T item;

      public Node(T item)
      {
         this.item = item;

         _list = nil;
         _next = nil;
         _previous = nil;
      }

      public Node(Maybe<LinkedList<T>> list, T item)
      {
         _list = list;
         this.item = item;

         _next = nil;
         _previous = nil;
      }

      public Maybe<LinkedList<T>> List
      {
         get => _list;
         set => _list = value;
      }

      public Maybe<Node> GetNext()
      {
         var _nextItem = lazy.maybe(() => _next);
         var _listItem = _nextItem.Then(_ => _list);
         var _head = _listItem.Then(l => l._head);
         if (_head)
         {
            Node next = _nextItem;
            Node head = _head;

            return next != head ? next : nil;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<Node> Next
      {
         get => _next;
         set => _next = value;
      }

      public Maybe<Node> GetPrevious()
      {
         var _previousItem = lazy.maybe(() => _previous);
         var _listItem = _previousItem.Then(_ => _list);
         var _head = _listItem.Then(l => l._head);
         if (_head)
         {
            Node previous = _previousItem;
            Node head = _head;

            return this != head ? previous : nil;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<Node> Previous
      {
         get => _previous;
         set => _previous = value;
      }

      public T Value
      {
         get => item;
         set => item = value;
      }

      public void Invalidate()
      {
         _list = nil;
         _next = nil;
         _previous = nil;
      }
   }

   public class Enumerator : IEnumerator<T>
   {
      protected LinkedList<T> list;
      protected Maybe<Node> _node;
      protected Maybe<T> _current;

      public Enumerator(LinkedList<T> list)
      {
         this.list = list;

         _node = list._head;
         _current = nil;
      }

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
         if (!_node)
         {
            return false;
         }

         _current = _node.Map(n => n.Value);
         _node = _node.Map(n => n.Next);
         var _nodeItem = lazy.maybe(() => _node);
         var _head = _nodeItem.Then(_ => list._head);
         if (_head)
         {
            Node node = _nodeItem;
            Node head = _head;
            if (node == head)
            {
               _node = nil;
            }
         }

         return true;
      }

      public void Reset()
      {
         _current = nil;
         _node = list._head;
      }

      public T Current => ~_current;

      object IEnumerator.Current => Current;
   }

   protected Maybe<Node> _head;
   protected int count;
   protected int version;
   protected Maybe<object> _syncRoot;

   public LinkedList()
   {
      _head = nil;
      count = 0;
      version = 0;
   }

   public LinkedList(IEnumerable<T> collection) : this()
   {
      collection.Must().Not.BeNull().OrThrow();

      foreach (var item in collection)
      {
         AddLast(item);
      }
   }

   public Maybe<Node> First => _head;

   public Maybe<Node> Last => _head.Map(h => h.Previous);

   public IEnumerator<T> GetEnumerator() => new Enumerator(this);

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Add(T item) => AddLast(item);

   public Node AddAfter(Node node, T value)
   {
      validateNode(node);
      var newNode = new Node(node.List, value);
      insertNodeBefore(node.Next, newNode);

      return newNode;
   }

   public void AddAfter(Node node, Node newNode)
   {
      validateNode(node);
      validateNewNode(newNode);
      insertNodeBefore(node.Next, newNode);
      newNode.List = this;
   }

   public Node AddBefore(Node node, T value)
   {
      validateNode(node);
      var newNode = new Node(node.List, value);
      insertNodeBefore(node, newNode);
      if (_head && node == ~_head)
      {
         _head = newNode;
      }

      return newNode;
   }

   public void AddBefore(Node node, Node newNode)
   {
      validateNode(node);
      validateNewNode(newNode);
      insertNodeBefore(node, newNode);
      newNode.List = this;

      if (!_head || node == ~_head)
      {
         _head = newNode;
      }
   }

   public Node AddFirst(T value)
   {
      var newNode = new Node(this, value);
      if (!_head)
      {
         insertNodeIntoEmptyList(newNode);
      }
      else
      {
         insertNodeBefore(_head, newNode);
         _head = newNode;
      }

      return newNode;
   }

   public void AddFirst(Node node)
   {
      validateNewNode(node);
      if (!_head)
      {
         insertNodeIntoEmptyList(node);
      }
      else
      {
         insertNodeBefore(_head, node);
         _head = node;
      }

      node.List = this;
   }

   public Node AddLast(T value)
   {
      var newNode = new Node(this, value);
      if (!_head)
      {
         insertNodeIntoEmptyList(newNode);
      }
      else
      {
         insertNodeBefore(_head, newNode);
      }

      return newNode;
   }

   public void AddList(Node node)
   {
      validateNewNode(node);
      if (!_head)
      {
         insertNodeIntoEmptyList(node);
      }
      else
      {
         insertNodeBefore(_head, node);
      }

      node.List = this;
   }

   public void Clear()
   {
      var _current = _head;
      while (_current)
      {
         var _temp = _current;
         _current = (~_current).Next;
         (~_temp).Invalidate();
      }

      _head = nil;
      count = 0;
      version++;
   }

   public bool Contains(T item) => Find(item);

   public void CopyTo(T[] array, int arrayIndex)
   {
      array.Must().Not.BeNull().OrThrow($"{nameof(array)} is null");
      arrayIndex.Must().BeBetween(0).Until(array.Length).OrThrow();
      (array.Length - arrayIndex).Must().BeLessThan(count).OrThrow();

      var _node = _head;
      if (!_node)
      {
         return;
      }

      var head = ~_head;
      do
      {
         var node = ~_node;
         array[arrayIndex++] = node.Value;
         _node = node.Next;
      } while (_node && ~_node != head);
   }

   public Maybe<Node> Find(T value)
   {
      value.Must().Not.BeNull().OrThrow();

      if (!_head)
      {
         return nil;
      }

      var head = ~_head;
      var _node = _head;
      var equalityComparer = EqualityComparer<T>.Default;
      if (_node)
      {
         var node = ~_node;
         while (!equalityComparer.Equals(node.Value, value))
         {
            _node = node.Next;
            node = ~_node;
            if (node == head)
            {
               return nil;
            }
         }

         return _node;
      }
      else
      {
         return nil;
      }
   }

   public Maybe<Node> FindLast(T value)
   {
      value.Must().Not.BeNull().OrThrow();

      if (!_head)
      {
         return nil;
      }

      var head = ~_head;
      var _previous = head.Previous;
      var _last = _previous;
      var equalityComparer = EqualityComparer<T>.Default;

      if (_last)
      {
         var last = ~_last;
         var previous = ~_previous;
         while (!equalityComparer.Equals(last.Value, value))
         {
            _last = last.Previous;
            if (_last && ~_last == previous)
            {
               return nil;
            }
         }

         return _last;
      }
      else
      {
         return nil;
      }
   }

   public bool Remove(T item)
   {
      var _node = Find(item);
      if (!_node)
      {
         return false;
      }
      else
      {
         removeNode(_node);
         return true;
      }
   }

   public void RemoveFirst()
   {
      _head.Must().HaveValue().OrThrow();
      removeNode(_head);
   }

   public void RemoveLast()
   {
      _head.Must().HaveValue().OrThrow();
      removeNode((~_head).Previous);
   }

   protected void insertNodeBefore(Node node, Node newNode)
   {
      newNode.Next = node;
      newNode.Previous = node.Previous;
      (~node.Previous).Next = newNode;
      node.Previous = newNode;

      version++;
      count++;
   }

   protected void insertNodeIntoEmptyList(Node newNode)
   {
      newNode.Next = newNode;
      newNode.Previous = newNode;
      _head = newNode;

      version++;
      count++;
   }

   protected void removeNode(Node node)
   {
      if (node.Next && ~node.Next == node)
      {
         _head = nil;
      }
      else
      {
         (~node.Next).Previous = node.Previous;
         (~node.Previous).Next = node.Next;
         if (~_head == node)
         {
            _head = node.Next;
         }
      }

      node.Invalidate();
      count--;
      version++;
   }

   protected void validateNewNode(Node node)
   {
      node.Must().Not.BeNull().OrThrow();
      node.List.Must().HaveValue().OrThrow();
   }

   protected void validateNode(Node node)
   {
      node.Must().Not.BeNull().OrThrow();
      node.List.Must().HaveValue().OrThrow();
      (~node.List).Must().Equal(this).OrThrow();
   }

   public void CopyTo(Array array, int index)
   {
      if (array is T[] itemArray)
      {
         CopyTo(itemArray, index);
      }
   }

   int ICollection.Count => count;

   public object SyncRoot
   {
      get
      {
         if (_syncRoot)
         {
            var obj = ~_syncRoot;
            Interlocked.CompareExchange<object>(ref obj, new object(), null);
            _syncRoot = obj;
         }

         return _syncRoot;
      }
   }

   public bool IsSynchronized => false;

   int ICollection<T>.Count => count;

   public bool IsReadOnly => false;

   int IReadOnlyCollection<T>.Count => count;
}