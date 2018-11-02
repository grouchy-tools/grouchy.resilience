using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.Resilience.CircuitBreaking
{
   public class ProgressiveCircuitBreakerOpeningRates : ICircuitBreakerOpeningRates
   {
      public int[] OpeningRates { get; } = {2, 5, 20};
   }
}