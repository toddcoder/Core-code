using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Configurations;
using Core.Monads;
using static Core.Monads.AttemptFunctions;
using static Core.Matching.MatchingExtensions;

namespace Core.Data.Parameters;

public class Parameters : IEnumerable<Parameter>, IHash<string, Parameter>
{
   public static IEnumerable<Parameter> ParametersFromString(string input)
   {
      foreach (var _parameter in input.Unjoin("/s* ',' /s*; f").Select(Parameter.FromString))
      {
         if (_parameter is (true, var parameter))
         {
            yield return parameter;
         }
      }
   }

   public static Result<Parameters> FromSetting(Maybe<Setting> parametersGroup) => tryTo(() => new Parameters(parametersGroup));

   protected StringHash<Parameter> parameters;

   public Parameters() => parameters = new StringHash<Parameter>(true);

   public Parameters(IEnumerable<Parameter> parameters) : this()
   {
      foreach (var parameter in parameters)
      {
         this.parameters[parameter.Name] = parameter;
      }
   }

   public Parameters(Maybe<Setting> _parametersSetting) : this()
   {
      if (_parametersSetting is (true, var parametersSetting))
      {
         foreach (var (key, parameter) in parametersSetting.Settings().Select(t => (t.key, Parameter.Parse(t.setting))))
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