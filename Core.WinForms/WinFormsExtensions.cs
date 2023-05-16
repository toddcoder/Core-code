using System;
using System.Windows.Forms;

namespace Core.WinForms;

public static class WinFormsExtensions
{
   public static void Do(this Control control, Action action)
   {
      if (control is null)
      {
         return;
      }

      if (!control.IsDisposed)
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

   public static T Get<T>(this Control control, Func<T> func)
   {
      if (control is null)
      {
         throw new NullReferenceException("Control is null");
      }

      if (!control.IsDisposed)
      {
         if (control.InvokeRequired)
         {
            return (T)control.Invoke(func);
         }
         else
         {
            return func();
         }
      }
      else
      {
         throw new ObjectDisposedException("Control already disposed");
      }
   }
}