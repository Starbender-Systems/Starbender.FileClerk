using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Starbender.FileClerk;

[DependsOn(
    typeof(FileClerkDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class FileClerkApplicationContractsModule : AbpModule
{

}
