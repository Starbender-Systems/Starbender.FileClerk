using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace Starbender.FileClerk.Demo.Security;

public class TestPrincipalFactory : ITransientDependency
{
    public static readonly Guid UserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");

    private readonly IPermissionDefinitionManager _permissionDefinitionManager;

    public TestPrincipalFactory(IPermissionDefinitionManager permissionDefinitionManager)
    {
        _permissionDefinitionManager = permissionDefinitionManager;
    }

    public async Task<ClaimsPrincipal> CreateAsync(
        AccessScenario scenario,
        string requiredPermission)
    {
        if (scenario == AccessScenario.Anonymous)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        return scenario == AccessScenario.AuthenticatedWithPermission
            ? await CreateWithPermissionsAsync([requiredPermission])
            : CreateAuthenticatedPrincipal([]);
    }

    public async Task<ClaimsPrincipal> CreateWithAllPermissionsAsync()
    {
        var permissions = await _permissionDefinitionManager.GetPermissionsAsync();
        return await CreateWithPermissionsAsync(permissions.Select(permission => permission.Name));
    }

    public async Task<ClaimsPrincipal> CreateWithPermissionsAsync(
        IEnumerable<string> permissionNames)
    {
        var grantedPermissions = new HashSet<string>();

        foreach (var permissionName in permissionNames)
        {
            var permission = await _permissionDefinitionManager.GetAsync(permissionName);
            while (permission != null)
            {
                grantedPermissions.Add(permission.Name);
                permission = permission.Parent;
            }
        }

        return CreateAuthenticatedPrincipal(grantedPermissions);
    }

    private static ClaimsPrincipal CreateAuthenticatedPrincipal(
        IEnumerable<string> permissionNames)
    {
        var claims = new List<Claim>
        {
            new(AbpClaimTypes.UserId, UserId.ToString()),
            new(AbpClaimTypes.UserName, "admin"),
            new(AbpClaimTypes.Email, "admin@abp.io")
        };

        claims.AddRange(permissionNames.Select(
            permissionName => new Claim(TestAuthorizationClaimTypes.Permission, permissionName)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}
