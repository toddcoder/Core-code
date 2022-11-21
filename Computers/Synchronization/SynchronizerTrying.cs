using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Computers.Synchronization;

public class SynchronizerTrying
{
   protected Synchronizer synchronizer;

   public SynchronizerTrying(Synchronizer synchronizer)
   {
      this.synchronizer = synchronizer;
   }

   public Result<Unit> Synchronize() => tryTo(() => synchronizer.Synchronize());

   public Result<Unit> Synchronize(params string[] fileNames) => tryTo(() => synchronizer.Synchronize(fileNames));
}