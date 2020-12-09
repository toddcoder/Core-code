using Core.Collections;

namespace Core.Objects
{
   public class ReadOnlyEquatableBase : EquatableBase
   {
      protected LateLazy<Hash<string, object>> values;
      protected LateLazy<int> hashCode;
      protected LateLazy<string> keys;

      public ReadOnlyEquatableBase()
      {
         values = new LateLazy<Hash<string, object>>();
         hashCode = new LateLazy<int>();
         keys = new LateLazy<string>();
      }

      protected override Hash<string, object> getValues(object obj)
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