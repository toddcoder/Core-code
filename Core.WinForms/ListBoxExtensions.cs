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

   public static Maybe<string> SelectedText(this ListBox listBox)
   {
      var index = listBox.SelectedIndex;
      return maybe<string>() & index > -1 & (() => listBox.Items[index].ToNonNullString());
   }

   public static Maybe<int> SelectedIndex(this ListBox listBox)
   {
      var index = listBox.SelectedIndex;
      return maybe<int>() & index > -1 & index;
   }

   public static Maybe<(string text, int index)> SelectedTextWithIndex(this ListBox listBox)
   {
      var index = listBox.SelectedIndex;
      return maybe<(string text, int index)>() & index > -1 & (() => (listBox.Items[index].ToNonNullString(), index));
   }
}