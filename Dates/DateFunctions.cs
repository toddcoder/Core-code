using System;
using Core.Monads;
using static System.DateTime;
using static Core.Monads.AttemptFunctions;

namespace Core.Dates;

public static class DateFunctions
{
   public static DateTime date(string source) => Parse(source);

   public static Result<DateTime> attemptDate(string source) => tryTo(() => date(source));
}