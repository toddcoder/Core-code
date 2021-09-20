using System.Linq;
using System.Threading.Tasks;
using Core.Collections;
using Core.Configurations;
using Core.Monads;

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

      public override Result<Unit> Dispatch()
      {
         var jobsGraph = configuration["jobs"];
         var tasks = jobNames
            .Select(jobName => jobsGraph[jobName])
            .Where(graph => graph.FlatMap("enabled", e => e.Value == "true", () => true))
            .Select(graph => new Job(graph, configuration))
            .Select(job => Task.Run(job.ExecutePlugin)).ToArray();
         Task.WaitAll(tasks);
      }

      public TypeManager TypeManager { get; set; }
   }
}