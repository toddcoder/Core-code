using System;
using System.Reflection;
using Core.Assertions;
using Core.Monads;

namespace Core.Objects;

public class Invoker
{
   protected const BindingFlags BASE_BINDINGS = BindingFlags.Public | BindingFlags.Instance;
   protected const BindingFlags METHOD_BINDINGS = BASE_BINDINGS | BindingFlags.InvokeMethod;
   protected const BindingFlags GET_PROPERTY_BINDINGS = BASE_BINDINGS | BindingFlags.GetProperty;
   protected const BindingFlags SET_PROPERTY_BINDINGS = BASE_BINDINGS | BindingFlags.SetProperty;
   protected const BindingFlags GET_FIELD_BINDINGS = BASE_BINDINGS | BindingFlags.GetField;
   protected const BindingFlags SET_FIELD_BINDINGS = BASE_BINDINGS | BindingFlags.SetField;

   public static Result<Invoker> From(object obj) =>
      from nonNull in obj.Must().Not.BeNull().OrFailure()
      from type in nonNull.GetType().Success()
      select new Invoker(nonNull, type);

   protected object obj;
   protected Type type;

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

   protected object invokeMember(string name, BindingFlags bindings, object[] args) => type.InvokeMember(name, bindings, null, obj, args);

   public T Invoke<T>(string name, params object[] args) => (T)invokeMember(name, METHOD_BINDINGS, args);

   public void Invoke(string name, params object[] args) => invokeMember(name, METHOD_BINDINGS, args);

   public T GetProperty<T>(string name, params object[] args) => (T)invokeMember(name, GET_PROPERTY_BINDINGS, args);

   public void SetProperty(string name, params object[] args) => invokeMember(name, SET_PROPERTY_BINDINGS, args);

   public T GetField<T>(string name, params object[] args) => (T)invokeMember(name, GET_FIELD_BINDINGS, args);

   public void SetField(string name, params object[] args) => invokeMember(name, SET_FIELD_BINDINGS, args);

   public InvokerTrying TryTo => new(this);
}