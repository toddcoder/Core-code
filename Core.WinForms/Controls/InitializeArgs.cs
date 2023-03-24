using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class InitializeArgs : EventArgs
{
   public InitializeArgs()
   {
      Argument = nil;
   }

   public bool Cancel { get; set; }

   public Optional<object> Argument;
}