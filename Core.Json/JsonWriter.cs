using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Core.Json
{
   public class JsonWriter : IDisposable
   {
      protected bool initialObject;
      protected MemoryStream stream;
      protected Utf8JsonWriter writer;

      public JsonWriter(bool initialObject = true)
      {
         this.initialObject = initialObject;

         stream = new MemoryStream();
         writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

         if (this.initialObject)
         {
            BeginObject();
         }
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

      public void Write(DateTime value) => writer.WriteStringValue(value);

      public void Write(Guid value) => writer.WriteStringValue(value);

      public void Write(string propertyName, string value) => writer.WriteString(propertyName, value);

      public void Write(string propertyName, int value) => writer.WriteNumber(propertyName, value);

      public void Write(string propertyName, double value) => writer.WriteNumber(propertyName, value);

      public void Write(string propertyName, bool value) => writer.WriteBoolean(propertyName, value);

      public void Write(string propertyName, DateTime value) => writer.WriteString(propertyName, value);

      public void Write(string propertyName, Guid value) => writer.WriteString(propertyName, value);

      public void Write(string propertyName, byte[] value) => writer.WriteBase64String(propertyName, value);

      public void WriteNull(string propertyName) => writer.WriteNull(propertyName);

      public override string ToString()
      {
         if (initialObject)
         {
            writer.WriteEndObject();
         }

         writer.Flush();
         return Encoding.UTF8.GetString(stream.ToArray());
      }

      public void Dispose()
      {
         writer?.Dispose();
         stream?.Dispose();
      }
   }
}