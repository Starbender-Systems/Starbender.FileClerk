using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Starbender.FileClerk.Configuration;

public sealed class UpdateFileClerkConfigurationDto
{
    public bool Enabled { get; set; }

    [Required]
    public IReadOnlyList<int> ProviderIds { get; set; } = [];
}
