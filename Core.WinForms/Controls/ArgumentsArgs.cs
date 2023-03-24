using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ArgumentsArgs : EventArgs
{
   public ArgumentsArgs()
   {
      Arguments = nil;
   }

   public Optional<object> Arguments { get; set; }
}