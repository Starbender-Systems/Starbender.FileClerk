using Localization.Resources.AbpUi;
using Starbender.FileClerk.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Starbender.FileClerk;

[DependsOn(
    typeof(FileClerkApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class FileClerkHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(FileClerkHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<FileClerkResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
