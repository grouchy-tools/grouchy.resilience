using System;
using Grouchy.Resilience.Abstractions.Retrying;

namespace Grouchy.Resilience.Retrying
{
   public class ExponentialRetryDelay : IRetryDelay
   {
      private readonly int _initialDelayMs;

      public ExponentialRetryDelay(int initialDelayMs)
      {
         _initialDelayMs = initialDelayMs;
      }

      public int DelayMs(int retryAttempt)
      {
         return _initialDelayMs * (int)Math.Pow(2, retryAttempt - 1);
      }
   }
}
