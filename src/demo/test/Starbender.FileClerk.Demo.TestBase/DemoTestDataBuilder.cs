using System;
using System.Threading.Tasks;
using Starbender.FileClerk.Demo.Security;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Starbender.FileClerk.Demo;

public class DemoTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IdentityUserManager _userManager;

    public DemoTestDataSeedContributor(
        ICurrentTenant currentTenant,
        IIdentityUserRepository userRepository,
        IdentityUserManager userManager)
    {
        _currentTenant = currentTenant;
        _userRepository = userRepository;
        _userManager = userManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            if (context?.TenantId != null ||
                await _userRepository.FindAsync(TestPrincipalFactory.UserId) != null)
            {
                return;
            }

            var user = new IdentityUser(
                TestPrincipalFactory.UserId,
                "access-test-admin",
                "access-test-admin@example.test");

            var result = await _userManager.CreateAsync(user, "1q2w3E*");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Could not create the authorization test user: {string.Join(", ", result.Errors)}");
            }
        }
    }
}
