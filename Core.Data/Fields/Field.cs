using System;
using Core.Collections;
using Core.Monads;
using Core.ObjectGraphs;
using Core.Objects;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Fields
{
   public class Field : PropertyInterface
   {
      public static Field Parse(ObjectGraph fieldGraph)
      {
         var name = fieldGraph.Name;
         var optional = false;
         var type = getType(fieldGraph.Type);
         var signature = "";
         if (fieldGraph.HasChildren)
         {
            string ifNone() => name;
            signature = fieldGraph.FlatMap("signature", o => o.Value, ifNone);
            optional = fieldGraph.FlatMap("optional", o => o.Value == "true", false);
            if (fieldGraph.Type.IsEmpty())
            {
               type = fieldGraph.Map("type", o => getType(o.Value));
            }
         }
         else if (fieldGraph.Value.IsNotEmpty())
         {
            signature = fieldGraph.Value;
         }
         else
         {
            signature = name;
         }

         if (name.StartsWith("."))
         {
            name = $"{name.Tail()}";
         }

         return new Field(name, signature, optional) { Type = type };
      }

      static IMaybe<Type> getType(string typeName)
      {
         if (typeName.IsEmpty())
         {
            return none<Type>();
         }
         else
         {
            var fullName = typeName.Substitute("^ '$'", "System.");
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