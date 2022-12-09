using System;

namespace Core.Applications;

public class CancelAction<TState> : CancelTask<TState>
{
   protected Action<TState> action;

   public CancelAction(TState state, Action<TState> action) : base(state) => this.action = action;

   public override void Dispatch(TState state) => action(state);
}