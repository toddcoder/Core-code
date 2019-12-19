﻿using Core.Assertions;
using Core.Collections;
using Core.Exceptions;

namespace Core.ObjectGraphs
{
   public class Requirement
   {
      ObjectGraph objectGraph;

      public Requirement(ObjectGraph objectGraph) => this.objectGraph = objectGraph;

      public ObjectGraph this[string graphName]
      {
         get
         {
            graphName.MustAs(nameof(graphName)).Not.BeNullOrEmpty().Assert();

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