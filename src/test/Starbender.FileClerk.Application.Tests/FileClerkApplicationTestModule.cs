using Volo.Abp.Modularity;

namespace Starbender.FileClerk;

[DependsOn(
    typeof(FileClerkApplicationModule),
    typeof(FileClerkDomainTestModule)
    )]
public class FileClerkApplicationTestModule : AbpModule
{

}
