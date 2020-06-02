using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Monads;
using Core.ObjectGraphs;
using static Core.Monads.AttemptFunctions;

namespace Core.Data.Fields
{
   public class Fields : IEnumerable<Field>
   {
      Hash<string, Field> fields;
      List<string> ordered;

      public static IResult<Fields> FromObjectGraph(IMaybe<ObjectGraph> fieldsGraph) => tryTo(() => new Fields(fieldsGraph));

      public Fields()
      {
         fields = new Hash<string, Field>();
         ordered = new List<string>();
      }

      public Fields(IEnumerable<Field> fields) : this()
      {
         foreach (var field in fields)
         {
            Add(field);
         }
      }

      public Fields(IMaybe<ObjectGraph> anyFieldsGraph) : this()
      {
         if (anyFieldsGraph.If(out var fieldsGraph))
         {
            foreach (var field in fieldsGraph.Children.Select(Field.Parse))
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

      public IMaybe<Field> this[string name] => fields.Map(name);

      public IMaybe<Field> Ordered(int index) => fields.Map(ordered[index]);

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