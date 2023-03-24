using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Documents;

public abstract class MenuText
{
   public static implicit operator MenuText(string text) => new MenuString(text);

   public static implicit operator MenuText(Func<string> func) => new MenuStringFunction(func);

   public static implicit operator MenuText(Func<Optional<string>> func) => new MenuResultStringFunction(func);

   public static MenuText Empty = new EmptyText();

   public abstract string Text { get; }

   public virtual bool Enabled => true;

   public abstract object ToObject();
}

public sealed class MenuString : MenuText
{
   private string value;

   internal MenuString(string value)
   {
      this.value = value;
   }

   public override string Text => value;

   public override object ToObject() => value;
}

public sealed class MenuStringFunction : MenuText
{
   internal Func<string> func;

   public MenuStringFunction(Func<string> func)
   {
      this.func = func;
   }

   public override string Text => func();

   public override object ToObject() => func;
}

public sealed class MenuResultStringFunction : MenuText
{
   internal Func<Optional<string>> func;

   public MenuResultStringFunction(Func<Optional<string>> func)
   {
      this.func = func;
   }

   public override string Text
   {
      get
      {
         var _result = func();
         if (_result is (true, var result))
         {
            return result;
         }
         else
         {
            return _result.Exception.Message;
         }
      }
   }

   public override bool Enabled => func();

   public override object ToObject() => func;
}

public sealed class EmptyText : MenuText
{
   public override string Text => throw fail("Text not provided");

   public override object ToObject() => fail("Object not provided");

   public override bool Enabled => false;
}