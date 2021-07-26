using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Arrays;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Enumerables;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Matching.MatchingExtensions;

namespace Core.Configurations
{
   public class Configuration : IHash<string, IConfigurationItem>, IConfigurationItem
   {
      public static implicit operator Configuration(string source)
      {
         var parser = new Parser(source);
         return parser.Parse().ForceValue();
      }

      public static Result<Configuration> FromString(string source)
      {
         var parser = new Parser(source);
         return parser.Parse();
      }

      protected static Set<Type> baseTypes;

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
               return none<object>();
            }
         }

         return newArray.Some<object>();
      }

      protected static Maybe<object> getConversion(Type type, string source)
      {
         string sourceWithoutQuotes()
         {
            var withoutQuotes = source.StartsWith(@"""") && source.EndsWith(@"""") ? source.Drop(1).Drop(-1) : source;
            var unescaped = withoutQuotes.ReplaceAll(("/tb/", "\t"), ("/cr/", "\r"), ("/lf/", "\n"), ("/bs/", "\\"));
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
               return makeArray(elementType, strings).Some();
            }
            else
            {
               if (FromString(source).If(out var arrayConfiguration))
               {
                  var root = arrayConfiguration.root;
                  var groups = root.Groups().Select(t => t.group).ToArray();
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
            var escaped = text.ReplaceAll(("\t", "/tb/"), ("\r", "/cr/"), ("\n", "/lf/"), ("\\", "/bs/"));
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

      static Configuration()
      {
         baseTypes = new Set<Type>
         {
            typeof(string), typeof(int), typeof(long), typeof(float), typeof(double), typeof(bool), typeof(DateTime), typeof(Guid), typeof(FileName),
            typeof(FolderName), typeof(byte[])
         };
      }

      protected static PropertyInfo[] getPropertyInfo(Type type)
      {
         return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty);
      }

      public static Result<Configuration> Serialize(Type type, object obj, string name)
      {
         if (type.IsValueType)
         {
            return $"Type provided ({type.FullName}) is a value type. Only classes are allowed".Failure<Configuration>();
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
                              if (Serialize(elementType, array.GetValue(i), $"${i}").If(out var elementConfiguration, out var exception))
                              {
                                 arrayGroup[$"${i}"] = elementConfiguration.root;
                              }
                              else
                              {
                                 return failure<Configuration>(exception);
                              }
                           }

                           group[key] = arrayGroup;
                        }
                     }
                     else
                     {
                        if (Serialize(propertyType, value, key).If(out var propertyConfiguration, out var exception))
                        {
                           group[key] = propertyConfiguration;
                        }
                        else
                        {
                           return failure<Configuration>(exception);
                        }
                     }
                  }
               }

               return new Configuration(group).Success();
            }
            catch (Exception exception)
            {
               return failure<Configuration>(exception);
            }
         }
      }

      public static Result<Configuration> Serialize<T>(T obj, string name) where T : class, new() => tryTo(() => Serialize(typeof(T), obj, name));

      protected Group root;

      internal Configuration(Group root)
      {
         this.root = root;
      }

      public string Key => root.Key;

      public IConfigurationItem this[string key] => root[key];

      public Maybe<string> GetValue(string key) => root.GetValue(key);

      public Result<string> RequireValue(string key) => root.RequireValue(key);

      public Maybe<Group> GetGroup(string key) => root.GetGroup(key);

      public Result<Group> RequireGroup(string key) => root.RequireGroup(key);

      public bool ContainsKey(string key) => root.ContainsKey(key);

      public Result<Hash<string, IConfigurationItem>> AnyHash() => root.AnyHash();

      protected static object getObject(Type type, Group group)
      {
         if (type.IsArray)
         {
            var elementType = type.GetElementType();
            var groups = group.Groups().Select(t => t.group).ToArray();

            return makeArray(elementType, groups).Required($"Couldn't make array of element type {elementType.FullName}");
         }
         else
         {
            return Activator.CreateInstance(type);
         }
      }

      public Result<object> Deserialize(Type type)
      {
         try
         {
            var obj = getObject(type, root);
            var allPropertyInfo = getPropertyInfo(type);
            foreach (var (key, value) in root.Values())
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

            foreach (var (key, group) in root.Groups())
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

            return obj.Success();
         }
         catch (Exception exception)
         {
            return failure<object>(exception);
         }
      }

      public Result<T> Deserialize<T>() where T : class, new()
      {
         return
            from obj in tryTo(() => Deserialize(typeof(T)))
            from cast in obj.CastAs<T>()
            select cast;
      }

      public override string ToString() => root.ToString(true);

      public Result<StringHash> ToStringHash()
      {
         try
         {
            return root.Values().ToHash(t => t.key, t => t.value).ToStringHash(true).Success();
         }
         catch (Exception exception)
         {
            return failure<StringHash>(exception);
         }
      }

      public IEnumerable<(string key, string value)> Values() => root.Values();

      public IEnumerable<(string key, Group group)> Groups() => root.Groups();
   }
}