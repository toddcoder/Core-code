using System;
using Core.Dates.Now;

namespace Core.Dates
{
   public class Trigger
   {
      public static implicit operator Trigger(string interval) => new Trigger(interval.ToTimeSpan());

      DateTime targetTime;
      TimeSpan interval;

      public Trigger(TimeSpan interval)
      {
         this.interval = interval;
         setTargetTime();
      }

      public bool Triggered
      {
         get
         {
            if (NowServer.Now > targetTime)
            {
               setTargetTime();
               return true;
            }
            else
               return false;
         }
      }

      void setTargetTime() => targetTime = NowServer.Now + interval;

      public void Reset() => setTargetTime();

      public override string ToString() => interval.ToString();
   }
}