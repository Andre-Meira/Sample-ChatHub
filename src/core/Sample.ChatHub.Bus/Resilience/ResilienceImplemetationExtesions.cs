using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

namespace Sample.ChatHub.Bus.Resilience;

internal static class ResilienceImplemetationExtesions
{
    public static IServiceCollection AddResilienceBusc(
        this IServiceCollection services, AsyncPolicy retryPolicy)
    {
        services.AddSingleton(e => retryPolicy);                               

        return services;
    }
}
