using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Shouldly;
using Starbender.FileClerk.Localization;
using Starbender.FileClerk.Samples;
using Volo.Abp.Authorization.Permissions;
using Xunit;

namespace Starbender.FileClerk.EntityFrameworkCore;

public class FileClerkCoreModuleComposition_Tests : FileClerkEntityFrameworkCoreTestBase
{
    [Fact]
    public void Core_Services_Should_Be_Resolvable()
    {
        GetRequiredService<ISampleAppService>().ShouldNotBeNull();
        GetRequiredService<IPermissionDefinitionManager>().ShouldNotBeNull();
        GetRequiredService<IStringLocalizer<FileClerkResource>>().ShouldNotBeNull();

        using var scope = ServiceProvider.CreateScope();
        scope.ServiceProvider.GetRequiredService<FileClerkDbContext>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<IFileClerkDbContext>().ShouldNotBeNull();
    }
}
