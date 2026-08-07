using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Starbender.FileClerk.Demo.BlazorWebAssembly.Blazor;

public class BlazorWebAssemblyStyleBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add(new BundleFile("main.css", true));
    }
}
