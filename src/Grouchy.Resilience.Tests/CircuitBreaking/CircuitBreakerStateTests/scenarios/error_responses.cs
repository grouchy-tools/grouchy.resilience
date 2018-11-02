using System.Linq;
using System.Threading.Tasks;
using Grouchy.Resilience.Tests.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.Resilience.Tests.CircuitBreaking.CircuitBreakerStateTests.scenarios
{
   // ReSharper disable once InconsistentNaming
   public class error_responses : scenario_base
   {
      protected override async Task RunScenario()
      {
         for (var i = 0; i < 200; i++)
         {
            ClosedPcts.Add(TestSubject.ClosedPct);
            ShouldAccepts.Add(TestSubject.ShouldAccept());
            TestSubject.LogFailureResponse(i.ToString());
            
            await Task.Delay(5);
         }
      }

      [Test]
      public void state_should_start_fully_closed()
      {
         Assert.That(ClosedPcts.First(), Is.EqualTo(100));
      }

      [Test]
      public void state_should_end_fully_open()
      {
         Assert.That(ClosedPcts.Last(), Is.EqualTo(0));
      }

      [Test]
      public void should_start_by_accepting_requests()
      {
         ShouldAccepts.First().ShouldBe(true);
      }

      [Test]
      public void should_end_by_not_accepting_requests()
      {
         ShouldAccepts.Last().ShouldBe(false);
      }

      [Test]
      public void state_should_progressively_get_more_open()
      {
         var groups = ClosedPcts.Batch(20).ToArray();

         for (var i = 1; i < groups.Length; i++)
         {
            Assert.That(groups[i].Count(b => false), Is.GreaterThanOrEqualTo(groups[i-1].Count(b => false)));
         }
      }
   }
}