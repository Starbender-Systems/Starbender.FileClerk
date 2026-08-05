using Starbender.FileClerk.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Starbender.FileClerk.Blazor;

public abstract class FileClerkComponentBase : AbpComponentBase
{
    protected FileClerkComponentBase()
    {
        LocalizationResource = typeof(FileClerkResource);
    }
}