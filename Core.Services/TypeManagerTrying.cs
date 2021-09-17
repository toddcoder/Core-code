using System;
using Core.Services;
using Standard.Types.Monads;
using static Standard.Types.Monads.AttemptFunctions;

namespace Standard.Services
{
   public class TypeManagerTrying
   {
      TypeManager typeManager;

      public TypeManagerTrying(TypeManager typeManager) => this.typeManager = typeManager;

      public IResult<Type> Type(string assemblyName, string typeName) => tryTo(() => typeManager.Type(assemblyName, typeName));
   }
}