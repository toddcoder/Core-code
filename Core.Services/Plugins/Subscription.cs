using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Collections;
using Core.Configurations;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Services.Plugins
{
   public class Subscription : Plugin, IRequiresTypeManager
   {
      protected StringSet jobNames;

      public Subscription(string name, Configuration configuration, Group jobGroup)
         : base(name, configuration, jobGroup)
      {
         jobNames = new StringSet(true);
      }

      public void Subscribe(string jobName) => jobNames.Add(jobName);

      public bool IsSubscribed(string jobName) => jobNames.Contains(jobName);

      protected IEnumerable<Job> getJobs(Group jobsGroup)
      {
         foreach (var jobName in jobNames)
         {
            var _job =
               from jobGroup in jobsGroup.RequireGroup(jobName)
               from newJob in Job.New(jobGroup, TypeManager, configuration)
               select newJob;
            if (_job.If(out var job, out var exception))
            {
               if (job.Enabled)
               {
                  yield return job;
               }
            }
            else
            {
               serviceMessage.EmitException(exception);
            }
         }
      }

      public override Result<Unit> Dispatch()
      {
         if (configuration.RequireGroup("jobs").If(out var jobsGroup, out var exception))
         {
            var tasks = getJobs(jobsGroup).Select(job => Task.Run(job.ExecutePlugin)).ToArray();
            Task.WaitAll(tasks);

            return unit;
         }
         else
         {
            return exception;
         }
      }

      public TypeManager TypeManager { get; set; }
   }
}