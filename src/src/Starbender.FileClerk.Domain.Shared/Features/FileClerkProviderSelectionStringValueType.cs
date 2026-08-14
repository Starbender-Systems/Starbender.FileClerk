using System;
using Volo.Abp.Validation.StringValues;

namespace Starbender.FileClerk.Features;

[Serializable]
[StringValueType(TypeName)]
public sealed class FileClerkProviderSelectionStringValueType : StringValueTypeBase
{
    public const string TypeName = "FILECLERK_PROVIDER_SELECTION";

    public FileClerkProviderSelectionStringValueType()
        : base(new FileClerkProviderSelectionValueValidator())
    {
    }
}

[Serializable]
public sealed class FileClerkProviderSelectionValueValidator : ValueValidatorBase
{
    public override bool IsValid(object? value)
    {
        return value is string stringValue && FileClerkProviderSelection.IsCanonical(stringValue);
    }
}
