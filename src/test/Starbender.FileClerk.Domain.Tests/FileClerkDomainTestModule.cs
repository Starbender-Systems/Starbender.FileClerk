using Volo.Abp.Modularity;

namespace Starbender.FileClerk;

[DependsOn(
    typeof(FileClerkDomainModule),
    typeof(FileClerkTestBaseModule)
)]
public class FileClerkDomainTestModule : AbpModule
{

}
