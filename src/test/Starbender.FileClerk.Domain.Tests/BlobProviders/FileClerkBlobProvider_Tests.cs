using System;
using Shouldly;
using Starbender.FileClerk.BlobProviders;
using Volo.Abp;
using Volo.Abp.Auditing;
using Xunit;

namespace Starbender.FileClerk.BlobProviders;

public sealed class FileClerkBlobProvider_Tests
{
    [Fact]
    public void Constructor_Should_Validate_And_Default_To_Disabled()
    {
        var provider = new FileClerkBlobProvider(
            "Memory",
            "Volo.Abp.BlobStoring.Memory",
            """{"connectionString":""}""");

        provider.Name.ShouldBe("Memory");
        provider.ImplementationType.ShouldBe("Volo.Abp.BlobStoring.Memory");
        provider.ConfigurationSchema.ShouldBe("""{"connectionString":""}""");
        provider.Enabled.ShouldBeFalse();
        provider.ShouldBeAssignableTo<IAuditedObject>();
        provider.ShouldNotBeAssignableTo<ISoftDelete>();
    }

    [Theory]
    [InlineData("not a type")]
    [InlineData("System..String")]
    [InlineData(".System.String")]
    public void Constructor_Should_Reject_Invalid_Type_Full_Names(string implementationType)
    {
        var exception = Should.Throw<BusinessException>(() =>
            new FileClerkBlobProvider(
                "Invalid",
                implementationType,
                """{"connectionString":""}"""));

        exception.Code.ShouldBe(FileClerkErrorCodes.InvalidImplementationType);
    }

    [Fact]
    public void Constructor_Should_Accept_Generic_And_Nested_Type_Full_Names()
    {
        var provider = new FileClerkBlobProvider(
            "Generic",
            "Example.Provider`1+Nested`2",
            """{"connectionString":""}""");

        provider.ImplementationType.ShouldBe("Example.Provider`1+Nested`2");
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("[]")]
    [InlineData("not-json")]
    public void Constructor_Should_Require_A_Connection_String_Schema(string schema)
    {
        var exception = Should.Throw<BusinessException>(() =>
            new FileClerkBlobProvider(
                "Invalid",
                "Example.Provider",
                schema));

        exception.Code.ShouldBe(FileClerkErrorCodes.InvalidConfigurationSchema);
    }

    [Fact]
    public void SetEnabled_Should_Change_Only_The_Global_Gate()
    {
        var provider = new FileClerkBlobProvider(
            "Memory",
            "Volo.Abp.BlobStoring.Memory",
            """{"connectionString":""}""");

        provider.SetEnabled(true);

        provider.Enabled.ShouldBeTrue();
        provider.Name.ShouldBe("Memory");
        provider.ImplementationType.ShouldBe("Volo.Abp.BlobStoring.Memory");
    }
}
