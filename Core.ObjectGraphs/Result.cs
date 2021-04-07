using Core.Collections;
using Core.Monads;

namespace Core.ObjectGraphs
{
   public class Result
   {
      protected ObjectGraph objectGraph;

      public Result(ObjectGraph objectGraph) => this.objectGraph = objectGraph;

      public IResult<ObjectGraph> this[string graphName]
      {
         get
         {
            if (objectGraph.If(graphName, out var graph))
            {
               graph.Replacer = objectGraph.Replacer;
               return graph.Success();
            }

            return ($"'{graphName}' graph is required under <{objectGraph.Path}> @" +
               $" {objectGraph.LineNumber}: {objectGraph.LineSource}").Failure<ObjectGraph>();
         }
      }
   }
}