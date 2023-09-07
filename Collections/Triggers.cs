using System.Collections.Generic;
using System.Linq;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class Triggers : StringHash<Triggers.TriggerType>
{
   public enum TriggerType
   {
      Set,
      Triggered,
      Read
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
      if (!Maybe[key])
      {
         this[key] = TriggerType.Set;
      }
   }

   public void Trigger(string key)
   {
      if (Maybe[key] is (true, TriggerType.Set))
      {
         this[key] = TriggerType.Triggered;
      }
   }

   public void Read(string key)
   {
      if (Maybe[key] is (true, TriggerType.Triggered))
      {
         this[key] = TriggerType.Read;
      }
   }

   public void Reset(string key) => Maybe[key] = nil;

   public bool IsTriggered(string key) => Maybe[key].Map(tt => tt == TriggerType.Triggered) | false;

   public bool IsSet(string key) => Maybe[key].Map(tt => tt == TriggerType.Set) | false;

   public bool IsRead(string key) => Maybe[key].Map(tt => tt == TriggerType.Read) | false;

   public void Update(string key)
   {
      if (Maybe[key] is (true, var triggerType))
      {
         this[key] = triggerType switch
         {
            TriggerType.Set => TriggerType.Triggered,
            TriggerType.Triggered => TriggerType.Read,
            _ => this[key]
         };
      }
      else
      {
         this[key] = TriggerType.Set;
      }
   }

   public int SetCount => Values.Count(v => v == TriggerType.Set);

   public int TriggeredCount => Values.Count(v => v == TriggerType.Triggered);

   public int ReadCount => Values.Count(v => v == TriggerType.Read);
}