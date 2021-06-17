using System;
using Core.Monads;
using Core.Objects;
using Core.RegexMatching;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using Group = Core.Configurations.Group;

namespace Core.Data.Parameters
{
   public class Parameter : PropertyInterface
   {
      public static Parameter Parse(Group parameterGroup)
      {
         var name = parameterGroup.Key;
         var signature = parameterGroup.GetValue("signature").DefaultTo(() => name);
         var typeName = parameterGroup.GetValue("type").DefaultTo(() => "$string");
         typeName = fixTypeName(typeName);
         var type = getType(typeName);
         var size = parameterGroup.GetValue("size").Map(s => s.ToInt());
         var output = parameterGroup.GetValue("output").Map(s => s == "true").DefaultTo(() => false);
         var value = parameterGroup.GetValue("value");
         var _default = parameterGroup.GetValue("default");

         return new Parameter(name, signature)
         {
            Type = type,
            Size = size,
            Output = output,
            Value = value,
            Default = _default
         };
      }

      protected static IMaybe<Type> getType(string typeName)
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