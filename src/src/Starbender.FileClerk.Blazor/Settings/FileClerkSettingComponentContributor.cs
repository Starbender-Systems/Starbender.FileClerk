using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Starbender.FileClerk.Localization;
using Starbender.FileClerk.Permissions;
using Volo.Abp.SettingManagement.Blazor;

namespace Starbender.FileClerk.Blazor.Settings;

public sealed class FileClerkSettingComponentContributor : ISettingComponentContributor
{
    public async Task ConfigureAsync(SettingComponentCreationContext context)
    {
        if (!await CheckPermissionsAsync(context))
        {
            return;
        }

        var localizer = context.ServiceProvider
            .GetRequiredService<IStringLocalizer<FileClerkResource>>();

        context.Groups.Add(new SettingComponentGroup(
            "FileClerk",
            localizer["FileClerk:FeatureManagement"],
            typeof(FileClerkSettingComponent),
            order: 110));
    }

    public Task<bool> CheckPermissionsAsync(SettingComponentCreationContext context)
    {
        return context.ServiceProvider
            .GetRequiredService<IAuthorizationService>()
            .IsGrantedAsync(FileClerkPermissions.ManageFileClerk);
    }
}
