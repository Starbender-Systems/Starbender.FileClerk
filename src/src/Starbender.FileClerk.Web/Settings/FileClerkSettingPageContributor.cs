using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Starbender.FileClerk.Localization;
using Starbender.FileClerk.Permissions;
using Starbender.FileClerk.Web.Components.FileClerkSetting;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;

namespace Starbender.FileClerk.Web.Settings;

public sealed class FileClerkSettingPageContributor : SettingPageContributorBase
{
    public FileClerkSettingPageContributor()
    {
        RequiredPermissions(FileClerkPermissions.ManageFileClerk);
    }

    public override Task ConfigureAsync(SettingPageCreationContext context)
    {
        var localizer = context.ServiceProvider
            .GetRequiredService<IStringLocalizer<FileClerkResource>>();

        context.Groups.Add(new SettingPageGroup(
            "FileClerk",
            localizer["FileClerk:FeatureManagement"],
            typeof(FileClerkSettingViewComponent),
            order: 110));

        return Task.CompletedTask;
    }
}
