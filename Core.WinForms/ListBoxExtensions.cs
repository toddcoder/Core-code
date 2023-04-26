using System;
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

   public static Maybe<int> FindItem(this ListBox listBox, Func<object, bool> predicate)
   {
      for (var i = 0; i < listBox.Items.Count; i++)
      {
         if (predicate(listBox.Items[i]))
         {
            return i;
         }
      }

      return nil;
   }

   public static Maybe<int> FindString(this ListBox listBox, Func<string, bool> predicate)
   {
      for (var i = 0; i < listBox.Items.Count; i++)
      {
         if (predicate(listBox.Items[i].ToString()))
         {
            return i;
         }
      }

      return nil;
   }

   public static Optional<int> SelectItem(this ListBox listBox, object item)
   {
      try
      {
         var _index =
            from notNullItem in item.NotNull()
            from foundIndex in listBox.FindItem(notNullItem.Equals)
            select foundIndex;
         if (_index is (true, var index))
         {
            listBox.SelectedIndex = index;
            return index;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Optional<int> SelectString(this ListBox listBox, string item, bool ignoreCase)
   {
      try
      {
         Func<string, bool> predicate;
         if (ignoreCase)
         {
            predicate = i => i.Same(item);
         }
         else
         {
            predicate = i => i.Equals(item);
         }

         var _index = listBox.FindString(predicate);
         if (_index is (true, var index))
         {
            listBox.SelectedIndex = index;
            return index;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}