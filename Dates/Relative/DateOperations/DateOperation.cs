using System;
using Core.Monads;

namespace Core.Dates.Relative.DateOperations;

public abstract class DateOperation : IComparable<DateOperation>
{
   protected int amount;

   public DateOperation(int amount) => this.amount = amount;

   public abstract OperationType Type { get; }

   public int CompareTo(DateOperation other)
   {
      switch (Type)
      {
         case OperationType.Year:
            return other.Type == OperationType.Year ? 0 : -1;
         case OperationType.Month:
            switch (other.Type)
            {
               case OperationType.Year:
                  return 1;
               case OperationType.Month:
                  return 0;
               case OperationType.Day:
                  return -1;
            }

            break;
         case OperationType.Day:
            return other.Type == OperationType.Day ? 0 : 1;
         default:
            return -2;
      }

      return -2;
   }

   public abstract Result<DateTime> Operate(DateTime dateTime);
}