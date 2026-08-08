using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Starbender.FileClerk.Demo.Security;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Modularity;
using Xunit;

namespace Starbender.FileClerk.Demo.Authorization;

public abstract class AdministrativeApplicationAccessTests<TStartupModule> :
    DemoApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    public static IEnumerable<object[]> AccessCaseIds =>
        AdministrativeAccessCases.All.Select(accessCase => new object[] { accessCase.Id });

    [Fact]
    public async Task Access_Catalog_Should_Be_Valid()
    {
        var cases = AdministrativeAccessCases.All;
        cases.Count.ShouldBeGreaterThan(0);
        cases.Select(accessCase => accessCase.Id).Distinct().Count().ShouldBe(cases.Count);

        var definitionManager = GetRequiredService<IPermissionDefinitionManager>();
        foreach (var accessCase in cases)
        {
            accessCase.Id.ShouldNotBeNullOrWhiteSpace();
            accessCase.RequiredPermission.ShouldNotBeNullOrWhiteSpace();
            (await definitionManager.GetOrNullAsync(accessCase.RequiredPermission)).ShouldNotBeNull();
            accessCase.CreateHttpRequest.ShouldNotBeNull();
            accessCase.InvokeApplicationAsync.ShouldNotBeNull();
        }
    }

    [Theory]
    [MemberData(nameof(AccessCaseIds))]
    public async Task Application_Action_Should_Enforce_Access_Matrix(string accessCaseId)
    {
        var accessCase = AdministrativeAccessCases.Find(accessCaseId);
        var state = await RunWithAllPermissionsAsync(
            () => accessCase.ArrangeAsync(ServiceProvider));

        await AssertDeniedAsync(accessCase, state, AccessScenario.Anonymous);
        await AssertDeniedAsync(
            accessCase,
            state,
            AccessScenario.AuthenticatedWithoutPermission);

        await RunAsAsync(
            AccessScenario.AuthenticatedWithPermission,
            accessCase.RequiredPermission,
            () => accessCase.InvokeApplicationAsync(ServiceProvider, state));
    }

    private async Task AssertDeniedAsync(
        AdministrativeAccessCase accessCase,
        object? state,
        AccessScenario scenario)
    {
        await Should.ThrowAsync<AbpAuthorizationException>(
            () => RunAsAsync(
                scenario,
                accessCase.RequiredPermission,
                () => accessCase.InvokeApplicationAsync(ServiceProvider, state)));
    }
}
