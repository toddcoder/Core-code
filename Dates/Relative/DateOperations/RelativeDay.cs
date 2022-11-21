using System;
using Core.Monads;

namespace Core.Dates.Relative.DateOperations;

public class RelativeDay : DateOperation
{
   public RelativeDay(int amount) : base(amount) { }

   public override OperationType Type => OperationType.Year;

   public override Result<DateTime> Operate(DateTime dateTime) => dateTime.AddDays(amount);
}