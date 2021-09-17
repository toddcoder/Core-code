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
using static Core.Monads.MonadFunctions;

namespace Core.Configurations
{
   public class Group : IConfigurationItem, IHash<string, IConfigurationItem>, IEnumerable<IConfigurationItem>
   {
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

      protected StringHash<IConfigurationItem> items;

      public Group(string key)
      {
         Key = key;

         items = new StringHash<IConfigurationItem>(true);
      }

      public string Key { get; }

      public IConfigurationItem this[string key]
      {
         get => items[key];
         set => items[key] = value;
      }

      public Maybe<string> GetValue(string key) => items.Map(key).Map(i => i.GetValue(key));

      public string ValueAt(string key) => GetValue(key).Required($"Couldn't find value '{key}'");

      public string[] GetArray(string key) => GetValue(key).Map(s => s.Split("/s* ',' /s*; f")).DefaultTo(() => new[] { key });

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

      public Result<Hash<string, IConfigurationItem>> AnyHash() => items.AsHash;

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

      public string ToString(int indent, bool ignoreSelf = false)
      {
         string indentation() => " ".Repeat(indent * 3);

         using var writer = new StringWriter();

         if (!ignoreSelf)
         {
            writer.WriteLine($"{indentation()}{Key} [");
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
            if (getConversion(elementType, item).If(out var objValue))
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
            var configuration = new Configuration(group);
            if (configuration.Deserialize(elementType).If(out var element))
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
            return source.ToInt().Some<object>();
         }
         else if (type == typeof(long))
         {
            return source.ToLong().Some<object>();
         }
         else if (type == typeof(float))
         {
            return source.ToFloat().Some<object>();
         }
         else if (type == typeof(double))
         {
            return source.ToDouble().Some<object>();
         }
         else if (type == typeof(bool))
         {
            return source.Same("true").Some<object>();
         }
         else if (type == typeof(DateTime))
         {
            return DateTime.Parse(source).Some<object>();
         }
         else if (type == typeof(Guid))
         {
            return Guid.Parse(source).Some<object>();
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
               var strings = source.Split("/s* ',' /s*; f");
               return makeArray(elementType, strings);
            }
            else
            {
               if (FromString(source).If(out var arrayGroup))
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
         else if (FromString(source).If(out var configuration))
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

      public static Result<Group> Serialize<T>(T obj, string name) where T : class, new() => tryTo(() => Serialize(typeof(T), obj, name));

      public static Result<Group> Serialize(Type type, object obj, string name)
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
                        group[key] = new Item(key, toString(value, propertyType));
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

                           group[key] = new Item(key, list.ToString(", "));
                        }
                        else
                        {
                           var arrayGroup = new Group(key);
                           for (var i = 0; i < array.Length; i++)
                           {
                              if (Serialize(elementType, array.GetValue(i), $"${i}").If(out var elementGroup, out var exception))
                              {
                                 arrayGroup[$"${i}"] = elementGroup;
                              }
                              else
                              {
                                 return exception;
                              }
                           }

                           group[key] = arrayGroup;
                        }
                     }
                     else
                     {
                        if (Serialize(propertyType, value, key).If(out var propertyGroup, out var exception))
                        {
                           group[key] = propertyGroup;
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

         return makeArray(type, array).Required($"Couldn't make array of element type {elementType.FullName}");
      }

      public Result<object> Deserialize(Type type)
      {
         try
         {
            var obj = getObject(type);
            var allPropertyInfo = getPropertyInfo(type);
            foreach (var (key, value) in Values())
            {
               var name = key.ToPascal();
               if (allPropertyInfo.FirstOrNone(p => p.Name.Same(name)).If(out var propertyInfo))
               {
                  var propertyType = propertyInfo.PropertyType;
                  if (getConversion(propertyType, value).If(out var objValue))
                  {
                     propertyInfo.SetValue(obj, objValue);
                  }
               }
            }

            foreach (var (key, group) in Groups())
            {
               var name = key.ToPascal();
               if (allPropertyInfo.FirstOrNone(p => p.Name.Same(name)).If(out var propertyInfo))
               {
                  var propertyType = propertyInfo.PropertyType;
                  var configuration = new Configuration(group);
                  if (configuration.Deserialize(propertyType).If(out var objValue))
                  {
                     propertyInfo.SetValue(obj, objValue);
                  }
               }
            }

            return obj;
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
            from cast in obj.CastAs<T>()
            select cast;
      }
   }
}