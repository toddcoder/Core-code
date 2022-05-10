using System.Collections.Generic;
using System.Windows.Forms;

namespace Core.WinForms
{
   public static class ListBoxExtensions
   {
      public static IEnumerable<object> AllItems(this ListBox listBox)
      {
         foreach (var item in listBox.Items)
         {
            yield return item;
         }
      }
   }
}