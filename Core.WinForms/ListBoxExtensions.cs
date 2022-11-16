using System.Collections.Generic;
using System.Windows.Forms;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms;

public static class ListBoxExtensions
{
   public static IEnumerable<object> AllItems(this ListBox listBox)
   {
      foreach (var item in listBox.Items)
      {
         yield return item;
      }
   }

   public static IEnumerable<T> AllItems<T>(this ListBox listBox)
   {
      foreach (var obj in listBox.AllItems())
      {
         yield return (T)obj;
      }
   }

   public static Maybe<(string text, int index)> SelectedText(this ListBox listBox)
   {
      var index = listBox.SelectedIndex;
      if (index > -1)
      {
         return (listBox.Items[index].ToNonNullString(), index);
      }
      else
      {
         return nil;
      }
   }
}