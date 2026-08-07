using Microsoft.Extensions.Localization;
using Starbender.FileClerk.Demo.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Starbender.FileClerk.Demo.Web;

[Dependency(ReplaceServices = true)]
public class DemoWebBrandingProvider : DefaultBrandingProvider
{
    private readonly IStringLocalizer<DemoResource> _localizer;

    public DemoWebBrandingProvider(IStringLocalizer<DemoResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
