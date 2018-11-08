using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.Resilience.CircuitBreaking;
using NUnit.Framework;

namespace Grouchy.Resilience.Tests.CircuitBreaking.CircuitBreakerStateTests.scenarios
{
   // ReSharper disable once InconsistentNaming
   public abstract class scenario_base
   {
      protected IList<double> ClosedPcts { get; private set; }
      protected IList<bool> ShouldAccepts { get; private set; }
      protected CircuitBreakerState TestSubject { get; private set; }

      [OneTimeSetUp]
      public async Task setup_scenario_base()
      {
         ClosedPcts = new List<double>();
         ShouldAccepts = new List<bool>();

         var circuitBreakerPeriod = new CircuitBreakerPeriod {PeriodMs = 100};
         var circuitBreakerOpeningRates = new BinaryCircuitBreakerOpeningRates();
         var circuitBreakerAnalyser = new DefaultCircuitBreakerAnalyser();
         
         TestSubject = new CircuitBreakerState("policy", circuitBreakerAnalyser, circuitBreakerOpeningRates, circuitBreakerPeriod);
         
         var cancellationTokenSource = new CancellationTokenSource();
         var monitorTask = Task.Run(() => TestSubject.MonitorAsync(cancellationTokenSource.Token));

         await RunScenario();

         cancellationTokenSource.Cancel();
         await monitorTask;
      }

      protected abstract Task RunScenario();
   }
}