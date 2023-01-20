using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;

namespace Core.Objects;

public class DataContainer : StringHash<object>
{
   protected Maybe<Action> _beforeExecute;
   protected Maybe<Action> _afterExecute;

   public DataContainer() : base(true)
   {
      _beforeExecute = initializeAction("BeforeExecute");
      _afterExecute = initializeAction("AfterExecute");
      Format = "";
   }

   public DataContainer(IEnumerable<KeyValuePair<string, object>> initializers) : this()
   {
      foreach (var (key, value) in initializers)
      {
         this[key] = value.Some();
      }
   }

   public string Format { get; set; }

   protected Maybe<Action> initializeAction(string key) => this.Map(key, o => o.IfCast<Action>());

   public void BeforeExecute()
   {
      if (_beforeExecute)
      {
         _beforeExecute.Value();
      }
   }

   public void AfterExecute()
   {
      if (_afterExecute)
      {
         _afterExecute.Value();
      }
   }

   public Hash<TKey, TValue> ToHash<TKey, TValue>(Func<string, TKey> toKey, Func<object, TValue> toValue) where TKey : class where TValue : class
   {
      var result = new Hash<TKey, TValue>();

      foreach (var (rawKey, rawValue) in this)
      {
         var key = toKey(rawKey);
         var value = toValue(rawValue);
         result[key] = value;
      }

      return result;
   }

   public StringHash<TValue> ToHash<TValue>(Func<object, TValue> toValue) where TValue : class
   {
      var result = new StringHash<TValue>(ignoreCase);

      foreach (var (key, obj) in this)
      {
         var value = toValue(obj);
         result[key] = value;
      }

      return result;
   }

   public DateTime AsDateTime(string key, DateTime defaultValue) => this.FlatMap(key, dt => (DateTime)dt, () => defaultValue);

   public string AsString(string key, string defaultValue = "") => this.FlatMap(key, s => (string)s, () => defaultValue);

   public int AsInt(string key, int defaultValue = 0) => this.FlatMap(key, i => (int)i, () => defaultValue);

   public double AsDouble(string key, double defaultValue = 0d) => this.FlatMap(key, d => (double)d, () => defaultValue);

   public bool AsBoolean(string key, bool defaultValue = false) => this.FlatMap(key, b => (bool)b, () => defaultValue);

   public override string ToString()
   {
      if (Format.IsEmpty())
      {
         return KeyArray().Select(key => $"{key} = {this[key]}").ToString(", ");
      }
      else
      {
         var builder = new StringBuilder(Format);
         foreach (var key in KeyArray())
         {
            builder.Replace("{" + key + "}", this[key]?.ToString() ?? "");
         }

         return builder.ToString();
      }
   }
}