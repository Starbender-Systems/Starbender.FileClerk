using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace Starbender.FileClerk.BlobProviders;
public class FileClerkBlobProvider :
    AuditedAggregateRoot<int>
{
    private static readonly Regex TypeFullNameRegex = new(
        @"^(?:@?[A-Za-z_][A-Za-z0-9_]*(?:`[1-9][0-9]*)?)(?:[.+](?:@?[A-Za-z_][A-Za-z0-9_]*(?:`[1-9][0-9]*)?))*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public virtual string Name { get; protected set; } = null!;

    public virtual string ImplementationType { get; protected set; } = null!;

    public virtual string ConfigurationSchema { get; protected set; } = null!;

    public virtual bool Enabled { get; protected set; }

    public virtual Guid? DeleterId { get; set; }

    public virtual DateTime? DeletionTime { get; set; }

    protected FileClerkBlobProvider()
    {
    }

    public FileClerkBlobProvider(
        string name,
        string implementationType,
        string configurationSchema)
    {
        SetName(name);
        SetImplementationType(implementationType);
        SetConfigurationSchema(configurationSchema);
        Enabled = false;
    }

    public virtual void SetEnabled(bool enabled)
    {
        Enabled = enabled;
    }

    private void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(
            name,
            nameof(name),
            FileClerkBlobProviderConsts.MaxNameLength);
    }

    private void SetImplementationType(string implementationType)
    {
        Check.NotNullOrWhiteSpace(
            implementationType,
            nameof(implementationType),
            FileClerkBlobProviderConsts.MaxImplementationTypeLength);

        if (!TypeFullNameRegex.IsMatch(implementationType))
        {
            throw new BusinessException(FileClerkErrorCodes.InvalidImplementationType)
                .WithData("ImplementationType", implementationType);
        }

        ImplementationType = implementationType;
    }

    private void SetConfigurationSchema(string configurationSchema)
    {
        Check.NotNullOrWhiteSpace(
            configurationSchema,
            nameof(configurationSchema),
            FileClerkBlobProviderConsts.MaxConfigurationSchemaLength);

        try
        {
            using var document = JsonDocument.Parse(configurationSchema);
            if (document.RootElement.ValueKind != JsonValueKind.Object ||
                !document.RootElement.TryGetProperty("connectionString", out _))
            {
                throw new BusinessException(FileClerkErrorCodes.InvalidConfigurationSchema);
            }
        }
        catch (JsonException exception)
        {
            throw new BusinessException(
                FileClerkErrorCodes.InvalidConfigurationSchema,
                innerException: exception);
        }

        ConfigurationSchema = configurationSchema;
    }
}
