using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Starbender.FileClerk.Samples;
using Xunit;

namespace Starbender.FileClerk.Demo.EntityFrameworkCore;

[Collection(DemoTestConsts.CollectionDefinitionName)]
public class FileClerkIntegrationTests : DemoEntityFrameworkCoreTestBase
{
    [Fact]
    public async Task FileClerk_Application_Service_Should_Be_Available()
    {
        var service = GetRequiredService<ISampleAppService>();

        var result = await service.GetAsync();

        result.Value.ShouldBe(42);
    }

    [Fact]
    public void FileClerk_DbContext_Should_Be_Replaced_By_Demo_DbContext()
    {
        using var scope = ServiceProvider.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<global::Starbender.FileClerk.EntityFrameworkCore.IFileClerkDbContext>();

        dbContext.ShouldBeOfType<DemoDbContext>();
    }
}
