using System;
using Core.Configurations;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Fields;

public class Field : PropertyInterface
{
   public static Maybe<Field> FromString(string input)
   {
      var _result = input.Matches("^ /(/w+) /('?')? /s* ('[' /(/w+) ']')? (/s* ':' /s* /('$'? [/w '.']+))? $; f");
      if (_result)
      {
         var result = _result.Value;
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

   public static Field Parse(Setting fieldSetting)
   {
      var name = fieldSetting.Key;
      var signature = fieldSetting.Maybe.String("signature") | name;
      var optional = fieldSetting.Maybe.String("optional").Map(s => s == "true") | false;
      var typeName = fieldSetting.Maybe.String("type");
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

   public override Type PropertyType => Type | (() => base.PropertyType);
}