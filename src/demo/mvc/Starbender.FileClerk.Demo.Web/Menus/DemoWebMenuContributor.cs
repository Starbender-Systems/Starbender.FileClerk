using System.Threading.Tasks;
using Starbender.FileClerk.Demo.Localization;
using Starbender.FileClerk.Demo.MultiTenancy;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace Starbender.FileClerk.Demo.Web.Menus;

public class DemoWebMenuContributor : IMenuContributor
{
    public Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var localizer = context.GetLocalizer<DemoResource>();
        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                DemoWebMenus.Home,
                localizer["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        var administration = context.Menu.GetAdministration();
        administration.Order = 6;
        var multiTenancyEnabled = MultiTenancyConsts.IsEnabled;
        if (multiTenancyEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);
        return Task.CompletedTask;
    }
}
