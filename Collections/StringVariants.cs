using System;
using System.Collections.Generic;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Collections
{
   public class StringVariants : IHash<string, string>
   {
      protected StringHash templates;
      protected Maybe<string> _templateName;
      protected (string key, string value)[] values;

      public StringVariants()
      {
         templates = new StringHash(true);
         _templateName = nil;
         values = Array.Empty<(string, string)>();
      }

      public StringVariants(Dictionary<string, string> dictionary) : this()
      {
         foreach (var item in dictionary)
         {
            this[item.Key] = item.Value;
         }
      }

      public string this[string key]
      {
         get => templates[key];
         set => templates[key] = value;
      }

      public bool ContainsKey(string key) => templates.ContainsKey(key);

      public Result<Hash<string, string>> AnyHash() => templates;

      public Maybe<string> Implement(string templateName, params (string key, string value)[] values)
      {
         this.values = values;
         return Implement(templateName);
      }

      public Maybe<string> Implement(string templateName)
      {
         if (this.Map(templateName, out var template))
         {
            var formatter = new Formatter();
            foreach (var (key, value) in values)
            {
               formatter[key] = value;
            }

            _templateName = templateName;
            return formatter.Format(template);
         }
         else
         {
            return nil;
         }
      }

      public Maybe<string> Implement(params (string key, string value)[] values)
      {
         return _templateName.Map(template => Implement(template, values));
      }

      public Maybe<string> Implement() => _templateName.Map(template => Implement(template, values));

      public override string ToString() => Implement() | "";
   }
}