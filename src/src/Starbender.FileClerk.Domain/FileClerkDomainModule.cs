using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Starbender.FileClerk;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(FileClerkDomainSharedModule)
)]
public class FileClerkDomainModule : AbpModule
{

}
