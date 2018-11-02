using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.Resilience.CircuitBreaking
{
   public class BinaryCircuitBreakerOpeningRates : ICircuitBreakerOpeningRates
   {
      public int[] OpeningRates { get; } = {};
   }
}