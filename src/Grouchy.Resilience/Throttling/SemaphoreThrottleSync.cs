using System.Threading;
using System.Threading.Tasks;
using Grouchy.Resilience.Abstractions.Throttling;

namespace Grouchy.Resilience.Throttling
{
   // TODO: Should support IDisposable
   public class SemaphoreThrottleSync : IThrottleSync
   {
      private readonly SemaphoreSlim _semaphore;

      public SemaphoreThrottleSync(int concurrentRequests)
      {
         _semaphore = new SemaphoreSlim(concurrentRequests);
      }

      public Task WaitAsync()
      {
         return _semaphore.WaitAsync();
      }

      public void Release()
      {
         _semaphore.Release();
      }
   }
}
