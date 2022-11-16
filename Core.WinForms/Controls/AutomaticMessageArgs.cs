using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class AutomaticMessageArgs : EventArgs
{
   protected Maybe<string> _text;

   public AutomaticMessageArgs()
   {
      _text = nil;
   }

   public string Text
   {
      set => _text = value;
   }

   internal Maybe<string> GetText() => _text;
}