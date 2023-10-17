using System;
using System.IO;
using System.Linq;
using System.Text;
using Core.Dates;
using Core.Monads;
using Core.Strings;
using System.Text.Json;
using Core.Arrays;

namespace Core.Json;

public class JsonWriter : IDisposable
{
   protected MemoryStream stream;
   protected Utf8JsonWriter writer;

   public JsonWriter()
   {
      stream = new MemoryStream();
      var options = new JsonWriterOptions
      {
         Indented = true
      };
      writer = new Utf8JsonWriter(stream, options);
   }

   public void BeginObject() => writer.WriteStartObject();

   public void BeginObject(string propertyName) => writer.WriteStartObject(propertyName);

   public void BeginArray() => writer.WriteStartArray();

   public void BeginArray(string propertyName) => writer.WriteStartArray(propertyName);

   public void EndArray() => writer.WriteEndArray();

   public void EndObject() => writer.WriteEndObject();

   public void Write(string value) => writer.WriteStringValue(value);

   public void Write(int value) => writer.WriteNumberValue(value);

   public void Write(double value) => writer.WriteNumberValue(value);

   public void Write(bool value) => writer.WriteBooleanValue(value);

   public void Write(DateTime value, bool zulu = false)
   {
      if (zulu)
      {
         writer.WriteStringValue(value.Zulu());
      }
      else
      {
         writer.WriteStringValue(value);
      }
   }

   public void Write(Guid value) => writer.WriteStringValue(value);

   public void WritePropertyNameIf(Maybe<string> _propertyName)
   {
      if (_propertyName is (true, var propertyName))
      {
         writer.WritePropertyName(propertyName);
      }
   }

   public void Write(string propertyName, string value) => writer.WriteString(propertyName, value);

   public void Write(string propertyName, int value) => writer.WriteNumber(propertyName, value);

   public void Write(string propertyName, double value) => writer.WriteNumber(propertyName, value);

   public void Write(string propertyName, bool value) => writer.WriteBoolean(propertyName, value);

   public void Write(string propertyName, DateTime value) => writer.WriteString(propertyName, value);

   public void Write(string propertyName, Guid value) => writer.WriteString(propertyName, value);

   public void Write(string propertyName, byte[] value) => writer.WriteString(propertyName, value.ToBase64());

   public void Write(string propertyName, string[] values)
   {
      //writer.WritePropertyName(propertyName);
      //JsonSerializer.Serialize(writer, values);
      writer.WriteStartArray(propertyName);
      foreach (var value in values)
      {
         writer.WriteStringValue(value);
      }
      writer.WriteEndArray();
   }

   public void Write<T>(string propertyName, T[] values)
   {
      var stringArray = values.Select(o => o.ToNonNullString()).ToArray();
      Write(propertyName, stringArray);
   }

   public void WriteNull(string propertyName) => writer.WriteNull(propertyName);

   public override string ToString()
   {
      writer.Flush();
      return Encoding.UTF8.GetString(stream.ToArray());
   }

   public void Dispose() => stream?.Dispose();
}