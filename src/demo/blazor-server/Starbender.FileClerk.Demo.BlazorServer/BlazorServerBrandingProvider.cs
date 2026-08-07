using Microsoft.Extensions.Localization;
using Starbender.FileClerk.Demo.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Starbender.FileClerk.Demo.BlazorServer.Blazor;

[Dependency(ReplaceServices = true)]
public class BlazorServerBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DemoResource> _localizer;

    public BlazorServerBrandingProvider(IStringLocalizer<DemoResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
