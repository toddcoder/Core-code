using System.Collections;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections
{
   public class OneUseLinkedList<T> : IEnumerable<T>
   {
      protected class Item
      {
         public Item(T value) => Value = value;

         public T Value { get; }

         public IMaybe<Item> Next { get; set; } = none<Item>();
      }

      protected IMaybe<Item> _head;
      protected IMaybe<Item> _tail;

      public OneUseLinkedList()
      {
         _head = none<Item>();
         _tail = none<Item>();
      }

      public OneUseLinkedList(IEnumerable<T> values) : this() => AddRange(values);

      public void Add(T value)
      {
         var item = new Item(value).Some();
         if (_head.IsSome && _tail.If(out var tail))
         {
            tail.Next = item;
         }
         else
         {
            _head = item;
         }

         _tail = item;
      }

      public void AddRange(IEnumerable<T> values)
      {
         foreach (var value in values)
         {
            Add(value);
         }
      }

      public IMaybe<T> Shift() => _head.Map(item =>
      {
         var value = item.Value;
         if (_head.If(out var head))
         {
            _head = head.Next;
         }
         else
         {
            _tail = none<Item>();
         }

         return value;
      });

      public IEnumerator<T> GetEnumerator()
      {
         var _current = _head;
         while (_current.If(out var current))
         {
            yield return current.Value;

            _current = current.Next;
         }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public IMaybe<T> Head => _head.Map(item => item.Value);

      public IMaybe<T> Tail => _tail.Map(item => item.Value);

      public bool More => _head.IsSome;
   }
}