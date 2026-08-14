using Starbender.FileClerk.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Starbender.FileClerk.Permissions;

public sealed class FileClerkPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(
            FileClerkPermissions.GroupName,
            L("FileClerk:PermissionGroup"));

        group.AddPermission(
            FileClerkPermissions.ManageFileClerk,
            L("FileClerk:ManageFileClerkPermission"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FileClerkResource>(name);
    }
}
