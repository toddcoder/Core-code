using System.Collections.Generic;
using System.Linq;

namespace Core.Collections;

public class Triggers : StringHash<Triggers.TriggerType>
{
   public enum TriggerType
   {
      Set,
      Triggered
   }

   public Triggers(bool ignoreCase) : base(ignoreCase)
   {
   }

   public Triggers(bool ignoreCase, int capacity) : base(ignoreCase, capacity)
   {
   }

   public Triggers(bool ignoreCase, IDictionary<string, TriggerType> dictionary) : base(ignoreCase, dictionary)
   {
   }

   public void Set(string key)
   {
      var _triggerType = Maybe[key];
      if (!_triggerType)
      {
         this[key] = TriggerType.Set;
      }
   }

   public bool IsSet(string key) => Maybe[key].Map(tt => tt == TriggerType.Set) | false;

   public void Trigger(string key)
   {
      var _triggerType = Maybe[key];
      if (_triggerType && _triggerType == TriggerType.Set)
      {
         this[key] = TriggerType.Triggered;
      }
   }

   public bool IsTriggered(string key) => Maybe[key].Map(tt => tt == TriggerType.Triggered) | false;

   public void Update(string key)
   {
      var _triggerType = Maybe[key];
      if (_triggerType is (true, var triggerType))
      {
         if (triggerType == TriggerType.Set)
         {
            this[key] = TriggerType.Triggered;
         }
      }
      else
      {
         this[key] = TriggerType.Set;
      }
   }

   public int SetCount => Values.Count(v => v == TriggerType.Set);

   public int TriggeredCount => Values.Count(v => v == TriggerType.Triggered);
}