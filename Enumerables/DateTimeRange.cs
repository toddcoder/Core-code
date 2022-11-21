using System;
using Core.Strings;

namespace Core.Enumerables;

public class DateTimeRange : XRange<DateTime, uint>
{
   public const string DEFAULT_FORMAT_STRING = "dd-MM-yyyy";

   public DateTimeRange(DateTime from, DateTime to, uint distance, bool includeFrom, bool includeTo) : base(from, to, distance,
      includeFrom, includeTo)
   {
      Format = "";
   }

   public string Format { get; set; }

   protected override DateTime nextValue(DateTime currentValue) => currentValue.AddDays((int)distance);

   protected override DateTime previousValue(DateTime currentValue) => currentValue.AddDays(-(int)distance);

   public override int Compare(DateTime x, DateTime y) => x.CompareTo(y);

   protected override string valueAsString(DateTime value)
   {
      var format = Format.IsNotEmpty() ? Format : DEFAULT_FORMAT_STRING;
      return value.ToString(format);
   }
}