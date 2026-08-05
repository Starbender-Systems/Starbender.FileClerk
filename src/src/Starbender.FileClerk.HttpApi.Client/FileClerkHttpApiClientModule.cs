using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Starbender.FileClerk;

[DependsOn(
    typeof(FileClerkApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class FileClerkHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(FileClerkApplicationContractsModule).Assembly,
            FileClerkRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<FileClerkHttpApiClientModule>();
        });

    }
}
