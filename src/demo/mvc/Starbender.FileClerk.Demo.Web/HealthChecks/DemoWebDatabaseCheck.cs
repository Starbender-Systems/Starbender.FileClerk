using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace Starbender.FileClerk.Demo.Web.HealthChecks;

public class DemoWebDatabaseCheck : IHealthCheck, ITransientDependency
{
    private readonly IIdentityRoleRepository _identityRoleRepository;

    public DemoWebDatabaseCheck(IIdentityRoleRepository identityRoleRepository)
    {
        _identityRoleRepository = identityRoleRepository;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _identityRoleRepository.GetListAsync(
                sorting: nameof(IdentityRole.Id),
                maxResultCount: 1,
                cancellationToken: cancellationToken
            );
            return HealthCheckResult.Healthy("Could connect to the shared demo database.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Could not query the shared demo database.", exception);
        }
    }
}
