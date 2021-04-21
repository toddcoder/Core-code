using System;
using System.IO;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Configurations
{
   public class Configuration : IHash<string, IConfigurationItem>, IConfigurationItem
   {
      public static IResult<Configuration> Serialize<T>(T obj) where T : class, new()
      {
         try
         {
            return "Not implemented".Failure<Configuration>();
         }
         catch (Exception exception)
         {
            return failure<Configuration>(exception);
         }
      }

      protected Group root;

      internal Configuration(Group root)
      {
         this.root = root;
      }

      public string Key => root.Key;

      public IConfigurationItem this[string key] => root[key];

      public IMaybe<string> GetValue(string key) => root.GetValue(key);

      public IResult<string> RequireValue(string key) => root.RequireValue(key);

      public IMaybe<Group> GetGroup(string key) => root.GetGroup(key);

      public IResult<Group> RequireGroup(string key) => root.RequireGroup(key);

      public bool ContainsKey(string key) => root.ContainsKey(key);

      public IResult<Hash<string, IConfigurationItem>> AnyHash() => root.AnyHash();

      public IResult<T> Deserialize<T>() where T : class, new()
      {
         try
         {
            return "Not implemented".Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public override string ToString() => root.ToString(true);
   }
}