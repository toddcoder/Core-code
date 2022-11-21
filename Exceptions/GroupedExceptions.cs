using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;

namespace Core.Exceptions;

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

   protected StringHash<Set<T>> data;
   protected StringHash stackTraces;

   public GroupedExceptions()
   {
      data = new StringHash<Set<T>>(true);
      stackTraces = new StringHash(true);
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

   protected GroupedExceptionItem getItem(KeyValuePair<string, Set<T>> item)
   {
      return new(item.Value, item.Key, stackTraces[item.Key]);
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}