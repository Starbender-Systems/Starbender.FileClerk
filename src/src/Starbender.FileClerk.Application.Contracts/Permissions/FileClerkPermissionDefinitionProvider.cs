using Starbender.FileClerk.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Starbender.FileClerk.Permissions;

public class FileClerkPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(FileClerkPermissions.GroupName, L("Permission:FileClerk"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FileClerkResource>(name);
    }
}
