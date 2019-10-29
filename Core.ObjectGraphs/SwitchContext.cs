using System;

namespace Core.ObjectGraphs
{
   public class SwitchContext
   {
      protected readonly ObjectGraph graph;

      internal SwitchContext(ObjectGraph graph) => this.graph = graph;

      public Switch<T> Case<T>(string name, Func<ObjectGraph, T> result) => new Switch<T>(graph).Case(name, result);
   }
}