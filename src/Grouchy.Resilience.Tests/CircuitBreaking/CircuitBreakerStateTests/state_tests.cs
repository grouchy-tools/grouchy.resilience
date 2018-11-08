using System.Linq;
using FakeItEasy;
using Grouchy.Resilience.Abstractions.CircuitBreaking;
using Grouchy.Resilience.CircuitBreaking;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.Resilience.Tests.CircuitBreaking.CircuitBreakerStateTests
{
   // ReSharper disable once InconsistentNaming
   public class state_tests
   {
      [Test]
      public void circuit_should_be_accepting_all_requests_on_creation()
      {
         var testSubject = new CircuitBreakerState("policy", A.Fake<ICircuitBreakerAnalyser>(), A.Fake<ICircuitBreakerOpeningRates>(), A.Fake<ICircuitBreakerPeriod>());

         var accepts = Enumerable.Range(0, 100).Select(c => testSubject.ShouldAccept()).ToArray();
         
         Assert.That(accepts.All(c => c), Is.True);
      }
      
      [Test]
      public void circuit_should_be_100_pct_closed_on_creation()
      {
         var testSubject = new CircuitBreakerState("policy", A.Fake<ICircuitBreakerAnalyser>(), A.Fake<ICircuitBreakerOpeningRates>(), A.Fake<ICircuitBreakerPeriod>());
         
         testSubject.ClosedPct.ShouldBe(100);
      }
   }
}