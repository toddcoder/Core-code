using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Objects
{
   public class InvokerTrying
   {
      protected Invoker invoker;

      public InvokerTrying(Invoker invoker) => this.invoker = invoker;

      public IResult<T> Invoke<T>(string name, params object[] args) => tryTo(() => invoker.Invoke<T>(name, args));

      public IResult<Unit> Invoke(string name, params object[] args) => tryTo(() =>
      {
         invoker.Invoke(name, args);
         return Unit.Value;
      });

      public IResult<T> GetProperty<T>(string name, params object[] args) => tryTo(() => invoker.GetProperty<T>(name, args));

      public IResult<Unit> SetProperty(string name, params object[] args) => tryTo(() => invoker.SetProperty(name, args));

      public IResult<T> GetField<T>(string name, params object[] args) => tryTo(() => invoker.GetField<T>(name, args));

      public void SetField(string name, params object[] args) => tryTo(() => invoker.SetField(name, args));
   }
}