using System;
using System.Reflection;
using Core.Monads;
using Core.Strings;
using static Core.Monads.AttemptFunctions;

namespace Core.Objects
{
   public static class Instance
   {
      public static T Create<T>() => Create<T>(null);

      public static IResult<T> TryCreate<T>() => tryTo(Create<T>);

      public static T Create<T>(params object[] args) => (T)typeof(T).Create(args);

      public static IResult<T> TryCreate<T>(params object[] args) => tryTo(() => Create<T>(args));

      public static object Create(this Type type) => type.Create(null);

      public static IResult<object> TryCreate(this Type type) => tryTo(() => Create(type));

      public static IResult<T> TryCreate<T>(this Type type)
         where T : new()
      {
         return
            from obj in tryTo(type.Create)
            from cast in obj.CastAs<T>()
            select cast;
      }

      public static object Create(this Type type, params object[] args)
      {
         if (type.IsPrimitive || type.FullName == "System.DateTime")
         {
            args[0] = args[0].ToNonNullString();
            return type.InvokeMember("Parse", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,
               null, null, args, null, null, null);
         }
         else
         {
            switch (type.FullName)
            {
               case "System.String":
                  args[0] = args[0].ToNonNullString().ToCharArray();
                  break;
               case "System.DBNull":
               case "System.Empty":
                  return type.InvokeMember("Value", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public,
                     null, null, null, null, null, null);
            }

            return Activator.CreateInstance(type, BindingFlags.CreateInstance, null, args, null);
         }
      }

      public static IResult<object> TryCreate(this Type type, params object[] args) => tryTo(() => Create(type, args));

      public static IResult<T> TryCreate<T>(this Type type, params object[] args) =>
         from obj in tryTo(() => type.Create(args))
         from cast in obj.CastAs<T>()
         select cast;

      public static object Create(this string typeName) => Type.GetType(typeName, true, true).Create();

      public static IResult<object> TryCreate(this string typeName) => tryTo(() => Create(typeName));

      public static IResult<T> TryCreate<T>(this string typeName)
         where T : new() =>
         from obj in tryTo(typeName.Create)
         from cast in obj.CastAs<T>()
         select cast;

      public static object Create(this string typeName, params object[] args) => Type.GetType(typeName, true, true).Create(args);

      public static IResult<object> TryCreate(this string typeName, params object[] args) => tryTo(() => Create(typeName, args));

      public static IResult<T> TryCreate<T>(this string typeName, params object[] args) =>
         from obj in tryTo(() => typeName.Create(args))
         from cast in obj.CastAs<T>()
         select cast;
   }
}