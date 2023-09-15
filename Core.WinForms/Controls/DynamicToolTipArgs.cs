using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class DynamicToolTipArgs : EventArgs
{
   public DynamicToolTipArgs()
   {
      ToolTipText = nil;
   }

   public Maybe<string> ToolTipText { get; set; }
}