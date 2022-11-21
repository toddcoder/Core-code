using System;

namespace Core.Dates.Now;

public class TestNow : NowBase
{
   protected DateTime now;

   public TestNow(DateIncrementer now) => this.now = now;

   public override DateTime Now => now;

   public override DateTime Today => Now.Truncate();
}