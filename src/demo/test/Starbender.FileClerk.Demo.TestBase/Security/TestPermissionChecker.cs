using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace Starbender.FileClerk.Demo.Security;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IPermissionChecker))]
public class TestPermissionChecker : IPermissionChecker, ITransientDependency
{
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    public TestPermissionChecker(ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        _currentPrincipalAccessor = currentPrincipalAccessor;
    }

    public Task<bool> IsGrantedAsync(string name)
    {
        return IsGrantedAsync(_currentPrincipalAccessor.Principal, name);
    }

    public Task<bool> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string name)
    {
        return Task.FromResult(IsGranted(claimsPrincipal, name));
    }

    public Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names)
    {
        return IsGrantedAsync(_currentPrincipalAccessor.Principal, names);
    }

    public Task<MultiplePermissionGrantResult> IsGrantedAsync(
        ClaimsPrincipal? claimsPrincipal,
        string[] names)
    {
        var result = new MultiplePermissionGrantResult();
        foreach (var name in names)
        {
            result.Result[name] = IsGranted(claimsPrincipal, name)
                ? PermissionGrantResult.Granted
                : PermissionGrantResult.Prohibited;
        }

        return Task.FromResult(result);
    }

    private static bool IsGranted(ClaimsPrincipal? principal, string name)
    {
        return principal?.Identity?.IsAuthenticated == true &&
               principal.FindAll(TestAuthorizationClaimTypes.Permission)
                   .Any(claim => claim.Value == name);
    }
}
