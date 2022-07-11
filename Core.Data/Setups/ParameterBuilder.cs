using System;
using Core.Data.Parameters;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class ParameterBuilder
{
   protected string name;
   protected SqlSetupBuilder setupBuilder;
   protected Maybe<string> _signature;
   protected Maybe<Type> _type;
   protected Maybe<int> _size;
   protected bool output;
   protected Maybe<string> _value;
   protected Maybe<string> _default;

   public ParameterBuilder(string name, SqlSetupBuilder setupBuilder)
   {
      this.name = name;
      this.setupBuilder = setupBuilder;

      _signature = nil;
      _type = nil;
      _size = nil;
      output = false;
      _value = nil;
      _default = nil;
   }

   public ParameterBuilder Signature(string signature)
   {
      _signature = signature;
      return this;
   }

   public ParameterBuilder Type(Type type)
   {
      _type = type;
      return this;
   }

   public ParameterBuilder Size(int size)
   {
      _size = size;
      return this;
   }

   public ParameterBuilder Output(bool output)
   {
      this.output = output;
      return this;
   }

   public ParameterBuilder Value(string value)
   {
      _value = value;
      return this;
   }

   public ParameterBuilder Default(string @default)
   {
      _default = @default;
      return this;
   }

   public SqlSetupBuilder EndParameter()
   {
      var signature = _signature.DefaultTo(() => name.ToUpper1());
      var parameter = new Parameter(name, signature)
      {
         Type = _type,
         Size = _size,
         Output = output,
         Value = _value,
         Default = _default
      };
      setupBuilder.AddParameter(parameter);

      return setupBuilder;
   }
}