using Volo.Abp.Reflection;

namespace Starbender.FileClerk.Permissions;

public static class FileClerkPermissions
{
    public const string GroupName = "FileClerk";
    public const string ManageFileClerk = GroupName + ".ManageFileClerk";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(FileClerkPermissions));
    }
}
