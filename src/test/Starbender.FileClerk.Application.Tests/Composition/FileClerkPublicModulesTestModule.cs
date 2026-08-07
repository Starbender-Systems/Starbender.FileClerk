using Volo.Abp.Modularity;

namespace Starbender.FileClerk.Composition;

[DependsOn(
    typeof(FileClerkTestBaseModule),
    typeof(FileClerkApplicationModule),
    typeof(FileClerkHttpApiModule),
    typeof(FileClerkHttpApiClientModule),
    typeof(FileClerk.Web.FileClerkWebModule),
    typeof(FileClerk.Blazor.Server.FileClerkBlazorServerModule),
    typeof(FileClerk.Blazor.WebAssembly.Bundling.FileClerkBlazorWebAssemblyBundlingModule),
    typeof(FileClerkInstallerModule))]
public class FileClerkPublicModulesTestModule : AbpModule
{
}
