using System;
using Core.Monads;

namespace Core.Dates.Relative.DateOperations;

public class AbsoluteDay : DateOperation
{
   public AbsoluteDay(int amount) : base(amount) { }

   public override OperationType Type => OperationType.Day;

   public override Result<DateTime> Operate(DateTime dateTime)
   {
      return amount.IsDay().AndYear(dateTime.Year).AndMonthValid(dateTime.Month);
   }
}