using System;
using Core.Monads;

namespace Core.Dates.Relative.DateOperations;

public class AbsoluteYear : DateOperation
{
   public AbsoluteYear(int amount) : base(amount) { }

   public override OperationType Type => OperationType.Year;

   public override Optional<DateTime> Operate(DateTime dateTime)
   {
      return amount.IsYear().AndMonth(dateTime.Month).AndDayValid(dateTime.Day);
   }
}