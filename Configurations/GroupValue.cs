using System;
using System.Linq;
using Core.Computers;
using Core.Strings;
using static Core.Objects.ConversionFunctions;

namespace Core.Configurations
{
   public class GroupValue
   {
      protected Group group;

      internal GroupValue(Group group)
      {
         this.group = group;
      }

      public Group Group(string key) => group.GetGroup(key).DefaultTo(() => new Group(key));

      public string String(string key, string defaultValue = "") => group.GetValue(key).DefaultTo(() => defaultValue);

      public int Int32(string key, int defaultValue = 0)
      {
         return group.GetValue(key).Map(i => Value.Int32(i, defaultValue)).DefaultTo(() => defaultValue);
      }

      public long Int64(string key, long defaultValue = 0)
      {
         return group.GetValue(key).Map(l => Value.Int64(l, defaultValue)).DefaultTo(() => defaultValue);
      }

      public float Single(string key, float defaultValue = 0)
      {
         return group.GetValue(key).Map(f => Value.Single(f, defaultValue)).DefaultTo(() => defaultValue);
      }

      public double Double(string key, double defaultValue = 0)
      {
         return group.GetValue(key).Map(d => Value.Double(d, defaultValue)).DefaultTo(() => defaultValue);
      }

      public bool Boolean(string key, bool defaultValue = false)
      {
         return group.GetValue(key).Map(b => Value.Boolean(b, defaultValue)).DefaultTo(() => defaultValue);
      }

      public DateTime DateTime(string key, DateTime defaultValue)
      {
         return group.GetValue(key).Map(d => Value.DateTime(d, defaultValue)).DefaultTo(() => defaultValue);
      }

      public DateTime DateTime(string key) => DateTime(key, System.DateTime.MinValue);

      public Guid Guid(string key) => group.GetValue(key).Map(Value.Guid).DefaultTo(() => System.Guid.Empty);

      public FileName FileName(string key) => group.ValueAt(key);

      public FolderName FolderName(string key) => group.ValueAt(key);

      public byte[] Bytes(string key) => String(key).FromBase64();

      public string[] Strings(string key) => Group(key).Values().Select(t => t.value).ToArray();
   }
}