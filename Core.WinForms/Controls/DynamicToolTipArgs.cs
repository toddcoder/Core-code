using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class DynamicToolTipArgs : EventArgs
{
   public DynamicToolTipArgs(Maybe<int> rectangleIndex)
   {
      ToolTipText = nil;
      RectangleIndex = rectangleIndex;
   }

   public Maybe<string> ToolTipText { get; set; }

   public Maybe<int> RectangleIndex { get; }
}