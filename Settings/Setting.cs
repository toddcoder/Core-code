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
using Core.Configurations;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using Group = Core.Configurations.Group;

namespace Core.Settings
{
   public class Setting : IEnumerable<Setting>
   {
      public const string ROOT_KEY = "__$root";

      protected static Set<Type> baseTypes;

      static Setting()
      {
         baseTypes = new Set<Type>
         {
            typeof(string), typeof(int), typeof(long), typeof(float), typeof(double), typeof(bool), typeof(DateTime), typeof(Guid), typeof(FileName),
            typeof(FolderName), typeof(byte[])
         };
      }

      public static Result<Setting> FromString(string source)
      {
         var parser = new Parser(source);
         return parser.Parse();
      }

      public static implicit operator Setting(string source) => FromString(source).ForceValue();

      protected string key;
      protected Either<string, StringHash<Setting>> _value;

      internal Setting(string key, Either<string, StringHash<Setting>> value)
      {
         this.key = key;

         _value = value;
         IsGeneratedKey = this.key.StartsWith("__$key");
      }

      public string Key => key;

      public Either<string, StringHash<Setting>> Value => _value;

      public bool IsArray { get; set; }

      public bool IsGeneratedKey { get; set; }

      public IEnumerator<Setting> GetEnumerator()
      {
         if (_value.IfRight(out var subSettings))
         {
            foreach (var subSetting in subSettings.Values)
            {
               yield return subSetting;
            }
         }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public SettingMaybe Maybe => new(this);

      protected void writeLeft(StringWriter writer, string text, int indentation)
      {
         var value = text.ReplaceAll(("\t", @"`t"), ("\r", @"`r"), ("\n", @"`n"));
         if (value.IsEmpty())
         {
            writer.WriteLine($"{Key}: \"{value}\"");
         }
         else if (IsArray)
         {
            var destringifier = DelimitedText.AsSql();
            var destringified = destringifier.Destringify(value);
            var array = destringified.Unjoin("/s* ',' /s*; f").Select(i => destringifier.Restringify(i, RestringifyQuotes.DoubleQuote));

            writer.WriteLine($"{Key}: {{");
            var repeat = " ".Repeat((indentation + 1) * 3);
            foreach (var item in array)
            {
               writer.WriteLine($"{repeat}{item}");
            }

            writer.WriteLine("}");
         }
         else if (value.StartsWith(@"""") && value.EndsWith(@""""))
         {
            var innerValue = value.Drop(1).Drop(-1).Replace(@"""", @"\""");
            writer.WriteLine($"{Key}: \"{innerValue}\"");
         }
         else
         {
            writer.WriteLine($"{Key}: {value}");
         }
      }

      public string ToString(int indent, bool ignoreSelf = false)
      {
         string indentation() => " ".Repeat(indent * 3);

         using var writer = new StringWriter();

         if (_value.Maybe.Left.Map(out var left))
         {
            writer.Write(indentation());
            writeLeft(writer, left, indent);
         }
         else if (_value.Maybe.Right.Map(out var right))
         {
            if (!ignoreSelf)
            {
               if (IsGeneratedKey)
               {
                  writer.WriteLine($"{indentation()}[");
               }
               else
               {
                  writer.WriteLine($"{indentation()}{Key} [");
               }

               indent++;
            }

            foreach (var (_, setting) in right)
            {
               writer.Write(setting.ToString(indent));
            }

            if (!ignoreSelf)
            {
               indent--;
               writer.WriteLine($"{indentation()}]");
            }
         }

         return writer.ToString();
      }

      public string ToString(bool ignoreSelf) => ToString(0, ignoreSelf);

      public override string ToString() => ToString(true);

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

      protected static Maybe<object> makeArray(Type elementType, Setting[] settings)
      {
         var list = new List<object>();
         foreach (var setting in settings)
         {
            var _element = setting.Deserialize(elementType);
            if (_element.Map(out var element))
            {
               list.Add(element);
            }
            else
            {
               return nil;
            }
         }

         return list.ToArray();
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
            var elementType = type.GetElementType();
            if (isBaseType(elementType))
            {
               var strings = source.Unjoin("/s* ',' /s*; f");
               return makeArray(elementType, strings);
            }
            else
            {
               if (FromString(source).Map(out var arraySetting))
               {
                  var groups = arraySetting.ToArray();
                  return makeArray(elementType, groups);
               }
               else
               {
                  return nil;
               }
            }
         }
         else if (FromString(source).Map(out var configuration))
         {
            return configuration.Deserialize(type).Maybe();
         }
         else
         {
            return nil;
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

      public static Result<Setting> Serialize<T>(T obj, string name = ROOT_KEY) where T : class, new()
      {
         try
         {
            return Serialize(typeof(T), obj, name);
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public static Result<Setting> Serialize(Type type, object obj, string name = ROOT_KEY)
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

               Maybe<Setting> _setting = nil;

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
                        _setting = new Setting(key, toString(value, propertyType));
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
                           _setting = new Setting(key, list.ToString(", ")) { IsArray = true };
                        }
                        else
                        {
                           //var arraySetting = new Group(key);
                           Maybe<Setting> _arraySetting = nil;
                           for (var i = 0; i < array.Length; i++)
                           {
                              var generatedKey = Parser.GenerateKey();
                              if (Serialize(elementType, array.GetValue(i), generatedKey).Map(out var elementSetting, out var exception))
                              {
                                 //arraySetting.SetItem(generatedKey, elementGroup);
                                 _arraySetting = new Setting(generatedKey, elementSetting);
                              }
                              else
                              {
                                 return exception;
                              }
                           }

                           setting.SetItem(key, arraySetting);
                        }
                     }
                     else
                     {
                        if (Serialize(propertyType, value, key).Map(out var propertyGroup, out var exception))
                        {
                           setting.SetItem(key, propertyGroup);
                        }
                        else
                        {
                           return exception;
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
            from cast in ConversionFunctions.Result.Cast<T>(obj)
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