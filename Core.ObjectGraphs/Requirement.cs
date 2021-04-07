using Core.Assertions;
using Core.Collections;
using Core.Exceptions;
using static Core.Assertions.AssertionFunctions;

namespace Core.ObjectGraphs
{
   public class Requirement
   {
      protected ObjectGraph objectGraph;

      public Requirement(ObjectGraph objectGraph) => this.objectGraph = objectGraph;

      public ObjectGraph this[string graphName]
      {
         get
         {
            assert(() => graphName).Must().Not.BeNullOrEmpty().OrThrow();

            if (objectGraph.If(graphName, out var foundGraph))
            {
               foundGraph.Replacer = objectGraph.Replacer;

               return foundGraph;
            }

            throw ($"'{graphName}' graph is required under <{objectGraph.Path}> @ {objectGraph.LineNumber}:" +
               $" {objectGraph.LineSource}").Throws();
         }
      }
   }
}