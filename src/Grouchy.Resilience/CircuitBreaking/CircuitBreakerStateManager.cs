using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.Resilience.CircuitBreaking
{
   public class CircuitBreakerStateManager : ICircuitBreakerManager, IDisposable
   {
      private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
      private readonly Dictionary<string, Entry> _cache = new Dictionary<string, Entry>();

      private bool _isDisposed;

      public void Register(
         string policy,
         ICircuitBreakerAnalyser circuitBreakerAnalyser,
         ICircuitBreakerOpeningRates circuitBreakerOpeningRates,
         ICircuitBreakerPeriod circuitBreakerPeriod)
      {
         var circuitBreakingState = new CircuitBreakerState(policy, circuitBreakerAnalyser, circuitBreakerOpeningRates, circuitBreakerPeriod);
         var task = Task.Run(() => circuitBreakingState.MonitorAsync(_stoppingCts.Token));

         _cache.Add(policy, new Entry(circuitBreakingState, task));
      }

      public ICircuitBreakerState GetState(string policy)
      {
         if (_cache.TryGetValue(policy, out var entry)) return entry.CircuitBreakerState;
            
         throw new InvalidOperationException("CircuitBreakerPolicy not found");
      }

      public IEnumerable<ICircuitBreakerState> GetStates()
      {
         return _cache.Select(e => e.Value.CircuitBreakerState);
      }

      public async Task StopMonitoringAsync(CancellationToken cancellationToken)
      {
         try
         {
            // Signal cancellation to all tasks monitoring state 
            _stoppingCts.Cancel();
         }
         finally
         {
            var waitForAllTasks = Task.WhenAll(_cache.Select(c => c.Value.Task));
            
            // Wait until all tasks complete or the stop token triggers
            await Task.WhenAny(waitForAllTasks, Task.Delay(Timeout.Infinite, cancellationToken));
         }
      }

      void IDisposable.Dispose()
      {
         if (_isDisposed) return;

         _stoppingCts?.Dispose();
         _isDisposed = true;
      }

      // Implicit tuple does not seem to be available for net451
      private class Entry
      {
         public Entry(ICircuitBreakerState circuitBreakerState, Task task)
         {
            CircuitBreakerState = circuitBreakerState;
            Task = task;
         }

         public readonly ICircuitBreakerState CircuitBreakerState;

         public readonly Task Task;
      }
   }
}