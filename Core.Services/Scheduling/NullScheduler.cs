namespace Core.Services.Scheduling;

public class NullScheduler : Scheduler
{
   public NullScheduler() : base("", false, true)
   {
   }

   public override bool Triggered => false;

   public override void Next()
   {
   }
}