using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace Starbender.FileClerk.Blazor.WebAssembly;

[DependsOn(
    typeof(FileClerkBlazorModule),
    typeof(FileClerkHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class FileClerkBlazorWebAssemblyModule : AbpModule
{

}
