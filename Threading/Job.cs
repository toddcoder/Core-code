using System;
using System.Runtime.InteropServices;
using System.Threading;
using Core.Exceptions;

namespace Core.Threading
{
   public class Job
   {
      [DllImport("kernel32.dll", SetLastError = true)]
      protected static extern IntPtr SetThreadAffinityMask(SafeThreadHandle handle, HandleRef mask);

      [DllImport("kernel32")]
      protected static extern int GetCurrentThreadId();

      [DllImport("kernel32", SetLastError = true)]
      protected static extern SafeThreadHandle OpenThread(int access, bool inherit, int threadId);

      public static int getCoreMask(int affinity) => 0 ^ 1 << affinity;

      public static void setProcessorAffinity(int coreMask)
      {
         var threadId = GetCurrentThreadId();
         SafeThreadHandle handle = null;
         var tempHandle = new object();

         try
         {
            handle = OpenThread(0x60, false, threadId);
            if (SetThreadAffinityMask(handle, new HandleRef(tempHandle, (IntPtr)coreMask)) == IntPtr.Zero)
            {
               throw "Failed to set processor affinity for thread".Throws();
            }
         }
         finally
         {
            handle?.Close();
         }
      }

      protected int affinity;
      protected ManualResetEvent manualResetEvent;
      protected object locker;

      public event EventHandler<JobExceptionArgs> JobException;
      public event EventHandler<JobEmptyQueueArgs> EmptyQueue;

      public Job(int affinity, ManualResetEvent manualResetEvent, object locker)
      {
         this.affinity = affinity;
         this.manualResetEvent = manualResetEvent;
         this.locker = locker;
      }

      public void Dispatch(JobQueue queue)
      {
         manualResetEvent.Reset();

         var thread = new Thread(() =>
         {
            var coreMask = getCoreMask(affinity);
            setProcessorAffinity(coreMask);

            while (true)
            {
               while (queue.Dequeue(affinity).If(out var action))
               {
                  try
                  {
                     action(affinity);

                  }
                  catch (Exception exception)
                  {
                     JobException?.Invoke(this, new JobExceptionArgs(affinity, exception));
                  }

                  Thread.Sleep(500);
               }

               lock (locker)
               {
                  var args = new JobEmptyQueueArgs(affinity);
                  EmptyQueue?.Invoke(this, args);
                  if (args.Quit)
                  {
                     break;
                  }
               }
            }

            manualResetEvent.Set();
         });

         thread.Start();
      }

      public void Execute(JobQueue queue)
      {
         while (queue.Dequeue(affinity).If(out var action))
         {
            try
            {
               action(affinity);
            }
            catch (Exception exception)
            {
               JobException?.Invoke(this, new JobExceptionArgs(affinity, exception));
            }
         }
      }
   }
}