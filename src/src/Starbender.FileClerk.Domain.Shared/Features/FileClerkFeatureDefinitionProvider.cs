using Starbender.FileClerk.Localization;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

namespace Starbender.FileClerk.Features;

public sealed class FileClerkFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(
            FileClerkFeatures.GroupName,
            L("FileClerk:FileClerk"));

        group.AddFeature(
            FileClerkFeatures.Enabled,
            defaultValue: "false",
            displayName: L("FileClerk:Enabled"),
            description: L("FileClerk:FileClerkEnabledDescription"),
            valueType: new ToggleStringValueType(),
            isVisibleToClients: true);

        group.AddFeature(
            FileClerkFeatures.Providers,
            defaultValue: "[]",
            displayName: L("FileClerk:Providers"),
            description: L("FileClerk:FileClerkProvidersDescription"),
            valueType: new FileClerkProviderSelectionStringValueType(),
            isVisibleToClients: true);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FileClerkResource>(name);
    }
}
