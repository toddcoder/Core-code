using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Objects
{
   public class InvokerTrying
   {
      protected Invoker invoker;

      public InvokerTrying(Invoker invoker) => this.invoker = invoker;

      public Result<T> Invoke<T>(string name, params object[] args) => tryTo(() => invoker.Invoke<T>(name, args));

      public Result<Unit> Invoke(string name, params object[] args) => tryTo(() =>
      {
         invoker.Invoke(name, args);
         return Unit.Value;
      });

      public Result<T> GetProperty<T>(string name, params object[] args) => tryTo(() => invoker.GetProperty<T>(name, args));

      public Result<Unit> SetProperty(string name, params object[] args) => tryTo(() => invoker.SetProperty(name, args));

      public Result<T> GetField<T>(string name, params object[] args) => tryTo(() => invoker.GetField<T>(name, args));

      public void SetField(string name, params object[] args) => tryTo(() => invoker.SetField(name, args));
   }
}