using System.Collections.Generic;
using Starbender.FileClerk.BlobProviders;
using Starbender.FileClerk.Configuration;

namespace Starbender.FileClerk.Web.Components.FileClerkSetting;

public sealed class FileClerkSettingViewModel
{
    public FileClerkConfigurationDto Configuration { get; init; } = new();
    public IReadOnlyList<FileClerkBlobProviderDto> Registry { get; init; } = [];
}
