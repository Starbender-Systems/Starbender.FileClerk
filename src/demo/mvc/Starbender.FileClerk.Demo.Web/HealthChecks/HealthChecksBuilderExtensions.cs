using System;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Starbender.FileClerk.Demo.Web.HealthChecks;

public static class HealthChecksBuilderExtensions
{
    public static void AddDemoWebHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck<DemoWebDatabaseCheck>(
            "Demo MVC database check",
            tags: ["database"]
        );
        services.ConfigureHealthCheckEndpoint("/health-status");

        var configuration = services.GetConfiguration();
        var healthCheckUrl = configuration["App:HealthCheckUrl"];
        if (string.IsNullOrEmpty(healthCheckUrl))
        {
            healthCheckUrl = "/health-status";
        }

        services
            .AddHealthChecksUI(settings =>
            {
                settings.AddHealthCheckEndpoint(
                    "Demo MVC Health Status",
                    configuration["App:HealthUiCheckUrl"] ?? healthCheckUrl
                );
            })
            .AddInMemoryStorage();

        services.MapHealthChecksUiEndpoints(options =>
        {
            options.UIPath = "/health-ui";
            options.ApiPath = "/health-api";
        });
    }

    private static IServiceCollection ConfigureHealthCheckEndpoint(
        this IServiceCollection services,
        string path
    )
    {
        services.Configure<AbpEndpointRouterOptions>(options =>
        {
            options.EndpointConfigureActions.Add(endpointContext =>
            {
                endpointContext.Endpoints.MapHealthChecks(
                    new PathString(path.EnsureStartsWith('/')),
                    new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                        AllowCachingResponses = false,
                    }
                );
            });
        });
        return services;
    }

    private static IServiceCollection MapHealthChecksUiEndpoints(
        this IServiceCollection services,
        Action<global::HealthChecks.UI.Configuration.Options>? setupOption = null
    )
    {
        services.Configure<AbpEndpointRouterOptions>(options =>
        {
            options.EndpointConfigureActions.Add(endpointContext =>
            {
                endpointContext.Endpoints.MapHealthChecksUI(setupOption);
            });
        });
        return services;
    }
}
