using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.Resilience.CircuitBreaking
{
   public class CircuitBreakerPeriod : ICircuitBreakerPeriod
   {
      public int PeriodMs { get; set; } = 10000;
   }
}