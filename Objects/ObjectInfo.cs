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
         if (_info is (true, var info))
         {
            var parameters = info.GetIndexParameters();
            if (_index is (true, var index))
            {
               return parameters.Length == 0 ? getValue(index) : info.GetValue(obj, getIndex(index));
            }
            else if (parameters.Length > 0)
            {
               return nil;
            }
            else
            {
               return info.GetValue(obj, null);
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
         if (_info is (true, var info))
         {
            if (_index is (true, var index))
            {
               if (parameters.Length == 0)
               {
                  var _ = setValue(value);
               }
               else
               {
                  info.SetValue(obj, val, getIndex(index));
               }
            }
            else
            {
               info.SetValue(obj, val, null);
            }
         }
      }
   }

   protected Maybe<object> getValue(int defaultIndex)
   {
      if (_info is (true, var info))
      {
         var result = info.GetValue(obj, null);
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
      if (_info is (true, var info))
      {
         var result = info.GetValue(obj, null);
         if (result is null)
         {
            return false;
         }
         else if (_index is (true, var index))
         {
            switch (result)
            {
               case Array array:
                  array.SetValue(value, index);
                  return true;
               case IList list:
                  list[index] = value;
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