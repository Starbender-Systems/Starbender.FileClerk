namespace Starbender.FileClerk;

public static class FileClerkErrorCodes
{
    public const string InvalidProviderName = "FileClerk:InvalidProviderName";
    public const string InvalidImplementationType = "FileClerk:InvalidImplementationType";
    public const string InvalidConfigurationSchema = "FileClerk:InvalidConfigurationSchema";
    public const string ProviderNotFound = "FileClerk:ProviderNotFound";
    public const string ProviderDisabled = "FileClerk:ProviderDisabled";
    public const string ProviderNotSelected = "FileClerk:ProviderNotSelected";
    public const string DuplicateProviderSelection = "FileClerk:DuplicateProviderSelection";
    public const string ProviderSelectionOutOfScope = "FileClerk:ProviderSelectionOutOfScope";
    public const string HostFileClerkDisabled = "FileClerk:HostFileClerkDisabled";
    public const string HostOnlyOperation = "FileClerk:HostOnlyOperation";
}
