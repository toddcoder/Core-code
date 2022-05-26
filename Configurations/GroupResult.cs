using System;
using System.Linq;
using Core.Computers;
using Core.Monads;
using Core.Strings;
using static Core.Objects.ConversionFunctions;

namespace Core.Configurations
{
   public class GroupResult
   {
      protected Group group;

      internal GroupResult(Group group)
      {
         this.group = group;
      }

      public Result<Group> Group(string key) => group.RequireGroup(key);

      public Result<string> String(string key) => group.RequireValue(key);

      public Result<string> this[string key]
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

      public Result<int> Int32(string key) => this[key].Map(Result.Int32);

      public Result<long> Int64(string key) => this[key].Map(Result.Int64);

      public Result<float> Single(string key) => this[key].Map(Result.Single);

      public Result<double> Double(string key) => this[key].Map(Result.Double);

      public Result<bool> Boolean(string key) => this[key].Map(Result.Boolean);

      public Result<DateTime> DateTime(string key) => this[key].Map(Result.DateTime);

      public Result<Guid> Guid(string key) => this[key].Map(Result.Guid);

      public Result<FileName> FileName(string key) => this[key].Map(s => (FileName)s);

      public Result<FolderName> FolderName(string key) => this[key].Map(s => (FolderName)s);

      public Result<byte[]> Bytes(string key) => this[key].Map(s => s.FromBase64());

      public Result<string[]> Strings(string key) => Group(key).Map(g => g.Values().Select(t => t.value).ToArray());
   }
}