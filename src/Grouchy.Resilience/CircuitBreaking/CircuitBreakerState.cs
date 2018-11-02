using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.Resilience.CircuitBreaking
{
   public class CircuitBreakerState : ICircuitBreakerState
   {
      private readonly ICircuitBreakerAnalyser _circuitBreakerAnalyser;
      private readonly ICircuitBreakerOpeningRates _circuitBreakerOpeningRates;
      private readonly ICircuitBreakerPeriod _circuitBreakerPeriod;
      private readonly LinkedList<Metrics> _historicMetrics = new LinkedList<Metrics>();
      private readonly object _metricsSync = new object();
      private readonly Random _random = new Random();

      private Metrics _currentMetrics = new Metrics();
      private int _tripLevel;

      public CircuitBreakerState(
         ICircuitBreakerAnalyser circuitBreakerAnalyser,
         ICircuitBreakerOpeningRates circuitBreakerOpeningRates,
         ICircuitBreakerPeriod circuitBreakerPeriod)
      {
         _circuitBreakerAnalyser = circuitBreakerAnalyser ?? throw new ArgumentNullException(nameof(circuitBreakerAnalyser));
         _circuitBreakerOpeningRates = circuitBreakerOpeningRates ?? throw new ArgumentNullException(nameof(circuitBreakerOpeningRates));
         _circuitBreakerPeriod = circuitBreakerPeriod ?? throw new ArgumentNullException(nameof(circuitBreakerPeriod));
      }

      public double ClosedPct
      {
         get
         {
            if (_tripLevel == 0) return 100;

            if (_tripLevel > OpeningRates.Length) return 0;

            return 100f / OpeningRates[_tripLevel - 1];
         }
      }

      public bool ShouldAccept()
      {
         // 100%
         if (_tripLevel == 0) return true;
         
         // 0%
         if (_tripLevel > OpeningRates.Length) return false;
         
         return _random.Next(OpeningRates[_tripLevel - 1]) == 0;
      }

      public void LogSuccessResponse(string key)
      {
         lock (_metricsSync)
         {
            _currentMetrics.SuccessResponses.TryGetValue(key, out var value); 
            _currentMetrics.SuccessResponses[key] = value + 1;
         }
      }

      public void LogFailureResponse(string key)
      {
         lock (_metricsSync)
         {
            _currentMetrics.FailureResponses.TryGetValue(key, out var value); 
            _currentMetrics.FailureResponses[key] = value + 1;
         }
      }

      public void LogTimeout()
      {
         lock (_metricsSync)
         {
            _currentMetrics.Timeouts++;
         }
      }

      public void LogException(Exception exception)
      {
         lock (_metricsSync)
         {
            _currentMetrics.Exceptions++;
         }
      }

      public void LogWithheld()
      {
         lock (_metricsSync)
         {
            _currentMetrics.Rejections++;
         }
      }

      public async Task MonitorAsync(CancellationToken cancellationToken)
      {
         // TODO: Ensure only single thread in here
         
         while (!cancellationToken.IsCancellationRequested)
         {
            // TODO: Get last n metrics, based on config
            
            Metrics lastMetrics;
            lock (_metricsSync)
            {
               _historicMetrics.AddFirst(_currentMetrics);
               lastMetrics = _currentMetrics;
               _currentMetrics = new Metrics();
            }
            
            var analysis = _circuitBreakerAnalyser.Analyse(new[] { lastMetrics });
            
            switch (analysis)
            {
               case Rating.Trip when CanTrip():
                  _tripLevel++;
                  break;
               case Rating.Reset when CanReset():
                  _tripLevel--;
                  break;
            }

            try
            {
               await Task.Delay(_circuitBreakerPeriod.PeriodMs, cancellationToken);
            }
            catch (TaskCanceledException)
            {
            }

            bool CanTrip() => _tripLevel < OpeningRates.Length + 1;
            bool CanReset() => _tripLevel > 0;
         }
      }
      
      private int[] OpeningRates => _circuitBreakerOpeningRates.OpeningRates ?? new int[] { };
   }
}
