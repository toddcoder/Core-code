using System;
using Core.Dates.Relative.DateOperations;
using Core.Monads;

namespace Core.Dates.Relative;

public class LastDayOfMonth : DateOperation
{
   public LastDayOfMonth() : base(0) { }

   public override OperationType Type => OperationType.Day;

   public override Result<DateTime> Operate(DateTime dateTime) => dateTime.LastOfMonth();
}