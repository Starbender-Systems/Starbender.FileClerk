using Starbender.FileClerk.Demo.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Starbender.FileClerk.Demo.BlazorWebAssembly.Blazor.Client;

public abstract class BlazorWebAssemblyComponentBase : AbpComponentBase
{
    protected BlazorWebAssemblyComponentBase()
    {
        LocalizationResource = typeof(DemoResource);
    }
}
