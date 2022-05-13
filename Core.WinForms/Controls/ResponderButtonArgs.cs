using System;

namespace Core.WinForms.Controls
{
   public class ResponderButtonArgs : EventArgs
   {
      public ResponderButtonArgs(string key)
      {
         Key = key;
      }

      public string Key { get; }
   }
}