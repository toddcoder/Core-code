using System;
using Core.Monads;

namespace Core.Dates.Relative.DateOperations;

public class RelativeMonth : DateOperation
{
   public RelativeMonth(int amount) : base(amount) { }

   public override OperationType Type => OperationType.Month;

   public override Result<DateTime> Operate(DateTime dateTime) => dateTime.AddMonths(amount);
}