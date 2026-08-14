using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Starbender.FileClerk.BlobProviders;
using Starbender.FileClerk.Configuration;

namespace Starbender.FileClerk.Web.Components.FileClerkSetting;

public sealed class FileClerkSettingViewComponent : ViewComponent
{
    private readonly IFileClerkConfigurationAppService _configurationService;
    private readonly IFileClerkBlobProviderAppService _providerService;

    public FileClerkSettingViewComponent(
        IFileClerkConfigurationAppService configurationService,
        IFileClerkBlobProviderAppService providerService)
    {
        _configurationService = configurationService;
        _providerService = providerService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var configuration = await _configurationService.GetAsync();
        var registry = configuration.CanManageProviderRegistry
            ? await _providerService.GetListAsync()
            : [];

        return View(new FileClerkSettingViewModel
        {
            Configuration = configuration,
            Registry = registry
        });
    }
}
