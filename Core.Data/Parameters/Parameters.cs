using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Configurations;
using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.Data.Parameters
{
   public class Parameters : IEnumerable<Parameter>, IHash<string, Parameter>
   {
      public static Result<Parameters> FromGroup(Maybe<Group> parametersGroup) => tryTo(() => new Parameters(parametersGroup));

      protected StringHash<Parameter> parameters;

      public Parameters() => parameters = new StringHash<Parameter>(true);

      public Parameters(IEnumerable<Parameter> parameters) : this()
      {
         foreach (var parameter in parameters)
         {
            this.parameters[parameter.Name] = parameter;
         }
      }

      public Parameters(Maybe<Group> _parametersGroup) : this()
      {
         if (_parametersGroup.If(out var parametersGroup))
         {
            foreach (var (key, parameter) in parametersGroup.Groups().Select(t => (t.key, Parameter.Parse(t.group))))
            {
               this[key] = parameter;
            }
         }
      }

      public Parameter this[string name]
      {
         get => parameters[name];
         set => parameters[name] = value;
      }

      public bool ContainsKey(string key) => parameters.ContainsKey(key);

      public Result<Hash<string, Parameter>> AnyHash() => parameters.AsHash;

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