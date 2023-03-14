using System;

namespace Core.Threading;

public class JobEmptyQueueArgs : EventArgs
{
   public JobEmptyQueueArgs(int affinity)
   {
      Affinity = affinity;
   }

   public int Affinity { get; }

   public bool Quit { get; set; }
}