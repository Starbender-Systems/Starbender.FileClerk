using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Application;
using Volo.Abp.BlobStoring;

namespace Starbender.FileClerk;

[DependsOn(
    typeof(FileClerkDomainModule),
    typeof(FileClerkApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule)
    )]
[DependsOn(typeof(AbpBlobStoringModule))]
    public class FileClerkApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<FileClerkApplicationModule>();
    }
}
