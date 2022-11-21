using System;

namespace Core.Dates.Now;

public class StandardNow : NowBase
{
   public override DateTime Now => DateTime.Now;

   public override DateTime Today => DateTime.Today;
}