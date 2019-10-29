using System;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.ObjectGraphs
{
   public class Switch<T>
   {
      protected Hash<string, Func<ObjectGraph, T>> cases;
      protected ObjectGraph graph;
      protected IMaybe<Func<T>> defaultResult;

      public Switch()
      {
         cases = new Hash<string, Func<ObjectGraph, T>>();
         defaultResult = none<Func<T>>();
      }

      internal Switch(ObjectGraph graph) : this() => this.graph = graph;

      internal void SetGraph(ObjectGraph newGraph) => graph = newGraph;

      public Switch<T> Case(string name, Func<ObjectGraph, T> result)
      {
         cases[name] = result;
         return this;
      }

      public Switch<T> Default(Func<T> defaultFunc)
      {
         if (defaultResult.IsNone)
         {
            defaultResult = defaultFunc.Some();
         }

         return this;
      }

      public T Get()
      {
         foreach (var item in cases)
         {
            if (graph.If(item.Key, out var subGraph))
            {
               return item.Value(subGraph);
            }
         }

         return defaultResult.Required("Default not called")();
      }

      public IMaybe<T> Maybe()
      {
         foreach (var item in cases)
         {
            if (graph.If(item.Key, out var subGraph))
            {
               return item.Value(subGraph).Some();
            }
         }

         return none<T>();
      }
   }
}