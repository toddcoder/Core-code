using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Arrays;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;

namespace Core.Configurations;

public class Setting : ConfigurationItem, IHash<string, string>, IEnumerable<ConfigurationItem>, IConfigurationItemGetter
{
   public const string ROOT_NAME = "_$root";

   public static Configuration operator +(Setting setting, FileName file) => new(file, setting.items, setting.Key);

   protected static Set<Type> baseTypes;

   static Setting()
   {
      baseTypes = new Set<Type>
      {
         typeof(string), typeof(int), typeof(long), typeof(float), typeof(double), typeof(bool), typeof(DateTime), typeof(Guid), typeof(FileName),
         typeof(FolderName), typeof(byte[])
      };
   }

   public static implicit operator Setting(string source) => FromString(source).ForceValue();

   public static Result<Setting> FromString(string source)
   {
      var parser = new Parser(source);
      return parser.Parse();
   }

   protected bool isGeneratedKey;
   internal StringHash<ConfigurationItem> items;

   public Setting(string key = ROOT_NAME)
   {
      Key = key;
      isGeneratedKey = key.StartsWith("__$key");

      items = new StringHash<ConfigurationItem>(true);
   }

   public override string Key { get; }

   public bool IsArray { get; set; }

   Maybe<Setting> IConfigurationItemGetter.GetSetting(string key)
   {
      var _configurationItem = items.Maybe[key];
      if (_configurationItem && _configurationItem.Value is Setting setting)
      {
         return setting;
      }
      else
      {
         return nil;
      }
   }

   Maybe<Item> IConfigurationItemGetter.GetItem(string key)
   {
      var _configurationItem = items.Maybe[key];
      if (_configurationItem && _configurationItem.Value is Item item)
      {
         return item;
      }
      else
      {
         return nil;
      }
   }

   public override void SetItem(string key, ConfigurationItem item) => items[key] = item;

   public bool IsGeneratedKey => isGeneratedKey;

   public string this[string key]
   {
      get => Value.String(key);
      set => items[key] = new Item(key, value);
   }

   public bool ContainsKey(string key) => items.ContainsKey(key);

   public Result<Hash<string, string>> AnyHash() => items.ToStringHash(i => i.Key, i => i.Value.ToString(), true);

   public StringHash ToStringHash() => Items().ToHash(t => t.key, t => t.text).ToStringHash(true);

   public override IEnumerable<(string key, string text)> Items()
   {
      foreach (var item in items.Where(i => i.Value is Item).Select(i => (Item)i.Value))
      {
         yield return (item.Key, item.Text);
      }
   }

   public override IEnumerable<(string key, Setting setting)> Settings()
   {
      foreach (var (key, item) in items.Where(i => i.Value is Setting))
      {
         yield return (key, (Setting)item);
      }
   }

   public override int Count => items.Count;

   public string ToString(int indent, bool ignoreSelf = false)
   {
      string indentation() => " ".Repeat(indent * 3);

      using var writer = new StringWriter();

      if (!ignoreSelf)
      {
         if (isGeneratedKey)
         {
            writer.WriteLine($"{indentation()}[");
         }
         else
         {
            writer.WriteLine($"{indentation()}{Key} [");
         }

         indent++;
      }

      foreach (var (_, value) in items)
      {
         switch (value)
         {
            case Setting setting:
               writer.Write(setting.ToString(indent));
               break;
            case Item item:
               item.Indentation = indent;
               writer.WriteLine($"{indentation()}{item}");
               break;
         }
      }

      if (!ignoreSelf)
      {
         indent--;
         writer.WriteLine($"{indentation()}]");
      }

      return writer.ToString();
   }

   public string ToString(bool ignoreSelf) => ToString(0, ignoreSelf);

   public override string ToString() => ToString(true);

   public IEnumerator<ConfigurationItem> GetEnumerator() => items.Values.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   protected static bool isBaseType(Type type) => baseTypes.Contains(type) || type.IsEnum;

   protected static object makeArray(Type elementType, string[] sourceArray)
   {
      var length = sourceArray.Length;
      var newArray = Array.CreateInstance(elementType, length);
      for (var i = 0; i < length; i++)
      {
         var item = sourceArray[i];
         var _object = getConversion(elementType, item);
         if (_object)
         {
            newArray.SetValue(_object.Value, i);
         }
      }

      return newArray;
   }

   protected static Maybe<object> makeArray(Type elementType, Setting[] settings)
   {
      var length = settings.Length;
      var newArray = Array.CreateInstance(elementType, length);
      for (var i = 0; i < length; i++)
      {
         var setting = settings[i];
         var _element = setting.Deserialize(elementType);
         if (_element)
         {
            newArray.SetValue(_element.Value, i);
         }
         else
         {
            return nil;
         }
      }

      return newArray;
   }

   protected static Maybe<object> getConversion(Type type, string source)
   {
      string sourceWithoutQuotes()
      {
         var withoutQuotes = source.StartsWith(@"""") && source.EndsWith(@"""") ? source.Drop(1).Drop(-1) : source;
         var unescaped = withoutQuotes.ReplaceAll(("`t", "\t"), ("`r", "\r"), ("`", "\n"), ("``", "`"));

         return unescaped;
      }

      if (type == typeof(string))
      {
         return sourceWithoutQuotes().Some<object>();
      }
      else if (type == typeof(int))
      {
         return ConversionFunctions.Value.Int32(source).Some<object>();
      }
      else if (type == typeof(long))
      {
         return ConversionFunctions.Value.Int64(source).Some<object>();
      }
      else if (type == typeof(float))
      {
         return ConversionFunctions.Value.Single(source).Some<object>();
      }
      else if (type == typeof(double))
      {
         return ConversionFunctions.Value.Double(source).Some<object>();
      }
      else if (type == typeof(bool))
      {
         return source.Same("true").Some<object>();
      }
      else if (type == typeof(DateTime))
      {
         return ConversionFunctions.Value.DateTime(source).Some<object>();
      }
      else if (type == typeof(Guid))
      {
         return ConversionFunctions.Value.Guid(source).Some<object>();
      }
      else if (type == typeof(FileName))
      {
         return new FileName(sourceWithoutQuotes()).Some<object>();
      }
      else if (type == typeof(FolderName))
      {
         return new FolderName(sourceWithoutQuotes()).Some<object>();
      }
      else if (type == typeof(byte[]))
      {
         return source.FromBase64().Some<object>();
      }
      else if (type.IsEnum)
      {
         return source.ToBaseEnumeration(type).Some<object>();
      }
      else if (type.IsArray)
      {
         var _arraySetting = lazy.result<Setting>();
         var elementType = type.GetElementType();
         if (isBaseType(elementType))
         {
            var strings = source.Unjoin("/s* ',' /s*; f");
            return makeArray(elementType, strings);
         }
         else if (_arraySetting.ValueOf(FromString(source)))
         {
            var settings = _arraySetting.Value.Settings().Select(t => t.setting).ToArray();
            return makeArray(elementType, settings);
         }
         else
         {
            return nil;
         }
      }
      else
      {
         return FromString(source).Map(setting => setting.Deserialize(type).Maybe());
      }
   }

   protected static string toString(object obj, Type type)
   {
      static string encloseInQuotes(string text)
      {
         var escaped = text.ReplaceAll(("`", "``"), ("\t", "`t"), ("\r", "`r"), ("\n", "`n"));
         return $"\"{escaped}\"";
      }

      if (type == typeof(byte[]))
      {
         return ((byte[])obj).ToBase64();
      }
      else if (type.IsArray)
      {
         var array = (Array)obj;
         var list = new List<string>();
         foreach (var item in array)
         {
            if (item is not null && isBaseType(item.GetType()))
            {
               list.Add(item.ToString());
            }
         }

         return list.ToString(", ");
      }
      else if (type == typeof(bool))
      {
         return obj.ToString().ToLower();
      }
      else if (type == typeof(string) || type == typeof(FileName) || type == typeof(FolderName))
      {
         return encloseInQuotes(obj.ToString());
      }
      else if (type.IsEnum)
      {
         return obj.ToString();
      }
      else
      {
         return obj.ToString();
      }
   }

   protected static PropertyInfo[] getPropertyInfo(Type type)
   {
      return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty);
   }

   public static Result<Setting> Serialize<T>(T obj, string name = ROOT_NAME) where T : class, new()
   {
      return tryTo(() => Serialize(typeof(T), obj, name));
   }

   public static Result<Setting> Serialize(Type type, object obj, string name = ROOT_NAME)
   {
      if (type.IsValueType)
      {
         return fail($"Type provided ({type.FullName}) is a value type. Only classes are allowed");
      }
      else
      {
         try
         {
            obj.Must().Not.BeNull().OrThrow();

            var setting = new Setting(name);

            var allPropertyInfo = getPropertyInfo(obj.GetType());
            foreach (var propertyInfo in allPropertyInfo)
            {
               var propertyType = propertyInfo.PropertyType;
               var key = propertyInfo.Name.ToCamel();
               var value = propertyInfo.GetValue(obj);
               if (value is not null)
               {
                  if (isBaseType(propertyType))
                  {
                     setting.SetItem(key, new Item(key, toString(value, propertyType)));
                  }
                  else if (value is Array array)
                  {
                     var elementType = propertyType.GetElementType();

                     if (isBaseType(elementType))
                     {
                        var list = new List<string>();
                        for (var i = 0; i < array.Length; i++)
                        {
                           list.Add(toString(array.GetValue(i), elementType));
                        }

                        var item = new Item(key, list.ToString(", "))
                        {
                           IsArray = true
                        };
                        setting.SetItem(key, item);
                     }
                     else
                     {
                        var arraySetting = new Setting(key);
                        for (var i = 0; i < array.Length; i++)
                        {
                           var generatedKey = Parser.GenerateKey();
                           var _elementSetting = Serialize(elementType, array.GetValue(i), generatedKey);
                           if (_elementSetting)
                           {
                              arraySetting.SetItem(generatedKey, _elementSetting);
                           }
                           else
                           {
                              return _elementSetting.Exception;
                           }
                        }

                        setting.SetItem(key, arraySetting);
                     }
                  }
                  else
                  {
                     var _propertySetting = Serialize(propertyType, value, key);
                     if (_propertySetting)
                     {
                        setting.SetItem(key, _propertySetting);
                     }
                     else
                     {
                        return _propertySetting.Exception;
                     }
                  }
               }
            }

            return setting;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
   }

   protected object getObject(Type type)
   {
      if (!type.IsArray)
      {
         return Activator.CreateInstance(type);
      }

      var elementType = type.GetElementType();
      var array = Settings().Select(i => i.setting).ToArray();

      return makeArray(elementType, array).Required($"Couldn't make array of element type {elementType.FullName}");
   }

   public Result<object> Deserialize(Type type)
   {
      try
      {
         var obj = getObject(type);
         return fill(ref obj, type).Map(_ => obj);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Result<T> Deserialize<T>() where T : class, new()
   {
      return
         from obj in tryTo(() => Deserialize(typeof(T)))
         from cast in ConversionFunctions.Result.Cast<T>(obj)
         select cast;
   }

   protected Result<Unit> fill(ref object obj, Type type)
   {
      try
      {
         var allPropertyInfo = getPropertyInfo(type);
         foreach (var (key, value) in Items())
         {
            var name = key.ToPascal();
            var _propertyInfo = allPropertyInfo.FirstOrNone(p => p.Name.Same(name));
            if (_propertyInfo)
            {
               var propertyInfo = _propertyInfo.Value;
               var propertyType = propertyInfo.PropertyType;
               var _object = getConversion(propertyType, value);
               if (_object)
               {
                  propertyInfo.SetValue(obj, _object.Value);
               }
            }
         }

         foreach (var (key, setting) in Settings())
         {
            var name = key.ToPascal();
            var _propertyInfo = allPropertyInfo.FirstOrNone(p => p.Name.Same(name));
            if (_propertyInfo)
            {
               var propertyType = _propertyInfo.Value.PropertyType;
               var _object = setting.Deserialize(propertyType);
               if (_object)
               {
                  _propertyInfo.Value.SetValue(obj, _object.Value);
               }
            }
         }

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Result<Unit> Fill(ref object obj)
   {
      if (obj is null)
      {
         return fail("Object may not be null");
      }
      else
      {
         var type = obj.GetType();
         return fill(ref obj, type);
      }
   }

   public void Clear() => items.Clear();

   public void Fill(StringHash stringHash)
   {
      foreach (var (key, value) in stringHash)
      {
         this[key] = value;
      }
   }

   public void Fill(IEnumerable<string> enumerable)
   {
      var index = 0;
      foreach (var item in enumerable)
      {
         this[$"${index++}"] = item;
      }
   }
}