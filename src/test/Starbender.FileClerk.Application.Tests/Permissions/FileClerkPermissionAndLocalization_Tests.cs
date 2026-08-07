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
        localizedName.Value.ShouldBe("File Clerk");
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
