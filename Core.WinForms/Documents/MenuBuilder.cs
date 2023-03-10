using System;
using System.Windows.Forms;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.WinForms.Documents;

public class MenuBuilder
{
   public class MenuEnd
   {
   }

   public class BuilderSubMenu
   {
   }

   public class BuilderIsChecked
   {
   }

   public static MenuBuilder operator +(MenuBuilder builder, string text) => builder.Text(text);

   public static MenuBuilder operator +(MenuBuilder builder, Func<string> textFunc) => builder.Text(textFunc);

   public static MenuBuilder operator +(MenuBuilder builder, Func<Result<string>> textFunc) => builder.Text(textFunc);

   public static MenuBuilder operator +(MenuBuilder builder, EventHandler handler) => builder.Handler(handler);

   public static MenuBuilder operator +(MenuBuilder builder, Action handler) => builder.Handler(handler);

   public static MenuBuilder operator +(MenuBuilder builder, Keys keys) => builder.Keys(keys);

   public static MenuBuilder operator +(MenuBuilder builder, bool enabled) => builder.Enabled(enabled);

   public static MenuBuilder operator +(MenuBuilder builder, BuilderIsChecked _) => builder.IsChecked(true);

   public static ToolStripMenuItem operator +(MenuBuilder builder, MenuEnd _) => builder.Menu();

   public static ToolStripMenuItem operator +(MenuBuilder builder, BuilderSubMenu _) => builder.SubMenu();

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
      keys = System.Windows.Forms.Keys.None;
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
      keys[System.Windows.Forms.Keys.Control] = true;
      return this;
   }

   public MenuBuilder Control(string key) => Control().Key(key);

   public MenuBuilder Alt()
   {
      keys[System.Windows.Forms.Keys.Alt] = true;
      return this;
   }

   public MenuBuilder Alt(string key) => Alt().Key(key);

   public MenuBuilder Shift()
   {
      keys[System.Windows.Forms.Keys.Shift] = true;
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

   public MenuBuilder Keys(Keys keys)
   {
      this.keys[keys] = true;
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
      _ => throw fail("Unexpected item")
   };

   public ToolStripMenuItem SubMenu() => menuText.ToObject() switch
   {
      string text => menus.SubMenu(text, index),
      _ => throw fail("Unexpected item")
   };
}