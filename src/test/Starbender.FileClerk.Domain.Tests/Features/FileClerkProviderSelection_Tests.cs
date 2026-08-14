using Starbender.FileClerk.Features;
using Shouldly;
using Xunit;

namespace Starbender.FileClerk.Features;

public sealed class FileClerkProviderSelection_Tests
{
    [Fact]
    public void Serialize_Should_Sort_And_Deduplicate()
    {
        FileClerkProviderSelection.Serialize([7, 2, 7, 3])
            .ShouldBe("[2,3,7]");
    }

    [Theory]
    [InlineData("[3,1,3]", new[] { 1, 3 })]
    [InlineData("[]", new int[0])]
    public void TryParse_Should_Accept_Integer_Arrays(
        string value,
        int[] expected)
    {
        FileClerkProviderSelection.TryParse(value, out var actual).ShouldBeTrue();
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{}")]
    [InlineData("[0]")]
    [InlineData("""[1,"2"]""")]
    public void TryParse_Should_Treat_Invalid_Legacy_Values_As_Empty(string? value)
    {
        FileClerkProviderSelection.TryParse(value, out var actual).ShouldBeFalse();
        actual.ShouldBeEmpty();
    }

    [Fact]
    public void Value_Type_Should_Require_Canonical_Json()
    {
        var valueType = new FileClerkProviderSelectionStringValueType();

        valueType.Validator.IsValid("[1,2]").ShouldBeTrue();
        valueType.Validator.IsValid("[2,1]").ShouldBeFalse();
        valueType.Validator.IsValid("[1,1]").ShouldBeFalse();
    }
}
