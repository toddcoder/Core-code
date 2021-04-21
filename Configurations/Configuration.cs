using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;
using static Core.RegularExpressions.RegexExtensions;

namespace Core.Configurations
{
   public class Configuration : IHash<string, IConfigurationItem>, IConfigurationItem
   {
      protected static Set<Type> allowedTypes;

      protected static bool isAllowed(Type type) => allowedTypes.Contains(type) || type.IsEnum || type.IsArray;

      protected object makeArray(Type elementType, Array array)
      {
         var length = array.Length;
         var newArray = Array.CreateInstance(elementType, length);
         for (int i = 0; i < length; i++)
         {
            var item = array.GetValue(i);
            if (getConversion(elementType, item.ToString()).If(out var ))
         }
      }

      protected static IMaybe<object> getConversion(Type type, string source)
      {
         if (type == typeof(string))
         {
            return source.Some<object>();
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
            return new FileName(source).Some<object>();
         }
         else if (type == typeof(FolderName))
         {
            return new FolderName(source).Some<object>();
         }
         else if (type.IsEnum)
         {
            return source.ToBaseEnumeration(type).Some<object>();
         }
         else if (type.IsArray)
         {
            var elementType = type.GetElementType();
            return source.Split("/s* ',' /s*").Select(s => getConversion(elementType, s)).WhereIsSome().ToArray().Some<object>();
         }
         else
         {
            return none<object>();
         }
      }

      protected static string toString(object obj, Type type)
      {
         if (type.IsArray)
         {
            var array = (Array)obj;
            var list = new List<string>();
            foreach (var item in array)
            {
               if (item is not null && isAllowed(item.GetType()))
               {
                  list.Add(item.ToString());
               }
            }

            return list.ToString(", ");
         }
         else
         {
            return obj.ToString();
         }
      }

      static Configuration()
      {
         allowedTypes = new Set<Type>
         {
            typeof(string), typeof(int), typeof(long), typeof(float), typeof(double), typeof(DateTime), typeof(Guid), typeof(FileName),
            typeof(FolderName)
         };
      }

      protected static PropertyInfo[] getPropertyInfo(Type type)
      {
         return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty);
      }

      public static IResult<Configuration> Serialize<T>(T obj, string name) where T : class, new()
      {
         try
         {
            asObject(() => obj).Must().Not.BeNull().OrThrow();

            var group = new Group(name);

            var allPropertyInfo = getPropertyInfo(obj.GetType());
            foreach (var propertyInfo in allPropertyInfo)
            {
               var type = propertyInfo.PropertyType;
               if (isAllowed(type))
               {
                  var key = propertyInfo.Name.ToCamel();
                  var value = propertyInfo.GetValue(obj);
                  if (value is not null)
                  {
                     group[key] = new Item(key, toString(value, type));
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

      protected Group root;

      internal Configuration(Group root)
      {
         this.root = root;
      }

      public string Key => root.Key;

      public IConfigurationItem this[string key] => root[key];

      public IMaybe<string> GetValue(string key) => root.GetValue(key);

      public IResult<string> RequireValue(string key) => root.RequireValue(key);

      public IMaybe<Group> GetGroup(string key) => root.GetGroup(key);

      public IResult<Group> RequireGroup(string key) => root.RequireGroup(key);

      public bool ContainsKey(string key) => root.ContainsKey(key);

      public IResult<Hash<string, IConfigurationItem>> AnyHash() => root.AnyHash();

      public IResult<T> Deserialize<T>() where T : class, new()
      {
         try
         {
            var obj = new T();
            var allPropertyInfo = getPropertyInfo(obj.GetType());
            foreach (var (key, value) in root.Values())
            {
               var name = key.ToPascal();
               if (allPropertyInfo.FirstOrNone(p => p.Name.Same(name)).If(out var propertyInfo))
               {
                  var type = propertyInfo.PropertyType;
                  if (getConversion(type, value).If(out var objValue))
                  {
                     propertyInfo.SetValue(obj, objValue);
                  }
               }
            }

            return obj.Success();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public override string ToString() => root.ToString(true);
   }
}