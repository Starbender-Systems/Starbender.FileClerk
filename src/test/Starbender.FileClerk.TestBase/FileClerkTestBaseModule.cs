using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace Starbender.FileClerk;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpGuidsModule)
)]
public class FileClerkTestBaseModule : AbpModule
{
    private static readonly string[] PersistenceDependentContributors =
    [
        "Volo.Abp.Identity.IdentityDataSeedContributor",
        "Volo.Abp.PermissionManagement.PermissionDataSeedContributor",
        "Starbender.FileClerk.Identity.FileClerkAdministratorDataSeedContributor",
        "Starbender.FileClerk.BlobProviders.FileClerkBlobProviderDataSeedContributor"
    ];

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysAllowAuthorization();
        context.Services.AddSingleton(Substitute.For<IFeatureGroupDefinitionRecordRepository>());
        context.Services.AddSingleton(Substitute.For<IFeatureDefinitionRecordRepository>());
        context.Services.AddSingleton(
            Substitute.For<IPermissionGroupDefinitionRecordRepository>());
        context.Services.AddSingleton(Substitute.For<IPermissionDefinitionRecordRepository>());

        PostConfigure<FeatureManagementOptions>(options =>
        {
            options.SaveStaticFeaturesToDatabase = false;
            options.IsDynamicFeatureStoreEnabled = false;
        });
        PostConfigure<PermissionManagementOptions>(options =>
        {
            options.SaveStaticPermissionsToDatabase = false;
            options.IsDynamicPermissionStoreEnabled = false;
        });

        PostConfigure<AbpDataSeedOptions>(options =>
        {
            foreach (var contributor in options.Contributors
                         .Where(type => PersistenceDependentContributors.Contains(type.FullName))
                         .ToArray())
            {
                options.Contributors.Remove(contributor);
            }
        });
    }

}
