﻿using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Documents;

public class Submenus : IHash<string, ToolStripMenuItem>, IEnumerable<ToolStripMenuItem>
{
   protected ToolStripMenuItem parent;
   protected string parentText;

   internal Submenus(ToolStripMenuItem parent)
   {
      this.parent = parent;
      parentText = parent.Text;
   }

   public bool ContainsKey(string key) => parent.DropDownItems.ContainsKey(Menus.SubmenuName(parentText, key));

   public Result<Hash<string, ToolStripMenuItem>> AnyHash() => fail("Not implemented");

   public HashInterfaceMaybe<string, ToolStripMenuItem> Items => new(this);

   ToolStripMenuItem IHash<string, ToolStripMenuItem>.this[string text]
   {
      get
      {
         var submenuName = Menus.SubmenuName(parentText, text);
         return (ToolStripMenuItem)parent.DropDownItems[submenuName];
      }
   }

   public IEnumerator<ToolStripMenuItem> GetEnumerator()
   {
      foreach (var dropDownItem in parent.DropDownItems)
      {
         if (dropDownItem is ToolStripMenuItem item)
         {
            yield return item;
         }
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}