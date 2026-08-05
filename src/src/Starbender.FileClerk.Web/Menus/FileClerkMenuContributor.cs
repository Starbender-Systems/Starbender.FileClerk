using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace Starbender.FileClerk.Web.Menus;

public class FileClerkMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        //Add main menu items.
        context.Menu.AddItem(new ApplicationMenuItem(FileClerkMenus.Prefix, displayName: "FileClerk", "~/FileClerk", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
