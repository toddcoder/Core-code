using System;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Objects
{
   public class PropertyEvaluatorTrying
   {
      PropertyEvaluator evaluator;

      public PropertyEvaluatorTrying(PropertyEvaluator evaluator) => this.evaluator = evaluator;

      public PropertyEvaluator Evaluator => evaluator;

      public IResult<object> this[string signature] => tryTo(() => evaluator[signature]);

      public IResult<object> this[Signature signature] => tryTo(() => evaluator[signature]);

      public IResult<Type> Type(string signature) => tryTo(() => evaluator.Type(signature));

      public IResult<Type> Type(Signature signature) => tryTo(() => evaluator.Type(signature));

      public IResult<bool> Contains(string signature) => tryTo(() => evaluator.Contains(signature));
   }
}