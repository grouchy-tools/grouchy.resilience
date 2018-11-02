using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.Resilience.Tests.CircuitBreaking.CircuitBreakerStateTests.scenarios
{
   // ReSharper disable once InconsistentNaming
   public class successful_responses : scenario_base
   {
      protected override async Task RunScenario()
      {
         for (var i = 200; i < 400; i++)
         {
            ClosedPcts.Add(TestSubject.ClosedPct);
            ShouldAccepts.Add(TestSubject.ShouldAccept());
            TestSubject.LogSuccessResponse(i.ToString());
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
      public void should_remain_fully_open()
      {
         Assert.That(ClosedPcts.All(c => c.Equals(100)), Is.True);
      }
      
      [Test]
      public void should_remain_accepting_all_requests()
      {
         Assert.That(ShouldAccepts.All(c => c), Is.True);
      }
   }
}