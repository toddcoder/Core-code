using System.Collections;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections
{
   public class OneUseLinkedList<T> : IEnumerable<T>
   {
      class Item
      {
         public Item(T value) => Value = value;

         public T Value { get; }

         public IMaybe<Item> Next { get; set; } = none<Item>();
      }

      IMaybe<Item> head;
      IMaybe<Item> tail;

      public OneUseLinkedList()
      {
         head = none<Item>();
         tail = none<Item>();
      }

      public OneUseLinkedList(IEnumerable<T> values) : this() => AddRange(values);

      public void Add(T value)
      {
         var item = new Item(value).Some();
         if (head.IsSome && tail.If(out var t))
         {
            t.Next = item;
         }
         else
         {
            head = item;
         }

         tail = item;
      }

      public void AddRange(IEnumerable<T> values)
      {
         foreach (var value in values)
         {
            Add(value);
         }
      }

      public IMaybe<T> Shift() => head.Map(item =>
      {
         var value = item.Value;
         if (head.If(out var h))
         {
            head = h.Next;
         }
         else
         {
            tail = none<Item>();
         }

         return value;
      });

      public IEnumerator<T> GetEnumerator()
      {
         var current = head;
         while (current.If(out var c))
         {
            yield return c.Value;

            current = c.Next;
         }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public IMaybe<T> Head => head.Map(item => item.Value);

      public IMaybe<T> Tail => tail.Map(item => item.Value);

      public bool More => head.IsSome;
   }
}