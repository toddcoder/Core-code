using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Configurations;
using Core.Monads;
using static Core.Monads.AttemptFunctions;
using static Core.Matching.MatchingExtensions;

namespace Core.Data.Fields
{
   public class Fields : IEnumerable<Field>
   {
      public static IEnumerable<Field> FieldsFromString(string input)
      {
         foreach (var _field in input.Unjoin("/s* ',' /s*; f").Select(Field.FromString))
         {
            if (_field.Map(out var field))
            {
               yield return field;
            }
         }
      }

      protected StringHash<Field> fields;
      protected List<string> ordered;

      public static Result<Fields> FromGroup(Maybe<Group> fieldsGroup) => tryTo(() => new Fields(fieldsGroup));

      public Fields()
      {
         fields = new StringHash<Field>(true);
         ordered = new List<string>();
      }

      public Fields(IEnumerable<Field> fields) : this()
      {
         foreach (var field in fields)
         {
            Add(field);
         }
      }

      public Fields(Maybe<Group> _fieldsGroup) : this()
      {
         if (_fieldsGroup.Map(out var fieldsGraph))
         {
            foreach (var field in fieldsGraph.Groups().Select(t => Field.Parse(t.group)))
            {
               Add(field);
            }
         }
      }

      public void Add(Field field)
      {
         fields[field.Name] = field;
         ordered.Add(field.Name);
      }

      public Maybe<Field> this[string name] => fields.Map(name);

      public Maybe<Field> Ordered(int index) => fields.Map(ordered[index]);

      public void DeterminePropertyTypes(object entity)
      {
         foreach (var item in fields)
         {
            item.Value.DeterminePropertyType(entity);
         }
      }

      public IEnumerator<Field> GetEnumerator() => fields.Values.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public int Count => fields.Count;
   }
}