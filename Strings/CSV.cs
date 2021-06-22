using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Strings
{
   public class Csv : IEnumerable<IEnumerable<string>>
   {
      public class Record : IEnumerable<string>
      {
         protected List<string> fields;
         protected bool isEmpty;

         public Record(string record, DelimitedText delimitedText) : this()
         {
            foreach (var field in record.Split("','; f"))
            {
               fields.Add(delimitedText.Restringify(field, RestringifyQuotes.None));
            }

            isEmpty = false;
         }

         public Record()
         {
            fields = new List<string>();
            isEmpty = true;
         }

         public string this[int index]
         {
            get => FieldExists(index) ? fields[index] : "";
            set
            {
               if (FieldExists(index))
               {
                  fields[index] = value;
               }
            }
         }

         public string[] Fields => fields.ToArray();

         public bool IsEmpty => isEmpty;

         public bool FieldExists(int index) => index > -1 && index < fields.Count;

         public override string ToString() => fields.Select(field => field.Has(",") ? "\"" + field + "\"" : field).ToString(",");

         public IEnumerator<string> GetEnumerator() => fields.GetEnumerator();

         IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
      }

      public static implicit operator Csv(string source) => new(source);

      protected DelimitedText delimitedText;
      protected List<Record> records;

      public Csv(string source)
      {
         records = new List<Record>();
         delimitedText = DelimitedText.AsBasic();
         var destringified = delimitedText.Destringify(source);
         if (source.IsNotEmpty())
         {
            foreach (var record in destringified.Split("/r /n | /r | /n; f"))
            {
               records.Add(getNewRecord(record, delimitedText));
            }
         }
      }

      internal Csv(IEnumerable<Record> records, DelimitedText delimitedText)
      {
         this.records = new List<Record>();
         this.delimitedText = delimitedText;
         var ignored = false;
         foreach (var record in records)
         {
            if (ignored)
            {
               this.records.Add(record);
            }
            else
            {
               ignored = true;
            }
         }
      }

      public Record this[int index] => index > -1 && index < records.Count ? records[index] : new Record();

      public List<Record> Records => records;

      internal DelimitedText DelimitedText => delimitedText;

      protected static Record getNewRecord(string record, DelimitedText delimitedText) => new(record, delimitedText);

      public override string ToString() => records.Select(record => record.ToString()).ToString("\r\n");

      public IEnumerator<IEnumerable<string>> GetEnumerator() => records.Select(record => (IEnumerable<string>)record).GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public IEnumerable<T> Objects<T>(params string[] signatures) where T : new()
      {
         var evaluator = new PropertyEvaluator(new T());
         return records.Where(record => !record.IsEmpty).Select(record => getObject<T>(evaluator, record, signatures));
      }

      protected static T getObject<T>(PropertyEvaluator evaluator, Record record, params string[] signatures) where T : new()
      {
         var entity = new T();
         evaluator.Object = some<T, object>(entity);
         var field = 0;
         foreach (var signature in signatures)
         {
            if (record.FieldExists(field))
            {
               evaluator[signature] = record[field++].ToObject();
            }
            else
            {
               break;
            }
         }

         return entity;
      }

      public IMaybe<T> FirstObject<T>(params string[] signatures) where T : new() => maybe(records.Count > 0, () =>
      {
         var evaluator = new PropertyEvaluator(new T());
         return getObject<T>(evaluator, records[0], signatures);
      });
   }
}