using System.Collections;
using System.Collections.Generic;
using Core.Collections;
using Core.Monads;

namespace Core.Settings
{
   public class Setting : IEnumerable<Setting>
   {
      public const string ROOT_KEY = "__$root";

      public static Result<Setting> FromString(string source)
      {
         var parser = new Parser(source);
         return parser.Parse();
      }

      protected string key;
      protected Either<string, StringHash<Setting>> _value;

      internal Setting(string key, Either<string, StringHash<Setting>> value)
      {
         this.key = key;
         _value = value;
      }

      public string Key => key;

      public Either<string, StringHash<Setting>> Value => _value;

      public bool IsArray { get; set; }

      public IEnumerator<Setting> GetEnumerator()
      {
         if (_value.IfRight(out var subSettings))
         {
            foreach (var subSetting in subSettings.Values)
            {
               yield return subSetting;
            }
         }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public SettingMaybe Maybe => new(this);
   }
}