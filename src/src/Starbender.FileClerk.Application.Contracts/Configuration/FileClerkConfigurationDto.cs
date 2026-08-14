using System.Collections.Generic;

namespace Starbender.FileClerk.Configuration;

public sealed class FileClerkConfigurationDto
{
    public bool EffectiveEnabled { get; set; }
    public IReadOnlyList<int> SelectedProviderIds { get; set; } = [];
    public IReadOnlyList<FileClerkProviderChoiceDto> AvailableProviders { get; set; } = [];
    public bool CanManageProviderRegistry { get; set; }
}
