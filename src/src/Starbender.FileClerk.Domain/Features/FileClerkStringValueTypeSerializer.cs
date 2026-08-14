using System;
using System.Text.Json;
using Volo.Abp.DependencyInjection;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Json;
using Volo.Abp.Validation.StringValues;

namespace Starbender.FileClerk.Features;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(StringValueTypeSerializer))]
public sealed class FileClerkStringValueTypeSerializer : StringValueTypeSerializer
{
    public FileClerkStringValueTypeSerializer(IJsonSerializer jsonSerializer)
        : base(jsonSerializer)
    {
    }

    public override string Serialize(IStringValueType stringValueType)
    {
        if (stringValueType is FileClerkProviderSelectionStringValueType)
        {
            return """{"Name":"FILECLERK_PROVIDER_SELECTION"}""";
        }

        return base.Serialize(stringValueType);
    }

    public override IStringValueType Deserialize(string value)
    {
        using var document = JsonDocument.Parse(value);
        if (document.RootElement.TryGetProperty("Name", out var name) &&
            name.GetString() is var serializedName &&
            (string.Equals(
                 serializedName,
                 FileClerkProviderSelectionStringValueType.TypeName,
                 StringComparison.Ordinal) ||
             string.Equals(
                 serializedName,
                 nameof(FileClerkProviderSelectionStringValueType),
                 StringComparison.Ordinal)))
        {
            return new FileClerkProviderSelectionStringValueType();
        }

        return base.Deserialize(value);
    }
}
