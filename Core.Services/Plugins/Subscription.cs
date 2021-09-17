using System.Linq;
using System.Threading.Tasks;
using Core.Services.Plugins;
using Standard.Configurations;
using Standard.ObjectGraphs;
using Standard.Types.Collections;

namespace Standard.Services.Plugins
{
   public class Subscription : Plugin, IRequiresTypeManager
   {
      Set<string> jobNames;

      public Subscription(string name, Configuration configuration, ObjectGraph jobGroup)
         : base(name, configuration, jobGroup) => jobNames = new Set<string>();

      public void Subscribe(string jobName) => jobNames.Add(jobName);

      public bool IsSubscribed(string jobName) => jobNames.Contains(jobName);

      public override void Dispatch()
      {
         var jobsGraph = configuration["jobs"];
         var tasks = jobNames
            .Select(jobName => jobsGraph[jobName])
            .Where(graph => graph.FlatMap("enabled", e => e.Value == "true", () => true))
            .Select(graph => new Job(graph, TypeManager, configuration))
            .Select(job => Task.Run(job.ExecutePlugin)).ToArray();
         Task.WaitAll(tasks);
      }

      public TypeManager TypeManager { get; set; }
   }
}