using System;
using Core.Monads;

namespace Core.Dates.Relative.DateOperations;

public class AbsoluteMonth : DateOperation
{
   public AbsoluteMonth(int amount) : base(amount) { }

   public override OperationType Type => OperationType.Month;

   public override Optional<DateTime> Operate(DateTime dateTime)
   {
      return amount.IsMonth().AndYear(dateTime.Year).AndDayValid(dateTime.Day);
   }
}