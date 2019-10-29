using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.ObjectGraphs
{
   public class ObjectGraphTrying
   {
      ObjectGraph graph;

      public ObjectGraphTrying(ObjectGraph graph) => this.graph = graph;

      public ObjectGraph Graph => graph;

      public IResult<ObjectGraph> this[string name]
      {
         get => assert(graph.ContainsKey(name), () => graph[name], () => $"Child {name} not found");
      }

      public IResult<object> Fill(object obj) => tryTo(() =>
      {
         graph.Fill(ref obj);
         return obj;
      });
   }
}