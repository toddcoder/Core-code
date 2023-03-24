using System;
using Core.Data.Fields;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class FieldBuilder
{
   public static FieldBuilder operator +(FieldBuilder builder, SqlSetupBuilderParameters.IFieldParameter parameter) => parameter switch
   {
      SqlSetupBuilderParameters.FieldName name => builder.Name(name),
      SqlSetupBuilderParameters.Optional optional => builder.Optional(optional),
      SqlSetupBuilderParameters.Signature signature => builder.Signature(signature),
      SqlSetupBuilderParameters.Type type => builder.Type(type),
      _ => throw new ArgumentOutOfRangeException(nameof(parameter))
   };

   protected SqlSetupBuilder setupBuilder;
   protected Optional<string> _name;
   protected Optional<string> _signature;
   protected bool optional;
   protected Optional<Type> _type;

   public FieldBuilder(SqlSetupBuilder setupBuilder)
   {
      this.setupBuilder = setupBuilder;
      this.setupBuilder.FieldBuilder(this);

      _name = nil;
      _signature = nil;
      optional = false;
      _type = nil;
   }

   public FieldBuilder Name(string name)
   {
      _name = name;
      return this;
   }

   public FieldBuilder Signature(string signature)
   {
      _signature = signature;
      return this;
   }

   public FieldBuilder Optional(bool optional)
   {
      this.optional = optional;
      return this;
   }

   public FieldBuilder Type(Type type)
   {
      _type = type;
      return this;
   }

   public Optional<Field> Build()
   {
      if (_name is (true, var name))
      {
         var signature = _signature | (() => name.ToUpper1());
         return new Field(name, signature, optional) { Type = _type };
      }
      else
      {
         return fail("Field name not provided");
      }
   }
}