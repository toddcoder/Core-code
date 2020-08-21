using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;

namespace Core.Exceptions
{
   public class GroupedExceptions<T> : ApplicationException, IEnumerable<GroupedExceptions<T>.GroupedExceptionItem>
   {
      public class GroupedExceptionItem : ApplicationException
      {
         public GroupedExceptionItem(Set<T> set, string key, string stackTrace)
         {
            Set = set;
            Key = key;
            StackTrace = stackTrace;
         }

         public Set<T> Set { get; }

         public string Key { get; }

         public new string StackTrace { get; }

         public override string ToString() => $"{Key}: {Set.ToString(", ")} [{StackTrace}]";
      }

      Hash<string, Set<T>> data;
      Hash<string, string> stackTraces;

      public GroupedExceptions()
      {
         data = new Hash<string, Set<T>>();
         stackTraces = new Hash<string, string>();
      }

      public void Add<TState>(TState state, Exception exception, Func<TState, Exception, T> extract)
      {
         var result = extract(state, exception);
         var key = exception.Message;
         if (data.ContainsKey(key))
         {
            data[key].Add(result);
         }
         else
         {
            data[key] = new Set<T> { result };
         }

         stackTraces[key] = exception.StackTrace ?? "";
      }

      public IEnumerator<GroupedExceptionItem> GetEnumerator() => data.Select(getItem).GetEnumerator();

      GroupedExceptionItem getItem(KeyValuePair<string, Set<T>> item)
      {
         return new GroupedExceptionItem(item.Value, item.Key, stackTraces[item.Key]);
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}