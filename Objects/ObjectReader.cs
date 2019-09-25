using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Core.Collections;
using Core.Monads;
using static System.Reflection.BindingFlags;
using static System.Reflection.MemberTypes;
using static Core.Monads.AttemptFunctions;

namespace Core.Objects
{
   public class ObjectReader
   {
      public static IResult<ObjectReader> ReadObject(object obj) =>
         from nonNull in assert(obj != null, () => obj, () => "Object is null")
         from type in obj.GetType().Success()
         from values in getValues(obj, type)
         select new ObjectReader(values);

      static IResult<Hash<string, object>> getValues(object obj, Type type) =>
         from members in getMembers(type)
         from values in getValues(obj, members)
         select values;

      static IResult<IEnumerable<MemberInfo>> getMembers(Type type)
      {
         const MemberTypes memberTypes = Field | Property;
         const BindingFlags bindingFlags = BindingFlags.Instance | GetField | GetProperty | NonPublic | Public;

         return
            from members in type.GetMembers(bindingFlags).Where(member => (member.MemberType & memberTypes) != 0).Success()
            select members;
      }

      static IResult<object> getValue(object obj, MemberInfo memberInfo)
      {
         switch (memberInfo)
         {
            case FieldInfo fieldInfo:
               return fieldInfo.GetValue(obj).Success();
            case PropertyInfo propertyInfo:
               return propertyInfo.GetValue(obj).Success();
            default:
               return $"{memberInfo.Name} is neither a field nor a property".Failure<object>();
         }
      }

      static IResult<Hash<string, object>> getValues(object obj, IEnumerable<MemberInfo> memberInfos)
      {
         var hash = new Hash<string, object>();

         foreach (var info in memberInfos)
         {
            if (getValue(obj, info).ValueOrResult<Hash<string, object>>(out var value, out var result))
            {
               hash[info.Name] = value;
            }
            else
            {
               return result;
            }
         }

         return hash.Success();
      }

      Hash<string, object> values;

      protected ObjectReader(Hash<string, object> values) => this.values = values;

      TResult invoke<TResult>(LambdaExpression expression)
      {
         var arguments = expression.Parameters
            .Select(p => p.Name)
            .Where(name => values.ContainsKey(name))
            .Select(name => values[name])
            .ToArray();

         return (TResult)expression.Compile().DynamicInvoke(arguments);
      }

      void _do(LambdaExpression expression)
      {
         var arguments = expression.Parameters
            .Select(p => p.Name)
            .Where(name => values.ContainsKey(name))
            .Select(name => values[name])
            .ToArray();

         expression.Compile().DynamicInvoke(arguments);
      }

      public TResult Invoke<T, TResult>(Expression<Func<T, TResult>> expression) => invoke<TResult>(expression);

      public TResult Invoke<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression)
      {
         return invoke<TResult>(expression);
      }

      public TResult Invoke<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> expression)
      {
         return invoke<TResult>(expression);
      }

      public TResult Invoke<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> expression)
      {
         return invoke<TResult>(expression);
      }

      public TResult Invoke<T1, T2, T3, T4, T5, TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> expression)
      {
         return invoke<TResult>(expression);
      }

      public TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6,
         TResult>> expression)
      {
         return invoke<TResult>(expression);
      }

      public void Do<T>(Expression<Action<T>> expression) => _do(expression);

      public void Do<T1, T2>(Expression<Action<T1, T2>> expression) => _do(expression);

      public void Do<T1, T2, T3>(Expression<Action<T1, T2, T3>> expression) => _do(expression);

      public void Do<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> expression) => _do(expression);

      public void Do<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> expression) => _do(expression);

      public void Do<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> expression) => _do(expression);

      public T Assign<T>(string name) => (T)values[name];

      public ObjectReaderTrying TryTo => new ObjectReaderTrying(this);
   }
}