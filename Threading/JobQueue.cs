using System;
using System.Linq;
using Core.Assertions;
using Core.DataStructures;
using Core.Monads;

namespace Core.Threading;

public class JobQueue
{
   protected int affinityCount;
   protected MaybeQueue<Action<int>>[] queues;
   protected object locker;
   protected int currentAffinity;

   public JobQueue(int affinityCount)
   {
      this.affinityCount = affinityCount;

      queues = Enumerable.Range(0, this.affinityCount).Select(_ => new MaybeQueue<Action<int>>()).ToArray();
      locker = new object();
      currentAffinity = 0;
   }

   public void ResetCurrentAffinity() => currentAffinity = 0;

   public void Enqueue(Action<int> action)
   {
      lock (locker)
      {
         queues[currentAffinity].Enqueue(action);
         currentAffinity = (currentAffinity + 1) % affinityCount;
      }
   }

   public Maybe<Action<int>> Dequeue(int affinity)
   {
      affinity.Must().BeBetween(0).Until(affinityCount).OrThrow();

      lock (locker)
      {
         return queues[affinity].Dequeue();
      }
   }

   public int Count(int affinity)
   {
      affinity.Must().BeBetween(0).Until(affinityCount).OrThrow();

      return queues[affinity].Count;
   }

   public int AllCount => Enumerable.Range(0, affinityCount).Select(affinity => queues[affinity].Count).Sum();
}