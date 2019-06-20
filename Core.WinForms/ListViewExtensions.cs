using System;
using System.Windows.Forms;

namespace Core.WinForms
{
   public static class ListViewExtensions
   {
      public static void AutoSizeColumns(this ListView listView)
      {
         listView.BeginUpdate();

         listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
         var headerWidths = new int[listView.Columns.Count];

         for (var i = 0; i < listView.Columns.Count; i++)
         {
            headerWidths[i] = listView.Columns[i].Width;
         }

         listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

         for (var i = 0; i < headerWidths.Length; i++)
         {
            listView.Columns[i].Width = Math.Max(listView.Columns[i].Width, headerWidths[i]);
         }

         listView.EndUpdate();
      }
   }
}