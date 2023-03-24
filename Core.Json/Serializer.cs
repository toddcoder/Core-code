using System;
using Core.Configurations;
using Core.Dates;
using Core.Matching;
using Core.Monads;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Json;

public class Serializer
{
   protected Setting setting;

   public Serializer(Setting setting)
   {
      this.setting = setting;
   }

   public Optional<string> Serialize()
   {
      try
      {
         using var writer = new JsonWriter();
         writer.BeginObject();
         foreach (var item in setting)
         {
            writeItem(writer, item);
         }

         writer.EndObject();

         return writer.ToString();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected static void writeItem(JsonWriter writer, ConfigurationItem configurationItem)
   {
      switch (configurationItem)
      {
         case Item { IsNull: true } nullItem:
            writeNull(writer, nullItem);
            break;
         case Item item:
            write(writer, item);
            break;
         case Setting { IsArray: true } arraySetting:
            writeArray(writer, arraySetting);
            break;
         case Setting objectSetting:
            writeObject(writer, objectSetting);
            break;
      }
   }

   protected static bool isGeneratedKey(string key) => key.IsMatch("^ '$' /d+ $; f");

   protected static Optional<string> getName(ConfigurationItem item) => maybe<string>() & !isGeneratedKey(item.Key) & (() => item.Key);

   protected static void writeText(JsonWriter writer, Optional<string> _name, string text)
   {
      var _int = lazy.maybe<int>();
      var _float = lazy.maybe<double>();
      var _bool = lazy.maybe<bool>();
      var _dateTime = lazy.maybe<DateTime>();

      writer.WritePropertyNameIf(_name);

      if (_int.ValueOf(Maybe.Int32(text)) is (true, var @int))
      {
         writer.Write(@int);
      }
      else if (_float.ValueOf(Maybe.Double(text)) is (true, var @float))
      {
         writer.Write(@float);
      }
      else if (_bool.ValueOf(Maybe.Boolean(text)) is (true, var @bool))
      {
         writer.Write(@bool);
      }
      else if (_dateTime.ValueOf(Maybe.DateTime(text)) is (true, var dateTime))
      {
         var zulu = dateTime.Zulu();
         writer.Write(zulu);
      }
      else
      {
         writer.Write(text);
      }
   }

   protected static void write(JsonWriter writer, Item item)
   {
      var _name = getName(item);
      writeText(writer, _name, item.Text);
   }

   protected static void writeNull(JsonWriter writer, Item item) => writer.WriteNull(item.Key);

   protected static void writeArray(JsonWriter writer, Setting setting)
   {
      var _name = getName(setting);
      writer.WritePropertyNameIf(_name);
      writer.BeginArray();

      foreach (var item in setting)
      {
         writeItem(writer, item);
      }

      writer.EndArray();
   }

   protected static void writeObject(JsonWriter writer, Setting setting)
   {
      var _name = getName(setting);
      writer.WritePropertyNameIf(_name);
      writer.BeginObject();

      foreach (var item in setting)
      {
         writeItem(writer, item);
      }

      writer.EndObject();
   }
}