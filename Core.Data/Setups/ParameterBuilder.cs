 using System;
using Core.Data.Parameters;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class ParameterBuilder
{
   public static ParameterBuilder operator +(ParameterBuilder builder, SqlSetupBuilderParameters.IParameterParameter parameter) => parameter switch
   {
      SqlSetupBuilderParameters.DefaultValue defaultValue => builder.Default(defaultValue),
      SqlSetupBuilderParameters.ParameterName name => builder.Name(name),
      SqlSetupBuilderParameters.Output output => builder.Output(output),
      SqlSetupBuilderParameters.Signature signature => builder.Signature(signature),
      SqlSetupBuilderParameters.Size size => builder.Size(size),
      SqlSetupBuilderParameters.Type type => builder.Type(type),
      SqlSetupBuilderParameters.ValueParameter valueParameter => builder.Value(valueParameter),
      _ => throw new ArgumentOutOfRangeException(nameof(parameter))
   };

   protected SqlSetupBuilder setupBuilder;
   protected Maybe<string> _name;
   protected Maybe<string> _signature;
   protected Maybe<Type> _type;
   protected Maybe<int> _size;
   protected bool output;
   protected Maybe<string> _value;
   protected Maybe<string> _default;

   public ParameterBuilder(SqlSetupBuilder setupBuilder)
   {
      this.setupBuilder = setupBuilder;
      this.setupBuilder.ParameterBuilder(this);

      _name = nil;
      _signature = nil;
      _type = nil;
      _size = nil;
      output = false;
      _value = nil;
      _default = nil;
   }

   public ParameterBuilder Name(string name)
   {
      _name = name;
      return this;
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

   public Result<Parameter> Build()
   {
      if (_name)
      {
         var name = _name.Value;
         var signature = _signature | (() => name.Substitute("^ '@'; f", "").ToUpper1());
         return new Parameter(name, signature)
         {
            Type = _type,
            Size = _size,
            Output = output,
            Value = _value,
            Default = _default
         };
      }
      else
      {
         return fail("Parameter name not provided");
      }
   }
}