using Shouldly;
using System.Threading.Tasks;
using Starbender.FileClerk.Demo.Security;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Xunit;

namespace Starbender.FileClerk.Demo.Samples;

/* This is just an example test class.
 * Normally, you don't test code of the modules you are using
 * (like IIdentityUserAppService here).
 * Only test your own application services.
 */
public abstract class SampleAppServiceTests<TStartupModule> : DemoApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IIdentityUserAppService _userAppService;

    protected SampleAppServiceTests()
    {
        _userAppService = GetRequiredService<IIdentityUserAppService>();
    }

    [Fact]
    public async Task Initial_Data_Should_Contain_Admin_User()
    {
        //Act
        var result = await RunAsAsync(
            AccessScenario.AuthenticatedWithPermission,
            IdentityPermissions.Users.Default,
            () => _userAppService.GetListAsync(new GetIdentityUsersInput()));

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldContain(u => u.UserName == "admin");
    }
}
