using System;
using Core.Collections;
using Core.Monads;
using Core.ObjectGraphs;
using Core.Objects;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Parameters
{
   public class Parameter : PropertyInterface
   {
      public static Parameter Parse(ObjectGraph parameterGraph)
      {
         var name = parameterGraph.Name;
         var type = none<Type>();
         bool output;
         var size = 255.Some();
         var parameterValue = parameterGraph.Value;
         var signature = maybe(parameterValue.IsNotEmpty(), () => parameterValue);
         if (signature.IsNone)
         {
            signature = name.GetSignature();
         }

         var value = parameterGraph.Map("value", o => o.Value);
         var @default = parameterGraph.Map("default", o => o.Value);
         if (signature.If(out var signatureString))
         {
            if (parameterGraph.Type.IsNotEmpty())
            {
               var typeName = parameterGraph.Type;
               typeName = fixTypeName(typeName);
               type = getType(typeName);
            }

            output = false;
            var matcher = new Matcher();
            if (matcher.IsMatch(signatureString, "'(' /(/d+) ')'"))
            {
               size = matcher[0, 1].ToInt().Some();
               matcher[0] = "";
               signatureString = matcher.ToString();
            }
         }
         else
         {
            signatureString = parameterGraph
               .Map("signature", g => g.Value)
               .DefaultTo(() => name.GetSignature().Required($"There is no signature for '{name}'"));

            var typeName = parameterGraph.FlatMap("type", g => g.Value, "System.String");
            typeName = fixTypeName(typeName);
            type = getType(typeName);
            size = parameterGraph.Map("size", o => o.Value.ToInt());
            output = parameterGraph.FlatMap("output", o => o.IsTrue, false);
         }

         return new Parameter(name, signatureString)
         {
            Type = type,
            Size = size,
            Output = output,
            Value = value,
            Default = @default
         };
      }

      protected static IMaybe<Type> getType(string typeName)
      {
         return maybe(typeName.IsNotEmpty(), () => System.Type.GetType(typeName, true, true));
      }

      protected static string fixTypeName(string typeName)
      {
         typeName = typeName.Substitute("^ '$'", "System.");
         return typeName.IsNotEmpty() && !typeName.Has(".") ? "System." + typeName : typeName;
      }

      public Parameter(string name, string signature)
         : base(name, signature) { }

      public Parameter(string name, string signature, Type type) : base(name, signature)
      {
         Type = type.Some();
         Size = none<int>();
         Output = false;
         Value = none<string>();
         Default = none<string>();
      }

      public IMaybe<Type> Type { get; set; }

      public IMaybe<int> Size { get; set; }

      public bool Output { get; set; }

      public IMaybe<string> Value { get; set; }

      public IMaybe<string> Default { get; set; }
   }
}