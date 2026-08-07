using Microsoft.Extensions.Localization;
using Starbender.FileClerk.Demo.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Starbender.FileClerk.Demo.BlazorWebAssembly.Blazor.Client;

[Dependency(ReplaceServices = true)]
public class BlazorWebAssemblyBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DemoResource> _localizer;

    public BlazorWebAssemblyBrandingProvider(IStringLocalizer<DemoResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
