using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Configurations
{
   public class Item : IConfigurationItem
   {
      public Item(string key, string value)
      {
         Key = key;
         Value = value;
      }

      public string Key { get; }

      public IConfigurationItem this[string key] => this;

      public Maybe<string> GetValue(string key) => maybe(key.Same(Key), () => Value);

      public IResult<string> RequireValue(string key) => assert(key.Same(Key), () => Value, () => $"Key '{key}' doesn't match item key");

      public Maybe<Group> GetGroup(string key) => none<Group>();

      public IResult<Group> RequireGroup(string key) => "Not a group".Failure<Group>();

      public string Value { get; }

      public override string ToString()
      {
         var value = Value.ReplaceAll(("\t", @"\t"), ("\r", @"\r"), ("\n", @"\n"), ("\\", @"\\"));
         if (value.StartsWith(@"""") && value.EndsWith(@""""))
         {
            var innerValue = value.Drop(1).Drop(-1).Replace(@"""", @"\""");
            return $"{Key}: \"{innerValue}\"";
         }
         else
         {
            return $"{Key}: {value}";
         }
      }
   }
}