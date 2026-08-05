using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Starbender.FileClerk;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class FileClerkInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<FileClerkInstallerModule>();
        });
    }
}
