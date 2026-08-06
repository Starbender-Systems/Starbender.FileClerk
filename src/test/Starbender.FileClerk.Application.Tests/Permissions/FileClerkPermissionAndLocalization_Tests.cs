using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Shouldly;
using Starbender.FileClerk.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.VirtualFileSystem;
using Xunit;

namespace Starbender.FileClerk.Permissions;

public class FileClerkPermissionAndLocalization_Tests :
    FileClerkApplicationTestBase<FileClerkApplicationTestModule>
{
    [Fact]
    public async Task Permission_Group_Should_Be_Defined_With_A_Localized_Display_Name()
    {
        var definitionManager = GetRequiredService<IPermissionDefinitionManager>();
        var group = (await definitionManager.GetGroupsAsync())
            .SingleOrDefault(x => x.Name == FileClerkPermissions.GroupName);

        group.ShouldNotBeNull();
        var displayName = group.DisplayName.ShouldBeOfType<LocalizableString>();
        displayName.Name.ShouldBe("Permission:FileClerk");

        var localizer = GetRequiredService<IStringLocalizer<FileClerkResource>>();
        var localizedName = localizer[displayName.Name];
        localizedName.ResourceNotFound.ShouldBeFalse();
        localizedName.Value.ShouldBe("FileClerk");
    }

    [Fact]
    public void Localization_Files_Should_Be_Valid_And_Have_The_Default_Key_Set()
    {
        var virtualFileProvider = GetRequiredService<IVirtualFileProvider>();
        var files = virtualFileProvider
            .GetDirectoryContents("/Localization/FileClerk")
            .Where(file => !file.IsDirectory && file.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            .OrderBy(file => file.Name)
            .ToList();

        files.ShouldNotBeEmpty();

        var resources = files.ToDictionary(
            file => Path.GetFileNameWithoutExtension(file.Name),
            file => ReadLocalizationFile(file),
            StringComparer.OrdinalIgnoreCase);

        resources.ShouldContainKey("en");
        var defaultKeys = resources["en"].Texts.Keys.OrderBy(key => key).ToArray();

        foreach (var (fileCulture, resource) in resources)
        {
            resource.Culture.ShouldBe(fileCulture);
            resource.Texts.Keys.OrderBy(key => key).ShouldBe(defaultKeys);
            resource.Texts.Values.ShouldAllBe(value => !string.IsNullOrWhiteSpace(value));
            _ = CultureInfo.GetCultureInfo(resource.Culture);
        }
    }

    private static LocalizationFile ReadLocalizationFile(Microsoft.Extensions.FileProviders.IFileInfo file)
    {
        using var stream = file.CreateReadStream();
        return JsonSerializer.Deserialize<LocalizationFile>(
                   stream,
                   new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
               ?? throw new InvalidDataException($"Could not deserialize localization file '{file.Name}'.");
    }

    private sealed record LocalizationFile(string Culture, Dictionary<string, string> Texts);
}
