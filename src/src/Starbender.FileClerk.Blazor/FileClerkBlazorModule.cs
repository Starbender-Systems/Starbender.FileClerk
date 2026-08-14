using Microsoft.Extensions.DependencyInjection;
using Starbender.FileClerk.Blazor.Menus;
using Starbender.FileClerk.Blazor.Settings;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Blazor;
using Volo.Abp.UI.Navigation;

namespace Starbender.FileClerk.Blazor;

[DependsOn(
    typeof(FileClerkApplicationContractsModule),
    typeof(AbpAspNetCoreComponentsWebThemingModule),
    typeof(AbpSettingManagementBlazorModule),
    typeof(AbpMapperlyModule)
)]
public class FileClerkBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<FileClerkBlazorModule>();

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new FileClerkMenuContributor());
        });

        Configure<SettingManagementComponentOptions>(options =>
        {
            options.Contributors.Add(new FileClerkSettingComponentContributor());
        });

        Configure<AbpRouterOptions>(options =>
        {
            options.AdditionalAssemblies.Add(typeof(FileClerkBlazorModule).Assembly);
        });
    }
}
