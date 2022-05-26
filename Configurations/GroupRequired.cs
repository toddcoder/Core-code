using System;
using Core.Computers;
using Core.Strings;

namespace Core.Configurations
{
   public class GroupRequired
   {
      protected Group group;

      internal GroupRequired(Group group)
      {
         this.group = group;
      }

      public Group Group(string key) => group.GroupAt(key);

      public string String(string key) => group.ValueAt(key);

      public string this[string key]
      {
         get => String(key);
         set => group[key] = value;
      }

      public int Int32(string key) => int.Parse(this[key]);

      public long Int64(string key) => long.Parse(this[key]);

      public float Single(string key) => float.Parse(this[key]);

      public double Double(string key) => double.Parse(this[key]);

      public bool Boolean(string key) => bool.Parse(this[key]);

      public DateTime DateTime(string key) => System.DateTime.Parse(this[key]);

      public Guid Guid(string key) => System.Guid.Parse(this[key]);

      public FileName FileName(string key) => group.ValueAt(key);

      public FolderName FolderName(string key) => group.ValueAt(key);

      public byte[] Bytes(string key) => this[key].FromBase64();
   }
}