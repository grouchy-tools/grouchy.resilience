using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.Resilience.Tests.CircuitBreaking.CircuitBreakerStateTests.scenarios
{
   // ReSharper disable once InconsistentNaming
   public class errors_then_successes : scenario_base
   {
      protected override async Task RunScenario()
      {
         for (var i = 0; i < 100; i++)
         {
            ClosedPcts.Add(TestSubject.ClosedPct);
            ShouldAccepts.Add(TestSubject.ShouldAccept());
            TestSubject.LogFailureResponse(HttpStatusCode.InternalServerError.ToString());
            await Task.Delay(5);
         }

         for (var i = 0; i < 100; i++)
         {
            ClosedPcts.Add(TestSubject.ClosedPct);
            ShouldAccepts.Add(TestSubject.ShouldAccept());
            TestSubject.LogSuccessResponse(HttpStatusCode.OK.ToString());
            await Task.Delay(5);
         }
      }

      [Test]
      public void state_should_start_fully_closed()
      {
         Assert.That(ClosedPcts.First(), Is.EqualTo(100));
      }

      [Test]
      public void state_should_end_fully_closed()
      {
         Assert.That(ClosedPcts.Last(), Is.EqualTo(100));
      }

      [Test]
      public void should_start_by_accepting_requests()
      {
         ShouldAccepts.First().ShouldBe(true);
      }

      [Test]
      public void should_end_by_accepting_requests()
      {
         ShouldAccepts.Last().ShouldBe(true);
      }

      [Test]
      public void fully_opened_should_be_reached()
      {
         Assert.That(ClosedPcts.Any(c => c.Equals(0)), Is.True);
      }
   }
}