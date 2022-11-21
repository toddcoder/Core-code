using System;
using System.Diagnostics;

namespace Core.Dates;

public class ElapsedTime
{
   public static implicit operator TimeSpan(ElapsedTime elapsedTime) => elapsedTime.Elapsed;

   public static implicit operator string(ElapsedTime elapsedTime) => elapsedTime.ToString();

   protected bool includeMilliseconds;
   protected Stopwatch stopwatch;

   public ElapsedTime(bool includeMilliseconds = true)
   {
      this.includeMilliseconds = includeMilliseconds;

      stopwatch = new Stopwatch();
      stopwatch.Start();
   }

   public TimeSpan Elapsed
   {
      get
      {
         stopwatch.Stop();
         return stopwatch.Elapsed;
      }
   }

   public override string ToString() => Elapsed.ToLongString(includeMilliseconds);
}