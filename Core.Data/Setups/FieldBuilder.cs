using System;
using Core.Data.Fields;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Setups;

public class FieldBuilder
{
   protected string name;
   protected SqlSetupBuilder setupBuilder;
   protected Maybe<string> _signature;
   protected bool optional;
   protected Maybe<Type> _type;

   public FieldBuilder(string name, SqlSetupBuilder setupBuilder)
   {
      this.name = name;
      this.setupBuilder = setupBuilder;

      _signature = nil;
      optional = false;
      _type = nil;
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

   public SqlSetupBuilder EndField()
   {
      var signature = _signature | (() => name.ToUpper1());
      var field = new Field(name, signature, optional) { Type = _type };
      setupBuilder.AddField(field);

      return setupBuilder;
   }
}