using System;
using System.Collections.Generic;
using Grouchy.Resilience.Abstractions.Retrying;

namespace Grouchy.Resilience.Retrying
{
   public class RetryManager : IRetryManager
   {
      private readonly Dictionary<string, Entry> _cache = new Dictionary<string, Entry>();

      public void Register(string policy, IRetryDelay retryDelay, IRetryPredicate retryPredicate)
      {
         _cache.Add(policy, new Entry(retryDelay, retryPredicate));
      }

      public IRetryDelay GetDelay(string policy)
      {
         if (_cache.TryGetValue(policy, out var entry)) return entry.Delay;
            
         throw new InvalidOperationException("RetryPolicy not found");
      }

      public IRetryPredicate GetPredicate(string policy)
      {
         if (_cache.TryGetValue(policy, out var entry)) return entry.Predicate;
            
         throw new InvalidOperationException("RetryPolicy not found");
      }
      
      // Implicit tuple does not seem to be available for net451
      private class Entry
      {
         public Entry(IRetryDelay delay, IRetryPredicate predicate)
         {
            Delay = delay;
            Predicate = predicate;
         }

         public readonly IRetryDelay Delay;

         public readonly IRetryPredicate Predicate;
      }
   }
}
