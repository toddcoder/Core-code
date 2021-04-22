using Core.Collections;

namespace Core.Objects
{
   public class ReadOnlyEquatableBase : EquatableBase
   {
      protected LateLazy<StringHash<object>> values;
      protected LateLazy<int> hashCode;
      protected LateLazy<string> keys;

      public ReadOnlyEquatableBase()
      {
         values = new LateLazy<StringHash<object>>();
         hashCode = new LateLazy<int>();
         keys = new LateLazy<string>();
      }

      protected override StringHash<object> getValues(object obj)
      {
         values.ActivateWith(() => base.getValues(obj));
         return values.Value;
      }

      public override int GetHashCode()
      {
         hashCode.ActivateWith(() => base.GetHashCode());
         return hashCode.Value;
      }

      public override string Keys
      {
         get
         {
            keys.ActivateWith(() => base.Keys);
            return keys.Value;
         }
      }
   }
}