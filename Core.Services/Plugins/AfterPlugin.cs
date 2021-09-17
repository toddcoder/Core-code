using System;
using Core.Services.Plugins;
using Standard.Configurations;
using Standard.ObjectGraphs;

namespace Standard.Services.Plugins
{
   public abstract class AfterPlugin : Plugin
   {
      protected ObjectGraph parentGraph;

      public AfterPlugin(string name, Configuration configuration, ObjectGraph jobGroup, ObjectGraph parentGraph)
         : base(name, configuration, jobGroup) => this.parentGraph = parentGraph;

      public override void Dispatch() { }

      public abstract void AfterSuccess();

      public abstract void AfterFailure(Exception exception);

      protected override void createSchedules() { }
   }
}