using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Core.Applications
{
   public class CancelTasks<TTask, TState> : IList<TTask>, IDisposable
      where TTask : CancelTask<TState>
   {
      protected List<TTask> tasks;

      public CancelTasks() => tasks = new List<TTask>();

      public IEnumerator<TTask> GetEnumerator()
      {
         foreach (var task in tasks)
         {
            yield return task;
         }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public void Add(TTask item) => tasks.Add(item);

      public void Clear() => tasks.Clear();

      public bool Contains(TTask item) => tasks.Contains(item);

      public void CopyTo(TTask[] array, int arrayIndex) => tasks.CopyTo(array, arrayIndex);

      public bool Remove(TTask item) => tasks.Remove(item);

      public int Count => tasks.Count;

      public bool IsReadOnly => false;

      public int IndexOf(TTask item) => tasks.IndexOf(item);

      public void Insert(int index, TTask item) => tasks.Insert(index, item);

      public void RemoveAt(int index) => tasks.RemoveAt(index);

      public TTask this[int index]
      {
         get => tasks[index];
         set => tasks[index] = value;
      }

      public void Start()
      {
         foreach (var task in tasks)
         {
            task.Start();
         }
      }

      public void WaitAll(TimeSpan timeout)
      {
         WaitHandle.WaitAll(tasks.Select(t => (WaitHandle)t.Reset).ToArray(), timeout);
      }

      protected void dispose()
      {
         foreach (var task in tasks)
         {
            task.Dispose();
         }
      }

      public void Dispose()
      {
         dispose();
         GC.SuppressFinalize(this);
      }

      ~CancelTasks() => dispose();
   }
}