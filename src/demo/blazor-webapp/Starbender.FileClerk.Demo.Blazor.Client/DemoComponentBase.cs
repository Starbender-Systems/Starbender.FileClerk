using Starbender.FileClerk.Demo.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Starbender.FileClerk.Demo.Blazor.Client;

public abstract class DemoComponentBase : AbpComponentBase
{
    protected DemoComponentBase()
    {
        LocalizationResource = typeof(DemoResource);
    }
}
