using Core.Collections;

namespace Core.ObjectGraphs.Configurations
{
   public class Configuration : IHash<string, ObjectGraph>
   {
      ObjectGraph rootGraph;

      public ObjectGraph this[string key] => rootGraph[key];

      public bool ContainsKey(string key) => rootGraph.ContainsKey(key);
   }
}