using System.Reflection;

namespace Core.Objects
{
   public class PropertyGetter : IGetter
   {
      protected PropertyInfo propertyInfo;

      public PropertyGetter(PropertyInfo propertyInfo) => this.propertyInfo = propertyInfo;

      public object GetValue(object obj) => propertyInfo.GetValue(obj);
   }
}