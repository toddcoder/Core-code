using System;
using System.Linq;
using Core.Computers;
using Core.Monads;
using Core.Strings;
using static Core.Objects.ConversionFunctions;

namespace Core.Configurations
{
   public class GroupMaybe
   {
      protected Group group;

      internal GroupMaybe(Group group)
      {
         this.group = group;
      }

      public Maybe<Group> Group(string key) => group.GetGroup(key);

      public Maybe<string> String(string key) => group.GetValue(key);

      public Maybe<string> this[string key]
      {
         get => String(key);
         set
         {
            if (value.Map(out var @string))
            {
               group[key] = @string;
            }
         }
      }

      public Maybe<int> Int32(string key) => this[key].Map(Maybe.Int32);

      public Maybe<long> Int64(string key) => this[key].Map(Maybe.Int64);

      public Maybe<float> Single(string key) => this[key].Map(Maybe.Single);

      public Maybe<double> Double(string key) => this[key].Map(Maybe.Double);

      public Maybe<bool> Boolean(string key) => this[key].Map(Maybe.Boolean);

      public Maybe<DateTime> DateTime(string key) => this[key].Map(Maybe.DateTime);

      public Maybe<Guid> Guid(string key) => this[key].Map(Maybe.Guid);

      public Maybe<FileName> FileName(string key) => this[key].Map(s => (FileName)s);

      public Maybe<FolderName> FolderName(string key) => this[key].Map(s => (FolderName)s);

      public Maybe<byte[]> Bytes(string key) => this[key].Map(s => s.FromBase64());

      public Maybe<string[]> Strings(string key) => Group(key).Map(g => g.Values().Select(t => t.value).ToArray());
   }
}