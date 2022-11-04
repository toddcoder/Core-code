using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Core.DataStructures;
using Core.Enumerables;

namespace Core.Threading;

public class JobPool
{
   protected bool multiThreaded;
   protected int refillThreshold;
   protected int processorCount;
   protected Job[] jobs;
   protected ManualResetEvent[] manualResetEvents;
   protected object locker;
   protected JobQueue queue;

   public event EventHandler<JobExceptionArgs> JobException;
   public event EventHandler<CompletedArgs> Completed;

   public JobPool(bool multiThreaded = true, int refillThreshold = 5)
   {
      this.multiThreaded = multiThreaded;
      this.refillThreshold = refillThreshold;

      processorCount = Environment.ProcessorCount;
      manualResetEvents = Enumerable.Range(0, processorCount).Select(_ => new ManualResetEvent(false)).ToArray();
      locker = new object();
      jobs = Enumerable.Range(0, processorCount).Select(i => new Job(i, manualResetEvents[i], locker)).ToArray();
      queue = new JobQueue(processorCount);
   }

   public int ProcessorCount => processorCount;

   public void Enqueue(Action<int> action) => queue.Enqueue(action);

   public void Dispatch()
   {
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      if (queue.AllCount > 0)
      {
         if (multiThreaded)
         {
            foreach (var job in jobs)
            {
               job.JobException += (sender, e) => JobException?.Invoke(sender, e);
               job.EmptyQueue += balanceQueues;

               job.Dispatch(queue);
            }

            WaitHandle.WaitAll(manualResetEvents);
         }
         else
         {
            foreach (var job in jobs)
            {
               job.Execute(queue);
            }
         }
      }

      stopwatch.Stop();
      Completed?.Invoke(this, new CompletedArgs(stopwatch.Elapsed));
   }

   protected void balanceQueues(object sender, JobEmptyQueueArgs e)
   {
      if (totalCount() < refillThreshold)
      {
         e.Quit = true;
         return;
      }

      var newQueue = new MaybeQueue<Action<int>>();

      for (var i = 0; i < processorCount; i++)
      {
         while (queue.Count(i) > 0)
         {
            var _action = queue.Dequeue(i);
            newQueue.Enqueue(_action);
         }
      }

      queue.ResetCurrentAffinity();
      while (newQueue.IsNotEmpty)
      {
         var _item = newQueue.Dequeue();
         queue.Enqueue(_item);
      }

      e.Quit = false;
   }

   protected int totalCount()
   {
      var count = 0;
      for (var i = 0; i < processorCount; i++)
      {
         count += queue.Count(i);
      }

      return count;
   }

   public string JobsStatuses
   {
      get
      {
         var list = new List<string>();
         var totalCount = 0;
         for (var i = 0; i < processorCount; i++)
         {
            var count = queue.Count(i);
            totalCount += count;
            list.Add($"[{i + 1} | {count,4}]");
         }

         list.Add($"[All | {totalCount,4}]");

         return list.ToString(" ");
      }
   }
}