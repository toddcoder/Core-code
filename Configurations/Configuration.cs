using System;
using System.Collections;
using System.Collections.Generic;
using Core.Collections;
using Core.Monads;
using Core.Objects;
using static Core.Monads.AttemptFunctions;
using static Core.Matching.MatchingExtensions;

namespace Core.Configurations
{
   public class Configuration : IHash<string, string>, IConfigurationItem, IEnumerable<IConfigurationItem>
   {
      public static implicit operator Configuration(string source) => FromString(source).ForceValue();

      public static Result<Configuration> FromString(string source)
      {
         var parser = new Parser(source);
         return parser.Parse().Map(group => new Configuration(group));
      }

      public static Result<Configuration> Serialize(Type type, object obj, string name)
      {
         return Group.Serialize(type, obj, name).Map(group => new Configuration(group));
      }

      public static Result<Configuration> Serialize<T>(T obj, string name) where T : class, new() => tryTo(() => Serialize(typeof(T), obj, name));

      protected Group root;

      public Configuration(Group root)
      {
         this.root = root;
      }

      public string Key => root.Key;

      public string this[string key]
      {
         get => root[key];
         set => root[key] = value;
      }

      public IConfigurationItem GetItem(string key) => root.GetItem(key);

      public Maybe<IConfigurationItem> GetSomeItem(string key) => root.GetSomeItem(key);

      public void SetItem(string key, IConfigurationItem item) => root.SetItem(key, item);

      public Maybe<string> GetValue(string key) => root.GetValue(key);

      public string ValueAt(string key) => GetValue(key).Required($"Couldn't find value '{key}'");

      public string[] GetArray(string key) => GetValue(key).Map(s => s.Split("/s* ',' /s*; f")).DefaultTo(() => new[] { key });

      public Result<string> RequireValue(string key) => root.RequireValue(key);

      public string At(string key) => GetValue(key).DefaultTo(() => "");

      public Maybe<Group> GetGroup(string key) => root.GetGroup(key);

      public Group GroupAt(string key) => GetGroup(key).Required($"Couldn't find group at '{key}'");

      public Result<Group> RequireGroup(string key) => root.RequireGroup(key);

      public bool ContainsKey(string key) => root.ContainsKey(key);

      public Result<Hash<string, string>> AnyHash() => root.AnyHash();

      public Result<object> Deserialize(Type type) => root.Deserialize(type);

      public Result<T> Deserialize<T>() where T : class, new()
      {
         return
            from obj in tryTo(() => Deserialize(typeof(T)))
            from cast in obj.CastAs<T>()
            select cast;
      }

      public IEnumerator<IConfigurationItem> GetEnumerator() => root.GetEnumerator();

      public override string ToString() => root.ToString(true);

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public StringHash ToStringHash() => root.ToStringHash();

      public IEnumerable<(string key, string value)> Values() => root.Values();

      public IEnumerable<(string key, Group group)> Groups() => root.Groups();

      public int Count => root.Count;
   }
}