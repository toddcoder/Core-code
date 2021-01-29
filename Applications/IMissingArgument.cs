﻿using System;
using Core.Monads;

namespace Core.Applications
{
   public interface IMissingArgument
   {
      IResult<object> BadType(string name, Type type, string rest);

      bool Handled(string name, string reset, bool isFirstMatch);
   }
}