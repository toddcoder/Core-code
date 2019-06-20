using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Monads;
using Core.Objects;
using Core.RegularExpressions;
using static Core.Monads.MonadFunctions;

namespace Core.Strings
{
   public class CSV : IEnumerable<IEnumerable<string>>
   {
      public class Record : IEnumerable<string>
      {
         protected List<string> fields;
         protected bool isEmpty;

         public Record(string record, Destringifier destringifier) : this()
         {
            foreach (var field in record.Split("','"))
            {
               fields.Add(destringifier.Restring(field, false));
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

         public override string ToString() => fields.Select(field => field.Has(",") ? "\"" + field + "\"" : field).Stringify(",");

         public IEnumerator<string> GetEnumerator() => fields.GetEnumerator();

         IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
      }

      public static implicit operator CSV(string source) => new CSV(source);

      protected Destringifier destringifier;
      protected List<Record> records;

      public CSV(string source)
      {
         records = new List<Record>();
         destringifier = new Destringifier(source);
         var destringified = destringifier.Parse();
         if (source.IsNotEmpty())
         {
            foreach (var record in destringified.Split("/r /n | /r | /n"))
            {
               records.Add(getNewRecord(record, destringifier));
            }
         }
      }

      internal CSV(IEnumerable<Record> records, Destringifier destringifier)
      {
         this.records = new List<Record>();
         this.destringifier = destringifier;
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

      internal Destringifier Destringifier => destringifier;

      protected static Record getNewRecord(string record, Destringifier destringifier) => new Record(record, destringifier);

      public override string ToString() => records.Select(record => record.ToString()).Stringify("\r\n");

      public IEnumerator<IEnumerable<string>> GetEnumerator() => records.Select(record => (IEnumerable<string>)record).GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public IEnumerable<T> Objects<T>(params string[] signatures)
         where T : new()
      {
         var evaluator = new PropertyEvaluator(new T());
         return records.Where(record => !record.IsEmpty).Select(record => getObject<T>(evaluator, record, signatures));
      }

      static T getObject<T>(PropertyEvaluator evaluator, Record record, params string[] signatures)
         where T : new()
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

      public IMaybe<T> FirstObject<T>(params string[] signatures)
         where T : new()
      {
         if (records.Count > 0)
         {
            var evaluator = new PropertyEvaluator(new T());
            return getObject<T>(evaluator, records[0], signatures).Some();
         }
         else
         {
            return none<T>();
         }
      }
   }
}