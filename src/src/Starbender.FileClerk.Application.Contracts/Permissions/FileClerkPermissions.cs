using Volo.Abp.Reflection;

namespace Starbender.FileClerk.Permissions;

public class FileClerkPermissions
{
    public const string GroupName = "FileClerk";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(FileClerkPermissions));
    }
}
