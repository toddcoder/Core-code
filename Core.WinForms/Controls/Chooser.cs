using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class Chooser
{
   protected StringHash choices;
   protected string[] keys;
   protected int index;
   protected Maybe<string> _key;

   public Chooser(StringHash choices)
   {
      this.choices = choices;

      keys = this.choices.KeyArray();
      index = -1;
      _key = nil;
   }

   public Maybe<string> Next()
   {
      if (!_key)
      {
         index = -1;
      }

      _key = ++index < keys.Length ? keys[index] : nil;

      return Value;
   }

   public Maybe<string> Key => _key;

   public Maybe<string> Value => _key.Map(key => choices.Map(key));

   public StringHash Choices => choices;
}