using Starbender.FileClerk.BlobProviders;
using Starbender.FileClerk.Features;
using Starbender.FileClerk.Identity;
using Volo.Abp.Data;
using Volo.Abp.Domain;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace Starbender.FileClerk;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpPermissionManagementDomainModule),
    typeof(FileClerkDomainSharedModule)
)]
public class FileClerkDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpFeatureOptions>(options =>
        {
            options.ValueProviders.Add<FileClerkFeatureValueProvider>();
        });

        PostConfigure<AbpDataSeedOptions>(options =>
        {
            options.Contributors.Remove<FileClerkBlobProviderDataSeedContributor>();
            options.Contributors.Remove<FileClerkAdministratorDataSeedContributor>();
            options.Contributors.Add<FileClerkBlobProviderDataSeedContributor>();
            options.Contributors.Add<FileClerkAdministratorDataSeedContributor>();
        });
    }
}
