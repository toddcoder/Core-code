using System;
using Core.Monads;

namespace Core.Dates.Relative.DateOperations
{
   public class RelativeYear : DateOperation
   {
      public RelativeYear(int amount) : base(amount) { }

      public override OperationType Type => OperationType.Year;

      public override IResult<DateTime> Operate(DateTime dateTime) => dateTime.AddYears(amount).Success();
   }
}