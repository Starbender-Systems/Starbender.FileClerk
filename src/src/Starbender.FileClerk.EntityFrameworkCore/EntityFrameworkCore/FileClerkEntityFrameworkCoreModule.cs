using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Starbender.FileClerk.EntityFrameworkCore;

[DependsOn(
    typeof(FileClerkDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class FileClerkEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<FileClerkDbContext>(options =>
        {
            options.AddDefaultRepositories<IFileClerkDbContext>(includeAllEntities: true);
            
            /* Add custom repositories here. Example:
            * options.AddRepository<Question, EfCoreQuestionRepository>();
            */
        });
    }
}
