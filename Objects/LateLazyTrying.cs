using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Applications.Async.AsyncFunctions;
using static Core.Monads.AttemptFunctions;

namespace Core.Objects;

public class LateLazyTrying<T>
{
   protected LateLazy<T> lateLazy;

   public LateLazyTrying(LateLazy<T> lateLazy)
   {
      this.lateLazy = lateLazy;
   }

   public Result<T> ActivateWith(Func<T> activator) => tryTo(() =>
   {
      lateLazy.ActivateWith(activator);
      return lateLazy.Value.Success();
   });

   public async Task<Completion<T>> ActivateWithAsync(Func<T> activator, CancellationToken token)
   {
      return await runAsync(t => ActivateWith(activator).Completion(t), token);
   }

   public Result<T> Value => tryTo(() => lateLazy.Value);

   public bool IsActivated => lateLazy.IsActivated;

   public Maybe<T> AnyValue => lateLazy.AnyValue;

   public bool HasActivator => lateLazy.HasActivator;
}