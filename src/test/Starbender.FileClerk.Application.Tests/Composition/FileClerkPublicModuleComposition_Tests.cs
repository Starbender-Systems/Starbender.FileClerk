using System.Linq;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;

namespace Starbender.FileClerk.Composition;

public class FileClerkPublicModuleComposition_Tests : FileClerkTestBase<FileClerkPublicModulesTestModule>
{
    [Fact]
    public void Supported_Public_Modules_Should_Initialize_Together()
    {
        var loadedModuleTypes = GetRequiredService<IModuleContainer>()
            .Modules
            .Select(module => module.Type)
            .ToHashSet();

        var expectedModuleTypes = new[]
        {
            typeof(FileClerkApplicationContractsModule),
            typeof(FileClerkApplicationModule),
            typeof(FileClerkHttpApiModule),
            typeof(FileClerkHttpApiClientModule),
            typeof(FileClerk.Web.FileClerkWebModule),
            typeof(FileClerk.Blazor.FileClerkBlazorModule),
            typeof(FileClerk.Blazor.Server.FileClerkBlazorServerModule),
            typeof(FileClerk.Blazor.WebAssembly.Bundling.FileClerkBlazorWebAssemblyBundlingModule),
            typeof(FileClerkInstallerModule)
        };

        foreach (var expectedModuleType in expectedModuleTypes)
        {
            loadedModuleTypes.ShouldContain(expectedModuleType);
        }
    }
}
