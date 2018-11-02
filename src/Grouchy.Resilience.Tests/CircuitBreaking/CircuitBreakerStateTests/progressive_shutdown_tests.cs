using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Grouchy.Resilience.Abstractions.CircuitBreaking;
using Grouchy.Resilience.CircuitBreaking;
using Grouchy.Resilience.Tests.Stubs;
using NUnit.Framework;
using Shouldly;
using Rating = Grouchy.Resilience.Abstractions.CircuitBreaking.Rating;

namespace Grouchy.Resilience.Tests.CircuitBreaking.CircuitBreakerStateTests
{
   // ReSharper disable once InconsistentNaming
   public class progressive_shutdown_tests
   {
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Trip }, 50)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Trip, Rating.Trip }, 20)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Trip, Rating.Trip, Rating.Trip }, 5)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Trip, Rating.Trip, Rating.Trip, Rating.Trip }, 0)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Trip, Rating.Reset }, 100)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Trip, Rating.Hold }, 50)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Trip, Rating.Trip, Rating.Reset }, 50)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Trip, Rating.Trip, Rating.Hold }, 20)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Trip, Rating.Trip, Rating.Reset, Rating.Reset }, 100)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Reset }, 100)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Reset, Rating.Trip }, 50)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Reset, Rating.Trip, Rating.Trip }, 20)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Reset, Rating.Reset }, 100)]
      [TestCase(new[] { 2, 5, 20 }, new[] { Rating.Reset, Rating.Reset, Rating.Trip }, 50)]
      [TestCase(new int[] { }, new[] { Rating.Trip }, 0)]
      [TestCase(new int[] { }, new[] { Rating.Reset }, 100)]
      [TestCase(new int[] { }, new[] { Rating.Hold }, 100)]
      [TestCase(new int[] { }, new[] { Rating.Trip, Rating.Trip, Rating.Reset }, 100)]
      [TestCase(new int[] { }, new[] { Rating.Trip, Rating.Reset, Rating.Reset }, 100)]
      [TestCase(new int[] { }, new[] { Rating.Reset, Rating.Trip }, 0)]
      [TestCase(new int[] { }, new[] { Rating.Reset, Rating.Reset }, 100)]
      public async Task state_should_be_closed_appropriately(int[] openingRates, Rating[] analyseResponses, double expectedClosedPct)
      {
         var state = await RunScenario(openingRates, analyseResponses);
         
         state.ClosedPct.ShouldBe(expectedClosedPct);
      }
      
      private static async Task<CircuitBreakerState> RunScenario(int[] openingRates, Rating[] analyseResponses)
      {
         var circuitBreakerPeriod = new CircuitBreakerPeriod {PeriodMs = 10};
         var circuitBreakerOpeningRates = A.Fake<ICircuitBreakerOpeningRates>();
         var circuitBreakerAnalyser = new StubCircuitBreakerAnalyser(analyseResponses);
         var testSubject = new CircuitBreakerState(circuitBreakerAnalyser, circuitBreakerOpeningRates, circuitBreakerPeriod);

         A.CallTo(() => circuitBreakerOpeningRates.OpeningRates).Returns(openingRates);
         
         var monitorTask = Task.Run(() => testSubject.MonitorAsync(CancellationToken.None));

         // Wait long enough for the monitor task to throw an exception, due to no more analyser responses
         await Task.Delay(100);

         try
         {
            await monitorTask;
         }
         catch (InvalidOperationException)
         {
         }

         return testSubject;
      }
   }
}