using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Computers.Synchronization
{
   public class SynchronizerTrying
   {
      Synchronizer synchronizer;

      public SynchronizerTrying(Synchronizer synchronizer)
      {
         this.synchronizer = synchronizer;
      }

      public IResult<Unit> Synchronize() => tryTo(() => synchronizer.Synchronize());

      public IResult<Unit> Synchronize(params string[] fileNames) => tryTo(() => synchronizer.Synchronize(fileNames));
   }
}