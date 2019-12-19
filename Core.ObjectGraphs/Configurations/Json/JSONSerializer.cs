using System;
using System.Linq;
using System.Reflection;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Monads;
using Core.Objects;
using Core.Strings;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class JsonSerializer
   {
      public static IResult<JsonSerializer> New(object obj, string indentString = "   ")
      {
         return
            from nonNull in obj.MustAs(nameof(obj)).Not.BeNull().Try()
            select new JsonSerializer(nonNull, indentString);
      }

      object obj;
      JsonBuilder builder;
      string indentString;

      protected JsonSerializer(object obj, string indentString = "   ")
      {
         this.obj = obj;
         builder = new JsonBuilder();
         this.indentString = indentString;
      }

      public IResult<string> Serialize()
      {
         return serializeMembers(obj).Map(u => builder.Generate(indentString));
      }

      IResult<Unit> serializeObject(string name, object obj)
      {
         builder.BeginObject(name.ToLower1());
         var members = serializeMembers(obj);
         if (members.IsSuccessful)
         {
            builder.End();
         }

         return members;
      }

      IResult<Unit> serializeObject(object obj)
      {
         builder.BeginObject();
         var members = serializeMembers(obj);
         if (members.IsSuccessful)
         {
            builder.End();
         }

         return members;
      }

      IResult<Unit> serializeMembers(object obj)
      {
         var type = obj.GetType();
         var evaluator = (IHash<string, object>)new PropertyEvaluator(obj);
         var infos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

         foreach (var info in infos)
         {
            var signature = info.Name;
            if (evaluator.Map(signature).If(out var value) && serializeValue(signature, value).ValueOrOriginal(out _, out var original))
            {
               return original;
            }
         }

         return Unit.Success();
      }

      IResult<Unit> serializeValue(object value) => serializeValue("", value);

      IResult<Unit> serializeValue(string name, object value)
      {
         var lName = name.ToLower1();
         if (value.IsNull())
         {
            builder.Add(lName);
         }
         else
         {
            var type = value.GetType();
            switch (value)
            {
               case bool b:
                  builder.Add(lName, b);
                  break;
               case string s:
                  builder.Add(lName, s);
                  break;
               case int i:
                  builder.Add(lName, i);
                  break;
               case double d:
                  builder.Add(lName, d);
                  break;
               case byte[] ba:
                  builder.Add(lName, ba);
                  break;
               case FileName file:
                  builder.Add(lName, file);
                  break;
               case FolderName folder:
                  builder.Add(lName, folder);
                  break;
               default:
                  if (type.IsEnum)
                  {
                     builder.Add(lName, (Enum)value);
                  }
                  else
                  {
                     switch (value)
                     {
                        case Guid g:
                           builder.Add(lName, g);
                           break;
                        case TimeSpan ts:
                           builder.Add(lName, ts);
                           break;
                        default:
                           if (type.IsArray)
                           {
                              var objArray = (Array)value;
                              builder.BeginArray(lName);

                              foreach (var result in objArray.Cast<object>().Select(serializeValue).Where(result => result.IsFailed))
                              {
                                 return result;
                              }

                              builder.End();
                           }
                           else
                           {
                              var result = serializeObject(lName, value);
                              if (result.IsFailed)
                              {
                                 return result;
                              }
                           }

                           break;
                     }
                  }

                  break;
            }
         }

         return Unit.Success();
      }

      public override string ToString() => builder.Generate(indentString);
   }
}