using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Starbender.FileClerk.Demo.Blazor;

public class DemoStyleBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add(new BundleFile("main.css", true));
    }
}
