using System.Collections.Generic;
using System.Windows.Forms;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms;

public static class ComboBoxExtensions
{
   public static IEnumerable<object> AllItems(this ComboBox comboBox)
   {
      foreach (var item in comboBox.Items)
      {
         yield return item;
      }
   }

   public static IEnumerable<T> AllItems<T>(this ComboBox comboBox)
   {
      foreach (var obj in comboBox.AllItems())
      {
         yield return (T)obj;
      }
   }

   public static Optional<string> SelectedText(this ComboBox comboBox)
   {
      var index = comboBox.SelectedIndex;
      return maybe<string>() & index > -1 & (() => comboBox.Items[index].ToNonNullString());
   }

   public static Optional<int> SelectedIndex(this ComboBox comboBox)
   {
      var index = comboBox.SelectedIndex;
      return maybe<int>() & index > -1 & index;
   }

   public static Optional<(string text, int index)> SelectedTextWithIndex(this ComboBox comboBox)
   {
      var index = comboBox.SelectedIndex;
      return maybe<(string text, int index)>() & index > -1 & (() => (comboBox.Items[index].ToNonNullString(), index));
   }
}