using Starbender.FileClerk.Localization;
using Volo.Abp.Application.Services;

namespace Starbender.FileClerk;

public abstract class FileClerkAppService : ApplicationService
{
    protected FileClerkAppService()
    {
        LocalizationResource = typeof(FileClerkResource);
        ObjectMapperContext = typeof(FileClerkApplicationModule);
    }
}
