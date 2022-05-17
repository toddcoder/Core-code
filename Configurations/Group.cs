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
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Configurations
{
   public class Group : IConfigurationItem, IHash<string, string>, IEnumerable<IConfigurationItem>
   {
      public const string ROOT_NAME = "_$root";

      public static Configuration operator +(Group group, FileName file) => new(file, group.items, group.Key);

      protected static Set<Type> baseTypes;

      static Group()
      {
         baseTypes = new Set<Type>
         {
            typeof(string), typeof(int), typeof(long), typeof(float), typeof(double), typeof(bool), typeof(DateTime), typeof(Guid), typeof(FileName),
            typeof(FolderName), typeof(byte[])
         };
      }

      public static implicit operator Group(string source) => FromString(source).ForceValue();

      public static Result<Group> FromString(string source)
      {
         var parser = new Parser(source);
         return parser.Parse();
      }

      protected bool isGeneratedKey;
      internal StringHash<IConfigurationItem> items;

      public Group(string key = ROOT_NAME)
      {
         Key = key;
         isGeneratedKey = Key.StartsWith("__$key");

         items = new StringHash<IConfigurationItem>(true);
      }

      public string Key { get; }

      public bool IsGeneratedKey => isGeneratedKey;

      public string this[string key]
      {
         get => ValueAt(key);
         set
         {
            if (value.StartsWith("["))
            {
               Group newGroup = $"{key}: {value}";
               items[key] = newGroup;
            }
            else
            {
               var item = new Item(key, value);
               items[key] = item;
            }
         }
      }

      public IConfigurationItem GetItem(string key) => items.Require(key).ForceValue();

      public Maybe<IConfigurationItem> GetSomeItem(string key) => items.Map(key);

      public bool If(string key, out IConfigurationItem item) => items.If(key, out item);

      public void SetItem(string key, IConfigurationItem item) => items[key] = item;

      public Maybe<string> GetValue(string key) => items.Map(key).Map(i => i.GetValue(key));

      public string ValueAt(string key) => GetValue(key).Required($"Couldn't find value '{key}'");

      public string[] GetArray(string key) => GetValue(key).Map(s => s.Unjoin("/s* ',' /s*; f")).DefaultTo(() => new[] { key });

      public Result<string> RequireValue(string key) => items.Require(key).Map(i => i.RequireValue(key));

      public string At(string key) => GetValue(key).DefaultTo(() => "");

      public Maybe<Group> GetGroup(string key)
      {
         if (items.If(key, out var item) && item is Group group)
         {
            return group;
         }
         else
         {
            return nil;
         }
      }

      public Group GroupAt(string key) => GetGroup(key).Required($"Couldn't find group at '{key}'");

      public Result<Group> RequireGroup(string key) => GetGroup(key).Result($"Key {key} not found");

      public bool ContainsKey(string key) => items.ContainsKey(key);

      public Result<Hash<string, string>> AnyHash() => items.ToStringHash(i => i.Key, i => i.Value.ToString(), true);

      public StringHash ToStringHash() => Values().ToHash(t => t.key, t => t.value).ToStringHash(true);

      public IEnumerable<(string key, string value)> Values()
      {
         foreach (var (key, item) in items.Where(i => i.Value is Item))
         {
            yield return (key, ((Item)item).Value);
         }
      }

      public IEnumerable<(string key, Group group)> Groups()
      {
         foreach (var (key, item) in items.Where(i => i.Value is Group))
         {
            yield return (key, (Group)item);
         }
      }

      public int Count => items.Count;

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
               case Group group:
                  writer.Write(group.ToString(indent));
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

      public IEnumerator<IConfigurationItem> GetEnumerator() => items.Values.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      protected static bool isBaseType(Type type) => baseTypes.Contains(type) || type.IsEnum;

      protected static object makeArray(Type elementType, string[] sourceArray)
      {
         var length = sourceArray.Length;
         var newArray = Array.CreateInstance(elementType, length);
         for (var i = 0; i < length; i++)
         {
            var item = sourceArray[i];
            if (getConversion(elementType, item).Map(out var objValue))
            {
               newArray.SetValue(objValue, i);
            }
         }

         return newArray;
      }

      protected static Maybe<object> makeArray(Type elementType, Group[] groups)
      {
         var length = groups.Length;
         var newArray = Array.CreateInstance(elementType, length);
         for (var i = 0; i < length; i++)
         {
            var group = groups[i];
            if (group.Deserialize(elementType).Map(out var element))
            {
               newArray.SetValue(element, i);
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
            return Value.Int32(source).Some<object>();
         }
         else if (type == typeof(long))
         {
            return Value.Int64(source).Some<object>();
         }
         else if (type == typeof(float))
         {
            return Value.Single(source).Some<object>();
         }
         else if (type == typeof(double))
         {
            return Value.Double(source).Some<object>();
         }
         else if (type == typeof(bool))
         {
            return source.Same("true").Some<object>();
         }
         else if (type == typeof(DateTime))
         {
            return Value.DateTime(source).Some<object>();
         }
         else if (type == typeof(Guid))
         {
            return Value.Guid(source).Some<object>();
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
            var elementType = type.GetElementType();
            if (isBaseType(elementType))
            {
               var strings = source.Unjoin("/s* ',' /s*; f");
               return makeArray(elementType, strings);
            }
            else
            {
               if (FromString(source).Map(out var arrayGroup))
               {
                  var groups = arrayGroup.Groups().Select(t => t.group).ToArray();
                  return makeArray(elementType, groups);
               }
               else
               {
                  return none<object>();
               }
            }
         }
         else if (FromString(source).Map(out var configuration))
         {
            return configuration.Deserialize(type).Maybe();
         }
         else
         {
            return none<object>();
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

      public static Result<Group> Serialize<T>(T obj, string name = ROOT_NAME) where T : class, new() => tryTo(() => Serialize(typeof(T), obj, name));

      public static Result<Group> Serialize(Type type, object obj, string name = ROOT_NAME)
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

               var group = new Group(name);

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
                        group.SetItem(key, new Item(key, toString(value, propertyType)));
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
                           group.SetItem(key, item);

                        }
                        else
                        {
                           var arrayGroup = new Group(key);
                           for (var i = 0; i < array.Length; i++)
                           {
                              var generatedKey = Parser.GenerateKey();
                              if (Serialize(elementType, array.GetValue(i), generatedKey).Map(out var elementGroup, out var exception))
                              {
                                 arrayGroup.SetItem(generatedKey, elementGroup);
                              }
                              else
                              {
                                 return exception;
                              }
                           }

                           group.SetItem(key, arrayGroup);
                        }
                     }
                     else
                     {
                        if (Serialize(propertyType, value, key).Map(out var propertyGroup, out var exception))
                        {
                           group.SetItem(key, propertyGroup);
                        }
                        else
                        {
                           return exception;
                        }
                     }
                  }
               }

               return group;
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
         var array = Groups().Select(i => i.group).ToArray();

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
            from cast in Result.Cast<T>(obj)
            select cast;
      }

      protected Result<Unit> fill(ref object obj, Type type)
      {
         try
         {
            var allPropertyInfo = getPropertyInfo(type);
            foreach (var (key, value) in Values())
            {
               var name = key.ToPascal();
               if (allPropertyInfo.FirstOrNone(p => p.Name.Same(name)).Map(out var propertyInfo))
               {
                  var propertyType = propertyInfo.PropertyType;
                  if (getConversion(propertyType, value).Map(out var objValue))
                  {
                     propertyInfo.SetValue(obj, objValue);
                  }
               }
            }

            foreach (var (key, group) in Groups())
            {
               var name = key.ToPascal();
               if (allPropertyInfo.FirstOrNone(p => p.Name.Same(name)).Map(out var propertyInfo))
               {
                  var propertyType = propertyInfo.PropertyType;
                  if (group.Deserialize(propertyType).Map(out var objValue))
                  {
                     propertyInfo.SetValue(obj, objValue);
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
   }
}