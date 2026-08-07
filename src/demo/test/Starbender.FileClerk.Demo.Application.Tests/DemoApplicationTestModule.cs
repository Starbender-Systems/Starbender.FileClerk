using Volo.Abp.Modularity;

namespace Starbender.FileClerk.Demo;

[DependsOn(
    typeof(DemoApplicationModule),
    typeof(DemoDomainTestModule)
)]
public class DemoApplicationTestModule : AbpModule
{

}
