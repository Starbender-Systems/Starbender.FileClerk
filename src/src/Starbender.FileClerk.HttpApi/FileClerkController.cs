using Starbender.FileClerk.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Starbender.FileClerk;

public abstract class FileClerkController : AbpControllerBase
{
    protected FileClerkController()
    {
        LocalizationResource = typeof(FileClerkResource);
    }
}
