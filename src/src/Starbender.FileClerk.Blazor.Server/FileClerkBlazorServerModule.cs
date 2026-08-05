using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace Starbender.FileClerk.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(FileClerkBlazorModule)
    )]
public class FileClerkBlazorServerModule : AbpModule
{

}
