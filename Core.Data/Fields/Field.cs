using System;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using Group = Core.Configurations.Group;

namespace Core.Data.Fields
{
   public class Field : PropertyInterface
   {
      public static Field Parse(Group fieldGroup)
      {
         var name = fieldGroup.Key;
         var signature = fieldGroup.GetValue("signature").DefaultTo(() => name);
         var optional = fieldGroup.GetValue("optional").Map(s => s == "true").DefaultTo(() => false);
         var typeName = fieldGroup.GetValue("type");
         var type = typeName.Map(getType);

         return new Field(name, signature, optional) { Type = type };
      }

      protected static IMaybe<Type> getType(string typeName)
      {
         if (typeName.IsEmpty())
         {
            return none<Type>();
         }
         else
         {
            var fullName = typeName.Substitute("^ '$'; f", "System.");
            return System.Type.GetType(fullName, false, true).Some();
         }
      }

      public Field(string name, string signature, bool optional) : base(name, signature) => Optional = optional;

      public Field(string name, Type type, bool optional = false) : base(name, name)
      {
         Optional = optional;
         Type = type.Some();
      }

      public Field(string name, string signature, Type type, bool optional = false) : base(name, signature)
      {
         Optional = optional;
         Type = type.Some();
      }

      public Field() : base("", "") => Type = none<Type>();

      public int Ordinal { get; set; }

      public bool Optional { get; set; }

      public IMaybe<Type> Type { get; set; }

      public override Type PropertyType => Type.DefaultTo(() => base.PropertyType);
   }
}