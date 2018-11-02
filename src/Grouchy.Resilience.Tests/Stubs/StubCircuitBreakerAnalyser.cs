using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.Resilience.Tests.Stubs
{
   public class StubCircuitBreakerAnalyser : ICircuitBreakerAnalyser
   {
      public ConcurrentQueue<Rating> AnalysisResponses { get; }
      
      public StubCircuitBreakerAnalyser(params Rating[] analysisResponses)
      {
         AnalysisResponses = new ConcurrentQueue<Rating>(analysisResponses);
      }
      
      public Rating Analyse(IEnumerable<Metrics> lastMetrics)
      {
         if (AnalysisResponses.TryDequeue(out var response))
         {
            return response;
         }
         
         throw new InvalidOperationException("No more responses queued");
      }
   }
}