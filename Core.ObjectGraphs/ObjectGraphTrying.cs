using Core.Monads;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.AttemptFunctions;

namespace Core.ObjectGraphs
{
   public class ObjectGraphTrying
   {
      protected ObjectGraph graph;

      public ObjectGraphTrying(ObjectGraph graph) => this.graph = graph;

      public ObjectGraph Graph => graph;

      public IResult<ObjectGraph> this[string name]
      {
         get => assert(() => graph).Must().HaveKeyOf(name).OrFailure().Map(d => d[name]);
      }

      public IResult<object> Fill(object obj) => tryTo(() =>
      {
         graph.Fill(ref obj);
         return obj;
      });
   }
}