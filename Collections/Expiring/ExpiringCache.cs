using System;
using System.Linq;
using System.Timers;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections.Expiring
{
   public class ExpiringCache<TKey, TValue> : IHash<TKey, TValue>
   {
      Hash<TKey, TValue> cache;
      Hash<TKey, ExpirationPolicy<TValue>> expirationPolicies;
      Maybe<Timer> anyTimer;
      object locker;
      Func<ExpirationPolicy<TValue>> newPolicy;

      public event EventHandler<ExpirationArgs<TKey, TValue>> Expired;

      public ExpiringCache(TimeSpan activeMonitoringInterval)
      {
         cache = new Hash<TKey, TValue>();
         expirationPolicies = new Hash<TKey, ExpirationPolicy<TValue>>();
         var newTimer = new Timer(activeMonitoringInterval.TotalMilliseconds);
         newTimer.Elapsed += (sender, e) =>
         {
            lock (locker)
            {
               var expiredKeys = cache
                  .Where(i => expirationPolicies.FlatMap(i.Key, v => v.ItemEvictable(i.Value), false))
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

         anyTimer = newTimer.Some();
         locker = new object();
         NewPolicy = () => new NonExpiration<TValue>();
      }

      public ExpiringCache()
      {
         cache = new Hash<TKey, TValue>();
         expirationPolicies = new Hash<TKey, ExpirationPolicy<TValue>>();
         anyTimer = none<Timer>();
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
         if (anyTimer.If(out var timer))
         {
            timer.Enabled = true;
         }
      }

      public void StopMonitoring()
      {
         if (anyTimer.If(out var timer))
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
               if (cache.If(key, out var value))
               {
                  var policy = expirationPolicies.Find(key, k => newPolicy(), true);
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

      public Result<Hash<TKey, TValue>> AnyHash() => cache.Success();

      public void Remove(TKey key)
      {
         lock (locker)
         {
            cache.Remove(key);
            expirationPolicies.Remove(key);
         }
      }
   }
}