using System;
using System.Linq;
using System.Timers;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections.Expiring;

public class ExpiringCache<TKey, TValue> : IHash<TKey, TValue>
{
   protected Hash<TKey, TValue> cache;
   protected Hash<TKey, ExpirationPolicy<TValue>> expirationPolicies;
   protected Maybe<Timer> _timer;
   protected object locker;
   protected Func<ExpirationPolicy<TValue>> newPolicy;

   public event EventHandler<ExpirationArgs<TKey, TValue>> Expired;

   public ExpiringCache(TimeSpan activeMonitoringInterval)
   {
      cache = new Hash<TKey, TValue>();
      expirationPolicies = new Hash<TKey, ExpirationPolicy<TValue>>();
      var newTimer = new Timer(activeMonitoringInterval.TotalMilliseconds);
      newTimer.Elapsed += (_, _) =>
      {
         lock (locker)
         {
            var expiredKeys = cache
               .Where(i => expirationPolicies.Maybe[i.Key].Map(v => v.ItemEvictable(i.Value)) | false)
               .Select(i => i.Key)
               .ToArray();
            foreach (var key in expiredKeys)
            {
               var args = new ExpirationArgs<TKey, TValue>(key, cache[key]);
               Expired?.Invoke(this, args);
               if (!args.CancelEviction)
               {
                  cache.Remove(key);
                  expirationPolicies.Remove(key);
               }
            }
         }
      };

      _timer = newTimer;
      locker = new object();
      NewPolicy = () => new NonExpiration<TValue>();
   }

   public ExpiringCache()
   {
      cache = new Hash<TKey, TValue>();
      expirationPolicies = new Hash<TKey, ExpirationPolicy<TValue>>();
      _timer = nil;
      locker = new object();
      NewPolicy = () => new NonExpiration<TValue>();
   }

   public Func<ExpirationPolicy<TValue>> NewPolicy
   {
      get => newPolicy;
      set => newPolicy = value;
   }

   public void StartMonitoring()
   {
      if (_timer is (true, var timer))
      {
         timer.Enabled = true;
      }
   }

   public void StopMonitoring()
   {
      if (_timer is (true, var timer))
      {
         timer.Enabled = false;
      }
   }

   public TValue this[TKey key]
   {
      get
      {
         lock (locker)
         {
            var _value = cache.Maybe[key];
            if (_value is (true, var value))
            {
               var policy = expirationPolicies.Find(key, _ => newPolicy(), true);
               policy.Reset();
               if (policy.ItemEvictable(value))
               {
                  cache.Remove(key);
                  expirationPolicies.Remove(key);

                  return default;
               }
               else
               {
                  return value;
               }
            }
            else
            {
               return default;
            }
         }
      }
      set
      {
         lock (locker)
         {
            cache[key] = value;
            expirationPolicies[key] = newPolicy();
         }
      }
   }

   public bool ContainsKey(TKey key)
   {
      lock (locker)
      {
         return cache.ContainsKey(key);
      }
   }

   public Result<Hash<TKey, TValue>> AnyHash() => cache;

   public HashInterfaceMaybe<TKey, TValue> Items => new(this);

   public void Remove(TKey key)
   {
      lock (locker)
      {
         cache.Remove(key);
         expirationPolicies.Remove(key);
      }
   }
}