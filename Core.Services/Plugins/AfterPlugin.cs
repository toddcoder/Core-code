using System;
using Core.Configurations;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Services.Plugins
{
   public abstract class AfterPlugin : Plugin
   {
      protected Group parentGroup;

      public AfterPlugin(string name, Configuration configuration, Group jobGroup, Group parentGroup) : base(name, configuration, jobGroup)
      {
         this.parentGroup = parentGroup;
      }

      public override Result<Unit> Dispatch() => unit;

      public abstract void AfterSuccess();

      public abstract void AfterFailure(Exception exception);

      protected override void createScheduler()
      {
      }
   }
}