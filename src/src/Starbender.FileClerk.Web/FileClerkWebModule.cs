using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starbender.FileClerk.Localization;
using Starbender.FileClerk.Web.Menus;
using Starbender.FileClerk.Web.Settings;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace Starbender.FileClerk.Web;

[DependsOn(
    typeof(FileClerkApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpSettingManagementWebModule),
    typeof(AbpMapperlyModule)
)]
public class FileClerkWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(FileClerkResource), typeof(FileClerkWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(FileClerkWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new FileClerkMenuContributor());
        });

        Configure<SettingManagementPageOptions>(options =>
        {
            options.Contributors.Add(new FileClerkSettingPageContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<FileClerkWebModule>();
        });

        context.Services.AddMapperlyObjectMapper<FileClerkWebModule>();

        Configure<RazorPagesOptions>(_ => { });

        if (hostingEnvironment.IsDevelopment())
        {
            context.Services.AddRazorPages()
                .AddRazorRuntimeCompilation();
        }
    }
}
