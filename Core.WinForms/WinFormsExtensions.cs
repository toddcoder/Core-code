using System;
using System.Windows.Forms;

namespace Core.WinForms
{
   public static class WinFormsExtensions
   {
      public static void Do(this Control control, Action action)
      {
         if (control.InvokeRequired)
         {
            control.Invoke(action);
         }
         else
         {
            action();
         }
      }
   }
}