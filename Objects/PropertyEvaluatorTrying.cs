using System;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Objects;

public class PropertyEvaluatorTrying
{
   protected PropertyEvaluator evaluator;

   public PropertyEvaluatorTrying(PropertyEvaluator evaluator) => this.evaluator = evaluator;

   public PropertyEvaluator Evaluator => evaluator;

   public Result<object> this[string signature] => tryTo(() => evaluator[signature]);

   public Result<object> Set(string signature, object value) => tryTo(() => evaluator[signature] = value);

   public Result<object> this[Signature signature] => tryTo(() => evaluator[signature]);

   public Result<object> Set(Signature signature, object value) => tryTo(() => evaluator[signature] = value);

   public Result<Type> Type(string signature) => tryTo(() => evaluator.Type(signature));

   public Result<Type> Type(Signature signature) => tryTo(() => evaluator.Type(signature));

   public Result<bool> Contains(string signature) => tryTo(() => evaluator.Contains(signature));
}