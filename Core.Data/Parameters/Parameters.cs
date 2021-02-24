using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Monads;
using Core.ObjectGraphs;
using static Core.Monads.AttemptFunctions;

namespace Core.Data.Parameters
{
   public class Parameters : IEnumerable<Parameter>, IHash<string, Parameter>
   {
      public static IResult<Parameters> FromObjectGraph(IMaybe<ObjectGraph> parametersGraph) => tryTo(() => new Parameters(parametersGraph));

      protected Hash<string, Parameter> parameters;

      public Parameters() => parameters = new Hash<string, Parameter>();

      public Parameters(IEnumerable<Parameter> parameters) : this()
      {
         foreach (var parameter in parameters)
         {
            this.parameters[parameter.Name] = parameter;
         }
      }

      public Parameters(IMaybe<ObjectGraph> parametersGraph) : this()
      {
         if (parametersGraph.If(out var pg))
         {
            foreach (var parameter in pg.Children.Select(Parameter.Parse))
            {
               this[parameter.Name] = parameter;
            }
         }
      }

      public Parameter this[string name]
      {
         get => parameters[name];
         set => parameters[name] = value;
      }

      public bool ContainsKey(string key) => parameters.ContainsKey(key);

      public IResult<Hash<string, Parameter>> AnyHash() => parameters.Success();

      public int Count => parameters.Count;

      public void DeterminePropertyTypes(object entity)
      {
         foreach (var pair in parameters)
         {
            pair.Value.DeterminePropertyType(entity);
         }
      }

      public IEnumerator<Parameter> GetEnumerator() => parameters.Values.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}