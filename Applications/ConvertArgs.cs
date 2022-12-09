using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications;

public class ConvertArgs : EventArgs
{
   public ConvertArgs(string source)
   {
      Source = source;
      Result = nil;
   }

   public string Source { get; }

   public Maybe<object> Result { get; set; }
}