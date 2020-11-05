using Core.Collections;

namespace Core.Objects
{
   public class ReadOnlyEquatableBase : EquatableBase
   {
      protected LateLazy<Hash<string, object>> values;

      public ReadOnlyEquatableBase()
      {
         values = new LateLazy<Hash<string, object>>();
      }

      protected override Hash<string, object> getValues(object obj)
      {
         values.ActivateWith(() => base.getValues(obj));
         return values.Value;
      }
   }
}