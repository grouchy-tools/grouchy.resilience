using Grouchy.Resilience.Retrying;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.Resilience.Tests.Retrying
{
   public class ExponentialRetryDelayTests
   {
      
      [TestCase(2, 2, 1)]
      [TestCase(2, 4, 2)]
      [TestCase(2, 8, 3)]
      [TestCase(2, 16, 4)]
      [TestCase(2, 1024, 10)]
      [TestCase(100, 100, 1)]
      [TestCase(100, 200, 2)]
      [TestCase(100, 400, 3)]
      [TestCase(100, 800, 4)]
      public void returns_increasing_larger_delay(int initialDelayMs, int expectedDelayMs, int retryAttempt)
      {
         var testSubject = new ExponentialRetryDelay(initialDelayMs);

         testSubject.DelayMs(retryAttempt).ShouldBe(expectedDelayMs);
      }
   }
}
