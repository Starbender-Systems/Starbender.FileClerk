using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;

namespace Starbender.FileClerk.Demo;

[DependsOn(
    typeof(DemoDomainModule),
    typeof(DemoTestBaseModule)
)]
public class DemoDomainTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(
            ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
    }
}
