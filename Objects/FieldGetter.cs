using System.Reflection;

namespace Core.Objects
{
   public class FieldGetter : IGetter
   {
      FieldInfo fieldInfo;

      public FieldGetter(FieldInfo fieldInfo) => this.fieldInfo = fieldInfo;

      public object GetValue(object obj) => fieldInfo.GetValue(obj);
   }
}