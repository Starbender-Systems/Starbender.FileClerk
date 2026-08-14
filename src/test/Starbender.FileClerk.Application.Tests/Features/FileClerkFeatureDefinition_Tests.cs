using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;
using Xunit;

namespace Starbender.FileClerk.Features;

public sealed class FileClerkFeatureDefinition_Tests :
    FileClerkApplicationTestBase<FileClerkApplicationTestModule>
{
    [Fact]
    public async Task Definitions_Should_Be_Localized_Visible_And_Use_Expected_Value_Types()
    {
        var manager = GetRequiredService<IFeatureDefinitionManager>();
        var enabled = await manager.GetAsync(FileClerkFeatures.Enabled);
        var providers = await manager.GetAsync(FileClerkFeatures.Providers);

        enabled.DefaultValue.ShouldBe("false");
        enabled.IsVisibleToClients.ShouldBeTrue();
        enabled.ValueType.ShouldBeOfType<ToggleStringValueType>();
        enabled.DisplayName.ShouldBeOfType<LocalizableString>().Name.ShouldBe("FileClerk:Enabled");
        enabled.Description.ShouldBeOfType<LocalizableString>().Name.ShouldBe("FileClerk:FileClerkEnabledDescription");

        providers.DefaultValue.ShouldBe("[]");
        providers.IsVisibleToClients.ShouldBeTrue();
        providers.ValueType.ShouldBeOfType<FileClerkProviderSelectionStringValueType>();
        providers.DisplayName.ShouldBeOfType<LocalizableString>().Name.ShouldBe("FileClerk:Providers");
        providers.Description.ShouldBeOfType<LocalizableString>().Name.ShouldBe("FileClerk:FileClerkProvidersDescription");
    }

    [Fact]
    public void Custom_Value_Type_Should_Round_Trip_Through_Feature_Management_Serializer()
    {
        var serializer = GetRequiredService<StringValueTypeSerializer>();

        var serialized = serializer.Serialize(
            new FileClerkProviderSelectionStringValueType());
        var deserialized = serializer.Deserialize(serialized);

        deserialized.ShouldBeOfType<FileClerkProviderSelectionStringValueType>();
    }
}
