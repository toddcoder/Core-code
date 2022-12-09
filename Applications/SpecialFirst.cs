using System;

namespace Core.Applications;

public class SpecialFirst
{
   protected Action action;
   protected bool firstFired;

   public SpecialFirst(Action action)
   {
      this.action = action;
      firstFired = false;
   }

   public void OnFirst()
   {
      if (!firstFired)
      {
         action();
         firstFired = false;
      }
   }

   public void Reset() => firstFired = false;

   public void Do() => action();
}