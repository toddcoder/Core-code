using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications
{
   public class ConvertArgs : EventArgs
   {
      public ConvertArgs(string source)
      {
         Source = source;
      }

      public string Source { get; }

      public IMaybe<object> Result { get; set; } = none<object>();
   }
}