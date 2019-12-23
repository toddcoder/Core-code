using System;
using System.Linq.Expressions;
using Core.Assertions.Collections;
using static Core.Assertions.AssertionFunctions;

namespace Core.ObjectGraphs
{
   public static class ObjectGraphExtensions
   {
      public static DictionaryAssertion<string, ObjectGraph> Must(this ObjectGraph objectGraph)
      {
         var hash = objectGraph.AnyHash().ForceValue();
         return new DictionaryAssertion<string, ObjectGraph>(hash);
      }

      public static DictionaryAssertion<string, ObjectGraph> Must(this Expression<Func<ObjectGraph>> expression)
      {
         var (name, value) = resolve(expression);
         var assertion = value.Must();

         return (DictionaryAssertion<string, ObjectGraph>)assertion.Named($"ObjectGraph {name} -> {value}");
      }
   }
}