using System;
using System.Collections.Generic;
using System.Linq;
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
      public static IEnumerable<Field> FieldsFromString(string input)
      {
         foreach (var _field in input.Unjoin("/s* ',' /s*; f").Select(FromString))
         {
            if (_field.Map(out var field))
            {
               yield return field;
            }
         }
      }

      public static Maybe<Field> FromString(string input)
      {
         if (input.Matches("^ /(/w+) /('?')? /s* ('[' /(/w+) ']')? (/s* ':' /s* /('$'? [/w '.']+))? $; f").Map(out var result))
         {
            var name = result.FirstGroup;
            var optional = result.SecondGroup == "?";
            var signature = result.ThirdGroup;
            if (signature.IsEmpty())
            {
               signature = name.ToTitleCase();
            }

            var typeName = result.FourthGroup;
            var _type = maybe(typeName.IsNotEmpty(), () => getType(typeName));

            return new Field(name, signature, optional) { Type = _type };
         }
         else
         {
            return nil;
         }
      }
      public static Field Parse(Group fieldGroup)
      {
         var name = fieldGroup.Key;
         var signature = fieldGroup.GetValue("signature").DefaultTo(() => name);
         var optional = fieldGroup.GetValue("optional").Map(s => s == "true").DefaultTo(() => false);
         var typeName = fieldGroup.GetValue("type");
         var type = typeName.Map(getType);

         return new Field(name, signature, optional) { Type = type };
      }

      protected static Maybe<Type> getType(string typeName)
      {
         if (typeName.IsEmpty())
         {
            return nil;
         }
         else
         {
            var fullName = typeName.Substitute("^ '$'; f", "System.");
            return System.Type.GetType(fullName, false, true);
         }
      }

      public Field(string name, string signature, bool optional) : base(name, signature) => Optional = optional;

      public Field(string name, Type type, bool optional = false) : base(name, name)
      {
         Optional = optional;
         Type = type;
      }

      public Field(string name, string signature, Type type, bool optional = false) : base(name, signature)
      {
         Optional = optional;
         Type = type;
      }

      public Field() : base("", "")
      {
         Type = nil;
      }

      public int Ordinal { get; set; }

      public bool Optional { get; set; }

      public Maybe<Type> Type { get; set; }

      public override Type PropertyType => Type.DefaultTo(() => base.PropertyType);
   }
}