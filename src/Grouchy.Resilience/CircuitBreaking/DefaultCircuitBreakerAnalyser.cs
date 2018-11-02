using System.Collections.Generic;
using System.Linq;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.Resilience.CircuitBreaking
{
   public class DefaultCircuitBreakerAnalyser : ICircuitBreakerAnalyser
   {
      private static bool ShouldTrip(Metrics metrics)
      {
         var errors = Sum(metrics.FailureResponses) + metrics.Timeouts + metrics.Exceptions;
         var total = errors + Sum(metrics.SuccessResponses);

         var errorRate = errors / (double) total;

         return errorRate > 0.5;
      }

      private static bool ShouldReset(Metrics metrics)
      {
         return Sum(metrics.SuccessResponses) > 0;
      }

      private static int Sum(IDictionary<string, int> occurrences)
      {
         return occurrences.Sum(c => c.Value);
      }

      public Rating Analyse(IEnumerable<Metrics> recentMetrics)
      {
         var metrics = recentMetrics.First();

         if (ShouldTrip(metrics)) return Rating.Trip;

         if (ShouldReset(metrics)) return Rating.Reset;

         return Rating.Hold;
      }
   }
}