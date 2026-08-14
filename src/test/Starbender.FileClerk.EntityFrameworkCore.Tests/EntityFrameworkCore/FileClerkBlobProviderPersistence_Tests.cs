using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Starbender.FileClerk.BlobProviders;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Starbender.FileClerk.EntityFrameworkCore;

public sealed class FileClerkBlobProviderPersistence_Tests :
    FileClerkEntityFrameworkCoreTestBase
{
    [Fact]
    public async Task Seeder_Should_Create_The_Ten_Disabled_Registrations_Idempotently()
    {
        var seeder = GetRequiredService<FileClerkBlobProviderDataSeedContributor>();
        var repository = GetRequiredService<IRepository<FileClerkBlobProvider, int>>();

        await seeder.SeedAsync(new DataSeedContext());
        await seeder.SeedAsync(new DataSeedContext());

        var providers = await repository.GetListAsync();
        providers.Count.ShouldBe(10);
        providers.ShouldAllBe(provider => !provider.Enabled);
        providers.Select(provider => provider.ImplementationType)
            .ShouldBe(FileClerkBlobProviderDataSeedContributor.ImplementationTypes);
        providers.ShouldAllBe(provider =>
            provider.ConfigurationSchema == """{"connectionString":""}""");
    }

    [Fact]
    public void Model_Should_Have_Unique_Name_And_Implementation_Type_Indexes()
    {
        var dbContext = GetRequiredService<FileClerkDbContext>();
        var entity = dbContext.Model.FindEntityType(typeof(FileClerkBlobProvider));
        entity.ShouldNotBeNull();

        entity.GetTableName().ShouldBe("FileClerkBlobProviders");
        entity.GetIndexes().Single(index =>
            index.Properties.Single().Name == nameof(FileClerkBlobProvider.Name))
            .IsUnique.ShouldBeTrue();
        entity.GetIndexes().Single(index =>
            index.Properties.Single().Name == nameof(FileClerkBlobProvider.ImplementationType))
            .IsUnique.ShouldBeTrue();
    }
}
