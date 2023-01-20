using System;
using System.Linq;
using System.Windows.Forms;
using Core.Applications;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.WinForms.Documents;

public class Menus : IHash<string, ToolStripMenuItem>
{
   protected StringHash<ToolStripItem> menuItems;
   protected Hash<ToolStripItem, Func<string>> dynamicTextItems;
   protected StringHash<int> tabIndexes;
   protected int tabIndex;
   protected Result<string> _currentMenu;

   public Menus()
   {
      menuItems = new StringHash<ToolStripItem>(true);
      dynamicTextItems = new Hash<ToolStripItem, Func<string>>();
      tabIndexes = new StringHash<int>(true);
      tabIndex = 0;
      _currentMenu = fail("Parent menu not set");
   }

   public void Menu(string text, string shortcut = "")
   {
      var item = new ToolStripMenuItem(text) { Name = MenuName(text) };
      if (shortcut.IsNotEmpty())
      {
         var _keys = shortcutKeys(shortcut);
         if (_keys)
         {
            item.ShortcutKeys = _keys;
         }
      }

      menuItems[item.Name] = item;
      tabIndexes[item.Name] = tabIndex++;
      _currentMenu = text;
   }

   public void ContextMenu(string text, EventHandler handler, string shortcut = "", bool isChecked = false)
   {
      var item = new ToolStripMenuItem(text) { Name = MenuName(text), Checked = isChecked };
      item.Click += handler;
      if (shortcut.IsNotEmpty())
      {
         var _keys = shortcutKeys(shortcut);
         if (_keys)
         {
            item.ShortcutKeys = _keys;
         }
      }

      menuItems[item.Name] = item;
      tabIndexes[item.Name] = tabIndex++;
   }

   public static string MenuName(string text) => $"menu{makeIdentifier(text)}";

   protected static string makeIdentifier(string text) => text.Replace("&", "").Substitute("/s+; f", "_").SnakeToCamelCase(false);

   public static string SubmenuName(string parentText, string text) => MenuName(parentText) + makeIdentifier(text);

   protected ToolStripMenuItem getParent(string text)
   {
      var name = MenuName(text);
      var parent = menuItems.Value(name);

      return (ToolStripMenuItem)parent;
   }

   protected static void setShortcut(ToolStripMenuItem item, string shortcut)
   {
      if (shortcut.IsNotEmpty())
      {
         var _keys = shortcutKeys(shortcut);
         if (_keys)
         {
            item.ShortcutKeys = _keys;
         }
      }
   }

   public void Menu(string parentText, string text, EventHandler handler, string shortcut = "", bool isChecked = false, int index = -1)
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

   public void Menu(string text, EventHandler handler, string shortcut = "", bool isChecked = false, int index = -1)
   {
      Menu(_currentMenu, text, handler, shortcut, isChecked, index);
   }

   public void Menu(string parentText, Func<string> textFunc, EventHandler handler, string shortcut = "", bool isChecked = false, int index = -1)
   {
      var parent = getParent(parentText);
      var text = textFunc();
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

      dynamicTextItems[item] = textFunc;
   }

   public void Menu(Func<string> textFunc, EventHandler handler, string shortcut = "", bool isChecked = false, int index = -1)
   {
      Menu(_currentMenu, textFunc, handler, shortcut, isChecked, index);
   }

   public void AddHandler(string parentText, string text, EventHandler handler)
   {
      var _toolStripMenuItem =
         from submenus in Submenus(parentText)
         from toolStripMenuItem in submenus.Maybe()[text]
         select toolStripMenuItem;
      if (_toolStripMenuItem)
      {
         _toolStripMenuItem.Value.Click += handler;
      }
   }

   public Maybe<Delegate> ReplaceHandler(string parentText, string text, EventHandler handler)
   {
      var _submenus = LazyMonads.lazy.maybe(() => Submenus(parentText));
      var _item = _submenus.Then(submenus => submenus.Map(text));
      var _delegate = _item.Then(item => item.ClearEvent("Click"));
      if (_delegate)
      {
         ToolStripMenuItem item = _item;
         Delegate @delegate = _delegate;

         item.Click += handler;

         return @delegate;
      }
      else
      {
         return nil;
      }
   }

   public Maybe<Delegate> ReplaceHandler(string text, EventHandler handler) => ReplaceHandler(_currentMenu, text, handler);

   public void MenuSeparator(string parentText)
   {
      var parent = getParent(parentText);
      var item = new ToolStripSeparator();
      parent.DropDownItems.Add(item);
   }

   public void MenuSeparator() => MenuSeparator(_currentMenu);

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

      var item = parent.DropDownItems[name];
      if (item != null && dynamicTextItems.ContainsKey(item))
      {
         dynamicTextItems.Remove(item);
      }

      parent.DropDownItems.Remove(item);
   }

   public void RemoveMenu(string text) => RemoveMenu(_currentMenu, text);

   public void RemoveMenu(string parentText, int index)
   {
      var parent = getParent(parentText);

      var item = parent.DropDownItems[index];
      if (item != null && dynamicTextItems.ContainsKey(item))
      {
         dynamicTextItems.Remove(item);
      }

      parent.DropDownItems.RemoveAt(index);
   }

   public void RemoveMenu(int index) => RemoveMenu(_currentMenu, index);

   protected static Result<Keys> shortcutKeys(string text)
   {
      var _result = text.Matches("^ /(['^%|']+)? /(/w+) $; f");
      if (_result)
      {
         var result = _result.Value;
         var keys = (Keys)0;
         var prefix = result[0, 1];
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

         return Result.Enumeration<Keys>(result[0, 2]).Map(k => keys | k).ExceptionMessage($"Couldn't translate {result[0, 2]} into a key");
      }
      else
      {
         return fail($"Didn't understand key {text}");
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

   protected ToolStripItem[] getMenuItems() => menuItems.Values
      .Select(i => new { MenuItem = i, Index = tabIndexes[i.Name] })
      .OrderBy(a => a.Index)
      .Select(a => a.MenuItem).ToArray();

   public void CreateContextMenu(Control control)
   {
      var menuContext = new ContextMenuStrip { Name = $"menuContext{control.Name}" };
      var items = getMenuItems();
      menuContext.Items.AddRange(items);
      control.ContextMenuStrip = menuContext;
   }

   public void StandardContextEdit(Document document)
   {
      ContextMenu("Undo", (_, _) => document.Undo(), "^Z");
      ContextMenu("Redo", (_, _) => document.Redo());
      ContextMenuSeparator();
      ContextMenu("Cut", (_, _) => document.Cut(), "^X");
      ContextMenu("Copy", (_, _) => document.Copy(), "^C");
      ContextMenu("Paste", (_, _) => document.Paste(), "^V");
      ContextMenu("Delete", (_, _) => document.Delete());
      ContextMenuSeparator();
      ContextMenu("Select All", (_, _) => document.SelectAll(), "^A");
   }

   public ToolStripMenuItem this[string text] => (ToolStripMenuItem)menuItems[MenuName(text)];

   public bool ContainsKey(string key) => menuItems.ContainsKey(MenuName(key));

   public Result<Hash<string, ToolStripMenuItem>> AnyHash() => menuItems.ToHash(i => i.Key, i => (ToolStripMenuItem)i.Value);

   public Maybe<Submenus> Submenus(string parentText) => this.Map(parentText, p => new Submenus(p));

   public void RefreshText()
   {
      foreach (var (item, func) in dynamicTextItems)
      {
         item.Text = func();
      }
   }
}