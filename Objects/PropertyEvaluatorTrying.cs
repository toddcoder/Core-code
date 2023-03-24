using System;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Objects;

public class PropertyEvaluatorTrying
{
   protected PropertyEvaluator evaluator;

   public PropertyEvaluatorTrying(PropertyEvaluator evaluator) => this.evaluator = evaluator;

   public PropertyEvaluator Evaluator => evaluator;

   public Optional<object> this[string signature] => tryTo(() => evaluator[signature]);

   public Optional<object> Set(string signature, object value) => tryTo(() => evaluator[signature] = value);

   public Optional<object> this[Signature signature] => tryTo(() => evaluator[signature]);

   public Optional<object> Set(Signature signature, object value) => tryTo(() => evaluator[signature] = value);

   public Optional<Type> Type(string signature) => tryTo(() => evaluator.Type(signature));

   public Optional<Type> Type(Signature signature) => tryTo(() => evaluator.Type(signature));

   public Optional<bool> Contains(string signature) => tryTo(() => evaluator.Contains(signature));
}