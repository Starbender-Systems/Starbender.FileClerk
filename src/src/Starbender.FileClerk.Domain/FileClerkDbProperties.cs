namespace Starbender.FileClerk;

public static class FileClerkDbProperties
{
    public static string DbTablePrefix { get; set; } = "FileClerk";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "FileClerk";
}
