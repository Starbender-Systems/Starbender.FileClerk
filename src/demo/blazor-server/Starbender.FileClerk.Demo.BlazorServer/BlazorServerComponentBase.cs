using Starbender.FileClerk.Demo.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Starbender.FileClerk.Demo.BlazorServer.Blazor;

public abstract class BlazorServerComponentBase : AbpComponentBase
{
    protected BlazorServerComponentBase()
    {
        LocalizationResource = typeof(DemoResource);
    }
}
