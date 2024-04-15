using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Sample.ChatHub.Bus.Resilience;

internal static class WaitAndRetryExtensions
{
    public static AsyncRetryPolicy CreateWaitAndRetryPolicy(IEnumerable<TimeSpan> sleepsBeetweenRetries, ILogger logger)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                sleepDurations: sleepsBeetweenRetries,
                onRetry: (_, span, retryCount, _) =>
                {
                    logger.LogWarning($" ***** {DateTime.Now:HH:mm:ss} | " +
                        $"Retentativa: {retryCount} | " +
                        $"Tempo de Espera em segundos: {span.TotalSeconds} **** ");
                });
    }    
}
