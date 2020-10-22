using System;
using System.Linq;
using System.Windows.Forms;
using Core.Applications;
using Core.Collections;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;

namespace Core.WinForms.Documents
{
   public class Menus : IHash<string, ToolStripMenuItem>
   {
      protected Hash<string, ToolStripItem> menuItems;
      protected Hash<string, int> tabIndexes;
      protected int tabIndex;

      public Menus()
      {
         menuItems = new Hash<string, ToolStripItem>();
         tabIndexes = new Hash<string, int>();
         tabIndex = 0;
      }

      public void Menu(string text, string shortcut = "")
      {
         var item = new ToolStripMenuItem(text) { Name = MenuName(text) };
         if (shortcut.IsNotEmpty() && shortcutKeys(shortcut).If(out var keys))
         {
            item.ShortcutKeys = keys;
         }

         menuItems[item.Name] = item;
         tabIndexes[item.Name] = tabIndex++;
      }

      public void ContextMenu(string text, EventHandler handler, string shortcut = "", bool isChecked = false)
      {
         var item = new ToolStripMenuItem(text) { Name = MenuName(text), Checked = isChecked };
         item.Click += handler;
         if (shortcut.IsNotEmpty() && shortcutKeys(shortcut).If(out var keys))
         {
            item.ShortcutKeys = keys;
         }

         menuItems[item.Name] = item;
         tabIndexes[item.Name] = tabIndex++;
      }

      public static string MenuName(string text) => "menu" + makeIdentifier(text);

      protected static string makeIdentifier(string text) => text.Replace("&", "").Substitute("/s+", "_").SnakeToCamelCase(false);

      public static string SubmenuName(string parentText, string text) => MenuName(parentText) + makeIdentifier(text);

      protected ToolStripMenuItem getParent(string text)
      {
         var name = MenuName(text);
         var parent = menuItems.Value(name);

         return (ToolStripMenuItem)parent;
      }

      protected static void setShortcut(ToolStripMenuItem item, string shortcut)
      {
         if (shortcut.IsNotEmpty() && shortcutKeys(shortcut).If(out var keys))
         {
            item.ShortcutKeys = keys;
         }
      }

      public void Menu(string parentText, string text, EventHandler handler, string shortcut = "", bool isChecked = false,
         int index = -1)
      {
         var parent = getParent(parentText);
         var item = new ToolStripMenuItem(text) { Name = SubmenuName(parentText, text), Checked = isChecked };
         item.Click += handler;
         setShortcut(item, shortcut);
         if (index == -1)
         {
            parent.DropDownItems.Add(item);
         }
         else
         {
            parent.DropDownItems.Insert(index, item);
         }
      }

      public void AddHandler(string parentText, string text, EventHandler handler)
      {
         if (Submenus(parentText).If(out var submenus) && submenus.If(text, out var toolStripMenuItem))
         {
            toolStripMenuItem.Click += handler;
         }
      }

      public IMaybe<Delegate> ReplaceHandler(string parentText, string text, EventHandler handler) =>
         from submenus in Submenus(parentText)
         from item in submenus.Map(text)
         from d in item.ClearEvent("Click").IfThen(d => item.Click += handler)
         select d;

      public void MenuSeparator(string parentText)
      {
         var parent = getParent(parentText);
         var item = new ToolStripSeparator();
         parent.DropDownItems.Add(item);
      }

      public void ContextMenuSeparator()
      {
         ToolStripItem item = new ToolStripSeparator();
         menuItems[item.Name] = item;
         tabIndexes[item.Name] = tabIndex++;
      }

      public void RemoveMenu(string parentText, string text)
      {
         var parent = getParent(parentText);
         var name = SubmenuName(parentText, text);
         parent.DropDownItems.RemoveByKey(name);
      }

      public void RemoveMenu(string parentText, int index)
      {
         var parent = getParent(parentText);
         parent.DropDownItems.RemoveAt(index);
      }

      protected static IResult<Keys> shortcutKeys(string text)
      {
         var matcher = text.Matcher("^ /(['^%|']+)? /(/w+) $");
         if (matcher.If(out var m))
         {
            var keys = (Keys)0;
            var prefix = m[0, 1];
            if (prefix.IsNotEmpty())
            {
               foreach (var sign in prefix)
               {
                  switch (sign)
                  {
                     case '^':
                        keys |= Keys.Control;
                        break;
                     case '%':
                        keys |= Keys.Alt;
                        break;
                     case '|':
                        keys |= Keys.Shift;
                        break;
                  }
               }
            }

            return m[0, 2]
               .AsEnumeration<Keys>()
               .Map(k => (keys | k).Success())
               .DefaultTo(() => $"Couldn't translate {m[0, 2]} into a key".Failure<Keys>());
         }
         else
         {
            return $"Didn't understand key {text}".Failure<Keys>();
         }
      }

      public void CreateMainMenu(Form form)
      {
         var menuStrip = new MenuStrip { Name = "menuMain", Dock = DockStyle.Top };
         var items = getMenuItems();
         menuStrip.Items.AddRange(items);
         form.MainMenuStrip = menuStrip;
         form.Controls.Add(menuStrip);
      }

      protected ToolStripItem[] getMenuItems() =>
         menuItems.Values
            .Select(i => new { MenuItem = i, Index = tabIndexes[i.Name] })
            .OrderBy(a => a.Index)
            .Select(a => a.MenuItem).ToArray();

      public void CreateContextMenu(Control control)
      {
         var menuContext = new ContextMenuStrip { Name = "menuContext" + control.Name };
         var items = getMenuItems();
         menuContext.Items.AddRange(items);
         control.ContextMenuStrip = menuContext;
      }

      public void StandardContextEdit(Document document)
      {
         ContextMenu("Undo", (sender, e) => document.Undo(), "^Z");
         ContextMenu("Redo", (sender, e) => document.Redo());
         ContextMenuSeparator();
         ContextMenu("Cut", (sender, e) => document.Cut(), "^X");
         ContextMenu("Copy", (sender, e) => document.Copy(), "^C");
         ContextMenu("Paste", (sender, e) => document.Paste(), "^V");
         ContextMenu("Delete", (sender, e) => document.Delete());
         ContextMenuSeparator();
         ContextMenu("Select All", (sender, e) => document.SelectAll(), "^A");
      }

      public ToolStripMenuItem this[string text] => (ToolStripMenuItem)menuItems[MenuName(text)];

      public bool ContainsKey(string key) => menuItems.ContainsKey(MenuName(key));

      public IResult<Hash<string, ToolStripMenuItem>> AnyHash() => menuItems.ToHash(i => i.Key, i => (ToolStripMenuItem)i.Value).Success();

      public IMaybe<Submenus> Submenus(string parentText) => this.Map(parentText, p => new Submenus(p));
   }
}