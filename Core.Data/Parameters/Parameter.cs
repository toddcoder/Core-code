using System;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;
using Group = Core.Configurations.Group;

namespace Core.Data.Parameters
{
   public class Parameter : PropertyInterface
   {
      public static Maybe<Parameter> FromString(string input)
      {
         if (input.Matches("^ '@'? /(/w+) /s* ('[' /(/w+) ']')? /s* ':' /s* /('$'? [/w '.']+) ('(' /(/d+) ')')? (/s+ /('output'))? $; f")
             .Map(out var result))
         {
            var name = result.FirstGroup;
            var signature = result.SecondGroup;
            if (signature.IsEmpty())
            {
               signature = name.ToTitleCase();
            }

            var typeName = fixTypeName(result.ThirdGroup);
            var _type = getType(typeName);
            var _size = Maybe.Int32(result.FourthGroup);
            var output = result.FifthGroup.Same("output");

            return new Parameter(name, signature)
            {
               Type = _type,
               Size = _size,
               Output = output,
               Value = nil,
               Default = nil
            };
         }
         else
         {
            return nil;
         }
      }

      public static Parameter Parse(Group parameterGroup)
      {
         var name = parameterGroup.Key;
         var signature = parameterGroup.GetValue("signature") | name;
         var typeName = parameterGroup.GetValue("type") | "$string";
         typeName = fixTypeName(typeName);
         var _type = getType(typeName);
         var _size = parameterGroup.GetValue("size").Map(s => ConversionFunctions.Value.Int32(s));
         var output = parameterGroup.GetValue("output").Map(s => s == "true") | false;
         var _value = parameterGroup.GetValue("value");
         var _default = parameterGroup.GetValue("default");

         return new Parameter(name, signature)
         {
            Type = _type,
            Size = _size,
            Output = output,
            Value = _value,
            Default = _default
         };
      }

      protected static Maybe<Type> getType(string typeName)
      {
         return maybe(typeName.IsNotEmpty(), () => System.Type.GetType(typeName, true, true));
      }

      protected static string fixTypeName(string typeName)
      {
         typeName = typeName.Substitute("^ '$'; f", "System.");
         return typeName.IsNotEmpty() && !typeName.Has(".") ? "System." + typeName : typeName;
      }

      public Parameter(string name, string signature) : base(name, signature)
      {
      }

      public Parameter(string name, string signature, Type type) : base(name, signature)
      {
         Type = type;
         Size = nil;
         Output = false;
         Value = nil;
         Default = nil;
      }

      public Maybe<Type> Type { get; set; }

      public Maybe<int> Size { get; set; }

      public bool Output { get; set; }

      public Maybe<string> Value { get; set; }

      public Maybe<string> Default { get; set; }
   }
}