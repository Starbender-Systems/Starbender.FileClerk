using System;

namespace Starbender.FileClerk.BlobProviders;

public sealed class FileClerkBlobProviderDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImplementationType { get; set; } = string.Empty;
    public string ConfigurationSchema { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
}
