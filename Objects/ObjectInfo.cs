using System;
using System.Collections;
using System.Reflection;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

internal class ObjectInfo
{
   public static Maybe<PropertyInfo> PropertyInfo(object obj, Signature signature)
   {
      var info = obj.GetType().GetProperty(signature.Name);
      return maybe(info != null, () => info);
   }

   protected object obj;
   protected Maybe<int> _index;
   protected Maybe<PropertyInfo> _info;

   public ObjectInfo(object obj, Signature signature)
   {
      this.obj = obj;
      _index = signature.Index;
      _info = PropertyInfo(obj, signature);
   }

   public ObjectInfo(object obj, Signature signature, PropertyInfo info)
   {
      this.obj = obj;
      _index = signature.Index;
      _info = info;
   }

   public ObjectInfo()
   {
      obj = new None<object>();
      _index = nil;
      _info = nil;
   }

   public object Object => obj;

   public Maybe<int> Index => _index;

   public Maybe<Type> PropertyType => _info.Map(pi => pi.PropertyType);

   public Maybe<object> Value
   {
      get
      {
         if (_info)
         {
            var parameters = _info.Value.GetIndexParameters();
            if (_index)
            {
               return parameters.Length == 0 ? getValue(_index) : _info.Value.GetValue(obj, getIndex(_index));
            }
            else if (parameters.Length > 0)
            {
               return nil;
            }
            else
            {
               return _info.Value.GetValue(obj, null);
            }
         }
         else
         {
            return nil;
         }
      }
      set
      {
         var val = value.Required("Value must be set to a Some");
         var parameters = _info.Required("No property exists for signature").GetIndexParameters();
         if (_info)
         {
            if (_index)
            {
               if (parameters.Length == 0)
               {
                  var _ = setValue(value);
               }
               else
               {
                  _info.Value.SetValue(obj, val, getIndex(_index));
               }
            }
            else
            {
               _info.Value.SetValue(obj, val, null);
            }
         }
      }
   }

   protected Maybe<object> getValue(int defaultIndex)
   {
      if (_info)
      {
         var result = _info.Value.GetValue(obj, null);
         if (result is null)
         {
            return result;
         }
         else
         {
            return result switch
            {
               Array array => array.GetValue(defaultIndex),
               IList list => list[defaultIndex],
               _ => nil
            };
         }
      }
      else
      {
         return nil;
      }
   }

   protected static object[] getIndex(int singleIndex) => new object[] { singleIndex };

   protected bool setValue(object value)
   {
      if (_info)
      {
         var result = _info.Value.GetValue(obj, null);
         if (result is null)
         {
            return false;
         }
         else if (_index)
         {
            switch (result)
            {
               case Array array:
                  array.SetValue(value, _index);
                  return true;
               case IList list:
                  list[_index] = value;
                  return true;
               default:
                  return false;
            }
         }
         else
         {
            return false;
         }
      }
      else
      {
         return false;
      }
   }
}