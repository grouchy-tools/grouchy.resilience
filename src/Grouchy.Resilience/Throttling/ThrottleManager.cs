using System;
using System.Collections.Generic;
using Grouchy.Resilience.Abstractions.Throttling;

namespace Grouchy.Resilience.Throttling
{
   public class ThrottleManager : IThrottleManager
   {
      private readonly Dictionary<string, IThrottleSync> _cache = new Dictionary<string, IThrottleSync>();

      public void Register(string policy, IThrottleSync throttleSync)
      {
         _cache.Add(policy, throttleSync);
      }

      public IThrottleSync GetSync(string policy)
      {
         if (_cache.TryGetValue(policy, out var sync)) return sync;
            
         throw new InvalidOperationException("ThrottlePolicy not found");
      }
   }
}