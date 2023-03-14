using System;
using System.IO;
using System.Text;
using Core.Dates;
using Core.Monads;
using Newtonsoft.Json;

namespace Core.Json;

public class JsonWriter : IDisposable
{
   protected MemoryStream stream;
   protected StreamWriter streamWriter;
   protected JsonTextWriter writer;

   public JsonWriter()
   {
      stream = new MemoryStream();
      streamWriter = new StreamWriter(stream);
      writer = new JsonTextWriter(streamWriter)
      {
         Formatting = Formatting.Indented,
         Indentation = 2,
         QuoteChar = '"',
         IndentChar = ' ',
         QuoteName = true
      };
   }

   public void BeginObject() => writer.WriteStartObject();

   public void BeginObject(string propertyName)
   {
      writer.WritePropertyName(propertyName);
      writer.WriteStartObject();
   }

   public void BeginArray() => writer.WriteStartArray();

   public void BeginArray(string propertyName)
   {
      writer.WritePropertyName(propertyName);
      writer.WriteStartArray();
   }

   public void EndArray() => writer.WriteEndArray();

   public void EndObject() => writer.WriteEndObject();

   public void Write(string value) => writer.WriteValue(value);

   public void Write(int value) => writer.WriteValue(value);

   public void Write(double value) => writer.WriteValue(value);

   public void Write(bool value) => writer.WriteValue(value);

   public void Write(DateTime value, bool zulu = false)
   {
      if (zulu)
      {
         writer.WriteValue(value.Zulu());
      }
      else
      {
         writer.WriteValue(value);
      }
   }

   public void Write(Guid value) => writer.WriteValue(value);

   public void WritePropertyNameIf(Maybe<string> _propertyName)
   {
      if (_propertyName is (true, var propertyName))
      {
         writer.WritePropertyName(propertyName);
      }
   }

   public void Write(string propertyName, string value)
   {
      writer.WritePropertyName(propertyName);
      writer.WriteValue(value);
   }

   public void Write(string propertyName, int value)
   {
      writer.WritePropertyName(propertyName);
      writer.WriteValue(value);
   }

   public void Write(string propertyName, double value)
   {
      writer.WritePropertyName(propertyName);
      writer.WriteValue(value);
   }

   public void Write(string propertyName, bool value)
   {
      writer.WritePropertyName(propertyName);
      writer.WriteValue(value);
   }

   public void Write(string propertyName, DateTime value)
   {
      writer.WritePropertyName(propertyName);
      writer.WriteValue(value);
   }

   public void Write(string propertyName, Guid value)
   {
      writer.WritePropertyName(propertyName);
      writer.WriteValue(value);
   }

   public void Write(string propertyName, byte[] value)
   {
      writer.WritePropertyName(propertyName);
      writer.WriteValue(value);
   }

   public void WriteNull(string propertyName)
   {
      writer.WritePropertyName(propertyName);
      writer.WriteNull();
   }

   public override string ToString()
   {
      writer.Flush();
      return Encoding.UTF8.GetString(stream.ToArray());
   }

   public void Dispose()
   {
      stream?.Dispose();
   }
}