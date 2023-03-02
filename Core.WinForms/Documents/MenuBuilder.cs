using System;
using System.Windows.Forms;
using Core.Monads;
using Core.Numbers;
using static Core.Objects.ConversionFunctions;

namespace Core.WinForms.Documents;

public class MenuBuilder
{
   protected Menus menus;
   protected MenuText menuText;
   protected EventHandler handler;
   protected string shortcut;
   protected bool isChecked;
   protected int index;
   protected bool enabled;
   protected Bits32<Keys> keys;

   public MenuBuilder(Menus menus)
   {
      this.menus = menus;

      menuText = MenuText.Empty;
      handler = (_, _) => { };
      shortcut = "";
      isChecked = false;
      index = -1;
      enabled = true;
      keys = Keys.None;
   }

   public MenuBuilder Text(string text)
   {
      menuText = text;
      return this;
   }

   public MenuBuilder Text(Func<string> textFunc)
   {
      menuText = textFunc;
      return this;
   }

   public MenuBuilder Text(Func<Result<string>> textFunc)
   {
      menuText = textFunc;
      return this;
   }

   public MenuBuilder Handler(EventHandler handler)
   {
      this.handler = handler;
      return this;
   }

   public MenuBuilder Handler(Action action)
   {
      handler = (_, _) => action();
      return this;
   }

   public MenuBuilder Shortcut(string shortcut)
   {
      this.shortcut = shortcut;
      return this;
   }

   public MenuBuilder Control()
   {
      keys[Keys.Control] = true;
      return this;
   }

   public MenuBuilder Control(string key) => Control().Key(key);

   public MenuBuilder Alt()
   {
      keys[Keys.Alt] = true;
      return this;
   }

   public MenuBuilder Alt(string key) => Alt().Key(key);

   public MenuBuilder Shift()
   {
      keys[Keys.Shift] = true;
      return this;
   }

   public MenuBuilder Shift(string key) => Shift().Key(key);

   public MenuBuilder Key(string key)
   {
      if (Maybe.Enumeration<Keys>(key) is (true, var keyValue))
      {
         keys[keyValue] = true;
      }

      return this;
   }

   public MenuBuilder IsChecked(bool isChecked)
   {
      this.isChecked = isChecked;
      return this;
   }

   public MenuBuilder Enabled(bool enabled)
   {
      this.enabled = enabled;
      return this;
   }

   public ToolStripMenuItem Menu() => menuText.ToObject() switch
   {
      string text => menus.Menu(text, handler, shortcut, isChecked, index, enabled, keys),
      Func<string> func => menus.Menu(func, handler, shortcut, isChecked, index, enabled, keys),
      Func<Result<string>> func => menus.Menu(func, handler, shortcut, isChecked, index, enabled, keys),
      _ => throw MonadFunctions.fail("Unexpected item")
   };

   public ToolStripMenuItem SubMenu() => menuText.ToObject() switch
   {
      string text => menus.SubMenu(text, index),
      _ => throw MonadFunctions.fail("Unexpected item")
   };
}