using System;
using Core.Configurations;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Services.Plugins;

public abstract class AfterPlugin : Plugin
{
   protected Setting parentSetting;

   public AfterPlugin(string name, Configuration configuration, Setting jobSetting, Setting parentSetting) : base(name, configuration, jobSetting)
   {
      this.parentSetting = parentSetting;
   }

   public override Result<Unit> Dispatch() => unit;

   public abstract void AfterSuccess();

   public abstract void AfterFailure(Exception exception);

   protected override void createScheduler()
   {
   }
}