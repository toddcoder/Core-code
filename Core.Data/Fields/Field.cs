﻿using System;
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
            string ifNone() => name.SnakeToCamelCase(true);
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
            signature = GetSignature(name).Required("Couldn't determine signature");
         }

         if (name.StartsWith("."))
         {
            name = $"{name.Tail()}";
         }

         return new Field(name, signature, optional) { Type = type };
      }

      public static IMaybe<string> GetSignature(string fieldName) => fieldName.SnakeToCamelCase(true).Some();

      static IMaybe<Type> getType(string typeName)
      {
         if (typeName.IsEmpty())
         {
            return none<Type>();
         }
         else
         {
            var fullName = typeName.Substitute("^ '$'", "System.");
            return System.Type.GetType(fullName, false, true).SomeIfNotNull();
         }
      }

      public Field(string name, string signature, bool optional) : base(name, signature) => Optional = optional;

      public Field() : base("", "") { }

      public int Ordinal { get; set; }

      public bool Optional { get; set; }

      public IMaybe<Type> Type { get; set; }

      public override Type PropertyType => Type.DefaultTo(() => base.PropertyType);
   }
}