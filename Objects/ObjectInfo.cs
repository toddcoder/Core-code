using System;
using System.Collections;
using System.Reflection;
using Core.Assertions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Objects
{
   internal class ObjectInfo
   {
      public static IMaybe<PropertyInfo> PropertyInfo(object obj, Signature signature)
      {
         var info = obj.GetType().GetProperty(signature.Name);
         return maybe(info != null, () => info);
      }

      object obj;
      IMaybe<int> index;
      IMaybe<PropertyInfo> info;

      public ObjectInfo(object obj, Signature signature)
      {
         this.obj = obj;
         index = signature.Index;
         info = PropertyInfo(obj, signature);
      }

      public ObjectInfo(object obj, Signature signature, PropertyInfo info)
      {
         this.obj = obj;
         index = signature.Index;
         this.info = info.Some();
      }

      public ObjectInfo()
      {
         obj = none<object>();
         index = none<int>();
         info = none<PropertyInfo>();
      }

      public object Object => obj;

      public IMaybe<int> Index => index;

      public IMaybe<Type> PropertyType => info.Map(pi => pi.PropertyType);

      public IMaybe<object> Value
      {
         get
         {
            if (info.If(out var inf))
            {
               var parameters = inf.GetIndexParameters();
               if (index.If(out var ind))
               {
                  if (parameters.Length == 0)
                  {
                     return getValue(ind);
                  }
                  else
                  {
                     return inf.GetValue(obj, getIndex(ind)).Some();
                  }
               }
               else if (parameters.Length > 0)
               {
                  return none<object>();
               }
               else
               {
                  return inf.GetValue(obj, null).Some();
               }
            }
            else
            {
               return none<object>();
            }
         }
         set
         {
            var val = value.Required("Value must be set to a Some");
            var parameters = info.Required("No property exists for signature").GetIndexParameters();
            if (info.If(out var inf))
            {
               if (index.If(out var i))
               {
                  if (parameters.Length == 0)
                  {
                     var valueWasSet = setValue(value);
                     assert(() => valueWasSet).Must().Be().OrThrow("$name couldn't be set");
                  }
                  else
                  {
                     inf.SetValue(obj, val, getIndex(i));
                  }
               }
               else
               {
                  assert(() => parameters).Must().BeEmpty().OrThrow();
                  inf.SetValue(obj, val, null);
               }
            }
         }
      }

      IMaybe<object> getValue(int defaultIndex)
      {
         if (info.If(out var inf))
         {
            var result = inf.GetValue(obj, null);
            if (result.IsNull())
            {
               return result.Some();
            }
            else
            {
               switch (result)
               {
                  case Array array:
                     return array.GetValue(defaultIndex).Some();
                  case IList list:
                     return list[defaultIndex].Some();
                  default:
                     return none<object>();
               }
            }
         }
         else
         {
            return none<object>();
         }
      }

      static object[] getIndex(int singleIndex) => new object[] { singleIndex };

      bool setValue(object value)
      {
         if (info.If(out var i))
         {
            var result = i.GetValue(obj, null);
            if (result.IsNull())
            {
               return false;
            }
            else if (index.If(out var idx))
            {
               switch (result)
               {
                  case Array array:
                     array.SetValue(value, idx);
                     return true;
                  case IList list:
                     list[idx] = value;
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
}