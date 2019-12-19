using System;
using System.Reflection;
using Core.Assertions;
using Core.Monads;

namespace Core.Objects
{
   public class Invoker
   {
      static BindingFlags baseBindings = BindingFlags.Public | BindingFlags.Instance;
      static BindingFlags methodBindings = baseBindings | BindingFlags.InvokeMethod;
      static BindingFlags getPropertyBindings = baseBindings | BindingFlags.GetProperty;
      static BindingFlags setPropertyBindings = baseBindings | BindingFlags.SetProperty;
      static BindingFlags getFieldBindings = baseBindings | BindingFlags.GetField;
      static BindingFlags setFieldBindings = baseBindings | BindingFlags.SetField;

      public static IResult<Invoker> From(object obj) =>
         from nonNull in obj.MustAs(nameof(obj)).Not.BeNull().Try()
         from type in nonNull.GetType().Success()
         select new Invoker(nonNull, type);

      object obj;
      Type type;

      protected Invoker(object obj, Type type)
      {
         this.obj = obj;
         this.type = type;
      }

      public Invoker(object obj)
      {
         this.obj = obj;
         type = obj.GetType();
      }

      object invokeMember(string name, BindingFlags bindings, object[] args) => type.InvokeMember(name, bindings, null, obj, args);

      public T Invoke<T>(string name, params object[] args) => (T)invokeMember(name, methodBindings, args);

      public void Invoke(string name, params object[] args) => invokeMember(name, methodBindings, args);

      public T GetProperty<T>(string name, params object[] args) => (T)invokeMember(name, getPropertyBindings, args);

      public void SetProperty(string name, params object[] args) => invokeMember(name, setPropertyBindings, args);

      public T GetField<T>(string name, params object[] args) => (T)invokeMember(name, getFieldBindings, args);

      public void SetField(string name, params object[] args) => invokeMember(name, setFieldBindings, args);

      public InvokerTrying TryTo => new InvokerTrying(this);
   }
}