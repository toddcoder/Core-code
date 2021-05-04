using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Core.Collections;
using Core.Strings;

namespace Core.Io.Delimited
{
   public class DelimitedTextReader : IDataReader, IEnumerable<string[]>
   {
      protected int fieldCount;
      protected string[] fields;
      protected Buffer buffer;
      protected List<string> headers;
      protected StringHash<int> indexes;
      protected Hash<int, Func<string, object>> converters;
      protected bool hasConverters;
      protected bool emptyRecord;
      protected int currentFieldCount;

      public DelimitedTextReader(TextReader reader, int fieldCount, char delimiter, int bufferSize = 0x1000)
      {
         this.fieldCount = fieldCount;
         fields = new string[fieldCount];
         buffer = new Buffer(reader, bufferSize, delimiter);
         headers = new List<string>();
         indexes = new StringHash<int>(true);
         converters = new Hash<int, Func<string, object>>();
         hasConverters = false;
         emptyRecord = false;
         currentFieldCount = this.fieldCount;
         Delimiter = delimiter;
      }

      public char Delimiter { get; }

      public bool FloatingFieldCount { get; set; }

      public void AddHeader(string header)
      {
         if (headers.Count < fieldCount)
         {
            indexes[header] = headers.Count;
            headers.Add(header);
         }
      }

      public void AddHeaders(params string[] inHeaders)
      {
         foreach (var header in inHeaders.TakeWhile(_ => headers.Count < fieldCount))
         {
            headers.Add(header);
         }
      }

      public void RegisterConverter(int fieldIndex, Func<string, object> conversion)
      {
         converters[fieldIndex] = conversion;
         hasConverters = true;
      }

      public bool StopAtPartialRecord { get; set; }

      public bool StoppedAtPartialRecord { get; set; }

      public bool ReadRecord()
      {
         StopAtPartialRecord = false;

         if (buffer.EndOfFile)
         {
            return false;
         }

         buffer.EndOfLine = false;
         currentFieldCount = 0;
         for (var i = 0; i < fieldCount && !buffer.EndOfLine && !buffer.EndOfFile; i++)
         {
            var field = buffer.NextField();
            fields[i] = field;
            currentFieldCount++;
         }

         emptyRecord = currentFieldCount == 0 && !buffer.EndOfFile;

         if (currentFieldCount < fieldCount)
         {
            for (var i = currentFieldCount; i < fieldCount; i++)
            {
               fields[i] = string.Empty;
            }

            if (StopAtPartialRecord)
            {
               StoppedAtPartialRecord = true;

               return false;
            }
         }

         return true;
      }

      public bool EmptyRecord => emptyRecord;

      public void CopyFields(string[] array) => Array.Copy(fields, array, FieldCount);

      public string GetName(int i) => headers[i];

      public string GetDataTypeName(int i) => typeof(string).FullName;

      public Type GetFieldType(int i) => typeof(string);

      public object GetValue(int i) => fields[i];

      public int GetValues(object[] values)
      {
         for (var i = 0; i < FieldCount; i++)
         {
            values[i] = fields[i];
         }

         return FieldCount;
      }

      public int GetOrdinal(string name) => indexes.Value(name);

      public bool GetBoolean(int i) => fields[i].ToBool();

      public byte GetByte(int i) => fields[i].ToByte();

      public long GetBytes(int i, long fieldOffset, byte[] buf, int bufferOffset, int length)
      {
         var value = fields[i];
         var chars = value.ToCharArray((int)fieldOffset, length);
         var source = new byte[chars.Length];
         for (var j = 0; j < chars.Length; j++)
         {
            source[j] = Convert.ToByte(chars[i]);
         }

         Array.Copy(source, 0, buf, fieldOffset, length);

         return length;
      }

      public char GetChar(int i) => fields[i].IsNotEmpty() ? fields[i][0] : char.MinValue;

      public long GetChars(int i, long fieldOffset, char[] buf, int bufferOffset, int length)
      {
         var value = fields[i];
         var chars = value.ToCharArray((int)fieldOffset, length);
         Array.Copy(chars, 0, buf, bufferOffset, length);

         return length;
      }

      public Guid GetGuid(int i) => new(fields[i]);

      public short GetInt16(int i) => short.Parse(fields[i]);

      public int GetInt32(int i) => fields[i].ToInt();

      public long GetInt64(int i) => fields[i].ToLong();

      public float GetFloat(int i) => fields[i].ToFloat();

      public double GetDouble(int i) => fields[i].ToDouble();

      public string GetString(int i) => fields[i];

      public decimal GetDecimal(int i) => fields[i].ToDecimal();

      public DateTime GetDateTime(int i) => fields[i].ToDateTime();

      public IDataReader GetData(int i) => i == 0 ? this : null;

      public bool IsDBNull(int i) => fields[i].IsEmpty();

      public int FieldCount => FloatingFieldCount ? currentFieldCount : fieldCount;

      object IDataRecord.this[int i] => fields[i];

      public object this[int i] => hasConverters ? converters.FlatMap(i, f => f(fields[i]), () => fields[i]) : fields[i];

      object IDataRecord.this[string name] => fields[indexes.Value(name)];

      public object this[string name] => fields[indexes.Value(name)];

      public void Close()
      {
      }

      public DataTable GetSchemaTable() => null;

      public bool NextResult() => false;

      public bool Read() => ReadRecord();

      public int Depth => 0;

      public bool IsClosed => buffer.EndOfFile;

      public int RecordsAffected => -1;

      public IEnumerator<string[]> GetEnumerator() => new DelimitedTextEnumerator(this);

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public void Dispose()
      {
      }
   }
}