using System.Linq;
using Core.Services.Plugins;
using Standard.Configurations;
using Standard.ObjectGraphs;
using Standard.Types.Collections;

namespace Standard.Services.Plugins
{
   public class Sequence : Plugin, IRequiresTypeManager
   {
      OrderedSet<Job> set;

      public Sequence(string name, Configuration configuration, ObjectGraph jobGroup)
         : base(name, configuration, jobGroup) => set = new OrderedSet<Job>();

      public override void Dispatch()
      {
         foreach (var job in set)
            job.ExecutePlugin();
      }

      public TypeManager TypeManager { get; set; }

      public override void SetUp()
      {
         base.SetUp();

         set = new OrderedSet<Job>(jobGroup["jobs"].Children
            .Where(g => g.FlatMap("enabled", h => h.IsTrue, true))
            .Select(g => new Job(g, TypeManager, configuration)));
      }
   }
}