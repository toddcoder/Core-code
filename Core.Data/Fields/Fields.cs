using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Configurations;
using Core.Monads;
using static Core.Monads.AttemptFunctions;
using static Core.Matching.MatchingExtensions;

namespace Core.Data.Fields;

public class Fields : IEnumerable<Field>
{
   public static IEnumerable<Field> FieldsFromString(string input)
   {
      foreach (var _field in input.Unjoin("/s* ',' /s*; f").Select(Field.FromString))
      {
         if (_field)
         {
            yield return _field;
         }
      }
   }

   protected StringHash<Field> fields;
   protected List<string> ordered;

   public static Optional<Fields> FromSetting(Optional<Setting> fieldsGroup) => tryTo(() => new Fields(fieldsGroup));

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

   public Fields(Optional<Setting> _fieldsSetting) : this()
   {
      if (_fieldsSetting is (true, var fieldsSetting))
      {
         foreach (var field in fieldsSetting.Settings().Select(t => Field.Parse(t.setting)))
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

   public Optional<Field> this[string name] => fields.Maybe(name);

   public Optional<Field> Ordered(int index) => fields.Maybe(ordered[index]);

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